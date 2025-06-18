import { Component, Injector, Renderer2, Injectable, ViewEncapsulation, ViewChild, Input, TemplateRef, ElementRef, HostListener, OnDestroy, Pipe, PipeTransform, Output, EventEmitter } from '@angular/core';

import { AppComponentBase } from '@shared/common/app-component-base';
import { DashboardCustomizationConst } from '@app/shared/common/customizable-dashboard/DashboardCustomizationConsts';
import FileSystemProvider from 'devextreme/file_management/remote_provider';
import { HttpClient, HttpHeaders, HttpEvent, HttpEventType } from '@angular/common/http';
import { DxFileManagerComponent } from 'devextreme-angular';
import { AppConsts } from '@shared/AppConsts';
import {
  EscrowDetailsServiceProxy, EscrowFileHistoriesServiceProxy, SrAssignedFilesDetailsServiceProxy, RenameModel, SrEscrowFileRemindersServiceProxy,
  CreateOrEditSrEscrowFileReminderDto, ReminderTypeList, SREscrowFileMastersServiceProxy,
  EscrowDirectMessageDetailsesServiceProxy,
  CreateOrEditEscrowFileHistoryDto,
  EscrowUserNotesesServiceProxy,
  CreateOrEditEscrowUserNotesDto

} from '@shared/service-proxies/service-proxies';
import * as $ from "jquery";
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common'
import { DomSanitizer } from '@angular/platform-browser';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { NgxExtendedPdfViewerService, IPDFViewerApplication, NgxExtendedPdfViewerComponent } from 'ngx-extended-pdf-viewer';
import WebViewer from '@pdftron/webviewer';
import { MSGReader } from 'wl-msg-reader';
import { ChatSignalrService } from '../../shared/layout/chat/chat-signalr.service';
import { Subscription, Subject } from 'rxjs';
declare var jQuery: any;
import { SignalRHelper } from '../../../shared/helpers/SignalRHelper';
import { SharedService } from './UserTypeChangeService';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { ClipboardService } from '../../../shared/utils/clipboard.service'
import { MessageEscrowOfficerComponent } from '../messageToEscrowOfficer/message-escrow-officer.component';
import { DateTime } from '@node_modules/@types/luxon';
import { StickyNotesComponent } from '../sticky-notes/sticky-notes.component';
import { EscrowUsertagsComponent } from '../escrow-usertags/escrow-usertags.component';
import { FileMainComponent } from './file-main/file-main.component';
import { FileOtherComponent } from './file-other/file-other.component';

@Component({
  selector: 'app-upload-file',
  templateUrl: './filelist.component.html',
  styleUrls: ['./filelist.component.less'],
  encapsulation: ViewEncapsulation.None,
})

export class FileViewComponent extends AppComponentBase {

  changingValue: Subject<boolean> = new Subject();
  @ViewChild(NgxExtendedPdfViewerComponent) private pdfComponent: NgxExtendedPdfViewerComponent;
  @ViewChild(DxFileManagerComponent, { static: false }) fileManager: DxFileManagerComponent;
  @ViewChild("targetDataGrid", { static: false }) fileManager1: DxFileManagerComponent;
  @ViewChild("data", { static: false }) data: DxFileManagerComponent;
  @ViewChild('fileOtherComponent') fileOtherComponent!: FileOtherComponent;
  dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;
  remoteProvider: FileSystemProvider;
  remoteProvider1: FileSystemProvider;
  ngxExtendedPdfViewerService: NgxExtendedPdfViewerService;
  clickEventsubscription: Subscription;
  btnstate: boolean = false;
  useridd: string;
  private newAttribute: any = {};
  viewer = 'google';
  selectedType = 'pptx'; //'docx';

  // doc = 'https://file-examples.com/wp-content/uploads/2017/02/file-sample_100kB.docx';
  // doc = 'https://files.fm/down.php?i=axwasezb&n=SSaD.docx';
  doc = 'https://files.fm/down.php?i=sdymh2y6';

  // https://github.com/guigrpa/docx-templates#readme
  filterLabel: string = '';  // or you can set a default value
  searchText: string = '';   // for search text input
  notes: { id: number; text: string }[] = []; // Array of sticky notes
  currentNote: string = ''; // Current note text
  currentNoteId: number | null = null; // ID of the note being edited, null for new notes

  split: any;
  typedata: any;
  datachange: any;
  bdata: any;
  datachanges: any = [];
  selectedGroup: any;
  checkR: boolean;
  checkE: boolean;
  checkA: boolean;
  checkD: boolean;
  checkS: boolean;
  R: any;
  E: any;
  A: any;
  D: any;
  S: any;
  folderPath: any;
  path1: any;
  wvInstance: any;
  spinnerUpl: boolean = false;
  fullname: any;
  file: any;
  source: any;
  fullpath: any;
  isFile: boolean = false;
  fileName: string = 'Select a file'
  sourcepath: any;
  sourcepathwithname: string;
  myurl: any;
  replacedstring: any;
  items: any;
  downloado: any;
  currentpath: any;
  currentpath1: any;
  output: any[];
  eFilter: any[];
  collect: '';
  downloadname: any;
  collect1: any;
  fileUrl;
  collection: any;
  Action: any;
  ErrorMessage: string;
  escrowname: any;
  Name: any;
  userTypes: any = [];
  type: any = [];
  userpermissionsall: any = [];
  myuser: any = ["BRX", "SRX", "BR1", "BR2", "BR3", "BR4", "BR5", "BR6", "BR7", "BR8", "BR9", "BR10", "SR1", "SR2", "SR3", "SR4", "SR5", "SR6", "SR7", "SR8", "SR9", "SR10", "RAL", "RBL", "RAS", "RBS", "RAO", "RBO", "LR1", "LR2", "LR3", "LP1", "LP2", "LP3", "TCX", "TCA", "LBX", "LBP", "EO1", "EA1", "EOX", "EAX", "TC1", "TC2", "TC3", "TC4", "TC5", "TC6", "TC7", "TC8", "TC9", "TC10", "LTC", "STC", "OTC"];
  tempcompany;
  static newusertype;
  static escrowno;
  static currentpathh;
  static currentpathh1;
  UsertypeModel: any;
  imageItemToDisplay: any = {};
  popupVisible = false;
  modalRef: BsModalRef;
  modalReff: BsModalRef;
  docx: boolean = false;
  doxFile: any;
  base64Data: any;
  retrieveResonse: any = {};
  retrievedFile: string;
  base64SrcSource: any;
  msgShow: boolean;
  eSign: boolean = false;
  EMBED_SESSION_URL: any;
  SignName: any;
  SignStatus: any;
  Sign: any;
  signPopup: boolean = false;
  fullnamepopup: boolean;
  fileFullName: any;
  fileFullName1: any;
  filenames: any;
  token: any;
  filenameold: any;
  filenamenew: any;
  fullnameold: any;
  parentpath: any;
  fullparentnew: any;
  fullparentold: any;
  shortfilename: any;
  shortfilenameold: any;
  bs: any;
  lst: any;
  change: any;
  fullnamepopup1: boolean;
  embedUrl: any;
  fileNamestrng: any;
  itemCount: any;
  itemCount1: any;
  static pathname: string;
  previousUrl: string = null;
  currentUrl: string = null;
  ItemsArray: any = {};
  tempsubcompany: any;
  someSubscription: any;
  typePDF: any = false;
  @Input() person;
  oldfileName: any;
  secondPart: any;
  signForFile: string;
  @ViewChild('fileInput')
  fileInputVariable: ElementRef;
  isUploadCalled: boolean = false;
  selectedFileForDownload = "";

  readPermission: boolean = false;
  editPermission: boolean = false;
  esignPermission: boolean = false;
  viewHistoryPermission: boolean = false;
  renamePermission: boolean = false;
  deletePermission: boolean = false
  downloadPermission: boolean = false
  viewFullNamePermission: boolean = false
  renameFileName: boolean = false;
  reminderPermission: boolean = false;
  reminderDto: CreateOrEditSrEscrowFileReminderDto = new CreateOrEditSrEscrowFileReminderDto()
  reminderTypeList: ReminderTypeList[] = [];
  reminderUserList: any = [];
  checkMessageType: boolean = false;
  checkUserSelected: boolean = false;
  @ViewChild('pasteInput') pasteInput!: ElementRef;
  showFileInput = false;
  emailContent: any;
  noUserMessage: string = '';
  @ViewChild('Rename1', { static: true }) renameTemplateRef !: TemplateRef<any>;
  @ViewChild('FullName1', { static: true }) fullNameTemplateRef !: TemplateRef<any>;
  @ViewChild('ViewEmlFile', { static: true }) viewEmlFileTemplate!: TemplateRef<any>;
  @ViewChild('ViewMsgFile', { static: true }) viewMsgFileTemplate!: TemplateRef<any>;
  @ViewChild('ViewPdfFile', { static: true }) viewPdfFileTemplate!: TemplateRef<any>;
  @ViewChild('otherAreaFileAction', { static: true }) otherAreaFileActionTemplate!: TemplateRef<any>;
  @ViewChild('Move', { static: true }) moveTemplateRef!: TemplateRef<any>;
  pastedFileContent: any;
  pastedValue: string | null = null;
  showContextMenu = false;
  contextMenuX = 0;
  contextMenuY = 0;
  readPermissionOtherArea: boolean = false;
  editPermissionOtherArea: boolean = false;
  editPermissionDocOtherArea: boolean = false;
  inputPerson: any = null;
  @ViewChild('DirectMessageToEscrow', { static: true }) DirectMessageToEscrowTemplate!: TemplateRef<any>;
  @ViewChild('messageToEscrowOfficerModal', { static: true }) messageToEscrowOfficerModal: MessageEscrowOfficerComponent;
  @ViewChild('stickyNotesComponent', { static: true }) stickyNotesComponent: StickyNotesComponent;
  @ViewChild('escrowUsertagsComponent', { static: true }) escrowUsertagsComponent: EscrowUsertagsComponent;
  @ViewChild('RenameMain', { static: true }) renameMainTemplateRef !: TemplateRef<any>;
  @ViewChild('FullNameMain', { static: true }) fullNameMainTemplateRef !: TemplateRef<any>;
  @ViewChild('Historyunique', { static: true }) viewHistoryMainTemplateRef !: TemplateRef<any>;
  @ViewChild('reminderModel', { static: true }) reminderModelMainTemplateRef !: TemplateRef<any>;
  @ViewChild('popupesign', { static: true }) esignModelMainTemplateRef !: TemplateRef<any>;
  @ViewChild('displayImagePopup', { static: true }) esignDeailsModelMainTemplateRef !: TemplateRef<any>;
  attachments: any;
  noteMessage: string;
  @ViewChild('fileMainComponent') fileMainComponent!: FileMainComponent;
  completeEnterprisePathOther: string = "";
  completeEnterprisePathMain: string = "";
  selectedFileForEsign: any;
  mainFileSelected: any;
  otherFileSelected: any;

  public constructor(
    private http: HttpClient,
    injector: Injector,
    private modalService: BsModalService,
    private location: Location,
    private sanitizer: DomSanitizer,
    private _activatedRoute: ActivatedRoute,
    private _escrowDetailsServiceProxy: EscrowDetailsServiceProxy,
    private _router: Router,
    private _chatSignalrService: ChatSignalrService,
    private sharedService: SharedService,
    private globalService: GlobalService,
    private escrowFileHistoriesServiceProxy: EscrowFileHistoriesServiceProxy,
    private _srAssignedFilesDetailsServiceProxy: SrAssignedFilesDetailsServiceProxy,
    private _srEscrowFileRemindersServiceProxy: SrEscrowFileRemindersServiceProxy,
    private _srEscrowFileMastersServiceProxy: SREscrowFileMastersServiceProxy,
    private _escrowDirectMessageDetailsesServiceProxy: EscrowDirectMessageDetailsesServiceProxy,
    private clipboardService: ClipboardService,
    private escrowUserNotesesServiceProxy: EscrowUserNotesesServiceProxy,
    private renderer: Renderer2,
  ) {
    super(injector);
    if (this.appSession.application) {
      SignalRHelper.initSignalR(() => { this._chatSignalrService.init(); });
    }
    this.clickEventsubscription = this.sharedService.getClickEvent().subscribe(() => {
      this.changeUserType();
    })
  }

  getEscrowUserList() {
    let url = AppConsts.remoteServiceBaseUrl;
    let folderPath = url + '/Home/';
    var queryParams = this.person;
    let EscrowId = atob(queryParams['e'])
    this.http.get(folderPath + "GetEscrowUserList?EscrowId=" + EscrowId).subscribe((response: any) => {
      this.myuser = response.result;
    })
  }
  onRefresh($event: any) {

  }
  userInput: string = '';

  saveNote() {
    const escrowNumber = localStorage.getItem('activeTab');
    const noteDto: CreateOrEditEscrowUserNotesDto = {
      id: 0,
      message: this.noteMessage,
      escrowNumber: escrowNumber,
      createdAt: null,
      createdBy: this.appSession.userId,
      init: () => { },
      toJSON: () => ({})
    };

    this.escrowUserNotesesServiceProxy.createOrEdit(noteDto).subscribe({
      next: () => {
        console.log('Note saved successfully!');
        this.closeModal();
      },
      error: (err) => {
        console.error('Error saving note:', err);
      }
    });
  }

  deleteNote(noteId: number): void {
    this.notes = this.notes.filter(note => note.id !== noteId); // Remove the note from the list
  }

  /**
   * Closes the modal and resets the modal state.
   */
  closeModal(): void {
    debugger;
    if (this.modalRef) {
      this.modalRef.hide();
    }
    this.resetModalState(); // Reset note-related fields
  }
  private resetModalState(): void {
    this.currentNote = ''; // Clear the current note
    this.currentNoteId = null; // Reset the note ID
  }

  LoadData() {
    let msg = this._activatedRoute.snapshot.queryParams['token'];
    let fileName;
    if (this._activatedRoute.snapshot.queryParams['f'] != undefined) {
      fileName = decodeURIComponent(this._activatedRoute.snapshot.queryParams['f'])
      fileName = atob(fileName.replace(' ', '+'));
    }
    let url = AppConsts.remoteServiceBaseUrl;
    this.folderPath = url + '/FileManager/';
    this.path1 = url;
    if (msg != undefined) {
      let decrypt = atob(msg);
      if (decrypt == "Saved") {

        this.message.success(this.l('EsignSuccessMessage'), this.l('Signed Successfully')).then(() => {
          this.http.get(this.folderPath + "DocUpdate?message=Sign&fileName=" + fileName + "&userId=" + this.appSession.userId).subscribe((response: any) => {

            console.log("File List: EsignSuccessMessage API")
          });
          this.http.get(this.folderPath + "E_SignDocDownload?mail=" + this.appSession.user.emailAddress + "&fileName=" + this._activatedRoute.snapshot.queryParams['f']).subscribe((response: any) => {
            console.log("File List: E_SignDocDownload API")
          });
          localStorage.setItem('Signing', 'False');
          localStorage.removeItem('Signing');
          this.location.back();
        });
      } else if (decrypt == "Error") {
        this.message.error(this.l('EsignErrorMessage'), this.l('Error Occurred While Signing')).then(() => {
          this.location.back();
        });
      } else if (decrypt == "Decline") {
        this.message.error(this.l('EsignErrorMessage'), this.l('Decline Signing')).then(() => {
          this.location.back();
        });
      } else if (decrypt == "Later") {
        this.message.error(this.l('EsignErrorMessage'), this.l('Later Signing')).then(() => {
          this.location.back();
        });
      }
    }
  }
  @HostListener('window:storage', ['$event'])
  onEvent(event) {

    if (localStorage.getItem('homeOpened') == 'false') {
      window.close();
    }
  }

  ngOnInit(): void {
    debugger;
    console.log(this.renderer);
    var queryParams = this.person;
    this.inputPerson = this.person;
    this.LoadData();
    this.getEscrowUserList();
    this.getEscrowOfficerDetails()

    if (localStorage.getItem('escrowOpened') == queryParams['e']) {
      localStorage.setItem('notab', 'false');
    }
    var title = atob(queryParams['e']);
    var userType = atob(queryParams['u']);
    this.useridd = abp.session.userId.toString();
    let test = queryParams['u'];
    let test1 = queryParams['token'];

    let subCompanyName = this.validFileName(atob(queryParams['sc']))
    let companyName = this.validFileName(atob(queryParams['c']))
    let EscrowTab = localStorage.getItem("activeTab")
    let escrowUserType = localStorage.getItem("accessTYpe" + EscrowTab);
    this.completeEnterprisePathOther = `${companyName}/${subCompanyName}/${EscrowTab}/Other/`
    this.completeEnterprisePathMain = `${companyName}/${subCompanyName}/${EscrowTab}`

    // Enterprise/Enterprise/ESCROW/Other/

    if (abp.session.userId === 1) { }
    else {
      if (test === undefined && test1 === undefined && FileViewComponent.newusertype === undefined) {
        alert("!Oops you can't go directly fileview page please go back to the dashboard and come through the blue button link")
        this._router.navigate(['/app/main/Userdashboard']);
      }
    }

    if (test != null && this.Action === undefined) {
      this.userTypes = [];
      if (FileViewComponent.newusertype === undefined) {
        FileViewComponent.newusertype = atob(queryParams['u']);
      }
      var selectedAccesstype = localStorage.getItem('accessTYpe' + atob(queryParams['e']))
      if (selectedAccesstype != undefined || selectedAccesstype != null) {
        this.Action = selectedAccesstype;
        this.UsertypeModel = selectedAccesstype;
      }
      else if (userType && selectedAccesstype == undefined) {
        this.Action = userType;
        this.UsertypeModel = userType;
      } else {
        this.Action = FileViewComponent.newusertype;
        this.UsertypeModel = FileViewComponent.newusertype;
      }
      FileViewComponent.escrowno = atob(queryParams['e']);
      this.escrowname = FileViewComponent.escrowno;
      this.Name = this.appSession.user.name + " " + this.appSession.user.surname;
      this.tempsubcompany = queryParams['sc'];
      this.tempcompany = queryParams['c'];
      if (test1 == undefined) {
        FileViewComponent.pathname = AppConsts.appBaseUrl + "/app/main/File?u=" + queryParams['u'] + "&e=" + queryParams['e'] + "&c=" + queryParams['sc'] + "";
      }
      FileViewComponent.currentpathh = this.validFileName(atob(queryParams['c'])) + "/" + this.validFileName(atob(queryParams['sc'])) + "/" + atob(queryParams['e']);
      FileViewComponent.currentpathh1 = this.validFileName(atob(queryParams['c'])) + "/" + this.validFileName(atob(queryParams['sc'])) + "/" + atob(queryParams['e']) + "/Other";
      this.currentpath = FileViewComponent.currentpathh;
      this.currentpath1 = FileViewComponent.currentpathh1;
    }
    else {
      this.userTypes = [];
      if (userType) {
        this.Action = userType;
        this.UsertypeModel = userType;
      } else {
        this.Action = FileViewComponent.newusertype;
        this.UsertypeModel = FileViewComponent.newusertype;
      }
      this.escrowname = FileViewComponent.escrowno;
      this.Name = this.appSession.user.name + " " + this.appSession.user.surname;
    }
    if (abp.session.userId === 1) { }
    else {
      this.show(this.useridd);
    }
    let url = AppConsts.remoteServiceBaseUrl;
    console.log("File List: FileSystemProvider  started for remoteProvider")
    this.remoteProvider = new FileSystemProvider({
      endpointUrl: url + "/FileManager/FileSystem?&company=" + this.validFileName(atob(queryParams['c'])) + "&subCompany=" + this.validFileName(atob(queryParams['sc'])) + "&escrow=" + FileViewComponent.escrowno + "&useriD=" + abp.session.userId + "&usertype=" + userType + "&usersname=" + this.appSession.user.name + " " + this.appSession.user.surname

    });
    console.log("File List: FileSystemProvider  started for remoteProvider1")
    // this.remoteProvider1 = new FileSystemProvider({
    //   endpointUrl: url + "/FileManager/FileSystem1?&company=" + this.validFileName(atob(queryParams['c'])) + "&subCompany=" + this.validFileName(atob(queryParams['sc'])) + "&escrow=" + FileViewComponent.escrowno + "&useriD=" + abp.session.userId
    // });
    console.log("File List:   method check()")

    this.folderPath = url + '/FileManager/';
    this.myurl = url;
    console.log("File List:   folderPath" + this.folderPath)
    console.log("File List:   myurl" + this.myurl)
  }
  filess: any = [];

  check(): void {
    setTimeout(() => {
      try {
        console.log("Count method started ")
        this.itemCount = this.fileManager.instance["_itemView"]._itemCount;
        this.itemCount1 = this.fileManager1.instance["_itemView"]._itemCount;
      } catch (error) {
        console.log("error in count log" + JSON.stringify(error))
      }
    }, 1000)
    console.log(new Date() + " Escrow No:" + this.escrowname + " User Name:" + this.Name + " User Type:" + this.Action + " Company:" + atob(this.tempcompany) + " Sub Company:" + atob(this.tempsubcompany));
  }

  show(elem): void {

    this._escrowDetailsServiceProxy.getAll(undefined, undefined, this.appSession.user.emailAddress, undefined,
      undefined, undefined, undefined, undefined, undefined, 100000).subscribe(result => {
        this.output = [];
        this.eFilter = [];
        let split = [];
        this.eFilter = result['items'];
        for (let i = 0; i < result['items'].length; i++) {
          this.output = this.eFilter[i];
          this.output = this.output['escrowDetail'];
          this.collect = this.output['escrowId'];
          this.collection = this.output['company'];
          this.collect1 = this.output['usertype'];
          split = this.collect1.split(',');

          for (let i = 0; i < split.length; i++) {
            let find = this.userTypes.filter(x => x == split[i]);
            if (find.length == 0) {
              this.userTypes.push(split[i]);
            }
            console.log(this.userTypes);
          }
        }
      });
  }
  handleShowingEvent(e) {
  }
  isRename: boolean = false;

  handleShownEvent(e) {
    debugger;
    this.readPermission = false;
    this.editPermission = false;
    this.viewHistoryPermission = false;
    this.renamePermission = false
    this.downloadPermission = false;
    this.viewFullNamePermission = false;
    this.deletePermission = false;
    this.esignPermission = false;
    this.renameFileName = false;
    this.reminderPermission = true;

    let accesstype = e.access
    if (accesstype.includes("R")) {
      this.readPermission = true;
      this.viewHistoryPermission = true;
      this.downloadPermission = true;
      this.viewFullNamePermission = true;

    }
    if (accesstype.includes("E")) {
      this.readPermission = true;
      this.editPermission = true
      this.viewHistoryPermission = true;
      this.downloadPermission = true;
      this.viewFullNamePermission = true;
      this.renamePermission = true;
      this.renameFileName = false;
    }
    if (accesstype.includes("A")) {
      this.readPermission = true;
      this.viewHistoryPermission = true;
      this.downloadPermission = true;
      this.viewFullNamePermission = true;
    }
    if (accesstype.includes("D")) {
      this.readPermission = true;
      this.editPermission = true;
      this.viewHistoryPermission = true;
      this.renamePermission = true
      this.downloadPermission = true;
      this.viewFullNamePermission = true;
      this.deletePermission = true;
      this.renameFileName = false;
    }
    if (accesstype.includes("S")) {

      this.esignPermission = true;
    }

    let status = e.status;


    if (status == 'Nobody signed yet' || status == null || status == 'Input Incomplete') {
      this.isRename = true;
    }
    else {
      this.isRename = false;
    }
    let signing = e.signing;
    if (signing != "Unsigned") {

      this.editPermission = false;
      this.renamePermission = false;
      this.renameFileName = true;
    } else {
      this.renamePermission = true;
      this.renameFileName = false;
    }
  }

  handelSaveMainFile(e) {
    debugger;
    this.eSign = false;
    this.mainFileSelected = e.selectedFile
    this.handleShownEvent(e.selectedFile)
    if (e.templateRef == "Rename") {
      this.parentpath = e.folderPath
      this.openRenameModalFromMain(e.selectedFile);
    }
    if (e.templateRef == "ViewFullName") {
      this.openFullNameFromMain(e.selectedFile)
    }
    if (e.templateRef == "ViewHistory") {
      this.openHistoryFromMain(e.selectedFile);
    }
    if (e.templateRef == "SendReminderMessage") {
      this.openReminderpopUpFromMain(e.selectedFile);
    }
    if (e.templateRef == "esign") {
      this.eSign = true;
      this.openEsignpopUpFromMain(e.selectedFile);
    }
    if (e.templateRef == "esignDetails") {
      this.openEsignDetailsPopUpFromMain(e.selectedFile);
    }
    if (e.templateRef == "view") {
      //  this.openEsignpopUpFromMain(e.selectedFile);
      this.View(this.esignModelMainTemplateRef, e.selectedFile);
    }
    if (e.templateRef == "viewEdit") {
      this.openEsignpopUpFromMain(e.selectedFile);
    }
    else if (e.templateRef == "delete") {  // You can define this condition as needed
      // Call the delete function here
      this.Delete2(e);  // Pass 'e' to the Delete function if needed
    }
  }

  // handelSaveOtherFile(e) {
  //   debugger;
  //   if (e.templateRef == "Rename1") {
  //     this.parentpath = e.folderPath
  //     this.openrenameModal1(e,e.selectedFile);
  //   } 
  //   if (e.templateRef == "ViewFullName") {
  //     this.openFullNameFromMain(e.selectedFile)
  //   }    

  //   if (e.templateRef == "viewEdit") {
  //     this.openEsignpopUpFromMain(e.selectedFile);
  //   }
  //   else if (e.templateRef == "delete") {  // You can define this condition as needed
  //     // Call the delete function here
  //     this.Delete2(e);  // Pass 'e' to the Delete function if needed
  //   }
  // }

  // openEsignpopUpFromMain(file) {
  //   this.e_Sign(this.esignModelMainTemplateRef);
  // this.modalRef = this.modalService.show(
  //   this.esignModelMainTemplateRef,
  //   Object.assign({}, { class: 'gray modal-xl', backdrop: false, ignoreBackdropClick: true })
  // );
  //}


  openEsignpopUpFromMain(file) {
    // this.modalRef = this.modalService.show(
    //   this.esignModelMainTemplateRef,
    //   Object.assign({}, { class: 'gray modal-xl', backdrop: false, ignoreBackdropClick: true })
    // );

    this.selectedFileForEsign = file.key;
    this.e_Sign(this.esignModelMainTemplateRef, file);
  }

  openEsignDetailsPopUpFromMain(file) {
    debugger
    this.displayImagePopupFromMain(file);
  }

  onItemClick(e) {
    debugger;
    if (e.itemData.options.download) {
      this.Download(e);
    }
    else if (e.itemData.options.Rename) {
      this.openrenameModal(e.itemData.options.Rename);
      // this.Rename(e);
    }
    else if (e.itemData.options.view) {
      //this.View(e.itemData.options.view);
    }
    else if (e.itemData.options.full) {
      //this.ViewFullName(e,);
      this.openFullName(e, e.itemData.options.full)
    }
    else if (e.itemData.options.renameFileName) {
      this.openRenameName(e, e.itemData.options.renameFileName)
    }
    else if (e.itemData.options.esign) {

      //this.e_Sign(e.itemData.options.esign);
    }
    else if (e.itemData.options.esignDetails) {
      let file = this.fileManager.instance.getSelectedItems();
      this.displayImagePopupFromMain(file[0]);
    }
    else if (e.itemData.options.history) {
      this.openHistoryModal(e.itemData.options.history);
    }
    else if (e.itemData.options.delete) {
      this.Delete(e);
    }
    else if (e.itemData.options.reminder) {
      this.openReminderpopUp(e.itemData.options.reminder)
    }
  }
  private convertFilesToBase64(files: any[]): Promise<string[]> {
    const promises = files.map(file => this.convertFileToBase64(file.dataItem));
    return Promise.all(promises);
  }

  openReminderpopUp(reminder: TemplateRef<any>) {
    this.reminderResponse = [];
    this.reminderUserList = [];
    this.reminderDto.reminderText = "";
    this.noUserMessage = ""
    this.toMessage = false;
    this.email = false;
    this.directMessage = false;
    let dir = this.fileManager.instance.getSelectedItems();
    let fileId = dir[0].dataItem.srAssignedFileId;
    let escrow = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + escrow);
    this._srEscrowFileMastersServiceProxy.getFileUserList(fileId, userType, escrow).subscribe((response: any) => {
      this.reminderUserList = response;
      if (!this.reminderUserList.length) {
        this.noUserMessage = "This file is not assigned to the Escrow Officer. You cannot send the notification."
      }
      this.reminderMessageResponse = false;
      this.reminderMailResponse = false;
      this.reminderDirectMessageResponse = false;
      this.modalRef = this.modalService.show(
        reminder,
        Object.assign({}, { class: 'gray modal-xl', backdrop: false, ignoreBackdropClick: true })
      );
    })
  }

  openReminderpopUpFromMain(file) {
    this.reminderResponse = [];
    this.reminderUserList = [];
    this.reminderDto.reminderText = "";
    this.noUserMessage = ""
    this.toMessage = false;
    this.email = false;
    this.directMessage = false;

    let fileId = file.srAssignedFileId;
    let escrow = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + escrow);
    this._srEscrowFileMastersServiceProxy.getFileUserList(fileId, userType, escrow).subscribe((response: any) => {
      this.reminderUserList = response;
      if (!this.reminderUserList.length) {
        this.noUserMessage = "This file is not assigned to the Escrow Officer. You cannot send the notification."
      }
      this.reminderMessageResponse = false;
      this.reminderMailResponse = false;
      this.reminderDirectMessageResponse = false;
      this.modalRef = this.modalService.show(
        this.reminderModelMainTemplateRef,
        Object.assign({}, { class: 'gray modal-xl', backdrop: false, ignoreBackdropClick: true })
      );
    })
  }

  private convertFileToBase64(file: any): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      const blob = new Blob([file.content], { type: file.mimeType });
      reader.readAsDataURL(blob);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
    });
  }

  base64Files: any;

  private getFileBase64(file: any): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      const blob = new Blob([file.content], { type: file.mimeType });
      reader.readAsDataURL(blob);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
    });
  }

  handleShownEventOtherArea(e) {

    let fileName = e.selectedItems[0].dataItem.name
    if (fileName.includes('.pdf')) {
      this.editPermissionOtherArea = true;
      this.readPermissionOtherArea = false;
      this.editPermissionDocOtherArea = false;
    }
    else if (fileName.includes('.docx') || fileName.includes('.doc')) {
      this.editPermissionOtherArea = true;
      this.readPermissionOtherArea = false;
      this.editPermissionDocOtherArea = true;
    }
    else {
      this.editPermissionOtherArea = false;
      this.readPermissionOtherArea = true;
      this.editPermissionDocOtherArea = false;
    }
  }

  HideOtherAreaFileAction() {
    debugger;
    this.check();
    this.modalReff.hide();

    if (this.editPermissionOtherArea) {
      if (confirm("Do you want to Exit Viewer Without Saving?")) {
        this.modalReff.hide();
      }
      else {
      }
    }
    else {
      this.modalReff.hide();
    }
  }

  handleSave(event) {
    debugger;
    let fileName = event.selectedFile.name
    this.otherFileSelected = event.selectedFile;
    if (fileName.includes('.pdf')) {
      this.editPermissionOtherArea = true;
      this.readPermissionOtherArea = false;
      this.editPermissionDocOtherArea = false;
    }
    else if (fileName.includes('.docx') || fileName.includes('.doc')) {
      this.editPermissionOtherArea = true;
      this.readPermissionOtherArea = false;
      this.editPermissionDocOtherArea = true;
    }
    else {
      this.editPermissionOtherArea = false;
      this.readPermissionOtherArea = true;
      this.editPermissionDocOtherArea = false;
    }
    this.onItemClick1(event);

  }

  onItemClick1(e) {
    try {
      debugger;
      if (e.templateRef == 'fownload') {
        this.Download1(e);
      }
      else if (e.templateRef == 'view1') {
        this.DownloadFileForDocViwerOther(e);
        // this.ViewEditFromOther(e.selectedFile);
        // this.DownloadFileForDocViwer(e.selectedFile);
        // this.View1(e.itemData.options.view);
      }
      else if (e.templateRef == 'renameFile') {
        this.openrenameModal1(e.selectedFile, this.renameTemplateRef);
      }
      else if (e.templateRef == 'Move') {
        this.parentpath = e.folderPath;
        this.openMoveModal(e.selectedFile, this.moveTemplateRef);
      }
      else if (e.templateRef == 'FullName1') {
        // this.ViewFullName(e);
        this.openFullName1(e.selectedFile, this.fullNameTemplateRef);
      }

      if (e.templateRef == 'deleteFile') {
        this.Delete1(e);
      }

      if (e.templateRef == 'Rename1') {
        this.openrenameModal1(e.itemData.options.Rename1, null);
      }

      // Additional conditions (currently commented out):
      // if (e.itemData.options.Move) {
      //   this.openMoveModal(e.selectedFile, e.itemData.options.Move);
      // }
      // if (e.itemData.options.Tags) {
      //   this.escrowUsertagsComponent.show();
      // }
    } catch (error) {
      console.error('Error in onItemClick1:', error);

    }
  }



  selectChangeHandler(event) {
    var queryParams = this.person;
    const Key = 'accessTYpe' + atob(queryParams['e']);
    localStorage.setItem(Key, this.UsertypeModel)
    FileViewComponent.newusertype = this.UsertypeModel;
    let url = AppConsts.remoteServiceBaseUrl;

    FileViewComponent.escrowno = atob(queryParams['e']);
    this.remoteProvider = new FileSystemProvider({
      endpointUrl: url + "/FileManager/FileSystem?&company=" + this.validFileName(atob(queryParams['c'])) + "&subCompany=" + this.validFileName(atob(queryParams['sc'])) + "&escrow=" + FileViewComponent.escrowno + "&useriD=" + abp.session.userId + "&usertype=" + this.UsertypeModel + "&usersname=" + this.appSession.user.name + " " + this.appSession.user.surname
    });
    setTimeout(() => {
      this.fileManager.instance.refresh().done((result) => {
        this.check();
      })
        .fail(function (error) {
        });
    }, 2000);
  }

  validFileName(folderName) {
    let newString = folderName.replace("<", "(").replace(">", ")").replace(":", ";").replace("*", "'").replace("/", "-").replace("?", "+").replace("|", "_").replace("*", ".").replace("\/", "=");
    let str = newString.charAt(newString.length - 1)
    if (str == ".") {
      newString = newString.replace(str, "");
    }
    return newString
  }

  uploadFile(filess) {
    debugger;
    for (let index = 0; index < filess.length; index++) {
      var element = filess[index];
      this.filess.push(element.name);
      this.file = filess
      this.isFile = true;
      const sanitizedFileName = this.sanitizeFileName(this.file[0].name);
      this.fileName = sanitizedFileName
      if (this.currentpath1 == "") {
        alert("Other file upload restricted due to zero file in main fileview");
      } else {
        const oldPath = this.currentpath1;
        var uploadPath = this.currentpath1;

        localStorage.setItem('escrowNewID', this.escrowname);

        let uploadpath1 = uploadPath.replace("/", "\\");

        for (var i = 0; i < this.file.length; i++) {

          var fileToUpload = <File>this.file[i];
          const sanitizedFileName = this.sanitizeFileName(fileToUpload.name);
          var fileName = sanitizedFileName

          // var fileName = fileToUpload.name;
          const formData = new FormData();
          var startIndex = this.fileName.indexOf("~");
          if (startIndex > 0) {
            const fileExtension = this.getFileExtension(this.fileName);
            //  fileName =  fileToUpload.name.slice(0,startIndex)+'.'+ fileExtension;
          }
          console.log("-------------" + this.folderPath + "ProcessRequest?path=" + uploadpath1 + "&useriD=" + abp.session.userId)

          formData.append('file', fileToUpload, fileName);
          this.http.post(this.folderPath + "ProcessRequest?path=" + uploadpath1 + "&useriD=" + abp.session.userId, formData, { reportProgress: true, observe: 'events' })
            .subscribe((res) => {
              debugger;
              let fileRes: any = res;
              try {
                let newResult = fileRes.body.result
                if (newResult.statusCode == 500) {
                  this.spinnerUpl = false;
                  this.file = null
                  this.isFile = false;
                  this.fileName = 'Select a file'
                  alert("This name file is already exists please change file name");
                  abp.notify.error('File already exists', 'Error');
                }
                if (newResult.statusCode == 200) {
                  this.spinnerUpl = false;
                  this.file = null
                  this.isFile = false;
                  this.fileName = 'Select a file'

                  abp.notify.success('File Uploaded Successfully', 'Success');
                  this.currentpath1 = oldPath;
                  let checkFirstFile = this.fileManager1.instance.getCurrentDirectory();
                  this.fileOtherComponent.getAllFiles();
                  console.log("checkFirstFile.path" + checkFirstFile.path)
                  if (checkFirstFile.path == "") {

                    var queryParams = this.person;
                    var userType = atob(queryParams['u']);
                    let url = AppConsts.remoteServiceBaseUrl;
                    console.log("File List: FileSystemProvider  started for remoteProvider")
                    this.remoteProvider = null;
                    this.remoteProvider1 = null;
                    setTimeout(() => {
                      console.log("File List: FileSystemProvider  started for remoteProvider1")
                      this.remoteProvider1 = new FileSystemProvider({
                        endpointUrl: url + "/FileManager/FileSystem1?&company=" + this.validFileName(atob(queryParams['c'])) + "&subCompany=" + this.validFileName(atob(queryParams['sc'])) + "&escrow=" + FileViewComponent.escrowno + "&useriD=" + abp.session.userId
                      });
                      this.ngAfterViewInit();
                    }, 1000)
                    this.ngOnInit();
                  }

                  this.fileManager1.instance.refresh().done((result) => {
                    this.check();
                  })
                    .fail(function (error) {
                      // handle error
                    });
                  console.log("test2");

                }

              }
              catch (error) { }
              this.fileInputVariable.nativeElement.value = "";

            });
          // this.http.post(this.folderPath + "ProcessRequest?path=" + uploadpath1 + "&useriD=" + abp.session.userId, formData, {
          //   reportProgress: true,
          //   observe: 'events'
          // }).subscribe({
          //   next: (event: HttpEvent<any>) => {
          //     switch (event.type) {
          //       case HttpEventType.Sent:
          //         console.log('Upload request sent!');
          //         break;

          //       case HttpEventType.UploadProgress:
          //         if (event.total) {
          //           const percentDone = Math.round(100 * event.loaded / event.total);
          //           console.log(`File is ${percentDone}% uploaded.`);
          //         }
          //         break;

          //       case HttpEventType.Response:
          //         debugger;
          //         console.log('Upload complete. Response:', event.body);
          //         break;
          //     }
          //   },
          //   error: (err) => {
          //     console.error('Upload error:', err);
          //   }
          // });
          this.deleteAttachment(0);
        }
        this.sourcepath = "";
        this.file = "";
      }
    }

  }
  deleteAttachment(index) {
    this.filess.splice(index, 1)
  }
  select(evt) {
    debugger;
    var files = evt.target.files;
    var file = files[0];
    if (files && file) {
      this.file = files
      this.isFile = true;
      const sanitizedFileName = this.sanitizeFileName(this.file[0].name);
      this.fileName = sanitizedFileName
    }
    this.uploadFile(files);

  }
  sanitizeFileName(fileName) {
    const sanitizedFileName = fileName.replace(/[&?=!@$%^+]/g, '_');
    const now = new Date();
    const dateTimeStamp = now.toISOString().replace(/[:.]/g, '-');
    const fileExtension = sanitizedFileName.substring(sanitizedFileName.lastIndexOf('.'));
    const baseName = sanitizedFileName.substring(0, sanitizedFileName.lastIndexOf('.'));
    return `${baseName}_${dateTimeStamp}${fileExtension}`;
  }

  // displayImagePopupFromMain(file) {
  //   debugger;
  //   if (file.name) {
  //     if (!file.name.includes(this.myurl)) {
  //       this.Sign = [];
  //       let source = this.myurl + "/Common/Paperless/" + file.name;
  //       let strng = file.key.replace(/#/g, "%23");
  //       var escrow = localStorage.getItem("activeTab");
  //       this.http.get(this.folderPath + "GetSignDetails?type=" + this.appSession.user.emailAddress + "&filename=" + strng + "&Escrow=" + escrow).subscribe((response: any) => {

  //         let data = response['result'];
  //         if (data.length > 0) {
  //           this.Sign = [];
  //           for (let i = 0; i < data.length; i++) {
  //             this.Sign.push(data[i]);
  //           }
  //           this.signPopup = true;
  //           document.body.style.backgroundColor = '#d3d3d3';
  //         }
  //       });
  //     }
  //   }
  // }

  displayImagePopupFromMain(file) {
    debugger;
    const mainContainer = document.querySelector('.page');
    if (mainContainer) {
      (mainContainer as HTMLElement).style.backgroundColor = 'black';
    }
    if (file.name && !file.name.includes(this.myurl)) {
      this.Sign = [];
      const source = this.myurl + "/Common/Paperless/" + file.name;
      const strng = file.key.replace(/#/g, "%23");
      const escrow = localStorage.getItem("activeTab");

      this.http.get(this.folderPath + "GetSignDetails?type=" + this.appSession.user.emailAddress + "&filename=" + strng + "&Escrow=" + escrow)
        .subscribe((response: any) => {
          const data = response['result'];
          if (data.length > 0) {
            this.Sign = data;
            this.signPopup = true;
          }
        });
    }
  }

  wvDocumentLoadedHandler(): void {
    debugger;
    if (!this.wvInstance) {
      console.error('wvInstance is not initialized');
      return;
    }

    const docViewer = this.wvInstance;
    const annotManager = docViewer.annotManager;
    const { Annotations } = docViewer;
    const rectangle = new Annotations.RectangleAnnotation();
    rectangle.PageNumber = 1;
    rectangle.X = 100;
    rectangle.Y = 100;
    rectangle.Width = 250;
    rectangle.Height = 250;
    rectangle.StrokeThickness = 5;
    rectangle.Author = annotManager.getCurrentUser();
    annotManager.addAnnotation(rectangle);
    annotManager.drawAnnotations(rectangle.PageNumber);
  }

  openFullName(e, full: TemplateRef<any>) {

    this.modalRef = this.modalService.show(
      full,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );
    let dir = this.fileManager.instance.getSelectedItems();
    this.fileFullName = dir[0].key;
  }

  openFullNameFromMain(file) {

    this.modalRef = this.modalService.show(
      this.fullNameMainTemplateRef,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );

    this.fileFullName = file.key;
  }

  fileAccessName: string = ''
  parentPath: string = ''
  openRenameName(e, renameFileName: TemplateRef<any>) {

    this.modalRef = this.modalService.show(
      renameFileName,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );
    let dir = this.fileManager.instance.getSelectedItems();
    this.fileFullName = dir[0].key;
    if (this.fileFullName) {
      let fileName = this.fileFullName.split("~");
      this.fileFullName = fileName[0];
      this.fileAccessName = fileName[1]
      this.parentPath = dir[0].parentPath
    }
  }

  saveRenameFileName() {
    debugger;
    let dir = this.fileManager.instance.getSelectedItems();
    let Id = dir[0].dataItem.srAssignedFileId;
    let fileFullName = this.fileFullName + "~" + this.fileAccessName;
    let data: RenameModel = new RenameModel();
    data.id = Id;
    data.newFileName = fileFullName;
    data.parentPath = this.parentPath;
    this._srAssignedFilesDetailsServiceProxy.renameFileName(data).subscribe(result => {
      this.modalRef.hide();
      this.fileManager.instance.refresh().done((result) => {
        this.check();
      })
        .fail(function (error) {
        });
    })
  }

  openFullName1(selectedFile: any, FullName1: TemplateRef<any>) {
    debugger;
    this.modalRef = this.modalService.show(FullName1, {
      class: 'gray modal-lg',
    });

    this.fileFullName = selectedFile.key;
  }

  ViewFullName($event) {
    let dir = this.fileManager.instance.getSelectedItems();
    this.fileFullName = dir[0].key;
    this.fullnamepopup = true;
    setTimeout(() => {
      this.fullnamepopup = false;

    }, 2000)
  }

  ViewFullName1($event) {
    let dir = this.fileManager1.instance.getSelectedItems();
    this.fileFullName1 = dir[0].key;
    this.fullnamepopup1 = true;
    setTimeout(() => {
      this.fullnamepopup1 = false;

    }, 2000)
  }

  View(popupview: TemplateRef<any>, selectedFile: any) {
    // Reset flags and variables
    this.docx = false;
    this.eSign = false;
    this.msgShow = false;

    // Clear any previous viewer content
    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }

    // Modal configuration
    let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };

    let dir = selectedFile;

    console.log('Selected Items:', dir);

    if (dir != null) {
      let item = dir; // Using the first (and only) file from dir
      this.fileNamestrng = item['key'].replace(/#/g, "%23"); // Ensure file name is encoded
      this.sourcepathwithname = this.completeEnterprisePathMain + "/" + this.fileNamestrng;

      // Access check: Assuming you have access control like in the previous code
      if (!item.access.includes("R")) {
        alert("This file doesn't have read access");
        return;
      }

      // Add salt for cache busting
      const salt = (new Date()).getTime();
      let url = AppConsts.appBaseUrl;

      // Make the HTTP request to update document status (if needed)
      this.http.get(this.folderPath + "DocUpdate?message=Read&filename=" + this.fileNamestrng + "&userId=" + this.appSession.userId).subscribe((response: any) => {
        let data = response['result'];

        // Construct the file source URL with cache-busting
        this.source = url + "/docs/Paperless/" + this.sourcepathwithname + "?" + salt;
        localStorage.setItem("SourcePath", this.source); // Store source path in localStorage for use in the modal

        // Show the modal with the viewer
        this.modalReff = this.modalService.show(popupview, config);
      });
    }
    this.replacedstring = "";
  }

  View1(temp: TemplateRef<any>) {

    this.eSign = false;
    let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
    let dir = this.fileManager1.instance.getSelectedItems();

    if (dir.length > 0) {
      if (document.getElementById('viewer')) {
        document.getElementById('viewer').remove();
      }
      if (document.getElementById('headerH')) {
        document.getElementById('headerH').remove();
      }
      for (let i = 0; i < dir.length; i++) {
        let item = dir[i];
        if (item['key'].includes(".pdf")) {
          this.docx = false;
          this.msgShow = true;
          this.sourcepathwithname = item['parentPath'] + "/" + item['key'];
          let strng = this.sourcepathwithname.replace(/#/g, "%23");
          const salt = (new Date()).getTime();
          let url = AppConsts.appBaseUrl;
          this.source = url + "/docs/Paperless/" + strng + "?" + salt;
          localStorage.setItem("SourcePath", this.source)
          this.modalReff = this.modalService.show(
            temp, config
          );
        } else if (item['key'].includes(".msg")) {
          this.msgShow = true;
          this.docx = false;
          this.msgFunction();
          this.modalReff = this.modalService.show(
            temp, config
          );
        }
        else {
          this.docx = true;
          this.msgShow = false;
          this.sourcepathwithname = item['parentPath'] + "/" + item['key'];
          let strng = this.sourcepathwithname.replace(/#/g, "%23");
          const salt = (new Date()).getTime();
          let url = AppConsts.appBaseUrl;
          this.source = url + "/docs/Paperless/" + strng + "?" + salt;
          localStorage.setItem("SourcePath", this.source)
          this.modalReff = this.modalService.show(temp, config);

          // Remove previous instances of viewer and header
          if (document.getElementById('viewer')) {
            document.getElementById('viewer').remove();
          }
          if (document.getElementById('headerH')) {
            document.getElementById('headerH').remove();
          }

          // Wait for the document to be fully loaded before executing the handler
          this.wvDocumentLoadedHandler = this.wvDocumentLoadedHandler.bind(this);

          // Optional: Use a setTimeout to delay and ensure document is loaded
          setTimeout(() => {
            console.log("wvDocumentLoadedHandler called");
            this.wvDocumentLoadedHandler();
          }, 1000); // Adjust delay as necessary

        }

      }
    }

    this.replacedstring = "";
  }

  ViewEditFromOther(selectedfile: any) {
    this.eSign = false;
    let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };

    // Assume selectedfile is already the single file object
    if (selectedfile) {
      // Remove previous viewer and header if they exist
      if (document.getElementById('viewer')) {
        document.getElementById('viewer').remove();
      }
      if (document.getElementById('headerH')) {
        document.getElementById('headerH').remove();
      }

      // Check file extension and handle accordingly
      if (selectedfile['key'].includes(".pdf")) {
        this.docx = false;
        this.msgShow = true;
        this.sourcepathwithname = this.completeEnterprisePathOther + selectedfile['key'];
        let strng = this.sourcepathwithname.replace(/#/g, "%23");
        const salt = (new Date()).getTime();
        let url = AppConsts.appBaseUrl;
        this.source = url + "/docs/Paperless/" + strng + "?" + salt;
        localStorage.setItem("SourcePath", this.source);
        this.modalReff = this.modalService.show(this.otherAreaFileActionTemplate, config);
      } else if (selectedfile['key'].includes(".msg")) {
        this.msgShow = true;
        this.docx = false;
        this.msgFunction();
        this.modalReff = this.modalService.show(this.otherAreaFileActionTemplate, config);
      } else {
        this.docx = true;
        this.msgShow = false;
        this.sourcepathwithname = this.completeEnterprisePathOther + selectedfile['key'];
        let strng = this.sourcepathwithname.replace(/#/g, "%23");
        const salt = (new Date()).getTime();
        let url = AppConsts.appBaseUrl;
        this.source = url + "/docs/Paperless/" + strng + "?" + salt;
        localStorage.setItem("SourcePath", this.source);
        this.modalReff = this.modalService.show(this.otherAreaFileActionTemplate, config);

        // Remove previous instances of viewer and header
        if (document.getElementById('viewer')) {
          document.getElementById('viewer').remove();
        }
        if (document.getElementById('headerH')) {
          document.getElementById('headerH').remove();
        }

        // Wait for the document to be fully loaded before executing the handler
        this.wvDocumentLoadedHandler = this.wvDocumentLoadedHandler.bind(this);

        // Optional: Use a setTimeout to delay and ensure document is loaded
        setTimeout(() => {
          console.log("wvDocumentLoadedHandler called");
          this.wvDocumentLoadedHandler();
        }, 1000); // Adjust delay as necessary
      }
    }

    this.replacedstring = "";
  }

  loadpermission() {
    if (this.fileManager != undefined) {
      let selected = this.fileManager.instance.getSelectedItems();
    }
  }

  Download(event) {
    debugger
    this.items = this.fileManager.instance.getSelectedItems();
    let compare;
    let strcheck;
    this.items.forEach(ele => {
      let action = ele.key;
      if (action.includes("~")) {
        action = action.substring(action.indexOf("~") + 1);
        compare = this.Action;
        const paramsPattern = /[^{\}]+(?=})/g;
        let extractParams = action.match(paramsPattern);
        for (let i = 0; i < extractParams.length; i++) {
          let my = extractParams[i].replace("{", "");
          let my1 = my;
          my = my.substring(0, my.indexOf('-'));
          my1 = my1.substring(my1.indexOf('-') + 1);
          if (my == compare) {
            strcheck = my1;
          }
        }
      }
    });
    let bcheck;
    if (strcheck != undefined) {
      bcheck = strcheck.indexOf("R");
    }
    if (bcheck === -1 && strcheck != undefined && abp.session.userId != 1) {
      this.ErrorMessage = "!Oops you don't have rights to download this file";
      return;
    }
    else {
      if (this.items.length > 0) {
        this.items.forEach(element => {
          this.show(this.items[0].name)
          let path = element.path;
          let key = element.key;
          let strng = path.replace(/#/g, "%23");
          let strng1 = key.replace(/#/g, "%23");
          let srId = this.items[0].dataItem.srAssignedFileId
          this.downloadname = key;
          const token = 'my JWT';
          const headers = new HttpHeaders().set('authorization', 'Bearer' + token);

          var userId = abp.session.userId;
          this.http.get(this.folderPath + "DownloadFile" + "?path=" + strng + "&key=" + strng1 + "&srAssignedFileId=" + srId + "&userId=" + userId, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {

            let fileRes: any = response;
            let dataType = response.type;
            let binaryData = [];
            binaryData.push(response);
            let downloadLink = document.createElement('a');
            downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));

            if (this.downloadname.includes("~")) {
              this.downloadname = this.downloadname.replace("~", "~");
            }

            if (strng1) {
              let startIndex = this.downloadname.indexOf("~");
              if (startIndex > 0) {
                const fileExtension = this.getFileExtension(this.downloadname);
              }
            }

            downloadLink.setAttribute('download', this.downloadname);
            document.body.appendChild(downloadLink);
            downloadLink.click();

            try {
              let newResult = fileRes.status;
              if (newResult == 500) {

                this.spinnerUpl = false;
                this.file = null
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              if (newResult == 200) {
                alert("File downloaded successfully :)");
                this.spinnerUpl = false;
                this.file = null;
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              else {
              }
            }
            catch (error) {
              alert(error);
            }

          });
        });

      }
      else {
        this.ErrorMessage = "Oops you didn't selected any file";
      }
    }
  }

  getFileExtension(filename) {
    const extension = filename.split('.').pop();
    return extension;
  }

  DownloadFileForDocViwer(event) {
    debugger

    let compare;
    let
      strcheck;
    var ele = event.selectedFile;
    let action = ele.key;
    if (action.includes("~")) {
      action = action.substring(action.indexOf("~") + 1);
      compare = this.Action;
      const paramsPattern = /[^{\}]+(?=})/g;
      let extractParams = action.match(paramsPattern);
      for (let i = 0; i < extractParams.length; i++) {
        let my = extractParams[i].replace("{", "");

        let my1 = my;
        my = my.substring(0, my.indexOf('-'));
        my1 = my1.substring(my1.indexOf('-') + 1);
        if (my == compare) {
          strcheck = my1;
        }
      }
    }

    let bcheck;
    if (strcheck != undefined) {
      bcheck = strcheck.indexOf("D");
    }
    if (bcheck === -1 && strcheck != undefined && abp.session.userId != 1) {
      this.ErrorMessage = "!Oops you don't have rights to download this file";
      return;
    }
    else {
      if (event.selectedFile != null) {
        var element = event.selectedFile
        this.show(event.selectedFile.name)
        let path = element.path;
        let key = element.key;
        let strng = path.replace(/#/g, "%23");
        let strng1 = key.replace(/#/g, "%23");
        this.downloadname = key;
        const token = 'my JWT';
        const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
        this.globalService.folderPath = this.folderPath;
        this.globalService.oldPathSelectedFile = strng;
        this.http.get(this.folderPath + "ConvertFileToBase64" + "?path=" + strng + "&key=" + strng1).subscribe((response: any) => {
          let fileRes: any = response;
          if (fileRes.result.fileType == "eml") {
            this.emailContent = this.sanitizer.bypassSecurityTrustHtml(fileRes.result.base64);
            let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
            this.modalReff = this.modalService.show(this.viewEmlFileTemplate, config);
          }
          else if (fileRes.result.fileType === "msg") {
            const decodedContent = atob(fileRes.result.base64);
            this.emailContent = this.sanitizer.bypassSecurityTrustHtml(decodedContent);
            const config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
            this.modalReff = this.modalService.show(this.viewMsgFileTemplate, config);
          }
          else if (fileRes.result.fileType === "pdf") {
            const decodedContent = atob(fileRes.result.base64);
            this.emailContent = this.sanitizer.bypassSecurityTrustHtml(decodedContent);
            const config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
            this.modalReff = this.modalService.show(this.otherAreaFileActionTemplate, config);
          }
          else {
            this.globalService.docFile = fileRes.result.base64;
            this.View1(this.otherAreaFileActionTemplate);
          }
        });
      }
      else {
        this.ErrorMessage = "Oops you didn't selected any file";
      }
    }
  }

  DownloadFileForDocViwerOther(event) {
    debugger;
    let compare;
    let
      strcheck;
    var ele = event.selectedFile;
    let action = ele.key;
    if (action.includes("~")) {
      action = action.substring(action.indexOf("~") + 1);
      compare = this.Action;
      const paramsPattern = /[^{\}]+(?=})/g;
      let extractParams = action.match(paramsPattern);
      for (let i = 0; i < extractParams.length; i++) {
        let my = extractParams[i].replace("{", "");

        let my1 = my;
        my = my.substring(0, my.indexOf('-'));
        my1 = my1.substring(my1.indexOf('-') + 1);
        if (my == compare) {
          strcheck = my1;
        }
      }
    }

    let bcheck;
    if (strcheck != undefined) {
      bcheck = strcheck.indexOf("D");
    }
    if (bcheck === -1 && strcheck != undefined && abp.session.userId != 1) {
      this.ErrorMessage = "!Oops you don't have rights to download this file";
      return;
    }
    else {
      if (event.selectedFile != null) {
        var element = event.selectedFile
        this.show(event.selectedFile.name)

        let path = this.completeEnterprisePathOther;
        let key = element.key;
        // let strng = path.replace(/#/g, "%23");
        // let strng1 = key.replace(/#/g, "%23");
        // this.downloadname = key;
        // const token = 'my JWT';
        // const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
        // this.globalService.folderPath = this.folderPath;
        // this.globalService.oldPathSelectedFile = strng;
        // Ensure both path and key are valid

        // Ensure both path and key are valid
        if (!path || !key) {
          console.error('Path or key is missing');
          return;
        }

        // Replace '#' with '%23' for both path and key
        let strng = path.replace(/#/g, "%23");
        let strng1 = key.replace(/#/g, "%23");

        // Combine path and filename to form the complete file path with extension
        this.globalService.oldPathSelectedFile = `${strng}/${strng1}`;

        console.log('Full path with extension:', this.globalService.oldPathSelectedFile);


        this.http.get(this.folderPath + "ConvertFileToBase64" + "?path=" + strng + "&key=" + strng1).subscribe((response: any) => {

          let fileRes: any = response;

          if (fileRes.result.fileType == "eml") {

            this.emailContent = this.sanitizer.bypassSecurityTrustHtml(fileRes.result.base64);
            let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
            this.modalReff = this.modalService.show(this.viewEmlFileTemplate, config);

          }
          else if (fileRes.result.fileType === "msg") {
            const decodedContent = atob(fileRes.result.base64);
            this.emailContent = this.sanitizer.bypassSecurityTrustHtml(decodedContent);
            const config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
            this.modalReff = this.modalService.show(this.viewMsgFileTemplate, config);
          }

          else {
            this.globalService.docFile = fileRes.result.base64;
            this.ViewEditFromOther(event.selectedFile);
          }

        });


      }
      else {
        this.ErrorMessage = "Oops you didn't selected any file";
      }
    }

  }

  private binaryToBase64(blob: Blob): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onloadend = () => {
        if (reader.result) {
          resolve(reader.result as string);
        } else {
          reject('Failed to convert binary data to base64');
        }
      };

      reader.onerror = (error) => reject('Error reading file: ');

      reader.readAsDataURL(blob);
    });
  }

  Download1(event) {
    debugger;
    this.items = this.fileManager1.instance.getSelectedItems();
    let compare;
    let
      strcheck;
    this.items.forEach(ele => {
      let action = ele.key;
      if (action.includes("~")) {
        action = action.substring(action.indexOf("~") + 1);
        compare = this.Action;
        const paramsPattern = /[^{\}]+(?=})/g;
        let extractParams = action.match(paramsPattern);
        for (let i = 0; i < extractParams.length; i++) {
          let my = extractParams[i].replace("{", "");

          let my1 = my;
          my = my.substring(0, my.indexOf('-'));
          my1 = my1.substring(my1.indexOf('-') + 1);
          if (my == compare) {
            strcheck = my1;
          }
        }
      }
    });

    let bcheck;
    if (strcheck != undefined) {
      bcheck = strcheck.indexOf("D");
    }
    if (bcheck === -1 && strcheck != undefined && abp.session.userId != 1) {
      this.ErrorMessage = "!Oops you don't have rights to download this file";
      return;
    }

    else {
      if (this.items.length > 0) {
        this.items.forEach(element => {
          this.show(this.items[0].name)
          let path = element.path;
          let key = element.key;
          let strng = path.replace(/#/g, "%23");
          let strng1 = key.replace(/#/g, "%23");
          this.downloadname = key;
          const token = 'my JWT';
          const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
          var userId = abp.session.userId;
          let srId = this.items[0].dataItem.srAssignedFileId
          this.http.get(this.folderPath + "DownloadFile" + "?path=" + strng + "&key=" + strng1 + "&srAssignedFileId=" + srId + "&userId=" + userId, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {

            let fileRes: any = response;
            let dataType = response.type;
            let binaryData = [];
            binaryData.push(response);
            let downloadLink = document.createElement('a');
            downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));
            if (strng1)
              downloadLink.setAttribute('download', this.downloadname);
            document.body.appendChild(downloadLink);
            downloadLink.click();
            try {
              let newResult = fileRes.status;
              if (newResult == 500) {
                this.spinnerUpl = false;
                this.file = null
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              if (newResult == 200) {
                alert("File downloaded successfully :)");
                this.spinnerUpl = false;
                this.file = null;
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              else {
              }

            }
            catch (error) {
              alert(error);
            }

          });
        });

      }
      else {
        this.ErrorMessage = "Oops you didn't selected any file";
      }
    }
  }

  addItem(index) {
    this.userpermissionsall.push(this.newAttribute);
    this.newAttribute = {};
  }

  deleteItem(index) {
    this.userpermissionsall.splice(index, 1)
  }

  selectAll(index: number): void {
    const selectedPermission = this.userpermissionsall[index];
    selectedPermission.selectAll = !selectedPermission.selectAll;
    selectedPermission.R = selectedPermission.selectAll;
    selectedPermission.E = selectedPermission.selectAll;
    selectedPermission.A = selectedPermission.selectAll;
    selectedPermission.D = selectedPermission.selectAll;
    selectedPermission.S = selectedPermission.selectAll;
  }
  selectAllup(index: number): void {
    const selectedPermission = this.userpermissionsall[index];
    selectedPermission.selectAll = !selectedPermission.selectAll;
    selectedPermission.R = selectedPermission.selectAll;
    selectedPermission.E = selectedPermission.selectAll;
    selectedPermission.A = selectedPermission.selectAll;
    selectedPermission.D = selectedPermission.selectAll;
    selectedPermission.S = selectedPermission.selectAll;
  }

  ddldata(test: any, index, val: any) {

    if (this.userpermissionsall.length > 1) {
      var checkSelectedUserType = this.userpermissionsall.find(x => x.first && x.first.includes(test));
      if (checkSelectedUserType != null) {
        abp.notify.warn('User Type is already selected ', 'Warn');
        let data = this.userpermissionsall;
        data[index] = {};
        this.userpermissionsall = data;
        return;
      }
    }
    let data = this.userpermissionsall;
    data[index].first = test;
    data[index].isChanged = true;

    if (val == "R") {
      data[index].R = !data[index].R;
    }
    else if (val == "E") {
      data[index].E = !data[index].E;
    }
    else if (val == "A") {
      data[index].A = !data[index].A;
    }
    else if (val == "D") {
      data[index].D = !data[index].D;
    }
    else if (val == "S") {
      data[index].S = !data[index].S;
    }
    this.userpermissionsall = data;
  }

  checkBoxChange(test: any, index, val: any) {


    let data = this.userpermissionsall;
    data[index].first = test;

    if (val == "R") {
      data[index].R = !data[index].R;
    }
    else if (val == "E") {
      data[index].E = !data[index].E;
    }
    else if (val == "A") {
      data[index].A = !data[index].A;
    }
    else if (val == "D") {
      data[index].D = !data[index].D;
    }
    else if (val == "S") {
      data[index].S = !data[index].S;
    }
    else if (val == "All") {
      data[index].All = !data[index].All;
    }
    this.userpermissionsall = data;
  }

  Renamee() {
    debugger;
    this.datachanges = [];
    let data = this.userpermissionsall;


    for (let x = 0; x < this.userpermissionsall.length; x++) {
      if (this.userpermissionsall[x].first.includes("-")) {

        this.typedata = this.userpermissionsall[x].first.split("-")[0];
      } else {
        this.typedata = this.userpermissionsall[x].first;
      }

      if (this.userpermissionsall[x].R == true) {
        this.R = "R";
      }
      else {
        this.R = "_";
      }
      if (this.userpermissionsall[x].E == true) {
        this.E = "E";
      }
      else {
        this.E = "_";
      }
      if (this.userpermissionsall[x].A == true) {
        this.A = "A";
      }
      else {
        this.A = "_";
      }
      if (this.userpermissionsall[x].D == true) {
        this.D = "D";
      }
      else {
        this.D = "_";
      }
      if (this.userpermissionsall[x].S == true) {
        this.S = "S";
      }
      else {
        this.S = "_";
      }
      this.datachange = "{" + this.typedata + "-" + this.R + this.E + this.A + this.D + this.S + "}";

      if (this.datachange == "{undefined-_____}") {
        abp.notify.error("You need to select the user type and permissions");
        return;
      }
      if (this.typedata == undefined || this.typedata == "Please Select") {
        abp.notify.error("You need to select the User Type");
        return;
      }
      let permissions = this.R + this.E + this.A + this.D + this.S;
      if (permissions == "_____") {
        abp.notify.error("You need to select atleast one permission for " + this.typedata);
        return;
      }

      this.datachanges.push(this.datachange);

    }
    this.bdata = JSON.stringify(this.datachanges);
    let datas = this.bdata.replaceAll('"', '').replaceAll(",", "").replaceAll("[", "").replaceAll("]", "");
    datas = datas;
    let sdata = this.change.substring(this.change.lastIndexOf("}") + 1, this.change.length);
    //sdata;
    let fulldata = datas + sdata;


    console.log(this.selectedGroup);
    this.btnstate = true;
    this.items = this.fileManager.instance.getSelectedItems();
    this.fullparentnew = this.parentpath + '\\' + this.filenames + "~" + fulldata;
    this.fullparentold = this.parentpath + '\\' + this.filenameold + "~" + this.change;
    this.shortfilename = this.filenames + "~" + fulldata;
    this.shortfilenameold = this.filenameold + "~" + this.change;

    let filenameold = this.fullparentold;
    let filenamenew = this.fullparentnew;
    console.log(filenameold);
    console.log(filenamenew);
    var userId = abp.session.userId.toString();
    var escrowNewId = localStorage.getItem("escrowNewID");
    const token = 'my JWT';
    const header1 = new HttpHeaders({ 'filenameold': this.fullparentold, 'filenamenew': this.fullparentnew, 'shortfilenameold': this.shortfilenameold, 'shortfilename': this.shortfilename, 'userType': this.datachanges, 'userId': abp.session.userId.toString(), 'escrowNewId': localStorage.getItem("activeTab") });
    header1.append('Content-Type', 'application/json');
    const header2 = new HttpHeaders({ 'filenameold': this.fullparentold, 'filenamenew': this.fullparentnew, 'shortfilenameold': this.shortfilenameold, 'shortfilename': this.shortfilename, 'userType': this.datachanges, 'userId': abp.session.userId.toString(), 'escrowNewId': localStorage.getItem("activeTab") });
    header2.append('Content-Type', 'application/json');
    //this.http.get<any>(this.path1 + "/Home/Rename", { headers: header1 });
    this.http.get<any>(this.path1 + "/Home/Rename?EscrowId=" + escrowNewId, { headers: header2 }).subscribe((response: any) => {


      // this.data = response;
      // this.bs = response.result.message;
      // return;
     
      this.HideRename();
      this.fileManager1.instance.refresh().done((result) => {
        this.check();
      })
        .fail(function (error) {
          // handle error
        });
      this.fileManager.instance.refresh().done((result) => {
        this.check();
      })
        .fail(function (error) {
          // handle error
        });

      this.btnstate = false;
      let fileRes: any = response;
      if (fileRes === null) {
        abp.notify.success('File renamed successfully', 'success');
        this.fileMainComponent.getAllFiles();
        return;
    }
      if (fileRes.result?.message === "File Already Signed") {
        abp.notify.error('You are not allowed to rename this file', 'error');
        this.fileMainComponent.getAllFiles();
        return;
   }
      abp.notify.success('File Renamed Successfully', 'Success');
      this.fileMainComponent.getAllFiles();
      try {
        let newResult = fileRes.status;
        if (newResult == 500) {
          this.spinnerUpl = false;
          this.file = null
          this.isFile = false;
          this.fileName = 'Select a file'
        }
        if (newResult == 200 ) {
        alert("File Renamed successfully :)");
        this.spinnerUpl = false;
        this.file = null;
        this.isFile = false;
        this.fileName = 'Select a file'
        }
        else {
        }
      }
      catch (error) {
        alert(error);
      }
    });
    if (this.bs != "File Already Signed") {
      const header = new HttpHeaders({ 'parentpath': this.parentpath, 'shortfilename': this.shortfilename });
      // headers.append('Content-Type', 'application/json');

      // this.http.get<any>(this.path1 + "/Home/SignRename", { headers: header }).subscribe((response: any) => {


      // });

      //this.datachanges=[];
    }
    else if (this.bs == "File Already Signed") {
      abp.notify.error('File Already Signed');
    }
    return;
  }


  Renamee1() {
    try {
      debugger;
      this.btnstate = true;
      //this.items = this.fileManager1.instance.getSelectedItems();
      let formattedPath = this.completeEnterprisePathOther.replace(/\//g, "\\");

      let fileNameOld = formattedPath + '\\' + this.filenameold;
      let fileNameNew = formattedPath + '\\' + this.filenames;
      const token = 'my JWT';
      const headers = new HttpHeaders({ 'filenameold': fileNameOld, 'filenamenew': fileNameNew });
      headers.append('Content-Type', 'application/json');

      this.http.post<any>(this.path1 + "/Home/DropAreaRename", {}, { headers: headers })
        .subscribe(
          (response: any) => {
            try {
              this.fileOtherComponent.getAllFiles();
              this.HideRename();
              this.btnstate = false;
              let fileRes: any = response;
              abp.notify.success('File Renamed Successfully', 'Success');

              let newResult = fileRes.status;
              if (newResult === 500) {
                this.spinnerUpl = false;
                this.file = null;
                this.isFile = false;
                this.fileName = 'Select a file';
              }
              else if (newResult === 200) {
                alert("File Renamed successfully :)");
                this.spinnerUpl = false;
                this.file = null;
                this.isFile = false;
                this.fileName = 'Select a file';
              }
            } catch (error) {
              console.error("Error processing response:", error);
              alert(error);
            }
          },
          (error) => {
            console.error("HTTP request failed:", error);
            alert("An error occurred while renaming the file.");
            this.btnstate = false;
          }
        );
    } catch (error) {
      console.error("Unexpected error in Renamee1():", error);
      alert("An unexpected error occurred.");
    }

    this.datachanges = [];
    return;
  }

  Moved() {

    debugger;
    this.datachanges = [];

    let data = this.userpermissionsall;
    for (let i = 0; i < data?.length; i++) {
      this.typedata = data[i].first;
      if (data[i].R == true) {
        this.R = "R";
      }
      else {
        this.R = "_";
      }
      if (data[i].E == true) {
        this.E = "E";
      }
      else {
        this.E = "_";
      }
      if (data[i].A == true) {
        this.A = "A";
      }
      else {
        this.A = "_";
      }
      if (data[i].D == true) {
        this.D = "D";
      }
      else {
        this.D = "_";
      }
      if (data[i].S == true) {
        this.S = "S";
      }
      else {
        this.S = "_";
      }
      this.datachange = "{" + this.typedata + "-" + this.R + this.E + this.A + this.D + this.S + "}";
      if (this.datachange == "{undefined-_____}") {
        abp.notify.error("You need to select the user type and permissions");
        return;
      }
      if (this.typedata == undefined || this.typedata == "Please Select") {
        abp.notify.error("You need to select the User Type");
        return;
      }
      let permissions = this.R + this.E + this.A + this.D + this.S;
      if (permissions == "_____") {
        abp.notify.error("You need to select atleast one permission for " + this.typedata);
        return;
      }
      this.datachanges.push(this.datachange);
    }

    this.bdata = JSON.stringify(this.datachanges);
    let datas = this.bdata.replaceAll('"', '').replaceAll(",", "").replaceAll("[", "").replaceAll("]", "");
    datas = datas;
    let sdata = this.change.substring(this.change.lastIndexOf("}") + 1, this.change.length);
    //sdata;
    let fulldata = datas + sdata;
    console.log(this.selectedGroup);
    this.btnstate = true;
    this.parentpath = this.completeEnterprisePathOther.replace(/\\/g, "/");
    this.fullparentnew = this.parentpath.replace("/Other", "") + this.filenames + "~" + fulldata;
    this.fullparentold = this.parentpath + this.filenameold + "~" + this.change;
    this.shortfilename = this.filenames + "~" + fulldata;
    this.shortfilenameold = this.filenameold + "~" + this.change;

    console.log("New Path: ", this.fullparentnew);
    console.log("Old Path: ", this.fullparentold);

    let filenameold = this.fullparentold;
    let filenamenew = this.fullparentnew;
    console.log(filenameold);
    console.log(filenamenew);
    var userId = abp.session.userId.toString();
    var escrowNewId = localStorage.getItem("activeTab");
    const token = 'my JWT';
    var typeExist = localStorage.getItem("typePdf");
    var oldName = localStorage.getItem("oldFileName");
    var fileExt = localStorage.getItem("fileExtension");

    const headers = new HttpHeaders({ 'filenameold': filenameold, 'filenamenew': filenamenew, 'userType': this.datachanges, 'userId': userId, 'escrowNewId': escrowNewId, 'fileExtension': fileExt, 'typeExist': typeExist, 'oldName': oldName });
    headers.append('Content-Type', 'application/json');

    this.http.get<any>(this.path1 + "/Home/Move", { headers: headers }).subscribe((response: any) => {
      debugger;
      if (response != null && response.result.statusCode == 409) {
        abp.notify.success(response.result.message, 'Success');
        this.HideMove();
        return;
      }

      this.HideMove();
      this.btnstate = false;
      let fileRes: any = response;
      this.fileMainComponent.getAllFiles();
      this.fileOtherComponent.getAllFiles();
      abp.notify.success('File Moved Successfully', 'Success');
      this.userpermissionsall = [];
      try {
        let newResult = fileRes.result.statusCode;
        if (newResult == 500) {
          this.spinnerUpl = false;
          this.file = null
          this.isFile = false;
          this.fileName = 'Select a file'
        }
        if (newResult == 200) {
          debugger;
          //alert("File Moved successfully :)");
          this.spinnerUpl = false;
          this.file = null;
          this.isFile = false;
          this.fileName = 'Select a file';
          // abp.notify.success('File Moved Successfully', 'Success');      
        }
        else {
        }

        // this.fileManager.instance.refresh().done((result) => {

        //   console.log("calling when refresh is done for fileManager")
        //   this.check();
        // })
        //   .fail(function (error) {
        //     // handle error
        //   });

        // const oldFileManager1 = this.fileManager1;
        // var checkFirstFile = this.fileManager1.instance.getCurrentDirectory();
        // if (checkFirstFile.path != "") {

        // }
        // this.fileManager1.instance.refresh()
        //   .done((result) => {

        //     console.log("calling when refresh is done for fileManager")
        //     this.check();

        //     this.fileManager1 = oldFileManager1;
        //   })
        //   .fail(function (error) {
        //     // handle error
        //   });

        // this.ngOnInit();

      }
      catch (error) {
        alert(error);
      }

      const header = new HttpHeaders({ 'parentpath': this.parentpath, 'shortfilename': this.shortfilename });
      headers.append('Content-Type', 'application/json');

      this.http.get<any>(this.path1 + "/Home/SignRename?EscrowId=" + escrowNewId, { headers: header }).subscribe((response: any) => {

      });

    });
    this.fileMainComponent.getAllFiles();
  }

  Delete(event) {
    debugger;

    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }

    this.items = this.fileManager.instance.getSelectedItems();

    if (this.items.length > 0) {
      Swal.fire({
        title: 'Are you sure?',
        text: `Do you really want to delete ${this.items[0].name}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
      }).then((result) => {
        if (result.isConfirmed) {
          this.items.forEach(element => {
            this.show(this.items[0].name);

            let path = element.path;
            let key = element.key;
            let strng = path.replace(/#/g, "%23");
            let strng1 = key.replace(/#/g, "%23");
            const token = 'my JWT';
            const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);

            this.http.get(this.folderPath + "DeleteFile" + "?path=" + strng + "&key=" + strng1, {
              headers,
              responseType: 'blob' as 'json'
            }).subscribe((response: any) => {
              let path = this.folderPath + "DeleteFile" + "?path=" + strng + "&key=" + strng1;

              this.fileMainComponent.getAllFiles();
              this.fileOtherComponent.getAllFiles();

              this.fileManager.instance.refresh().done((result) => {
                this.check();
              }).fail(function (error) {
                // handle error
              });

              let fileRes: any = response;
              this.check();

              try {
                let newResult = fileRes.status;

                if (newResult == 500) {
                  this.spinnerUpl = false;
                  this.file = null;
                  this.isFile = false;
                  this.fileName = 'Select a file';
                }

                if (newResult == 200) {
                  alert("File deleted successfully :)");
                  this.spinnerUpl = false;
                  this.file = null;
                  this.isFile = false;
                  this.fileName = 'Select a file';
                }
              } catch (error) {
                alert(error);
              }
            });
          });
        } else {
          return;
        }
      });
    }
  }


  Delete2(event) {
    debugger;

    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }

    const item = event.selectedFile;

    Swal.fire({
      title: 'Are you sure?',
      text: `Do you really want to delete ${item.name}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.show(item.name);

        let path = event.folderPath;
        let key = event.selectedFile.key;
        let strng = path.replace(/#/g, "%23");
        let strng1 = key.replace(/#/g, "%23");
        let path1 = strng + "/" + event.selectedFile.name;

        const token = 'my JWT';
        const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);

        this.http.get(this.folderPath + "DeleteFile" + "?path=" + path1 + "&key=" + strng1, {
          headers,
          responseType: 'blob' as 'json'
        }).subscribe((response: any) => {
          this.fileMainComponent.getAllFiles();
          this.fileOtherComponent.getAllFiles();

          this.fileManager.instance.refresh().done((result) => {
            this.check();
          }).fail(function (error) {
            // handle error
          });

          let fileRes: any = response;
          this.check();

          try {
            let newResult = fileRes.status;

            if (newResult === 500) {
              this.spinnerUpl = false;
              this.file = null;
              this.isFile = false;
              this.fileName = 'Select a file';
            }

            if (newResult === 200) {
              Swal.fire('Deleted!', 'File deleted successfully :)', 'success');
              this.spinnerUpl = false;
              this.file = null;
              this.isFile = false;
              this.fileName = 'Select a file';
            }
          } catch (error) {
            Swal.fire('Error', error.toString(), 'error');
          }
        });
      } else {
        return;
      }
    });
  }


  Delete1(event) {
    debugger;

    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }

    const item = event.selectedFile;

    Swal.fire({
      title: 'Are you sure?',
      text: `Do you really want to delete ${item.name}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.show(item.name);

        let path = event.folderPath;
        let key = item.key;
        let strng = path.replace(/#/g, "%23");
        let strng1 = key.replace(/#/g, "%23");
        let path1 = strng + "/" + item.name;

        const token = 'my JWT';
        const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);

        this.http.get(this.folderPath + "DeleteFile" + "?path=" + path1 + "&key=" + strng1, {
          headers,
          responseType: 'blob',
        }).subscribe((response: any) => {
          debugger;
          this.fileMainComponent.getAllFiles();
          this.fileOtherComponent.getAllFiles();

          let fileRes: any = response;

          try {
            this.fileOtherComponent.getAllFiles();
            let newResult = fileRes.status;
            if (newResult === 500) {
              this.spinnerUpl = false;
              this.file = null;
              this.isFile = false;
              this.fileName = 'Select a file';
            }

            if (newResult === 200) {
              debugger;
              Swal.fire('Deleted!', 'File deleted successfully :)', 'success');
              this.spinnerUpl = false;
              this.file = null;
              this.isFile = false;
              this.fileName = 'Select a file';

              // this.fileMainComponent.getAllFiles();
              this.fileOtherComponent.getAllFiles();
              // this.fileManager1.instance.refresh().done(() => {
              //   this.check();
              // });
            }

            // Handle other statuses if needed
          } catch (error) {
            Swal.fire('Error', error.toString(), 'error');
          }
        });
      } else {
        return;
      }
    });
  }


  Edit(even) {
    this.check();

    this.items = this.fileManager.instance.getSelectedItems();
    let strcheck;
    this.items.forEach(ele => {
      let compare = ""
      let action = ele.key;
      action = action.substring(action.indexOf("~") + 1);
      compare = this.Action;
      compare = compare.replace("{", "");
      compare = compare.replace("}", "");
      compare = compare.substring(0, compare.indexOf('-'));
      const paramsPattern = /[^{\}]+(?=})/g;
      let extractParams = action.match(paramsPattern);
      for (let i = 0; i < extractParams.length; i++) {
        let my = extractParams[i].replace("{", "");
        let my1 = my;
        my = my.substring(0, my.indexOf('-'));
        my1 = my1.substring(my1.indexOf('-') + 1);
        if (my == compare) {
          strcheck = my1;
        }
      }
    });
    if (strcheck.indexOf("E") === -1) {
      this.ErrorMessage = "!Oops you don't have rights to Edit this file";
    }
    else {

    }
  }

  e_Sign(popupesign: TemplateRef<any>, selectedFile: any) {
    debugger;
    console.log(popupesign);
    this.check();
    let config = { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true };

    // We now use the passed fileName directly, no need to use fileManager.instance.getSelectedItems()
    let file = selectedFile; // Assuming you have the file key from fileName. Adjust accordingly if you need more info from the file.
    let strcheck;

    // Process the file directly (no need for the loop)
    let compare = "";
    let action = file.key;
    action = action.substring(action.indexOf("~") + 1);
    compare = this.Action;
    compare = compare.replace("{", "");
    compare = compare.replace("}", "");

    const paramsPattern = /[^{\}]+(?=})/g;
    let extractParams = action.match(paramsPattern);

    if (extractParams) {
      for (let i = 0; i < extractParams.length; i++) {
        let my = extractParams[i].replace("{", "");
        let my1 = my;
        my = my.substring(0, my.indexOf('-'));
        my1 = my1.substring(my1.indexOf('-') + 1);
        if (my == compare || (my == 'BRX' || my == 'SRX')) {
          strcheck = my1;
        }
      }
    }

    if (strcheck.indexOf("S") === -1) {
      this.ErrorMessage = "!Oops you don't have E-sign Permission for this file";
    } else {
      this.docx = false;
      this.msgShow = false;

      // Handle a single file instead of multiple items
      let strng = file.key.replace(/#/g, "%23");

      var zohoKey = 2001;
      if (zohoKey != 2001) {
        this.http.get(this.folderPath + "E_SignView?path=" + selectedFile.name + "&key=" + strng + "&user=" + this.appSession.userId.toString()).subscribe((response: any) => {
          let data = response['result'];
          let name = data["firstPara"];
          console.log("signing test" + data);
          if (name == "Signed") {
            alert("No Action Required!");
          } else {
            this.eSign = true;
            this.modalReff = this.modalService.show(
              popupesign, config
            );
            this.embedUrl = name;
            console.log("signing test" + this.embedUrl);
            this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
            console.log("signing test" + this.EMBED_SESSION_URL);
            localStorage.setItem('Signing', 'true');
          }
        });
      } else {
        let srId = selectedFile.srAssignedFileId;
        let escrow = localStorage.getItem("activeTab");
        let userType = localStorage.getItem("accessTYpe" + escrow);

        this.selectedFileForDownload = strng;
        this.http.get(AppConsts.remoteServiceBaseUrl + "/Home/GetEmbeddedLink?filePath=" + strng + "&escrow=" + escrow + "&userType=" + userType + "&srAssignedFileId=" + srId).subscribe((response: any) => {
          if (response.result) {
            this.embedUrl = response.result;
            this.eSign = true;
            debugger;
            this.modalReff = this.modalService.show(
              popupesign, config
            );
            console.log("signing test" + this.embedUrl);
            this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
            debugger;
          } else {
            Swal.fire({
              title: 'No signature required for this file.',
              text: '',
              icon: '',
              confirmButtonText: 'OK'
            });
          }
        });
      }
    }
  }

  // e_Sign(popupesign: TemplateRef<any>) {
  //   console.log(popupesign);
  //   this.check();
  //   let config = { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true };
  //   this.items = this.fileManager.instance.getSelectedItems();
  //   let strcheck;
  //   this.items.forEach(ele => {
  //     let compare = ""
  //     let action = ele.key;
  //     action = action.substring(action.indexOf("~") + 1);
  //     compare = this.Action;
  //     compare = compare.replace("{", "");
  //     compare = compare.replace("}", "");
  //     //compare = compare.substring(0,compare.indexOf('-'));
  //     const paramsPattern = /[^{\}]+(?=})/g;
  //     let extractParams = action.match(paramsPattern);
  //     for (let i = 0; i < extractParams.length; i++) {
  //       let my = extractParams[i].replace("{", "");
  //       let my1 = my;
  //       my = my.substring(0, my.indexOf('-'));
  //       my1 = my1.substring(my1.indexOf('-') + 1);
  //       if (my == compare || (my == 'BRX' || my == 'SRX')) {
  //         strcheck = my1;
  //       }
  //     }
  //   });

  //   if (strcheck.indexOf("S") === -1) {
  //     this.ErrorMessage = "!Oops you don't have  E-sign Permission for this file";
  //   }
  //   else {
  //     this.docx = false;
  //     this.msgShow = false;
  //     let dir = this.fileManager.instance.getSelectedItems();
  //     let strng;
  //     if (dir.length > 0) {
  //       for (let i = 0; i < dir.length; i++) {
  //         let item = dir[i];
  //         let source = item['parentPath'] + "/" + item['key'];
  //         strng = item['key'].replace(/#/g, "%23");

  //         var zohoKey = 2001;
  //         if (zohoKey != 2001) {
  //           this.http.get(this.folderPath + "E_SignView?path=" + item['parentPath'] + "&key=" + strng + "&user=" + this.appSession.userId.toString()).subscribe((response: any) => {
  //             let data = response['result'];
  //             let name = data["firstPara"];
  //             console.log("signing test" + data);
  //             if (name == "Signed") {
  //               alert("No Action Required!");
  //             } else {
  //               this.eSign = true;
  //               this.modalReff = this.modalService.show(
  //                 popupesign, config
  //               );
  //               this.embedUrl = name;
  //               console.log("signing test" + this.embedUrl);
  //               this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
  //               console.log("signing test" + this.EMBED_SESSION_URL);
  //               localStorage.setItem('Signing', 'true');
  //             }
  //           });
  //         } else {
  //           let srId = dir[0].dataItem.srAssignedFileId
  //           let escrow = localStorage.getItem("activeTab")
  //           let userType = localStorage.getItem("accessTYpe" + escrow);

  //           this.selectedFileForDownload = strng;
  //           this.http.get(AppConsts.remoteServiceBaseUrl + "/Home/GetEmbeddedLink?filePath=" + strng + "&escrow=" + escrow + "&userType=" + userType + "&srAssignedFileId=" + srId).subscribe((response: any) => {
  //             if (response.result) {


  //               this.embedUrl = response.result;
  //               this.eSign = true;
  //               debugger
  //               this.modalReff = this.modalService.show(
  //                 popupesign, config
  //               );
  //               console.log("signing test" + this.embedUrl);
  //               this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
  //               debugger
  //             }
  //             else {
  //               Swal.fire({
  //                 title: 'No signature required for this file.',
  //                 text: '',
  //                 icon: '',
  //                 confirmButtonText: 'OK'
  //               });
  //             }

  //           })

  //         }
  //       }
  //     }
  //   }

  // }

  // eSignPopupFromMain(popupesign: TemplateRef<any>){
  //   console.log(popupesign);
  //   this.check();
  //   let config = { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true };
  //  // this.items = this.fileManager.instance.getSelectedItems();
  //   let strcheck;
  //   this.items.forEach(ele => {
  //     let compare = ""
  //     let action = ele.key;
  //     action = action.substring(action.indexOf("~") + 1);
  //     compare = this.Action;
  //     compare = compare.replace("{", "");
  //     compare = compare.replace("}", "");
  //     //compare = compare.substring(0,compare.indexOf('-'));
  //     const paramsPattern = /[^{\}]+(?=})/g;
  //     let extractParams = action.match(paramsPattern);
  //     for (let i = 0; i < extractParams.length; i++) {
  //       let my = extractParams[i].replace("{", "");
  //       let my1 = my;
  //       my = my.substring(0, my.indexOf('-'));
  //       my1 = my1.substring(my1.indexOf('-') + 1);
  //       if (my == compare || (my == 'BRX' || my == 'SRX')) {
  //         strcheck = my1;
  //       }
  //     }
  //   });

  //   if (strcheck.indexOf("S") === -1) {
  //     this.ErrorMessage = "!Oops you don't have  E-sign Permission for this file";
  //   }
  //   else {
  //     this.docx = false;
  //     this.msgShow = false;
  //     let dir = this.fileManager.instance.getSelectedItems();
  //     let strng;
  //     if (dir.length > 0) {
  //       for (let i = 0; i < dir.length; i++) {
  //         let item = dir[i];
  //         let source = item['parentPath'] + "/" + item['key'];
  //         strng = item['key'].replace(/#/g, "%23");

  //         var zohoKey = 2001;
  //         if (zohoKey != 2001) {
  //           this.http.get(this.folderPath + "E_SignView?path=" + item['parentPath'] + "&key=" + strng + "&user=" + this.appSession.userId.toString()).subscribe((response: any) => {
  //             let data = response['result'];
  //             let name = data["firstPara"];
  //             console.log("signing test" + data);
  //             if (name == "Signed") {
  //               alert("No Action Required!");
  //             } else {
  //               this.eSign = true;
  //               this.modalReff = this.modalService.show(
  //                 popupesign, config
  //               );
  //               this.embedUrl = name;
  //               console.log("signing test" + this.embedUrl);
  //               this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
  //               console.log("signing test" + this.EMBED_SESSION_URL);
  //               localStorage.setItem('Signing', 'true');
  //             }
  //           });
  //         } else {
  //           let srId = dir[0].dataItem.srAssignedFileId
  //           let escrow = localStorage.getItem("activeTab")
  //           let userType = localStorage.getItem("accessTYpe" + escrow);

  //           this.selectedFileForDownload = strng;
  //           this.http.get(AppConsts.remoteServiceBaseUrl + "/Home/GetEmbeddedLink?filePath=" + strng + "&escrow=" + escrow + "&userType=" + userType + "&srAssignedFileId=" + srId).subscribe((response: any) => {
  //             if (response.result) {


  //               this.embedUrl = response.result;
  //               this.eSign = true;
  //               debugger
  //               this.modalReff = this.modalService.show(
  //                 popupesign, config
  //               );
  //               console.log("signing test" + this.embedUrl);
  //               this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
  //               debugger
  //             }
  //             else {
  //               Swal.fire({
  //                 title: 'No signature required for this file.',
  //                 text: '',
  //                 icon: '',
  //                 confirmButtonText: 'OK'
  //               });
  //             }

  //           })

  //         }
  //       }
  //     }
  //   }
  // }

  Hide() {
    debugger;
    this.check();
    // this.modalReff.hide();

    if (this.docx && this.editPermission) {
      if (confirm("Do you want to Exit Document Viewer Without Saving?")) {
        this.modalReff.hide();
      }
      else {
      }
    }
    else {
      // this.modalReff.hide();
      this.modalService.hide();
      this.renderer.removeClass(document.body, 'modal-open');
    }
  }


  DownloadEsignedPdf() {
    debugger
    let dir = this.mainFileSelected;
    let fileId = dir.srAssignedFileId;
    this.http.get(AppConsts.remoteServiceBaseUrl + "/Home/downloadZohoPdf?filePath=" + this.selectedFileForDownload + "&srAssignedFileId=" + fileId).subscribe((response: any) => {

      if (response != null) {
        if (response.result.signingStatus != "Unsigned") {
          this.isRename = false;
          this.editPermission = false;
          this.renamePermission = false;
          this.renameFileName = true;

          this.fileMainComponent.getAllFiles();
          this.fileOtherComponent.getAllFiles();
          setTimeout(() => {
            this.fileManager.instance.refresh().done((result) => {
              console.log("calling when refresh is done for fileManager after sign")
              this.fileManager.instance.option('selectedItems', []);
              this.check();
            })
          }, 1000);
        } else {
          this.isRename = true;
          this.editPermission = true;
          this.renamePermission = true;
        }
      }

    })
  }

  SignStatusChange() {

  }

  HideHistory() {

    this.modalService.hide();
    this.renderer.removeClass(document.body, 'modal-open');
  }
  HideRename() {

    this.modalService.hide();
    this.renderer.removeClass(document.body, 'modal-open');
  }
  HideMove() {

    this.modalService.hide();
    this.renderer.removeClass(document.body, 'modal-open');
  }

  SaveChanges() {
    debugger;
    this.check();
    this.getCurrentDocumentAsBlob().then(res => {

      var fd = new FormData();
      let name = this.fullpath.substring(this.fullpath.lastIndexOf('/') + 1)
      fd.append('file', res, name);
      let path = this.fullpath.substring(0, this.fullpath.lastIndexOf('/'))
      let dir = this.mainFileSelected;
      let fileId = dir.srAssignedFileId;
      this.http.post(this.folderPath + "Edit?path=" + path + '&srAssignedFileId=' + fileId, fd, { reportProgress: true, observe: 'events' })
        .subscribe(res => {
          let fileRes: any = res;
          this.modalReff.hide();
        });
      this.http.get(this.folderPath + "DocUpdate?message=Input&filename=" + this.fileNamestrng + "&userId=" + this.appSession.userId).subscribe((response: any) => {
      });
    });
  }

  public async getCurrentDocumentAsBlob(): Promise<Blob> {
    debugger
    const PDFViewerApplication: IPDFViewerApplication = (window as any).PDFViewerApplication;
    const data = await PDFViewerApplication.pdfDocument.saveDocument(PDFViewerApplication.pdfDocument.annotationStorage);
    this.fullpath = PDFViewerApplication['baseUrl'];
    return new Blob([data], { type: 'application/pdf' });
  }

  openModal(template: TemplateRef<any>) {
    this.check();
    this.modalRef = this.modalService.show(
      template,
      Object.assign({}, { class: 'gray modal-lg' })
    );
  }

  openHistoryModal(popupHistory: TemplateRef<any>) {
    this.check();
    let dir = this.fileManager.instance.getSelectedItems();
    let fileId = dir[0].dataItem.srAssignedFileId;
    this.getFileHistory(fileId)
    this.modalRef = this.modalService.show(
      popupHistory,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );
  }

  openHistoryFromMain(file) {

    this.check();
    let fileId = file.srAssignedFileId;
    this.getFileHistory(fileId)
    this.modalRef = this.modalService.show(
      this.viewHistoryMainTemplateRef,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );
  }

  getFileHistory(fileId) {
    this.escrowFileHistoriesServiceProxy.getAll(undefined, undefined, fileId, undefined, 0, 100000).subscribe((response: any) => {
      this.ItemsArray = response.items;
    })
  }

  openrenameModal(popuprename: TemplateRef<any>) {
    this.check();
    let dir = this.fileManager.instance.getSelectedItems();
    if (dir.length > 0) {
      for (let i = 0; i < dir.length; i++) {
        let item = dir[i];
        this.filenames = item['name'];

        this.filenamenew = this.filenames;
        this.filenameold = item['name'];
        this.fullnameold = item['key'];
        let splitname = this.fullnameold.split("~");
        this.change = splitname[1];
        this.lst = splitname[1];

        let types = this.change.substring(0, this.change.lastIndexOf("}")).replaceAll("}", "").replace("{", "");
        let type = types.split("{");
        let firstusertype: any = [];
        let userpermissions: any = [];

        let userpermissionsall: any = [];
        for (let i = 0; i < type.length; i++) {
          this.split = type[i].split('-');
          firstusertype.push(this.split[0]);
          userpermissions.push(this.split[1]);
          if (this.split[1].includes("R")) {
            this.checkR = true;
          }
          else {
            this.checkR = false;
          }
          if (this.split[1].includes("E")) {
            this.checkE = true;
          }
          else {
            this.checkE = false;
          }
          if (this.split[1].includes("A")) {
            this.checkA = true;
          }
          else {
            this.checkA = false;
          }
          if (this.split[1].includes("D")) {
            this.checkD = true;
          }
          else {
            this.checkD = false;
          }
          if (this.split[1].includes("S")) {
            this.checkS = true;
          }
          else {
            this.checkS = false;
          }

          let find = this.myuser.find(x => x.usertype == this.split[0]);
          if (find) {
            let All = false;
            if (this.checkR && this.checkE && this.checkA && this.checkD && this.checkS) {
              All = true;
            }

            // data[index].first = test +" - " + find.name ;
            userpermissionsall.push({ first: this.split[0] + "- " + find.name, All: All, R: this.checkR, E: this.checkE, A: this.checkA, D: this.checkD, S: this.checkS });
          }
        }


        this.type = firstusertype;
        debugger;
        this.userpermissionsall = userpermissionsall;
        this.parentpath = item['parentPath'];
        this.parentpath = this.parentpath.replace('/', '\\').replace('/', '\\');
        // this.Rename(this.filenameold, this.filenamenew);
      }
    }

    this.modalRef = this.modalService.show(
      popuprename,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );
  }


  openRenameModalFromMain(fileName) {
    debugger;
    this.check();

    // Assuming fileName is an object similar to the items from the selected file
    if (fileName) {
      this.filenames = fileName['name'];
      this.filenamenew = this.filenames;
      this.filenameold = fileName['name'];
      this.fullnameold = fileName['key'];
      let splitname = this.fullnameold.split("~");
      this.change = splitname[1];
      this.lst = splitname[1];

      let types = this.change.substring(0, this.change.lastIndexOf("}")).replaceAll("}", "").replace("{", "");
      let type = types.split("{");
      let firstusertype: any = [];
      let userpermissions: any = [];
      let userpermissionsall: any = [];
      for (let i = 0; i < type.length; i++) {
        this.split = type[i].split('-');
        firstusertype.push(this.split[0]);
        userpermissions.push(this.split[1]);
        if (this.split[1].includes("R")) {
          this.checkR = true;
        }
        else {
          this.checkR = false;
        }
        if (this.split[1].includes("E")) {
          this.checkE = true;
        }
        else {
          this.checkE = false;
        }
        if (this.split[1].includes("A")) {
          this.checkA = true;
        }
        else {
          this.checkA = false;
        }
        if (this.split[1].includes("D")) {
          this.checkD = true;
        }
        else {
          this.checkD = false;
        }
        if (this.split[1].includes("S")) {
          this.checkS = true;
        }
        else {
          this.checkS = false;
        }

        let find = this.myuser.find(x => x.usertype == this.split[0]);
        if (find) {
          let All = false;
          if (this.checkR && this.checkE && this.checkA && this.checkD && this.checkS) {
            All = true;
          }

          // data[index].first = test +" - " + find.name ;
          userpermissionsall.push({ first: this.split[0] + "- " + find.name, All: All, R: this.checkR, E: this.checkE, A: this.checkA, D: this.checkD, S: this.checkS });
        }
      }

      this.type = firstusertype;
      this.userpermissionsall = userpermissionsall;
      this.parentpath = this.parentpath.replace('/', '\\').replace('/', '\\');
    }

    // Show the modal with the necessary configurations
    this.modalRef = this.modalService.show(
      this.renameMainTemplateRef,
      Object.assign({}, { class: 'gray modal-lg', backdrop: false, ignoreBackdropClick: true })
    );
  }


  // openrenameModal1(popuprename: TemplateRef<any>) {
  //     
  //   this.check();
  //   let dir = this.fileManager1.instance.getSelectedItems();
  //   if (dir.length > 0) {
  //     for (let i = 0; i < dir.length; i++) {
  //       let item = dir[i];
  //       this.filenames = item['key'];

  //       // Extract filename and extension correctly
  //       let dotPosition = this.filenames.lastIndexOf(".");
  //       if (dotPosition !== -1) {
  //         this.filenamenew = this.filenames.substring(0, dotPosition); // Name without extension
  //         this.secondPart = this.filenames.substring(dotPosition + 1); // Extension
  //       } else {
  //         this.filenamenew = this.filenames; // If no extension
  //         this.secondPart = ""; 
  //       }

  //       this.filenameold = item['key'];
  //       this.fullnameold = item['key'];

  //       // Ensure filename splitting is done correctly
  //       let splitname = this.fullnameold.split(".");
  //       this.change = splitname[0];
  //       this.lst = splitname[0];

  //       // Fix string operations on types
  //       this.type = this.change.replace(/[{}]/g, ""); // Removes { and }

  //       // Fix parent path replacements
  //       this.parentpath = item['parentPath'].replaceAll('/', '\\');

  //       // Show modal
  //       this.modalRef = this.modalService.show(
  //         popuprename,
  //         Object.assign({}, { class: 'gray modal-lg' })
  //       );
  //     }
  //   }
  // }

  openrenameModal1(selectedFile: any, popuprename: TemplateRef<any>) {
    try {
      debugger;
      this.check();
      let item = selectedFile;
      this.filenames = item.key;

      // Extract the last index of the dot (for extension)
      let dotPosition = this.filenames.lastIndexOf(".");

      if (dotPosition !== -1) {
        // If the dot position exists, remove the last extension if it's duplicated
        this.filenamenew = this.filenames.substring(0, dotPosition);  // Get the name without extension
        this.secondPart = this.filenames.substring(dotPosition + 1);  // Get the extension

        // Check if the last two parts are the same extension (e.g., file.docx.docx)
        if (this.filenamenew.endsWith("." + this.secondPart)) {
          // Remove the extra extension
          let newDotPosition = this.filenamenew.lastIndexOf(".");
          this.filenamenew = this.filenamenew.substring(0, newDotPosition);
        }
      } else {
        // If no extension, just assign the filename as is
        this.filenamenew = this.filenames;
        this.secondPart = "";
      }

      // Store the original filename and path
      this.filenameold = item.key;
      this.fullnameold = item.key;

      // Extract name without extension
      let splitname = this.fullnameold.split(".");
      this.change = splitname[0];
      this.lst = splitname[0];

      // Clean the name (optional)
      this.type = this.change.replace(/[{}]/g, "");

      // this.parentpath = item.path.replaceAll('/', '\\');
      this.parentpath = item.path ? item.path.replaceAll('/', '\\') : '';


      // Show the rename modal
      this.modalRef = this.modalService.show(popuprename, {
        class: 'gray modal-lg',
      });
    } catch (error) {
      console.error("Error in openrenameModal1:", error);
    }
  }



  openTagsModal1(popuptags: TemplateRef<any>) {
    this.modalRef = this.modalService.show(
      popuptags,
      Object.assign({}, { class: 'gray modal-lg' })
    );

  }

  openMoveModal(selectedFile: any, popupMove: TemplateRef<any>) {
    debugger;

    this.check();
    this.typePDF = "false";

    // Show modal
    this.modalRef = this.modalService.show(popupMove, {
      class: 'gray modal-lg',
    });
    let dir = [selectedFile];

    if (dir.length > 0) {
      for (let i = 0; i < dir.length; i++) {
        let item = dir[i];
        this.filenames = item['name'];
        this.oldfileName = this.currentpath1 + "/" + this.filenames;
        this.filenamenew = this.filenames;
        this.filenameold = item['name'];
        this.fullnameold = item['key'];

        var dotPosition = this.fullnameold.lastIndexOf(".");
        var firstPart = this.fullnameold.substring(0, dotPosition);
        var secondPart = this.fullnameold.substring(dotPosition + 1);
        localStorage.setItem("fileExtension", secondPart);

        if (secondPart == "docx" || secondPart == "doc" || secondPart == "pdf") {

          if (!firstPart.includes("~")) {
            if (secondPart == "pdf") {
              this.typePDF = "true";
              this.oldfileName = this.currentpath1.replaceAll("/", "\\") + '\\' + this.filenames.replaceAll("/", "\\");
            }

            secondPart = ".pdf";
            this.filenames = firstPart;
            this.fullnameold = firstPart + "~" + secondPart;
          }
          localStorage.setItem('oldFileName', this.oldfileName);
          localStorage.setItem("typePdf", this.typePDF);
        }

        let splitname = this.fullnameold.split("~");
        this.change = splitname[1];
        this.signForFile = "~";
        if (firstPart.includes("~")) {
          this.lst = splitname[1];
          // this.change = '';
          // this.signForFile = '';
        }

        this.parentpath = item.path;
        this.parentpath = this.parentpath.replace('/', '\\').replace('/', '\\');
        let types = this.change.substring(0, this.change.lastIndexOf("}")).replaceAll("}", "").replace("{", "");
        let type = types.split("{");
        let firstusertype: any = [];
        let userpermissions: any = [];

        let userpermissionsall: any = [];
        if (!userpermissions) {
          this.split = type[i].split('-');
          firstusertype.push(this.split[0]);
          userpermissions.push(this.split[1]);
        }
        else {

          for (let i = 0; i < type.length; i++) {
            this.split = type[i].split('-');
            if (this.split.length != 1) {
              firstusertype.push(this.split[0]);
              userpermissions.push(this.split[1]);
              if (this.split[1].includes("R")) {
                this.checkR = true;
              }
              else {
                this.checkR = false;
              }
              if (this.split[1].includes("E")) {
                this.checkE = true;
              }
              else {
                this.checkE = false;
              }
              if (this.split[1].includes("A")) {
                this.checkA = true;
              }
              else {
                this.checkA = false;
              }
              if (this.split[1].includes("D")) {
                this.checkD = true;
              }
              else {
                this.checkD = false;
              }
              if (this.split[1].includes("S")) {
                this.checkS = true;
              }
              else {
                this.checkS = false;
              }
              userpermissionsall.push({ first: this.split[0], R: this.checkR, E: this.checkE, A: this.checkA, D: this.checkD, S: this.checkS });
            }

            else {
              userpermissionsall.push({});
            }
          }

        }


        this.type = firstusertype;

        this.userpermissionsall = userpermissionsall;
        this.parentpath = item.path;
        this.parentpath = this.parentpath.replace('/', '\\').replace('/', '\\');
        // this.Rename(this.filenameold, this.filenamenew);
      }
    }

    // this.Rename(this.filenameold, this.filenamenew);


    this.modalRef = this.modalService.show(
      popupMove,
      Object.assign({}, { class: 'gray modal-lg' })
    );
  }
  ngAfterViewInit() {
    debugger;
    let dir = this.fileManager1.instance.getSelectedItems();
    let strng;
    if (dir.length > 0) {
      for (let i = 0; i < dir.length; i++) {
        let item = dir[i];
        let source = item['parentPath'] + "/" + item['key'];
        strng = source.replace(/#/g, "%23");
        var maindiv = document.getElementById('modal-content');
        var newdiv = document.createElement('div');
        newdiv.id = "viewer";
        newdiv.className = "viewer";
        var newdiv1 = document.createElement('div');
        newdiv1.id = "headerH";
        newdiv1.className = "headerH";
        newdiv1.innerText = "WebViewer";
        if (!document.getElementById('headerH')) {
          maindiv.appendChild(newdiv1);
        }
        if (!document.getElementById('viewer')) {
          maindiv.appendChild(newdiv);
        }
      }
      let url = AppConsts.appBaseUrl;

    }
    var temp = document.getElementById('viewer');

    WebViewer({
      path: '/lib',
      initialDoc: '/path/to/document.pdf',
    }, document.getElementById('viewer'))
      .then((instance) => {
        this.wvInstance = instance;
        instance.docViewer.on('documentLoaded', this.wvDocumentLoadedHandler.bind(this));
      });

    this._chatSignalrService.componentMethodCalled$.subscribe(() => {
      //this.ngOnInit();
      // window.location.reload();

      let checkFirstFile = this.fileManager.instance.getCurrentDirectory();
      if (checkFirstFile.path == "") {
        //window.location.reload();
      }
      else {
        this.fileMainComponent.getAllFiles();
        this.fileOtherComponent.getAllFiles();
        // this.fileManager.instance.refresh().done((result) => {
        //   this.check();
        // })
        //   .fail(function (error) {
        //     // handle error
        //   });
        // this.fileManager1.instance.refresh().done((result) => {
        //   this.check();
        // })
        //   .fail(function (error) {
        //     // handle error
        //   });
      }
    });
    setTimeout(() => {

      // this.fileManager1.instance.refresh().done((result) => {
      //   this.check();
      // })
      //   .fail(function (error) {
      //     // handle error
      //   });
      // this.fileManager.instance.refresh().done((result) => {
      //   this.check();
      // })
      //   .fail(function (error) {
      //     // handle error
      //   });

      this.fileMainComponent.getAllFiles();
      this.fileOtherComponent.getAllFiles();
    }, 2000)

  }

  msgFunction() {
    function isSupportedFileAPI() {
      return window.File && window.FileReader && window.FileList && window.Blob;
    }
    function formatEmail(data) {
      return data.name ? data.name + " [" + data.email + "]" : data.email;
    }

    function parseHeaders(headers) {
      var parsedHeaders = {};
      if (!headers) {
        return parsedHeaders;
      }
      var headerRegEx = /(.*)\: (.*)/g;
      let m;
      while (m = headerRegEx.exec(headers)) {
        parsedHeaders[m[1]] = m[2];
      }
      return parsedHeaders;
    }
    function _arrayBufferToString(buf, callback) {
      var bb = new Blob([new Uint8Array(buf)]);
      var f = new FileReader();
      f.onload = function (e) {
        callback(e.target.result);
      };
      f.readAsText(bb);
    }
    function getMsgDate(rawHeaders) {
      var headers = parseHeaders(rawHeaders);
      if (!headers['Date']) {
        return '-';
      }
      return new Date(headers['Date']);
    }

    if (isSupportedFileAPI()) {
      let dir = this.fileManager1.instance.getSelectedItems();
      if (dir.length > 0) {

        for (let i = 0; i < dir.length; i++) {
          var selectedFile = dir[i];
          let filename = selectedFile['parentPath'] + "/" + selectedFile['key'];
          let strng = filename.replace(/#/g, "%23");
          let strng1 = selectedFile['key'].replace(/#/g, "%23");

          const token = 'my JWT';
          const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
          let userId = abp.session.userId;
          this.http.get(this.folderPath + "DownloadFile" + "?path=" + strng + "&key=" + strng1 + "&userId=" + userId, { headers, responseType: 'blob' as 'json' }).subscribe((res: any) => {

            var request = res;
            if (!request) {
              $('.msg-info, .incorrect-type').hide();
              return;
            }
            if (selectedFile.name.indexOf('.msg') == -1) {
              $('.msg-info').hide();
              $('.incorrect-type').show();
              return;
            }
            $('.msg-file-name').html(selectedFile['key']);
            var fileReader = new FileReader();
            fileReader.onload = function (evt) {

              var buffer: any = evt.target.result;
              var msgReader = new MSGReader(buffer);
              var fileData: any = msgReader.getFileData();
              if (fileData) {
                $('.msg-from').html(formatEmail({ name: fileData.senderName, email: fileData.senderEmail }));

                $('.msg-to').html(jQuery.map(fileData.recipients, function (recipient, i) {
                  return formatEmail(recipient);
                }).join('<br/>'));
                var fgh = new Date(getMsgDate(fileData.headers));
                $('.msg-date').html(fgh.toString());
                $('.msg-subject').html(fileData.subject);
                var optn = fileData.body;
                $('.msg-body').html(
                  fileData.body ? optn.toString() : "");

                if (fileData.bodyHTML) {
                  $('.msg-body-html').html(fileData.bodyHTML).closest('div.field-block').show();
                } else {
                  $('.msg-body-html').closest('div.field-block').hide();
                }
                $('.msg-attachment').html(jQuery.map(fileData.attachments, function (attachment, i: number) {
                  var file = msgReader.getAttachment(i);
                  var fileUrl = URL.createObjectURL(new File([file.content], attachment.fileName,
                    { type: attachment.mimeType ? attachment.mimeType : "application/octet-stream" }));
                  return attachment.fileName + ' [' + attachment.contentLength + 'bytes]' +
                    (attachment.pidContentId ? '; ID = ' + attachment.pidContentId : '') +
                    '; <a href="' + fileUrl + '">Download</a>';
                }).join('<br/>'));
                $('.msg-info').show();
              } else {
                $('.msg-info').hide();
                $('.incorrect-type').show();
              }
            };
            fileReader.readAsArrayBuffer(request);
          });
        }
      } else {
        $('.file-api-not-available').show();
      }
    }
  }

  sendMessage(fileName): void {

    this.fileFullName = fileName;

    const tenancyName = this.appSession.tenant ? this.appSession.tenant.tenancyName : null;
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 8,
      message: this.fileFullName + "File Edit Success.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 9,
      message: this.fileFullName + "File Edit Success.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 141,
      message: this.fileFullName + "File Edit Success.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 142,
      message: this.fileFullName + "File Edit Success.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 143,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 144,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 145,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 146,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 147,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 148,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 149,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 150,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 151,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
    this._chatSignalrService.sendMessage({
      tenantId: null,
      userId: 152,
      message: "File Edit Successful.",
      tenancyName: tenancyName,
      userName: this.appSession.user.userName,
      profilePictureId: this.appSession.user.profilePictureId
    }, () => {

    });
  }

  changeUserType() {

    var queryParams = this.person;
    var activeTab = localStorage.getItem("activeTab")

    if (activeTab == this.escrowname) {
      const Key = 'accessTYpe' + atob(queryParams['e']);
      var userType = localStorage.getItem(Key);
      this.Action = userType;
      this.UsertypeModel = userType;
    }

  }
  SaveFileDoc(): void {

    this.changingValue.next(true);
    this.modalReff.hide();
  }
  toMessage: boolean = false;
  email: boolean = false;
  directMessage: boolean = false;
  reminderResponse: any = []


  onCheckboxChange(type: string, event: Event): void {

    const isChecked = (event.target as HTMLInputElement).checked;
    if (type === 'message') {
      this.toMessage = isChecked;
    }

    else if (type === 'email') {
      this.email = isChecked;
    }

    else if (type === 'directMessage') {
      this.directMessage = isChecked;
    }

  }
  reminderMessageResponse: boolean = false
  reminderMailResponse: boolean = false
  reminderDirectMessageResponse: boolean = false
  checkReminderMessage: boolean = false

  saveReminder() {
    debugger;
    this.reminderResponse = [];
    this.checkMessageType = false;
    this.checkReminderMessage = false
    if (this.toMessage) {
      let reminderType = new ReminderTypeList()
      reminderType.reminderType = "Message"
      this.reminderTypeList.push(reminderType)
    }
    if (this.email) {
      let reminderType = new ReminderTypeList()
      reminderType.reminderType = "Email"
      this.reminderTypeList.push(reminderType)
    }
    if (this.directMessage) {
      let reminderType = new ReminderTypeList()
      reminderType.reminderType = "DirectMessage"
      this.reminderTypeList.push(reminderType)
    }

    this.reminderDto.reminderType = this.reminderTypeList;
    if (this.reminderTypeList.length == 0) {
      this.checkMessageType = true
      return;
    }
    var find = this.reminderUserList.find(x => x.isChecked)
    if (!find) {
      this.checkUserSelected = true;
      return;
    }
    if (!this.reminderDto.reminderText) {
      this.checkReminderMessage = true
      return;
    }

    this.reminderDto.sentFrom = this.appSession.user.emailAddress;
    let escrow = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + escrow);
    this.reminderDto.sentFromUserType = userType;
    // let dir = this.fileManager.instance.getSelectedItems();
    this.reminderDto.srEscrowFileMasterId = this.mainFileSelected.srAssignedFileId; //dir[0].dataItem.srAssignedFileId;
    this.reminderDto.escrowNumber = escrow;
    this.reminderDto.assignedFileUser = this.reminderUserList;
    this.reminderUserList = [];
    this.reminderMessageResponse = false;
    this.reminderMailResponse = false;
    this.reminderDirectMessageResponse = false;

    this._srEscrowFileRemindersServiceProxy.createOrEdit(this.reminderDto).subscribe((response: any) => {

      this.reminderUserList = response;
      this.reminderDto = new CreateOrEditSrEscrowFileReminderDto();
      if (this.toMessage) {
        this.reminderMessageResponse = true;
      }
      if (this.email) {
        this.reminderMailResponse = true;
      }
      if (this.directMessage) {
        this.reminderDirectMessageResponse = true;
      }

      this.reminderTypeList = [];
      this.toMessage = false;
      this.email = false;
      this.directMessage = false;

    })
  }
  @HostListener('window:paste', ['$event'])
  onPaste(event: ClipboardEvent) {

    if (this.UsertypeModel === 'EOX') {
      const clipboardData = event.clipboardData || (window as any).clipboardData;

      if (clipboardData && clipboardData.files && clipboardData.files.length > 0) {
        const files = clipboardData.files;
        const allowedFileTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'text/plain', 'application/rtf', 'message/rfc822']; // MIME types for PDF, DOC, DOCX, TXT, RTF, EML

        let hasValidFile = false;

        for (let i = 0; i < files.length; i++) {
          const fileType = files[i].type;
          if (allowedFileTypes.includes(fileType)) {
            hasValidFile = true;
            this.uploadFile(files); // Upload the file
            break; // Exit loop after uploading the first valid file
          }
        }

        if (!hasValidFile) {
          abp.notify.error('Only PDF, DOC, DOCX, TXT, RTF, and EML files are allowed.', 'Error');
        }
      } else {
        abp.notify.error('No files found in the clipboard.', 'Error');
      }
    } else {
      abp.notify.error('You don’t have the permission to paste the file', 'Error');
    }
  }


  async onPasteFromContextMenu() {
    const myDiv = document.getElementById('appDragDropAreaContent');
    if (myDiv) {
      myDiv.focus();
      //document.onpaste('paste')
    }

    const enterEvent = new KeyboardEvent('keyup', { key: 'ctrl +v' });

    console.log("started from Clipboard");
    const clipboardItems = await navigator.clipboard || (window as any).read();
    console.log("reading from Clipboard" + clipboardItems);
    await this.processClipboardItems(clipboardItems);
  }



  onRightClickPaste(event: MouseEvent) {

    event.preventDefault();
    //this.readClipboard()

    //     this.contextMenuX = event.clientX; // Get mouse position
    //     this.contextMenuY = event.clientY;
    //     this.showContextMenu = true;

    //  $("#pasteButton").css({'display':'block'})
    //  $("#pasteButton").css({'top': event.clientY  +'px'})
    //  $("#pasteButton").css({'left':event.clientX +'px'})
    //  $("#pasteButton").focus();

    // //  const eventdata = new KeyboardEvent('keydown', {
    // //   key: 'v',
    // //   ctrlKey: true,
    // //   bubbles: true,
    // // });

    // document.dispatchEvent(eventdata);
    // document.execCommand('paste')
    // const pasteEvent = new KeyboardEvent('keydown', {
    //   key: 'v',
    //   code: 'KeyV',
    //   ctrlKey: true,
    //   bubbles: true,
    //   cancelable: true,
    // });
    // document.dispatchEvent(pasteEvent);
    this.readClipboard();

    // // Dispatch the event to the contenteditable div
    // const contentEditableDiv = document.getElementById('appDragDropAreaContent');
    // if (contentEditableDiv) {
    //   contentEditableDiv.focus();
    //   document.execCommand('paste')
    //   //contentEditableDiv.execCommand('paste')
    // }

  }
  async readClipboard() {
    await this.clipboardService.readFromClipboard();
  }

  private async processClipboardItems(clipboardItems: any) {
    debugger;
    console.log("processClipboardItems");
    for (const item of clipboardItems) {
      console.log("processClipboardItems Response" + item);
      if (item.types.includes('application/pdf')) {
        console.log("processClipboardItems Response for pdf file" + item);
        const file = await item.getType('application/pdf');
        //this.pastedFile = new File([file], 'pasted.pdf', { type: 'application/pdf' });
        // console.log('Pasted PDF file:', this.pastedFile);
        this.uploadFile(item.file);
        console.log("processClipboardItems completed for pdf file");
        break;
      }
    }
  }

  SaveChangesOtherArea() {
    debugger;
    this.check();
    this.getCurrentDocumentAsBlob().then(res => {

      var fd = new FormData();
      let name = this.fullpath.substring(this.fullpath.lastIndexOf('/') + 1)
      fd.append('file', res, name);
      let path = this.fullpath.substring(0, this.fullpath.lastIndexOf('/'))
      let dir = this.otherFileSelected;
      let fileId = dir.srAssignedFileId;
      this.http.post(this.folderPath + "Edit?path=" + path + '&srAssignedFileId=' + fileId, fd, { reportProgress: true, observe: 'events' })
        .subscribe(res => {

          let fileRes: any = res;
          this.modalReff.hide();
        });
      this.http.get(this.folderPath + "DocUpdate?message=Input&filename=" + this.fileNamestrng + "&userId=" + this.appSession.userId).subscribe((response: any) => {

      });
    });
  }

  isEscrowOfficerUserExist: boolean = false;
  openMessagePopup() {
    // let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
    // this.modalRef = this.modalService.show(this.DirectMessageToEscrowTemplate, config);
    this.messageToEscrowOfficerModal.show();
  }
  openNotesPopup() {

    this.stickyNotesComponent.show();
  }

  Opentags() {

    this.escrowUsertagsComponent.show();

  }

  getEscrowOfficerDetails() {
    var escrow = localStorage.getItem("activeTab");
    this._escrowDetailsServiceProxy.getEscrowOfficerDetails(escrow).subscribe((response: any) => {
      this.isEscrowOfficerUserExist = response != null ? true : false;
    })
  }

  getFileIcon() {
    alert("^")
  }

  filterGrid() {
    if (this.filterLabel) {
      if (this.searchText) {
        //  const find = this.escrowList;
        // let data = find.filter(x => x[this.filterLabel].toLowerCase().includes(this.searchText.toLowerCase()))

        // this.primengTableHelper.records = data;
        // this.primengTableHelper.totalRecordsCount = data.length;

      } else {

        // this.primengTableHelper.records = this.escrowList;
        // this.primengTableHelper.totalRecordsCount = this.escrowList.length;
      }
    } else {

    }
  }

  closeSignPopup() {
    this.signPopup = false;
    this.renderer.removeStyle(document.body, 'background-color');
  }

}

@Injectable({
  providedIn: 'root'
})
export class GlobalService {
  docFile: any = {}
  folderPath: string = "";
  oldPathSelectedFile = ""
}

