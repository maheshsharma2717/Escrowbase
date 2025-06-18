import { Component, ElementRef, OnDestroy, AfterViewInit, ViewChild, Input, Optional, Inject } from '@angular/core';
import { create, RichEdit, DocumentFormat } from 'devexpress-richedit';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
import { GlobalService } from '@app/main/File/filelist.component';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-richedit',
  templateUrl: './richedit.component.html',
  styleUrls: ['./richedit.component.css']
})
export class RicheditComponent implements OnDestroy, AfterViewInit {
  private rich: RichEdit | null = null;
  apiUrl: any = ""

  options: any = {
    readOnly: false,
    height: '90vh'
  };

  @Input() changing: Subject<boolean>;

  constructor(private element: ElementRef, private http: HttpClient, private globalService: GlobalService,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string


  ) {
    this.apiUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
  }

  ngOnInit() {
    this.changing.subscribe(v => {
      this.saveChanges();
    });
  }

  ngAfterViewInit(): void {
    debugger
    // Initialize the RichEdit component
    this.rich = create(this.element.nativeElement.firstElementChild, this.options);
    this.loadDocument();
  }

  loadDocument() {
    debugger
    // Load the document based on its format
    if (!this.globalService.docFile) {
      console.error('No document file provided.');
      return;
    }
    this.rich.readOnly = false;
    let base64string = this.globalService.docFile;
    let extension = this.getDocumentExtension(this.globalService.oldPathSelectedFile);
    let documentFormat: DocumentFormat | undefined;
    switch (extension) {
      case 'docx':
        documentFormat = DocumentFormat.OpenXml;
        break;
      case 'doc':
        documentFormat = DocumentFormat.PlainText;
        break;
      case 'pdf':
        //  documentFormat = DocumentFormat.pdf;
        break;
      case 'txt':
        this.rich.readOnly = true;
        documentFormat = DocumentFormat.PlainText;
        break;
      case 'rtf':
        this.rich.readOnly = true;
        documentFormat = DocumentFormat.Rtf;
        break;
      case 'eml':
        this.rich.readOnly = true;
        //documentFormat = DocumentFormat.Rtf; // Or use a custom format if needed
        break;
      default:
        console.error('Unsupported document format:', extension);
        return;
    }

    // Open the document in RichEdit
    if (documentFormat) {


      this.rich?.openDocument(base64string, this.globalService.oldPathSelectedFile, documentFormat);
    }
  }

  getDocumentExtension(fileName: string): string {
    return fileName.split('.').pop().toLowerCase();
  }

  saveChanges() {
    console.log('this.rich:', this.rich); // Debugging statement
    debugger;
    // Save the changes made in the RichEdit component
    if (this.rich) {
      this.rich.exportToBase64((base64String: string) => {
        const payload = {
          fileName:  this.globalService.oldPathSelectedFile || 'document.docx',
          base64Content: base64String,
          filePath: this.globalService.oldPathSelectedFile
        };

        const path = `${this.apiUrl}/FileManager/`;

        this.http.post(path + 'SaveDocument', payload).subscribe(
          response => {
            console.log('Document saved successfully.');
          },
          error => {
            console.error('Error saving document:', error);
          }
        );
      }, DocumentFormat.OpenXml); // Save as .docx format by default
    }
  }

  arrayBufferToBase64(buffer: ArrayBuffer): string {
    let binary = '';
    const bytes = new Uint8Array(buffer);
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
  }

  ngOnDestroy() {
    // Dispose of the RichEdit instance when the component is destroyed
    if (this.rich) {
      this.rich.dispose();
      this.rich = null;
    }
  }
}
