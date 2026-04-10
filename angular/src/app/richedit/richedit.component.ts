import { Component, ElementRef, OnDestroy, AfterViewInit, ViewChild, Input } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { GlobalService } from '@app/main/File/filelist.component';
@Component({
  selector: 'app-richedit',
  templateUrl: './richedit.component.html',
  styleUrls: ['./richedit.component.css']
})
export class RicheditComponent implements OnDestroy, AfterViewInit {
  @ViewChild('docxContainer', { static: true }) docxContainer: ElementRef<HTMLDivElement> | undefined;
  @Input() changing: Subject<boolean>;
  private changingSubscription: Subscription | undefined;
  errorMessage = '';
  fallbackText = '';
  fallbackHtml = '';
  isHtmlFallback = false;
  isDocx = false;
  isLoading = false;

  constructor(private globalService: GlobalService) {}

  ngOnInit() {
    this.changingSubscription = this.changing?.subscribe(() => this.loadDocument());
  }

  async ngAfterViewInit(): Promise<void> {
    await this.loadDocument();
  }

  async loadDocument(): Promise<void> {
    this.errorMessage = '';
    this.fallbackText = '';
    this.fallbackHtml = '';
    this.isHtmlFallback = false;

    if (!this.globalService.docFile) {
      this.errorMessage = 'No document file provided.';
      return;
    }

    const extension = this.getDocumentExtension(this.globalService.oldPathSelectedFile || '');
    this.isDocx = extension === 'docx';
    if (!this.isDocx) {
      const decoded = this.decodeText(this.globalService.docFile);
      if (this.looksLikeHtml(decoded)) {
        this.isHtmlFallback = true;
        this.fallbackHtml = decoded;
      } else {
        this.fallbackText = decoded;
      }
      return;
    }

    if (this.docxContainer?.nativeElement) {
      this.docxContainer.nativeElement.innerHTML = '';
    }

    this.isLoading = true;
    try {
      const moduleLoader = new Function('m', 'return import(m);') as (modulePath: string) => Promise<any>;
      const docxPreviewModule = await moduleLoader('https://cdn.jsdelivr.net/npm/docx-preview@0.3.6/+esm');
      const renderAsync = docxPreviewModule?.renderAsync;
      if (!renderAsync) {
        throw new Error('docx-preview renderAsync is unavailable');
      }

      const documentBuffer = this.base64ToArrayBuffer(this.globalService.docFile);
      await renderAsync(documentBuffer, this.docxContainer.nativeElement, undefined, {
        className: 'docx',
        inWrapper: true,
        breakPages: true,
        ignoreWidth: false,
        ignoreHeight: false,
        ignoreFonts: false,
        renderHeaders: true,
        renderFooters: true,
        renderFootnotes: true,
        renderEndnotes: true,
        renderComments: true
      });
    } catch (error) {
      this.errorMessage = 'DOCX preview could not be loaded.';
    } finally {
      this.isLoading = false;
    }
  }

  getDocumentExtension(fileName: string): string {
    const parts = fileName.split('.');
    return parts.length > 1 ? (parts.pop() || '').toLowerCase() : '';
  }

  private base64ToArrayBuffer(base64: string): ArrayBuffer {
    const binaryString = atob(base64 || '');
    const length = binaryString.length;
    const bytes = new Uint8Array(length);
    for (let i = 0; i < length; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
  }

  private decodeText(base64OrText: string): string {
    if (!base64OrText) {
      return '';
    }
    try {
      return decodeURIComponent(escape(atob(base64OrText)));
    } catch {
      return base64OrText;
    }
  }

  private looksLikeHtml(content: string): boolean {
    if (!content) {
      return false;
    }

    const trimmed = content.trim().toLowerCase();
    return trimmed.startsWith('<!doctype html') || trimmed.startsWith('<html') || trimmed.includes('<body');
  }

  ngOnDestroy() {
    this.changingSubscription?.unsubscribe();
  }
}
