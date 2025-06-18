import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PrimengTableHelper } from '@shared/helpers/PrimengTableHelper';
import { CreateOrEditEscrowUserNotesDto, EscrowUserNotesesServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/common/app-component-base';
@Component({
  selector: 'app-sticky-notes',
  templateUrl: './sticky-notes.component.html',
  styleUrls: ['./sticky-notes.component.css']
})

export class StickyNotesComponent extends AppComponentBase {


  @ViewChild('createOrEditStickyNoteModal', { static: true }) modal: ModalDirective;

  stickyNoteContent: string = '';
  stickyNoteError: string = '';
  isSavingStickyNote: boolean = false;
  stickyNotesHistoryTabActive: boolean = false;
  primengTableHelper: PrimengTableHelper = new PrimengTableHelper();
  noteMessage: string;


  constructor(
    private escrowUserNotesesServiceProxy: EscrowUserNotesesServiceProxy,
    injector: Injector,
  ) {
    super(injector);
  }


  ngOnInit(): void {
    this.getAllStickyNotes();  // Load the sticky notes when the component is initialized
  }

  // Fetch all sticky notes
  getAllStickyNotes(): void {
    this.escrowUserNotesesServiceProxy.getEscrowUserNotesByUserId(this.appSession.userId).subscribe({
      next: (response) => {

        const count = response.length;
        // Assuming 'response.items' contains the sticky notes
        this.primengTableHelper.records = response;
        this.primengTableHelper.totalRecordsCount = count;
      },
      error: (err) => {
        console.error('Error fetching sticky notes:', err);
        this.stickyNoteError = 'Error fetching sticky notes.';
      }
    });
  }

  closeStickyNoteModal(): void {
    this.modal.hide();
  }
  show() {
    this.modal.show();
  }

  createStickyNoteTabEvent(event: any): void {
    this.stickyNotesHistoryTabActive = false;
    this.clearStickyNoteInput();
  }

  stickyNotesHistoryTabEvent(event: any): void {
    if (event.active) { // Ensure the event is triggered for the active tab
      this.stickyNotesHistoryTabActive = true;
      this.loadStickyNotesHistory();
      this.getAllStickyNotes();
    }
  }

  clearStickyNoteInput(): void {
    this.stickyNoteContent = '';
    this.stickyNoteError = '';
  }

  saveStickyNote(): void {
    const escrowNumber = localStorage.getItem('activeTab');
    const noteDto: CreateOrEditEscrowUserNotesDto = {
      id: 0,
      message: this.noteMessage,
      escrowNumber: escrowNumber,
      createdAt: null,
      createdBy: this.appSession.userId,
      init: () => { },
      toJSON: () => ({

        id: 0,
        message: this.noteMessage,
        escrowNumber: escrowNumber,
        createdAt: null,
        createdBy: this.appSession.userId,
      })
    };

    this.escrowUserNotesesServiceProxy.createOrEdit(noteDto).subscribe({
      next: () => {
        console.log('Note saved successfully!');
        Swal.fire({
          title: 'Note saved successfully!',
          icon: 'success',
          confirmButtonText: 'OK'
        });

        this.noteMessage = '';
        this.getAllStickyNotes();
      },
      error: (err) => {
        console.error('Error saving note:', err);
      }
    });
  }

  loadStickyNotesHistory(event?: any): void {
    this.primengTableHelper.isLoading = true;

    // Simulate loading history
    setTimeout(() => {
      this.primengTableHelper.records = [
        {
          noteContent: 'First Sticky Note',
          createdDate: new Date(),
          status: 'Active'
        },
        {
          noteContent: 'Second Sticky Note',
          createdDate: new Date(),
          status: 'Inactive'
        }
      ];
      this.primengTableHelper.totalRecordsCount = this.primengTableHelper.records.length;
      this.primengTableHelper.isLoading = false;
    }, 1000);
  }

  editStickyNote(note: any): void {
    console.log('Editing note:', note);
  }

  viewStickyNote(note: any): void {
    console.log('Viewing note:', note);
  }

  deleteStickyNote(note: any): void {
    if (confirm('Are you sure you want to delete this note?')) {
      console.log('Deleting note:', note);
    }
  }

}


