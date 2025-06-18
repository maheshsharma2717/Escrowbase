import { Component, Injector, ViewEncapsulation, ViewChild,Input, Renderer2, Inject, TemplateRef, OnInit, EventEmitter, ElementRef, HostListener, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DashboardCustomizationConst } from '@app/shared/common/customizable-dashboard/DashboardCustomizationConsts';
import FileSystemProvider from 'devextreme/file_management/remote_provider';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DxFileManagerComponent} from 'devextreme-angular';

import { AppConsts } from '@shared/AppConsts';
import { SrFileMappingsServiceProxy, EscrowDetailsServiceProxy, UserFileLogsServiceProxy, SrEscrowsServiceProxy, UserNotification, SrAssignedFilesDetailsServiceProxy, CheckDatabaseOutput, JsonClaimMapDto } from '@shared/service-proxies/service-proxies';
import * as $ from "jquery";
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Location } from '@angular/common'
import { DomSanitizer } from '@angular/platform-browser';
import { ModalDirective, BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { NgxExtendedPdfViewerService, IPDFViewerApplication, NgxExtendedPdfViewerComponent } from 'ngx-extended-pdf-viewer';
import WebViewer from '@pdftron/webviewer';
import { MSGReader } from 'wl-msg-reader';
import { ChatSignalrService } from '../../shared/layout/chat/chat-signalr.service';
import { Observable, Subscription, interval } from 'rxjs';
declare function _download(): any;
declare var jQuery: any;
import {FormControl} from '@angular/forms';
import { SignalRHelper } from '../../../shared/helpers/SignalRHelper';
import { timeInterval, timeout } from 'rxjs/operators';
import { data } from 'jquery';
import { Table } from 'primeng/table';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { forEach, lastIndexOf } from 'lodash-es';
import { key } from 'localforage';
import { Title } from '@angular/platform-browser';
 
import {DefaultLayoutComponent} from '../../shared/layout/themes/default/default-layout.component'

@Component({
  selector: 'app-upload-file',
  templateUrl: './filelist.component.html',
  styleUrls: ['./filelist.component.less'],
  encapsulation: ViewEncapsulation.None

})



export class FileViewComponent extends AppComponentBase {
  //@ViewChild(ModalDirective) public lgModal: ModalDirective;
  @ViewChild(NgxExtendedPdfViewerComponent) private pdfComponent: NgxExtendedPdfViewerComponent;
  @ViewChild(DxFileManagerComponent, { static: false }) fileManager: DxFileManagerComponent;

  @ViewChild("targetDataGrid", { static: false }) fileManager1: DxFileManagerComponent;
  @ViewChild("data", { static: false }) data: DxFileManagerComponent;
 
  dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;
  remoteProvider: FileSystemProvider;
  remoteProvider1: FileSystemProvider;
  ngxExtendedPdfViewerService: NgxExtendedPdfViewerService;

  btnstate: boolean=false;
  useridd: string;
  private fieldArray: Array<any> = [];
  private newAttribute: any = {};
  split:any;
  typedata:any;
  datachange:any;
  bdata:any;
  datachanges:any =[];
  selectedGroup: any;
  checkR:boolean;
  checkE:boolean;
  checkA:boolean;
  checkD:boolean;
  checkS:boolean;
  R:any;
  E:any;
  A:any;
  D:any;
  S:any;
  folderPath: any;
  path1:any;
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
  type:any=[];
  userpermissionsall:any=[];
  myuser:any=["BRX","SRX","BR1","BR2","BR3","BR4","BR5","BR6","BR7","BR8","BR9","BR10","SR1","SR2","SR3","SR4","SR5","SR6","SR7","SR8","SR9","SR10","RAL","RBL","RAS","RBS","RAO","RBO","LR1","LR2","LR3","LP1","LP2","LP3","TCX","TCA","LBX","LBP","EO1","EA1","EOX","EAX","TC1","TC2","TC3","TC4","TC5","TC6","TC7","TC8","TC9","TC10"];
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
  filenames:any;
  token:any;
  filenameold:any;
  filenamenew:any;
  fullnameold:any;
  parentpath:any;
  fullparentnew:any;
  fullparentold:any;
  shortfilename:any;
  shortfilenameold:any;
  bs:any;
  lst:any;
  change:any;
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
  @Input() person;
 
  
  public constructor(
    private titleService: Title,
    private http: HttpClient,
    injector: Injector,
    private modalService: BsModalService,
    private _srEscrowsServiceProxy: SrEscrowsServiceProxy,
    private location: Location,
    private sanitizer: DomSanitizer,
    private _userFileLogsServiceProxy: UserFileLogsServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _escrowDetailsServiceProxy: EscrowDetailsServiceProxy,
    private _router: Router,
    private _chatSignalrService: ChatSignalrService
  ) 
  {
    super(injector);
    if (this.appSession.application) {
      SignalRHelper.initSignalR(() => { this._chatSignalrService.init(); });

    }
    

    
    // setTimeout(() => {
    //   this.sendMessage();
    // }, 3000);


   
  }

  LoadData(){
    

 //var    queryParams =this._activatedRoute.snapshot.queryParams;
 let msg = this._activatedRoute.snapshot.queryParams['token'];
    //let msg = queryParams['token'];
    let fileName;
    if (this._activatedRoute.snapshot.queryParams['f'] != undefined) {

      fileName = decodeURIComponent(this._activatedRoute.snapshot.queryParams['f'])
      fileName = atob(fileName.replace(' ', '+'));
    }
    let url = AppConsts.remoteServiceBaseUrl;
    this.folderPath = url + '/FileManager/';
    this.path1=url;
    if (msg != undefined) {
      let decrypt = atob(msg);
      if (decrypt == "Saved") {
        
        this.message.success(this.l('EsignSuccessMessage'), this.l('Signed Successfully')).then(() => {
          this.http.get(this.folderPath + "DocUpdate?message=Sign&fileName=" + fileName + "&userId=" + this.appSession.userId).subscribe((response: any) => {
          });
          this.http.get(this.folderPath + "E_SignDocDownload?mail=" + this.appSession.user.emailAddress + "&fileName=" + this._activatedRoute.snapshot.queryParams['f']).subscribe((response: any) => {
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

      if(localStorage.getItem('homeOpened') == 'false'){
          window.close();
      }
  }


  ngOnInit(): void {
    
    var queryParams = this.person;
    this.LoadData();
     if(localStorage.getItem('escrowOpened') == queryParams['e']){+
        localStorage.setItem('notab','false');
        //let b = 'http://localhost:4201/app/main/Notab';
    // window.location.href = b;
    }
  // localStorage.setItem('escrowOpened', queryParams['e']);

   
var title= atob(queryParams['e']);
     //this.titleService.setTitle(title);
    this.useridd = abp.session.userId.toString();
    let test = queryParams['u'];
    let test1 = queryParams['token'];
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
 this.Action = FileViewComponent.newusertype;
      FileViewComponent.escrowno = atob(queryParams['e']);
      this.escrowname = FileViewComponent.escrowno;
      this.Name = this.appSession.user.name + " " + this.appSession.user.surname;
      this.UsertypeModel = FileViewComponent.newusertype;
      this.tempsubcompany = queryParams['sc'];
      this.tempcompany = queryParams['c'];
      if (test1 == undefined) {

        FileViewComponent.pathname = AppConsts.appBaseUrl + "/app/main/File?u=" + queryParams['u'] + "&e=" + queryParams['e'] + "&c=" +queryParams['sc'] + "";
      }
      FileViewComponent.currentpathh = this.validFileName(atob(queryParams['c'])) + "/" + this.validFileName(atob(queryParams['sc'])) + "/" + atob(queryParams['e']);
      FileViewComponent.currentpathh1 = this.validFileName(atob(queryParams['c'])) + "/" + this.validFileName(atob(queryParams['sc'])) + "/" + atob(queryParams['e']) + "/Other";

      this.currentpath = FileViewComponent.currentpathh;
      this.currentpath1 = FileViewComponent.currentpathh1;
    }
    else {
      this.userTypes = [];
      this.Action = FileViewComponent.newusertype;
      this.UsertypeModel = FileViewComponent.newusertype;
      this.escrowname = FileViewComponent.escrowno;
      this.Name = this.appSession.user.name + " " + this.appSession.user.surname;
    }
    if (abp.session.userId === 1) { }
    else {
      this.show(this.useridd);
    }
    let url = AppConsts.remoteServiceBaseUrl;
    this.remoteProvider = new FileSystemProvider({
      endpointUrl: url + "/FileManager/FileSystem?&company=" + this.validFileName(atob(queryParams['c'])) + "&subCompany=" + this.validFileName(atob(queryParams['sc'])) + "&escrow=" + FileViewComponent.escrowno + "&useriD=" + abp.session.userId + "&usertype=" + FileViewComponent.newusertype + "&usersname=" + this.appSession.user.name + " " + this.appSession.user.surname

    });
    this.remoteProvider1 = new FileSystemProvider({
      endpointUrl: url + "/FileManager/FileSystem1?&company=" + this.validFileName(atob(queryParams['c'])) + "&subCompany=" + this.validFileName(atob(queryParams['sc'])) + "&escrow=" + FileViewComponent.escrowno + "&useriD=" + abp.session.userId
    });
    this.check();
    this.folderPath = url + '/FileManager/';
    this.myurl = url;
  }
  filess: any = [];
  check = () => {
    setTimeout(() => {
try{
      this.itemCount = this.fileManager.instance["_itemView"]._itemCount;
      this.itemCount1 = this.fileManager1.instance["_itemView"]._itemCount;
}catch(error){}
    }, 3000)
    console.log(new Date() + " Escrow No:"+this.escrowname+" User Name:"+this.Name+" User Type:"+this.Action+" Company:"+atob(this.tempcompany)+" Sub Company:"+atob(this.tempsubcompany));
  }

  show(elem): void {

    this._escrowDetailsServiceProxy.getAll(undefined, undefined, this.appSession.user.emailAddress, undefined,
      undefined, undefined, undefined, undefined, undefined, undefined).subscribe(result => {
        this.output = [];
        this.eFilter = [];
        let split = [];

        this.eFilter = result['items'];

        for (let i = 0; i < result['items'].length; i++) {

          this.output = this.eFilter[i];
          this.output = this.output['escrowDetail'];
          this.collect = this.output['escrowId'];
          this.collection = this.output['company'];
          let new1 = FileViewComponent.currentpathh.substring(0, FileViewComponent.currentpathh.indexOf('/'));
          let comp = this.validFileName(this.collection);//.replace(/\s/g, "");
          // var typess = this.userTypes;

          if (this.collect === FileViewComponent.escrowno && comp === new1) {
            

            this.collect1 = this.output['usertype'];

            split = this.collect1.split(',');

            for (let i = 0; i < split.length; i++) {
              
              this.userTypes.push(split[i]);

            }

          }

          this.userTypes = this.userTypes.filter((test, index, array) =>

            index === array.findIndex((findTest) =>
              findTest === test
            ))
          const result = this.userTypes.find(item => item.name === "SRA");

        }
      });
  }
  handleShowingEvent(e) {
    
    // Handler of the 'showing' event
}
isRename:boolean = false;
handleShownEvent(e) {
  
  let status = e.selectedItems[0].dataItem.status;
  if(status == 'Nobody signed yet'|| status==null){
this.isRename = true;
  }
  else{
    this.isRename = false;
  }
    // Handler of the 'shown' event
}
  onItemClick(e) {
    
    if (e.itemData.options.download) {
      this.Download(e);
    }
    else if (e.itemData.options.Rename){
      this.openrenameModal(e.itemData.options.Rename);
     // this.Rename(e);
    }
    else if (e.itemData.options.view) {
      this.View(e.itemData.options.view);
    }
    else if (e.itemData.options.full) {
      //this.ViewFullName(e,);
      this.openFullName(e,e.itemData.options.full)
    }
    if (e.itemData.options.esign) {
      
      this.e_Sign(e.itemData.options.esign);
    }
    if (e.itemData.options.history) {
      this.openHistoryModal(e.itemData.options.history);
    }
    if (e.itemData.options.delete) {
      this.Delete(e);
    }
  }
 
  
  onItemClick1(e) {
    
    if (e.itemData.options.download) {
      this.Download1(e);
    }
    else if (e.itemData.options.view) {
      this.View1(e.itemData.options.view);
    }
    else if (e.itemData.options.full) {
      //this.ViewFullName(e,);
      this.openFullName1(e,e.itemData.options.full)
    }
    if (e.itemData.options.delete) {
      this.Delete1(e);
    }
    else if (e.itemData.options.Move){
      this.openMoveModal(e.itemData.options.Move);
    }
  }
  // MyRefresh(){

  //   let checkFirstFile = this.fileManager.instance.getCurrentDirectory();
  //   if(checkFirstFile.path==""){
  //     window.location.reload();
  //   }
  //   else{
  //    this.fileManager.instance.refresh();

  //          this.fileManager1.instance.refresh();

  //         }
  //      };

  selectChangeHandler(event) {
    FileViewComponent.newusertype = this.UsertypeModel;
    window.location.href = '/app/main/File?u=' + btoa(this.UsertypeModel) + '&e=' + btoa(FileViewComponent.escrowno) + '&c=' + this.tempcompany + '&sc=' + this.tempsubcompany;
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
      this.fileName = this.file[0].name
      this.sourcepath = this.fileManager.instance.getCurrentDirectory();
      if (this.sourcepath == "") {
        alert("Other file upload restricted due to zero file in main fileview");
      } else {
        debugger;
        var uploadPath = this.sourcepath.path + "/Other";
        debugger; 
        var escrowNewId = this.sourcepath.path.split('/');
        escrowNewId = escrowNewId[2];

        localStorage.setItem('escrowNewID',escrowNewId);

        let uploadpath1 = uploadPath.replace("/", "\\");
         for(var i = 0; i < this.file.length; i++) {
          var fileToUpload = <File>this.file[i];
          const formData = new FormData();
          formData.append('file', fileToUpload, fileToUpload.name);
          this.http.post(this.folderPath + "ProcessRequest?path=" + uploadpath1 + "&&useriD=" + abp.session.userId, formData, { reportProgress: true, observe: 'events' })
            .subscribe(res => {
              let fileRes: any = res;
              try {
                let newResult = fileRes.body.result
                this.fileManager1.instance.refresh();
                this.check();
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
                  this.fileManager1.instance.refresh();
                  abp.notify.success('File Uploaded Successfully', 'Success');
                }
                let checkFirstFile = this.fileManager1.instance.getCurrentDirectory();
                if (checkFirstFile.path == "") {
                  window.location.reload();
                } else {
                  this.fileManager1.instance.refresh(); this.check();
                }
              }
              catch (error) { }
            });

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
    
    var files = evt.target.files;
    var file = files[0];
    if (files && file) {
      this.file = files
      this.isFile = true;
      this.fileName = this.file[0].name
    }
    this.uploadFile(files);
  }
  displayImagePopup(e) {
    if (e.file.relativeName) {
      if (!e.file.relativeName.includes(this.myurl)) {
        this.Sign = [];
        let source = this.myurl + "/Common/Paperless/" + e.file.relativeName;
        let strng = e.file.key.replace(/#/g, "%23");
        this.http.get(this.folderPath + "GetSignDetails?type=" + this.appSession.user.emailAddress + "&filename=" + strng).subscribe((response: any) => {
          let data = response['result'];
          if (data.length > 0) {
            for (let i = 0; i < data.length; i++) {
              this.Sign.push(data[i]);
            }
            this.signPopup = true;


            setTimeout(() => {
              this.signPopup = false;

            }, 3000)
          }
        });
      }

    }

  }
  wvDocumentLoadedHandler(): void {
    const docViewer = this.wvInstance;
    const annotManager = this.wvInstance.annotManager;
    const { Annotations } = this.wvInstance;
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
 openFullName(e,full: TemplateRef<any>){
  this.modalRef = this.modalService.show(
    full,
    Object.assign({}, { class: 'gray modal-lg' })
  );
  let dir = this.fileManager.instance.getSelectedItems();
    this.fileFullName = dir[0].key;
 }
 openFullName1(e,full: TemplateRef<any>){
  this.modalRef = this.modalService.show(
    full,
    Object.assign({}, { class: 'gray modal-lg' })
  );
  let dir = this.fileManager1.instance.getSelectedItems();
    this.fileFullName1 = dir[0].key;
 }
//  openFullName2($event,full: TemplateRef<any>){
//   this.modalRef = this.modalService.show(
//     this.full,
//     Object.assign({}, { class: 'gray modal-lg' })
//   );
//   let dir = this.fileManager1.instance.getSelectedItems();
//     this.fileFullName1 = dir[0].key;
// }

  ViewFullName($event) {
 let dir = this.fileManager.instance.getSelectedItems();
    this.fileFullName = dir[0].key;
     this.fullnamepopup = true;
    setTimeout(() => {
      this.fullnamepopup = false;

    }, 5000)
  }
  ViewFullName1($event) {
    

    let dir = this.fileManager1.instance.getSelectedItems();
    this.fileFullName1 = dir[0].key;
    this.fullnamepopup1 = true;
    setTimeout(() => {
      this.fullnamepopup1 = false;

    }, 5000)
  }
  View(temp: TemplateRef<any>) {
    
    console.log(temp);
    this.docx = true; this.eSign = false;
    this.msgShow = false;
    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }
    let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
    let dir = this.fileManager.instance.getSelectedItems();
    if (dir.length > 0) {
      for (let i = 0; i < dir.length; i++) {

        let item = dir[i];
        this.fileNamestrng = item['key'].replace(/#/g, "%23");
        this.sourcepathwithname = item['parentPath'] + "/" + this.fileNamestrng;
        const salt = (new Date()).getTime();
        let url = AppConsts.appBaseUrl;
        this.http.get(this.folderPath + "DocUpdate?message=Read&filename=" + this.fileNamestrng + "&userId=" + this.appSession.userId).subscribe((response: any) => {
          let data = response['result'];
          this.source = url + "/docs/Paperless/" + this.sourcepathwithname + "?" + salt;
          this.modalReff = this.modalService.show(
            temp, config
          );
        });

      }
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
          this.docx = true;
          this.msgShow = false;
          this.sourcepathwithname = item['parentPath'] + "/" + item['key'];
          let strng = this.sourcepathwithname.replace(/#/g, "%23");
          const salt = (new Date()).getTime();
          let url = AppConsts.appBaseUrl;
          this.source = url + "/docs/Paperless/" + strng + "?" + salt;
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
          this.modalReff = this.modalService.show(
            temp, config
          );
          if (document.getElementById('viewer')) {
            document.getElementById('viewer').remove();
          }
          if (document.getElementById('headerH')) {
            document.getElementById('headerH').remove();
          }
          this.wvDocumentLoadedHandler = this.wvDocumentLoadedHandler.bind(this);
          this.ngAfterViewInit();
        }
      }
    }

    this.replacedstring = "";
  }
  Download(event) {
    this.items = this.fileManager.instance.getSelectedItems();
    let compare;
    let strcheck;
    this.items.forEach(ele => {
      let action = ele.key;
      if (action.includes("~'~")) {
        action = action.substring(action.indexOf("~'~") + 3);
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
          this.http.get(this.folderPath + "DownloadFile" + "?path=" + strng + "&key=" + strng1, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {

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
  Download1(event) {
    this.items = this.fileManager1.instance.getSelectedItems();
    let compare;
    let strcheck;
    this.items.forEach(ele => {
      let action = ele.key;
      if (action.includes("~'~")) {
        action = action.substring(action.indexOf("~'~") + 3);
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
          this.http.get(this.folderPath + "DownloadFile" + "?path=" + strng + "&key=" + strng1, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {

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
    debugger;
    this.userpermissionsall.push(this.newAttribute);
    this.newAttribute = {};
    //var currentElement = this.userpermissionsall[index];  
    //this.userpermissionsall.splice(index, 0, currentElement);
  }
  ddldata(test: any, index, val:any){
    debugger;
    let data  = this.userpermissionsall;
    data[index].first = test;
  
    if(val == "R"){
      data[index].R = !data[index].R;
    }
    else if(val == "E"){
      data[index].E = !data[index].E;
    }
    else if(val == "A"){
      data[index].A = !data[index].A;
    }
    else if(val == "D"){
      data[index].D = !data[index].D;
    }
    else if(val == "S"){
      data[index].S = !data[index].S;
    }
    this.userpermissionsall = data;
  }

  Renamee(){
    
    let data = this.userpermissionsall;
    for(let i=0;i<data.length;i++){
      this.typedata=data[i].first;
      if (data[i].R == true){
      this.R="R";
      }
      else{
        this.R="_";
      }
      if (data[i].E == true){
        this.E="E";
        }
        else{
          this.E="_";
        }
        if (data[i].A == true){
          this.A="A";
          }
          else{
            this.A="_";
          }
          if (data[i].D == true){
            this.D="D";
            }
            else{
              this.D="_";
            }
            if (data[i].S == true){
              this.S="S";
              }
              else{
                this.S="_";
              }
              this.datachange="{"+this.typedata+"-"+this.R+this.E+this.A+this.D+this.S+"}";
              this.datachanges.push(this.datachange);
             
              }
              this.bdata= JSON.stringify(this.datachanges);
             // this.bdata=getStates().map(x=>x.StateName).join(",")
//               for(let a=0;a<this.datachanges.length;a++){
//                 this.bdata =this.datachange+this.datachange[a]; 
              
// }

let datas =this.bdata.replaceAll('"','').replaceAll(",","").replaceAll("[","").replaceAll("]","");
datas=datas;
let sdata=this.change.substring(this.change.lastIndexOf("}") + 1, this.change.length);
sdata;
let fulldata=datas+sdata;


    console.log(this.selectedGroup);
    this.btnstate = true;
    this.items = this.fileManager.instance.getSelectedItems();
    this.fullparentnew= this.parentpath+'\\'+this.filenames+"~'~"+fulldata;
 this.fullparentold= this.parentpath+'\\'+this.filenameold+"~'~"+this.change;
 this.shortfilename=this.filenames+"~'~"+fulldata;
 this.shortfilenameold=this.filenames+"~'~"+this.change;

     let filenameold =this.fullparentold;
    let filenamenew =this.fullparentnew;
    const token = 'my JWT';
    const headers = new HttpHeaders({ 'filenameold': filenameold,  'filenamenew': filenamenew,'shortfilenameold':this.shortfilenameold});
    headers.append('Content-Type','application/json');
    
    this.http.get<any>(this.path1 +"/Home/Rename", { headers:headers}).subscribe((response: any) => {
      
      this.data = response;
      this.bs=response.result.message;
      return;
 });
 if(this.bs != "File Already Signed"){
  const header = new HttpHeaders({'parentpath':this.parentpath,'shortfilename':this.shortfilename});
  headers.append('Content-Type','application/json');

this.http.get<any>(this.path1 +"/Home/SignRename", { headers:header}).subscribe((response: any) => {
this.HideRename();
this.fileManager.instance.refresh();
this.btnstate = false;
let fileRes: any = response;
          abp.notify.success('File Renamed Successfully', 'Success');
          try {
            let newResult = fileRes.status;
            if (newResult == 500) {
              this.spinnerUpl = false;
              this.file = null
              this.isFile = false;
              this.fileName = 'Select a file'
            }
            if (newResult == 200) {
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

this.datachanges=[];
}
else if(this.bs == "File Already Signed"){
  abp.notify.error('File Already Signed');
}
    return;
  }


  Moved(){
    debugger;
     
    let data = this.userpermissionsall;
    for(let i=0;i<data.length;i++){
      this.typedata=data[i].first;
      if (data[i].R == true){
      this.R="R";
      }
      else{
        this.R="_";
      }
      if (data[i].E == true){
        this.E="E";
        }
        else{
          this.E="_";
        }
        if (data[i].A == true){
          this.A="A";
          }
          else{
            this.A="_";
          }
          if (data[i].D == true){
            this.D="D";
            }
            else{
              this.D="_";
            }
            if (data[i].S == true){
              this.S="S";
              }
              else{
                this.S="_";
              }
              this.datachange="{"+this.typedata+"-"+this.R+this.E+this.A+this.D+this.S+"}";
              this.datachanges.push(this.datachange);
             
              }
              debugger;
              this.bdata= JSON.stringify(this.datachanges);
             // this.bdata=getStates().map(x=>x.StateName).join(",")
//               for(let a=0;a<this.datachanges.length;a++){
//                 this.bdata =this.datachange+this.datachange[a]; 
              
// }
debugger;
let datas =this.bdata.replaceAll('"','').replaceAll(",","").replaceAll("[","").replaceAll("]","");
datas=datas;
let sdata=this.change.substring(this.change.lastIndexOf("}") + 1, this.change.length);
sdata;
let fulldata=datas+sdata;
    this.btnstate = true;
    this.items = this.fileManager.instance.getSelectedItems();
 
    if(this.lst==undefined){
      this.fullparentold= this.parentpath+'\\'+this.filenameold;
    }
    else{
      this.fullparentold= this.parentpath+'\\'+this.filenameold+"~'~"+this.change;
    }
   debugger;
    this.fullparentnew=this.parentpath.split("/");
    this.parentpath=this.fullparentnew[0];
    this.fullparentnew = this.fullparentnew[0]+'\\'+this.filenames+"~'~"+fulldata;

 //this.fullparentold= this.parentpath+'\\'+this.filenameold+"~'~"+this.change;
 this.shortfilename=this.filenames+"~'~"+this.change;
     let filenameold =this.fullparentold;
      filenameold =this.fullparentold.replaceAll("/","\\");
    let filenamenew =this.fullparentnew;
    const token = 'my JWT';
    debugger;
    var userId = abp.session.userId.toString();
    var escrowNewId = localStorage.getItem('escrowNewID');

    const headers = new HttpHeaders({ 'filenameold': filenameold,  'filenamenew': filenamenew, 'userType': this.datachanges, 'userId': userId, 'escrowNewId': escrowNewId});
    headers.append('Content-Type','application/json');
    
    this.http.get<any>(this.path1 +"/Home/Move", { headers:headers}).subscribe((response: any) => {
      return;
 });

 const header = new HttpHeaders({'parentpath':this.parentpath,'shortfilename':this.shortfilename});
    headers.append('Content-Type','application/json');

 this.http.get<any>(this.path1 +"/Home/SignRename", { headers:header}).subscribe((response: any) => {
  this.HideMove();
  
  this.fileManager.instance.refresh();
  this.fileManager1.instance.refresh();
  this.btnstate = false;
  let fileRes: any = response;
            abp.notify.success('File Moved Successfully', 'Success');
            try {
              let newResult = fileRes.status;
              if (newResult == 500) {
                this.spinnerUpl = false;
                this.file = null
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              if (newResult == 200) {
                alert("File Moved successfully :)");
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
    
    //let fullpath=this.folderPath+"Home/" + "Rename" + "?filenameold=" + filenameold + "&filenamenew=" + filenamenew;
    return;
  }


  Delete(event) {
    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }
    this.items = this.fileManager.instance.getSelectedItems();
    if (this.items.length > 0) {
      this.items.forEach(element => {
        if (confirm("Do you really want to delete " + this.items[0].name)) {
          this.show(this.items[0].name)
          let path = element.path;
          let key = element.key;
          let strng = path.replace(/#/g, "%23");
          let strng1 = key.replace(/#/g, "%23");
          const token = 'my JWT';
          const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
          this.http.get(this.folderPath + "DeleteFile" + "?path=" + strng + "&key=" + strng1, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {
           
            let path=this.folderPath + "DeleteFile" + "?path=" + strng + "&key=" + strng1;
            this.fileManager.instance.refresh();
            let fileRes: any = response;
            this.check();
            try {
              let newResult = fileRes.status;
              if (newResult == 500) {
                this.spinnerUpl = false;
                this.file = null
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              if (newResult == 200) {
                alert("File deleted successfully :)");
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
        }
        else {
          return;
        }
      });
    }
  }
  Delete1(event) {
    
    if (document.getElementById('viewer')) {
      document.getElementById('viewer').remove();
    }
    if (document.getElementById('headerH')) {
      document.getElementById('headerH').remove();
    }
    this.items = this.fileManager1.instance.getSelectedItems();
    if (this.items.length > 0) {
      this.items.forEach(element => {
        if (confirm("Do you really want to delete " + element.key)) {
          this.show(this.items[0].name)
          let path = element.path;
          let key = element.key;
          let strng = path.replace(/#/g, "%23");
          let strng1 = key.replace(/#/g, "%23");
          this.downloadname = key;
          const token = 'my JWT';
          const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
          this.http.get(this.folderPath + "DeleteFile" + "?path=" + strng + "&key=" + strng1, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {
            this.fileManager1.instance.refresh();
            this.check();
            let fileRes: any = response;
            abp.notify.success('File Deleted Successfully', 'Success');
            try {
              let newResult = fileRes.status;
              if (newResult == 500) {
                this.spinnerUpl = false;
                this.file = null
                this.isFile = false;
                this.fileName = 'Select a file'
              }
              if (newResult == 200) {
                alert("File deleted successfully :)");
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
        } else { return; }
      });
    }
  }
  Edit(even) {
    this.check();

    this.items = this.fileManager.instance.getSelectedItems();
    let strcheck;
    this.items.forEach(ele => {
      let compare = ""
      let action = ele.key;
      action = action.substring(action.indexOf("~'~") + 3);
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
  e_Sign(temp: TemplateRef<any>) {
    
    console.log(temp);
    this.check();
    let config = { class: 'gray modal-lg', backdrop: true, ignoreBackdropClick: true };
    this.items = this.fileManager.instance.getSelectedItems();
    let strcheck;
    this.items.forEach(ele => {
      let compare = ""
      let action = ele.key;
      action = action.substring(action.indexOf("~'~") + 3);
      compare = this.Action;
      compare = compare.replace("{", "");
      compare = compare.replace("}", "");
      //compare = compare.substring(0,compare.indexOf('-'));
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
    if (strcheck.indexOf("S") === -1) {
      this.ErrorMessage = "!Oops you don't have  E-sign Permission for this file";
    }
    else {
      this.docx = false;
      this.msgShow = false;
      let dir = this.fileManager.instance.getSelectedItems();
      let strng;
      if (dir.length > 0) {
        for (let i = 0; i < dir.length; i++) {
          let item = dir[i];
          let source = item['parentPath'] + "/" + item['key'];
          strng = item['key'].replace(/#/g, "%23");

          
          this.http.get(this.folderPath + "E_SignView?path=" + item['parentPath'] + "&key=" + strng + "&user=" + this.appSession.userId.toString()).subscribe((response: any) => {
            

            let data = response['result'];
            let name = data["firstPara"];
            console.log("signing test" + data);
            if (name == "Signed") {
              alert("No Action Required!");
            } else {
              this.eSign = true;
              this.modalReff = this.modalService.show(
                temp, config
              );
              this.embedUrl = name;
              console.log("signing test" + this.embedUrl);
              this.EMBED_SESSION_URL = this.sanitizer.bypassSecurityTrustResourceUrl(this.embedUrl);
              console.log("signing test" + this.EMBED_SESSION_URL);
              localStorage.setItem('Signing', 'true');
            }
          });
        }
      }
    }
  }
  Hide() {
    this.check();
   // this.modalReff.hide();
    if (this.docx) {
      if (confirm("Do you want to Exit Document Viewer Without Saving?")) {
        this.modalReff.hide();
        
      }
      else {
       
      }
    }
    else {
      this.modalReff.hide();
    }
  }
  
  HideHistory() {
   
        this.modalService.hide();
  }
  HideRename() {
   
    this.modalService.hide();
}
HideMove() {
   
  this.modalService.hide();
}
  SaveChanges() {

    this.check();
    this.getCurrentDocumentAsBlob().then(res => {

      var fd = new FormData();
      let name = this.fullpath.substring(this.fullpath.lastIndexOf('/') + 1)
      fd.append('file', res, name);
      let path = this.fullpath.substring(0, this.fullpath.lastIndexOf('/'))
      this.http.post(this.folderPath + "Edit?path=" + path, fd, { reportProgress: true, observe: 'events' })
        .subscribe(res => {
          let fileRes: any = res;
          this.modalReff.hide();
          this.fileManager.instance.refresh();
          this.check();
          this.sendMessage(name);
        });
      this.http.get(this.folderPath + "DocUpdate?message=Input&filename=" + this.fileNamestrng + "&userId=" + this.appSession.userId).subscribe((response: any) => {

      });
    });

  }
  public async getCurrentDocumentAsBlob(): Promise<Blob> {
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
    let item1 = dir[0].key.replace("#", "%23");
    let itema = dir[0].dataItem.access;
    let url = AppConsts.remoteServiceBaseUrl;
    this.folderPath = url + '/FileManager/';
    this.http.get(this.folderPath + "fileHistoryView?userId=" + this.appSession.userId + "&type=" + itema + "&fileName=" + item1).subscribe((response: any) => {
      let item = response.result;
      this.ItemsArray = [];
      for (let i = 0; i < item.length; i++) {
        let temp: any = {};
        temp.Name = item[i].name;
        temp.updateOn = item[i].updatedOn;
        if (itema.includes('S') && item[i].signStatus != null) {
          temp.Status = item[i].signStatus;
        }
        else if (itema.includes('E') && item[i].signStatus != null) {
          temp.Status = item[i].inputStatus;
        }
        else if (itema.includes('R') && item[i].signStatus != null) {
          temp.Status = item[i].readStatus;
        }
        this.ItemsArray.push(temp);
      }

      this.modalRef = this.modalService.show(
        popupHistory,
        Object.assign({}, { class: 'gray modal-lg' })
      );
    });

  }

openrenameModal(popuprename: TemplateRef<any>) {
    
    this.check();
    let dir = this.fileManager.instance.getSelectedItems();
    if (dir.length > 0) {
      for (let i = 0; i < dir.length; i++) {
        let item = dir[i];
 this.filenames = item['name'];
 this.filenamenew = this.filenames;
 this.filenameold =item['name'];
 this.fullnameold=item['key'];
 let splitname = this.fullnameold.split("~'~");
this.change=splitname[1];
this.lst=splitname[1];

let types=this.change.substring(0,this.change.lastIndexOf("}")).replaceAll("}","").replace("{","");
let type= types.split("{");
let firstusertype: any=[];
let userpermissions:any=[];

let userpermissionsall:any=[];
for (let i = 0; i< type.length; i++){
  this.split = type[i].split('-');
  firstusertype.push(this.split[0]);
  userpermissions.push(this.split[1]);
if(this.split[1].includes("R")){
this.checkR=true;
}
else{
  this.checkR=false;
}
if(this.split[1].includes("E")){
  this.checkE=true;
  }
  else{
    this.checkE=false;
  }
  if(this.split[1].includes("A")){
    this.checkA=true;
    }
    else{
      this.checkA=false;
    }
    if(this.split[1].includes("D")){
      this.checkD=true;
      }
      else{
        this.checkD=false;
      }
      if(this.split[1].includes("S")){
        this.checkS=true;
        }
        else{
         this.checkS=false;
        }
        userpermissionsall.push({first:this.split[0],R:  this.checkR,E:  this.checkE,A: this.checkA,D: this.checkD,S:this.checkS });
      }

        
this.type=firstusertype;

this.userpermissionsall=userpermissionsall;
 this.parentpath=item['parentPath'];
 this.parentpath = this.parentpath.replace('/','\\').replace('/','\\');
// this.Rename(this.filenameold, this.filenamenew);
}}

      this.modalRef = this.modalService.show(
        popuprename,
        Object.assign({}, { class: 'gray modal-lg' })
      );
  }
  openMoveModal(popupMove: TemplateRef<any>) {
    debugger;
    this.check();
    let dir = this.fileManager1.instance.getSelectedItems();
    if (dir.length > 0) {
      for (let i = 0; i < dir.length; i++) {
        let item = dir[i];
 this.filenames = item['name'];
 this.filenamenew = this.filenames;
 this.filenameold =item['name'];
 this.fullnameold=item['key'];
 let splitname = this.fullnameold.split("~'~");
this.change=splitname[1];
this.lst=splitname[1];
 this.parentpath=item['parentPath'];
 this.parentpath = this.parentpath.replace('/','\\').replace('/','\\');

 


let types=this.change.substring(0,this.change.lastIndexOf("}")).replaceAll("}","").replace("{","");
let type= types.split("{");
let firstusertype: any=[];
let userpermissions:any=[];
debugger;
let userpermissionsall:any=[];
if(!userpermissions)
{
  debugger;
  this.split = type[i].split('-');
firstusertype.push(this.split[0]);
userpermissions.push(this.split[1]);
}
else{
 
    for (let i = 0; i< type.length; i++){
      this.split = type[i].split('-');
      if(this.split.length!=1){
      firstusertype.push(this.split[0]);
      userpermissions.push(this.split[1]);
      if(this.split[1].includes("R")){
      this.checkR=true;
      }
      else{
      this.checkR=false;
      }
      if(this.split[1].includes("E")){
      this.checkE=true;
      }
      else{
       this.checkE=false;
      }
      if(this.split[1].includes("A")){
       this.checkA=true;
       }
       else{
         this.checkA=false;
       }
       if(this.split[1].includes("D")){
         this.checkD=true;
         }
         else{
           this.checkD=false;
         }
         if(this.split[1].includes("S")){
           this.checkS=true;
           }
           else{
            this.checkS=false;
           }
           userpermissionsall.push({first:this.split[0],R:  this.checkR,E:  this.checkE,A: this.checkA,D: this.checkD,S:this.checkS });
          }
         
          else{
            userpermissionsall.push({first:"BR1",R:  false,E:  false,A: false,D: false,S:false });
          }    }  
        
  }

     
this.type=firstusertype;

this.userpermissionsall=userpermissionsall;
this.parentpath=item['parentPath'];
this.parentpath = this.parentpath.replace('/','\\').replace('/','\\');
// this.Rename(this.filenameold, this.filenamenew);
}}

// this.Rename(this.filenameold, this.filenamenew);


      this.modalRef = this.modalService.show(
        popupMove,
        Object.assign({}, { class: 'gray modal-lg' })
      );
  }
  ngAfterViewInit() {
    this.check();
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
      WebViewer({
        path: 'wv-resources/lib',
        pdftronServer: 'https://demo.pdftron.com/',
        initialDoc: url + "/docs/Paperless/" + strng
      }, document.getElementById('viewer')).then(instance => {
        this.wvInstance = instance;
      })
    }

    this._chatSignalrService.componentMethodCalled$.subscribe(() => {
      //this.ngOnInit();
      // window.location.reload();
      
      let checkFirstFile = this.fileManager.instance.getCurrentDirectory();
      if (checkFirstFile.path == "") {
        window.location.reload();
      }
      else {
        this.fileManager.instance.refresh();
        this.fileManager1.instance.refresh();
      }
    });

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
          this.http.get(this.folderPath + "DownloadFile" + "?path=" + strng + "&key=" + strng1, { headers, responseType: 'blob' as 'json' }).subscribe((res: any) => {

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
}
