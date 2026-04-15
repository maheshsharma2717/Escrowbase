import { Component, Input, OnInit, OnDestroy, AfterViewInit, ChangeDetectorRef, ViewChild, ElementRef, NgZone } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { GlobalService } from '../main/File/filelist.component';

declare var Quill: any;

@Component({
  selector: 'app-richedit',
  templateUrl: './richedit.component.html',
  styleUrls: ['./richedit.component.css']
})
export class RicheditComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('quillEditorContainer', { static: false }) quillEditorContainer: ElementRef;
  @Input() changing: Subject<boolean>;
  private changingSubscription: Subscription | undefined;
  
  private quill: any;
  errorMessage = '';
  fallbackText = '';
  fallbackHtml = '';
  isHtmlFallback = false;
  isSupported = false;
  isLoading = false;

  constructor(
    private globalService: GlobalService,
    private cd: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  ngOnInit() {
    this.checkSupport();
    this.changingSubscription = this.changing?.subscribe(() => this.loadDocument());
  }

  ngAfterViewInit(): void {
    // Delay initialization to ensure the modal DOM is fully ready and attached
    setTimeout(() => {
      this.ngZone.runOutsideAngular(() => {
        this.initializeQuill();
        this.ngZone.run(async () => {
          await this.loadDocument();
        });
      });
    }, 200);
  }

  private initializeQuill() {
    if (this.quill || !this.quillEditorContainer) return;

    // Register this instance as the active editor for the GlobalService
    this.globalService.editor = this;

    try {
      if (typeof Quill !== 'undefined') {
        const toolbarOptions = [
          ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
          ['blockquote', 'code-block'],
          [{ 'header': 1 }, { 'header': 2 }],               // custom button values
          [{ 'list': 'ordered'}, { 'list': 'bullet' }],
          [{ 'script': 'sub'}, { 'script': 'super' }],      // superscript/subscript
          [{ 'indent': '-1'}, { 'indent': '+1' }],          // outdent/indent
          [{ 'direction': 'rtl' }],                         // text direction
          [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
          [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
          [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
          [{ 'font': [] }],
          [{ 'align': [] }],
          ['clean'],                                         // remove formatting button
          ['link', 'image']                                 // link and image
        ];

        this.quill = new Quill(this.quillEditorContainer.nativeElement, {
          modules: {
            toolbar: toolbarOptions
          },
          theme: 'snow'
        });
      } else {
        console.error('Quill library not found');
      }
    } catch (err) {
      console.error('Error initializing Quill:', err);
    }
  }

  private checkSupport() {
    const extension = this.getDocumentExtension(this.globalService.oldPathSelectedFile || '');
    // Spire.Doc converts doc, docx, rtf to HTML for us
    this.isSupported = ['docx', 'doc', 'rtf', 'txt', 'html', 'eml'].includes(extension);
  }

  async loadDocument(): Promise<void> {
    this.errorMessage = '';
    this.fallbackText = '';
    this.fallbackHtml = '';
    this.isHtmlFallback = false;

    if (!this.globalService.docFile) {
      if (this.quill) {
        this.quill.root.innerHTML = '';
      }
      return;
    }

    this.checkSupport();

    if (this.isSupported && this.quill) {
      this.isLoading = true;
      this.cd.detectChanges();
      
      try {
        const content = this.globalService.docFile;
        const decoded = this.decodeText(content);
        
        // Use clipboard API for safer HTML injection in Quill 1.x
        this.quill.clipboard.dangerouslyPasteHTML(decoded);
        
        this.isLoading = false;
      } catch (err) {
        this.errorMessage = 'Error loading document into editor.';
        console.error(err);
        this.isLoading = false;
      }
    } else {
      const decoded = this.decodeText(this.globalService.docFile);
      if (this.looksLikeHtml(decoded)) {
        this.isHtmlFallback = true;
        this.fallbackHtml = decoded;
      } else {
        this.fallbackText = decoded;
      }
    }
    this.cd.detectChanges();
  }

  public saveDocument(): Promise<string> {
    return new Promise((resolve, reject) => {
      if (!this.quill) {
        reject('Quill not initialized');
        return;
      }
      
      const html = this.quill.root.innerHTML;
      // Encode HTML to base64 for safe transport
      const base64 = btoa(unescape(encodeURIComponent(html)));
      resolve(base64);
    });
  }

  getDocumentExtension(fileName: string): string {
    if (!fileName) return '';
    const parts = fileName.split('.');
    return parts.length > 1 ? (parts.pop() || '').toLowerCase() : '';
  }

  private decodeText(base64: string): string {
    if (!base64) return '';
    try {
      return decodeURIComponent(escape(atob(base64)));
    } catch {
      return base64;
    }
  }

  private looksLikeHtml(content: string): boolean {
    if (!content) return false;
    const trimmed = content.trim().toLowerCase();
    return trimmed.startsWith('<!doctype html') || trimmed.startsWith('<html') || trimmed.includes('<body') || trimmed.includes('<div');
  }

  ngOnDestroy() {
    this.changingSubscription?.unsubscribe();
  }
}
