// import { Component, Injector, OnInit } from '@angular/core';
// import { AppComponentBase } from '@shared/common/app-component-base';
// import { EscrowFileTagsesServiceProxy } from '@shared/service-proxies/service-proxies';

// @Component({
//   selector: 'app-escrow-usertags',
//   templateUrl: './escrow-usertags.component.html',
//   styleUrls: ['./escrow-usertags.component.css']
// })
// export class EscrowUsertagsComponent extends AppComponentBase {
//   show() {
//     throw new Error('Method not implemented.');
//   }

//   constructor(
//     private escrowFileTagsesServiceProxy: EscrowFileTagsesServiceProxy,
//      injector: Injector,
//   ) {
//     super(injector);
//   }

//   ngOnInit(): void {
//   }

// }


import { Component, EventEmitter, Injector, Input, NgModule, OnInit, Output, ViewChild } from '@angular/core';
import { PrimengTableHelper } from '@shared/helpers/PrimengTableHelper';
import { CreateOrEditEscrowFileTagsDto, CreateOrEditEscrowUserNotesDto, EscrowFileTagsesServiceProxy, EscrowUserNotesesServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { FormsModule } from '@node_modules/@angular/forms/forms';
import { DateTime } from 'luxon';
// import { ColorPickerModule } from 'ngx-color-picker';


interface Tag {
  id: number;
  name: string;
  description: string;
  color: string;
}

@Component({
  selector: 'app-escrow-usertags',
  templateUrl: './escrow-usertags.component.html',
  styleUrls: ['./escrow-usertags.component.css']
})

export class EscrowUsertagsComponent extends AppComponentBase {
  @ViewChild('tagTabSet', { static: false }) tagTabSet: TabsetComponent;
  @ViewChild('createOrEditEscrowUserTagModal', { static: true }) modal: ModalDirective;
  @Input() selectedFile: any;
  @Output() modalClosed: EventEmitter<void> = new EventEmitter<void>();
  userType: string;
  stickyNoteContent: string = '';
  stickyNoteError: string = '';
  isSavingStickyNote: boolean = false;
  stickyNotesHistoryTabActive: boolean = false;
  primengTableHelper: PrimengTableHelper = new PrimengTableHelper();
  noteMessage: string = '';
  secondaryColor: string = '#000000';
  primaryColor: string = '#ffffff';
  isBackgroundPickerOpen: boolean = false;
  isDescriptionPickerOpen: boolean = false;
  selectedColor: any;
  editMode: boolean = false;
  editingTagId: number | null = null;
  inputRef: HTMLInputElement;
  selectColor(color: any) {
    this.selectedColor = color;
  }
  manageTagList: any = [];
  fileTags: any[] = [];
  constructor(
    private escrowFileTagsServiceProxy: EscrowFileTagsesServiceProxy,
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit(): void {

  }

  closeStickyNoteModal(): void {
    this.modal.hide();
  }
  show() {
    this.modal.show();
    this.getAllFileTags();
  }

  saveTag() {
    debugger;
    const escrowNumber = localStorage.getItem('activeTab');

    if (!this.noteMessage) {
      abp.notify.error('Please add note message', 'Error', {
        positionClass: 'toast-top-center'
      });
      return;
    }

    this.selectedColor = {
      noteMessage: this.noteMessage,
      bgColor: this.primaryColor,
      fontColor: this.secondaryColor
    };

    const tagDto: CreateOrEditEscrowFileTagsDto = {
      id: this.editMode && this.editingTagId ? this.editingTagId : null,
      tagDescription: this.selectedColor.noteMessage,
      tagColor: this.selectedColor.bgColor + " , " + this.selectedColor.fontColor,
      escrowNumber: escrowNumber,
      createdAt: DateTime.now().toUTC(),
      fileName: this.selectedFile?.name,
      //createdBy: this.appSession.userId,
      init: () => { },
      toJSON: () => ({
        id: this.editMode && this.editingTagId ? this.editingTagId : null,
        tagDescription: this.selectedColor.noteMessage,
        tagColor: this.selectedColor.bgColor + " , " + this.selectedColor.fontColor,
        escrowNumber: escrowNumber,
        createdAt: DateTime.now().toUTC(),
        // createdBy: this.appSession.userId,
        fileName: this.selectedFile?.name,
      })
    };

    this.escrowFileTagsServiceProxy.createOrEdit(tagDto).subscribe({
      next: (response) => {
        if (!response.success) {
          abp.notify.error(response.message, 'Error', {
            positionClass: 'toast-top-center'
          });
          return;
        }
      
        Swal.fire({
          title: this.editMode ? 'Tag updated successfully.' : 'Tag saved successfully.',
          icon: 'success',
          confirmButtonText: 'OK'
        });

        this.getAllFileTags();
        this.noteMessage = '';
        this.primaryColor = '#ffffff';      // reset to default or any desired color
        this.secondaryColor = '#000000';
        this.selectedColor = null;
        this.editMode = false;
        this.editingTagId = null;
        this.textinput();
       

        setTimeout(() => {
          this.tagTabSet.tabs[1].active = true;
        });
      },
      error: (err) => {
        console.error('Error saving tag:', err);
      }
    });
  }
 
  deleteTag(id: number) {
    debugger;
    let EscrowTab = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + EscrowTab);
    const disallowedTypes = ['EOX', 'EA1', 'EO1', 'EAX'];
    if (disallowedTypes.includes(userType)) {
      abp.notify.error("You are not allowed to delete the tags.");
      return;
    }    
    Swal.fire({
      title: 'Are you sure?',
      text: 'Do you really want to delete this tag?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result: { isConfirmed: any; }) => {
      if (result.isConfirmed) {
        this.escrowFileTagsServiceProxy.delete(id).subscribe({
          next: () => {
            Swal.fire({
              title: 'Tag removed.',
              text: '',
              icon: 'success',
              confirmButtonText: 'OK'
            });
            this.getAllFileTags(); // Refresh tag list
          },
          error: (err) => {
            console.error('Error deleting tag:', err);
            Swal.fire('Error', 'Failed to delete tag. Please try again.', 'error');
          }
        });
      }
    });
  }

  editTag(id: number) {
    debugger;
    const tag = this.manageTagList.find(item => item.escrowFileTags.id === id);
    if (tag) {
      this.noteMessage = tag.escrowFileTags.tagDescription;
      const [bgColor, fontColor] = tag.escrowFileTags.tagColor.split(',').map(c => c.trim());
      this.selectedColor = {
        bgColor,
        fontColor
      };
      this.primaryColor = bgColor;
      this.secondaryColor = fontColor;

      this.editMode = true;
      this.editingTagId = id;

      setTimeout(() => {
        debugger;
        this.tagTabSet.tabs[0].active = true;
      });
    }
  }

  closeTagModal(): void {
    debugger;
    //this.modalClosed.emit();
    this.modal.hide();
    this.noteMessage = '';
    this.primaryColor = '#ffffff';
    this.secondaryColor = '#000000';
    this.selectedColor = {
      bgColor: this.primaryColor,
      fontColor: this.secondaryColor
    };

    this.editMode = false;
    this.editingTagId = null;
  }

  getAllFileTags() {
    debugger;
    this.escrowFileTagsServiceProxy.getAll(undefined, undefined, 0, 10000).subscribe((response: any) => {
      this.manageTagList = response.items;
    });
  }

  createStickyNoteTabEvent() {

  }

  manageTagsTabEvent() {

  }

  get inputStyles() {
    if (!this.selectedColor || !this.selectedColor.bgColor || !this.selectedColor.fontColor) {
      return {

        'color': 'black'
      };
    }

    return {
      'background-color': this.selectedColor.bgColor,
      'color': this.selectedColor.fontColor,
      'border': '1px solid #2196F3'
    }
  }

  textinput() {
    this.selectedColor = {
      noteMessage: this.noteMessage,
      bgColor: this.primaryColor,
      fontColor: this.secondaryColor
    };
  }

  adjustHeight(event: any) {
    const textarea = event.target;
    textarea.style.height = 'auto';
    textarea.style.height = `${textarea.scrollHeight}px`;
  }

  checkLimit(text: string): void {
    if (text.length === 60) {
      abp.notify.error("Maximum 60 characters allowed.");
    }
  }

  clearBackgroundColor() {
    this.primaryColor = '';
    this.isBackgroundPickerOpen = false;
    document.getElementById('color1')?.blur();
  }

  clearDescriptionColor() {
    this.secondaryColor = '';
    this.isDescriptionPickerOpen = false;
    document.getElementById('color2')?.blur();
  }

  onColorChange(event: any) {
    // Your existing color change logic
  }

  onMouseUpColorPicker(event: any) {
    // Your existing logic
  }
}


