// clipboard.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClipboardService {

  isClipboardItemSupported(): boolean {
    return typeof (window as any).ClipboardItem !== 'undefined';
  }

  async copyToClipboard(text: string | Blob, type: 'text/plain' | 'image/png' = 'text/plain') {
    if (this.isClipboardItemSupported()) {
      try {
        const clipboardItemData = new (window as any).ClipboardItem({ [type]: text });
        //  await navigator.clipboard.write([clipboardItemData]);
        console.log(`Copied ${type} to clipboard.`);
      } catch (error) {
        console.error('Failed to copy:', error);
      }
    } else {
      console.warn('ClipboardItem API not supported.');
    }
  }

  async readFromClipboard(): Promise<void> {

      
    const contentEditableDiv = document.getElementById('appDragDropAreaContent');
    if (contentEditableDiv) {
      contentEditableDiv.focus();
    }
    try {
      const clipboard = navigator.clipboard as any;
      if (clipboard && clipboard.read) {

        const clipboardItems = await clipboard.read();
        console.log(`Clipboard Items:`, clipboardItems);
        for (const item of clipboardItems) {
          for (const type of item.types) {
            const blob = await item.getType(type);
            console.log(`Clipboard contains ${type}:`, blob);
          }
        }
      } else {
        console.warn('Clipboard read() API not supported.');
      }
    } catch (error) {
      console.error('Error accessing clipboard:', error);
    }
  }
}

