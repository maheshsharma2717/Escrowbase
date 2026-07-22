import { Component, HostListener, Injector, Input, EventEmitter, OnInit, Output, Optional, Inject } from '@angular/core';
import { Observable } from '@node_modules/rxjs';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EscrowFileTagsesServiceProxy, SREscrowFileMastersServiceProxy, API_BASE_URL } from '@shared/service-proxies/service-proxies';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
declare var Swal: any;
@Component({
  selector: 'app-file-main',
  templateUrl: './file-main.component.html',
  styleUrls: ['./file-main.component.css']
})
export class FileMainComponent extends AppComponentBase {
  @Input() inputPerson: any;
  @Output() saveEvent = new EventEmitter<any>();
  @Input() onRefresh = new EventEmitter<any>();
  @Output() onDragEnded = new EventEmitter<void>();
  @Input() isThunderbirdMode: boolean = false;
  selectedFiles: Set<string> = new Set();
  isAllSelected: boolean = false;
  selectedFile: any;
  contextMenuPosition = { x: 0, y: 0 };
  showContextMenu: boolean;
  apiUrl: string = '';
  files: any = [];
  selectedFileIndex: number | null = null;
  manageTagList: any[] = [];
  manageTagList2: any[] = [];
  allTagsList: any[] = [];
  fileTagService: any;
  isCollapsed: boolean = false;

  filteredFiles = [...this.files];
  availableTags: string[] = [];
  showTagSearch = false;
  showDropdown = false;
  searchQuery2: string = '';
  userTypeFromStorage: any;
  downloadname: any;
  folderPath: any;
  parentPath: any;
  tableData: any = [];
  @Input() editPermission: boolean;
  @Input() readPermission: boolean;
  @Input() viewHistoryPermission: boolean;
  @Input() downloadPermission: boolean;
  @Input() viewFullNamePermission: boolean;
  @Input() renamePermission: boolean;
  renameFileName: boolean;
  reminderPermission: boolean;
  @Input() deletePermission: boolean;
  @Input() esignPermission: boolean;
  isRename: boolean;
  isFullScreenMain: boolean = false;
  currentSortField: string = '';
  ascendingOrder: boolean = true;
  currentPage = 1;
  pageSize = 10;
  paginatedData = [];
  pages: number[] = [];
  selectedIndex: number | null = null;
  private debounceTimers: { [id: number]: any } = {};


  constructor(
    private escrowFileTagsServiceProxy: EscrowFileTagsesServiceProxy,
    private _srEscrowFileMastersServiceProxy: SREscrowFileMastersServiceProxy,

    private http: HttpClient,

    injector: Injector,
    public sanitizer: DomSanitizer,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string,
  ) {
    super(injector);
    this.apiUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
  }

  private globalScrollListener = (event: Event) => {
    const targetElement = event.target as HTMLElement;
    if (targetElement && targetElement.closest && targetElement.closest('.radical-context-menu')) {
      return;
    }

    if (this.showContextMenu) {
      this.showContextMenu = false;
    }
  };

  ngOnInit(): void {
    window.addEventListener('scroll', this.globalScrollListener, true);
    this.getAllFiles();
    this.updatePaginatedData();
    this.generatePageNumbers();
  }

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.globalScrollListener, true);
  }

  getAllFiles(): void {
    this.selectedFiles.clear();
    this.isAllSelected = false;
    var queryParams = this.inputPerson;
    let Name = this.appSession.user.name + " " + this.appSession.user.surname;
    let subCompanyName = this.validFileName(atob(queryParams['sc']))
    let companyName = this.validFileName(atob(queryParams['c']))
    let EscrowTab = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + EscrowTab);
    this.userTypeFromStorage = userType;
    this.folderPath = `${companyName}/${subCompanyName}/${EscrowTab}/`
    this.parentPath = `${companyName}/${subCompanyName}/${EscrowTab}`
debugger;

    this
      .getAllFilesApi(companyName, subCompanyName, EscrowTab, this.appSession.user.id.toString(), userType)
      .subscribe(
        (response) => {
debugger;
          this.tableData = response.result;
        },
        (error) => {
          console.error('Error fetching files:', error);
        }
      );
  }

  getFileIcon(file: any): SafeHtml {
    let fileName = file.key || file.name;
    let fileExtension = fileName.split('.').pop()?.toLowerCase();
    let iconHtml: string;

    switch (fileExtension) {
      case "pdf":
        iconHtml = "<i class='fas fa-file-pdf' style='color: #e74c3c;'></i>";
        break;
      case "xls":
      case "xlsx":
        iconHtml = "<i class='fas fa-file-excel' style='color: #27ae60;'></i>"; // Green Excel
        break;
      case "doc":
      case "docx":
        iconHtml = "<i class='fas fa-file-word' style='color: #2b78e4;'></i>"; // Blue Word
        break;
      case "txt":
        iconHtml = "<i class='fas fa-file-alt' style='color: #f39c12;'></i>"; // Orange Text
        break;
      case "eml":
        iconHtml = "<i class='fas fa-envelope' style='color: #16a085;'></i>"; // Green Email
        break;
      case "msg":
        iconHtml = "<i class='fas fa-envelope-open-text' style='color: #d35400;'></i>"; // Brown Message
        break;
      case "rtf":
        iconHtml = "<img src='https://cdn-icons-png.flaticon.com/512/337/337932.png' width='20' height='20' title='RTF File' alt='RTF File Icon'>"; // Purple RTF
        break;
      default:
        iconHtml = "<i class='fas fa-file' style='color: #95a5a6;'></i>"; // Default gray file
        break;
    }
    return this.sanitizer.bypassSecurityTrustHtml(iconHtml);
  }


  toggleSelectAll(event: any) {
    this.isAllSelected = event.target.checked;
    if (this.isAllSelected) {
      this.tableData.forEach(file => {
        if (file.srAssignedFileId) {
          this.selectedFiles.add(file.srAssignedFileId);
        } else if (file.key) {
          this.selectedFiles.add(file.key);
        }
      });
    } else {
      this.selectedFiles.clear();
    }
  }

  toggleSelection(file: any) {
    const id = file.srAssignedFileId || file.key;
    if (this.selectedFiles.has(id)) {
      this.selectedFiles.delete(id);
    } else {
      this.selectedFiles.add(id);
    }
    this.isAllSelected = this.selectedFiles.size === this.tableData.length && this.tableData.length > 0;
  }

  downloadSelected() {
    if (this.selectedFiles.size === 0) {
      abp.notify.warn('Please select files to download');
      return;
    }

    if (this.selectedFiles.size > 5) {
      this.downloadAsZip();
      return;
    }

    this.tableData.forEach(file => {
      const id = file.srAssignedFileId || file.key;
      if (this.selectedFiles.has(id)) {
        // Create a mock event for DownloadFile
        this.selectedFile = file;
        this.DownloadFile({});
      }
    });
  }

  downloadAsZip() {
    const filesToDownload = this.tableData.filter(file => {
      const id = file.srAssignedFileId || file.key;
      return this.selectedFiles.has(id);
    }).map(file => ({
      key: file.key,
      name: file.name
    }));

    const input = {
      parentPath: this.parentPath,
      files: filesToDownload,
      userId: this.appSession.user.id
    };

    this.notify.info('Compressing and downloading files...');

    this.http.post(`${this.apiUrl}/FileManager/DownloadZip`, input, { responseType: 'blob' })
      .subscribe((blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Documents_${new Date().getTime()}.zip`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        this.notify.success('Download completed');
      }, error => {
        console.error(error);
        this.notify.error('Failed to download zip');
      });
  }

  deleteSelected() {
    if (this.selectedFiles.size === 0) {
      abp.notify.warn('Please select files to delete');
      return;
    }

    const userRole = this.userTypeFromStorage || '';
    const isOfficer = userRole.startsWith("EO") || userRole.startsWith("EA");

    const filesToDelete = this.tableData.filter(file => {
      const id = file.srAssignedFileId || file.key;
      if (!this.selectedFiles.has(id)) return false;

      // Officers can always delete
      if (isOfficer) return true;

      // Regular users must have explicit delete permission
      let accesstype = file.access || '';
      return accesstype.includes("D") || accesstype.includes("DEL") || accesstype.includes("DELETE");
    });

    if (filesToDelete.length === 0) {
      abp.notify.warn('You do not have permission to delete the selected files');
      return;
    }

    var obj = {
      selectedFiles: filesToDelete,
      templateRef: 'deleteAll',
      folderPath: this.parentPath
    }
    this.saveEvent.emit(obj);
  }

  validFileName(folderName) {
    let newString = folderName.replace("<", "(").replace(">", ")").replace(":", ";").replace("*", "'").replace("/", "-").replace("?", "+").replace("|", "_").replace("*", ".").replace("\/", "=");
    let str = newString.charAt(newString.length - 1)
    if (str == ".") {
      newString = newString.replace(str, "");
    }
    return newString
  }

  getAllFilesApi(company: string, subCompany: string, escrow: string, userId: string, userType: string): Observable<any> {
    let argumentsData = {
      pathInfo: [
        { key: company, name: company },
        { key: subCompany, name: subCompany },
        { key: escrow, name: escrow }
      ],
    };

    const argumentsString = JSON.stringify(argumentsData);

    // Use template literals for better readability and proper URL construction
    const url = `${this.apiUrl}/FileManager/FileSystem?company=${encodeURIComponent(company)}&subCompany=${encodeURIComponent(subCompany)}&escrow=${encodeURIComponent(escrow)}&userId=${encodeURIComponent(userId)}&usertype=${userType}&usersname=${encodeURIComponent(this.appSession.user.name)} ${encodeURIComponent(this.appSession.user.surname)}&arguments=${encodeURIComponent(argumentsString)}`;

    return this.http.get(url);
  }

  @HostListener("document:click")
  @HostListener("document:contextmenu")
  onDocumentClick() {
    this.showContextMenu = false;
  }

  @HostListener('document:dragenter', ['$event'])
  onDragEnter(event: DragEvent) {
    event.preventDefault();
  }

  @HostListener('document:dragover', ['$event'])
  onDragOver(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'copy';
    }
  }

  onRightClick(event: MouseEvent, index: number, selectedFile: any, mode: string) {
    this.handleShownEvent(selectedFile)
    event.preventDefault();
    this.selectedFile = selectedFile;
    this.selectedIndex = index;
    this.contextMenuPosition = { x: event.clientX, y: event.clientY };
    this.showContextMenu = true;
    const mouseX = event.clientX;
    const mouseY = event.clientY;
    
    setTimeout(() => {
      // 1. Render at mouse position first (after document:contextmenu has closed others)
      this.contextMenuPosition = { x: mouseX, y: mouseY };
      this.showContextMenu = true;

      // 2. Wait for Angular to update the DOM, then measure and adjust
      setTimeout(() => {
        const menus = document.getElementsByClassName("radical-context-menu");
        if (menus.length > 0) {
          const menuElement = menus[0] as HTMLElement;
          const menuWidth = menuElement.offsetWidth || 200;
          const menuHeight = menuElement.offsetHeight || 350;

          const screenWidth = window.innerWidth;
          const screenHeight = window.innerHeight;

          let adjustedX = mouseX;
          if (mouseX + menuWidth > screenWidth) {
            adjustedX = screenWidth - menuWidth;
          }

          let adjustedY = mouseY;
          if (mouseY + menuHeight > screenHeight) {
            // Open upwards from the mouse
            adjustedY = mouseY - menuHeight;
            if (adjustedY < 0) adjustedY = 10;
          }

          this.contextMenuPosition = { x: adjustedX, y: adjustedY };
        }
      }, 10);
    }, 0);
  }

  DownloadFile(event) {

    let compare;
    let strcheck;
    if (!this.selectedFile.key) {
      return;
    }
    let action = this.selectedFile.key;
    if (action.includes("~")) {
      action = action.substring(action.indexOf("~") + 1);
      compare = this.userTypeFromStorage;
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
      bcheck = strcheck.indexOf("R");
    }
    if (bcheck === -1 && strcheck != undefined && abp.session.userId != 1) {
      return;
    } else {
      let path = this.folderPath;
      let key = this.selectedFile.key;
      let encodedPath = path.replace(/#/g, "%23");
      let encodedKey = key.replace(/#/g, "%23");
      let srId = this.selectedFile.srAssignedFileId || 0;
      this.downloadname = key;
      let userId = abp.session.userId;
      const token = abp.auth.getToken();

      const downloadUrl = this.apiUrl + "/FileManager/DownloadFile" +
        "?path=" + encodeURIComponent(encodedPath + this.selectedFile.name) +
        "&key=" + encodeURIComponent(encodedKey) +
        "&srAssignedFileId=" + srId +
        "&userId=" + userId +
        "&enc_auth_token=" + encodeURIComponent(token);

      const iframe = document.createElement('iframe');
      iframe.style.display = 'none';
      iframe.src = downloadUrl;
      document.body.appendChild(iframe);

      setTimeout(() => {
        document.body.removeChild(iframe);
      }, 60000);
    }
  }

  getFileExtension(filename) {
    const extension = filename.split('.').pop();
    return extension;
  }

  onItemClickMain(eventType) {

    var obj = {
      selectedFile: this.selectedFile,
      templateRef: eventType,
      folderPath: this.parentPath
    }
    this.saveEvent.emit(obj);
  }

  changeColor(event: MouseEvent) {
    const tbodyElement = event.target as HTMLElement;

    if (tbodyElement.tagName === 'TBODY') {
      tbodyElement.style.backgroundColor = tbodyElement.style.backgroundColor === 'white' ? '#royalblue' : 'white';
    }
  }

  canDeleteSelected(): boolean {
    const userRole = this.userTypeFromStorage || '';
    const isOfficer = userRole.startsWith("EO") || userRole.startsWith("EA");
    if (isOfficer) return true;

    if (this.selectedFiles.size === 0) return false;

    const selectedList = this.tableData.filter(file => {
      const id = file.srAssignedFileId || file.key;
      return this.selectedFiles.has(id);
    });

    if (selectedList.length === 0) return false;

    return selectedList.every(file => {
      let accesstype = file.access || '';
      return accesstype.includes("D") || accesstype.includes("DEL") || accesstype.includes("DELETE");
    });
  }

  handleShownEvent(e) {
    this.readPermission = false;
    this.editPermission = false;
    this.viewHistoryPermission = false;
    this.downloadPermission = false;
    this.viewFullNamePermission = false;
    this.renamePermission = false;
    this.deletePermission = false;
    this.esignPermission = false;
    this.reminderPermission = false;
    this.renameFileName = true;

    let accesstype = e.access || '';

    if (accesstype.includes("R")) {
      this.readPermission = true;
      this.viewHistoryPermission = true;
      this.downloadPermission = true;
      this.viewFullNamePermission = true;
    }
    if (accesstype.includes("E") || accesstype.includes("INPUT")) {
      this.readPermission = true;
      this.editPermission = true;
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

    const userRole = this.userTypeFromStorage || '';
    const isOfficer = userRole.startsWith("EO") || userRole.startsWith("EA");

    if (isOfficer || accesstype.includes("D") || accesstype.includes("DEL") || accesstype.includes("DELETE")) {
      this.readPermission = true;
      this.editPermission = true;
      this.viewHistoryPermission = true;
      this.renamePermission = true;
      this.downloadPermission = true;
      this.viewFullNamePermission = true;
      this.deletePermission = true;
      this.renameFileName = false;
    }
    if (isOfficer || accesstype.includes("S") || accesstype.includes("SIGN")) {
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
      let hasEditOrDeleteAccess = false;
      if (accesstype) {
        hasEditOrDeleteAccess = accesstype.includes("D") || accesstype.includes("DEL") || 
                                accesstype.includes("DELETE") || accesstype.includes("E") || 
                                accesstype.includes("INPUT");
      }
      if (isOfficer || hasEditOrDeleteAccess) {
        this.renamePermission = true;
        this.renameFileName = false;
      } else {
        this.renamePermission = false;
        this.renameFileName = true;
      }
    }
    console.log("CHILD handleShownEvent details:", {
      filename: e.name,
      access: e.access,
      signing: e.signing,
      userRole: userRole,
      isOfficer: isOfficer,
      renamePermission: this.renamePermission
    });
  }

  @Output() fullScreenToggled = new EventEmitter<boolean>();

  toggleFullScreenMain(fullscreenElement: HTMLElement) {

    this.isFullScreenMain = !this.isFullScreenMain;
    this.fullScreenToggled.emit(this.isFullScreenMain);
  }

  sortTable(field: string) {
    if (this.currentSortField === field) {
      this.ascendingOrder = !this.ascendingOrder;
    } else {
      this.currentSortField = field;
      this.ascendingOrder = true;
    }

    this.tableData.sort((a, b) => {
      const aValue = a[field];
      const bValue = b[field];

      if (aValue === bValue) return 0;
      return (aValue > bValue ? 1 : -1) * (this.ascendingOrder ? 1 : -1);
    });
  }

  get totalPages(): number {
    return Math.ceil(this.tableData.length / this.pageSize);
  }

  updatePaginatedData() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedData = this.tableData.slice(startIndex, endIndex);
  }

  generatePageNumbers() {
    this.pages = Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePaginatedData();
    }
  }

  onOtherActionChanged(file: any): void {

    if (!file?.srAssignedFileId) {
      this.notify.warn('Invalid file selected');
      return;
    }
    this._srEscrowFileMastersServiceProxy.getSREscrowFileMasterForEdit(file.srAssignedFileId)
      .subscribe(result => {
        const detail = result.srEscrowFileMaster;
        detail.otherAction = !file.otherAction;
        this._srEscrowFileMastersServiceProxy.createOrEdit(detail)
          .subscribe(() => {
            file.otherAction = detail.otherAction; // Update local state
            this.notify.success('Updated successfully');
          }, err => {
            this.notify.error('Update failed');
          });
      });
  }

  onTextareaInput(event: Event, file: any): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';

    if (!file?.srAssignedFileId) {
      return;
    }

    if (this.debounceTimers[file.srAssignedFileId]) {
      clearTimeout(this.debounceTimers[file.srAssignedFileId]);
    }

    this.debounceTimers[file.srAssignedFileId] = setTimeout(() => {
      this._srEscrowFileMastersServiceProxy.getSREscrowFileMasterForEdit(file.srAssignedFileId)
        .subscribe(result => {
          const detail = result.srEscrowFileMaster;
          detail.otherActionNote = file.otherActionNote;

          this._srEscrowFileMastersServiceProxy.createOrEdit(detail)
            .subscribe(() => {
              // success
            }, err => {
              // error
            });
        });
    }, 500);
  }

  collapseTextarea(event: Event, file: any): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = '20px';
    file.otherActionNote = file.otherActionNote?.trim();
  }



  expandTextarea(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    setTimeout(() => {
      textarea.style.height = 'auto';
      textarea.style.height = textarea.scrollHeight + 'px';
    }, 0);
  }

  onDragStart(event: DragEvent, file: any) {
    if (!file || !file.key) {
      return;
    }
      let key = file.key;
       let path = this.folderPath;
      let encodedPath = path.replace(/#/g, "%23");
      let encodedKey = key.replace(/#/g, "%23");
      let srId = file.srAssignedFileId || '';
      let userId = this.appSession.userId;
      const token = abp.auth.getToken();

      const downloadUrl = this.apiUrl + "/FileManager/DownloadFile" +
        "?path=" + encodeURIComponent(encodedPath + file.name) +
        "&key=" + encodeURIComponent(encodedKey) +
        "&srAssignedFileId=" + srId +
        "&userId=" + userId +
        "&enc_auth_token=" + encodeURIComponent(token);

      const payload = {
        downloadUrl: downloadUrl,
        fileName: file.key,
        token: token ? `Bearer ${token}` : undefined,
        screenX: (event as MouseEvent).screenX,
        screenY: (event as MouseEvent).screenY
      };
    try {
      if (event.dataTransfer) {
        event.dataTransfer.effectAllowed = 'all';

        const dragIcon = document.createElement('div');
        dragIcon.textContent = `📥 ${file.name}`;
       //dragIcon.textContent = `📄 ${file.name}`;
        dragIcon.style.position = 'absolute';
        dragIcon.style.top = '-1000px';
        dragIcon.style.backgroundColor = 'white';
        dragIcon.style.padding = '15px 15px';
        dragIcon.style.border = '1px solid #ccc';
        dragIcon.style.borderRadius = '4px';
        dragIcon.style.fontSize = '15px';
        dragIcon.style.boxShadow = '0 2px 5px rgba(24, 24, 24, 0.32)';
        dragIcon.style.zIndex = '9999';
        dragIcon.style.color = '#333';
        document.body.appendChild(dragIcon);

        event.dataTransfer.setDragImage(dragIcon, 55, 55);

        setTimeout(() => {
          if (document.body.contains(dragIcon)) {
            document.body.removeChild(dragIcon);
          }
        }, 100);

        
        if (!this.isThunderbirdMode) {
            event.dataTransfer.setData('text/uri-list', '\\\\.\\NUL');
          // event.dataTransfer.setData('text/uri-list', `${payload}`);
        }
     
        // To start a drag, we must add some valid data format or the drag will abort in Chromium.
        event.dataTransfer.setData('text/plain', '\u200B');
        event.dataTransfer.setData('text/html', '<span style="display:none;">\u200B</span>');
        event.dataTransfer.setData('application/x-escrow-file', file.name);           
        //event.dataTransfer.effectAllowed = "copy";

      }
    } catch (error) {
      console.error('Error in onDragStart:', error);
    }
  }

  onDragEnd(event: DragEvent, file: any) {
    // Instantly ping the C# background service to delete dummy shortcuts  
    if (!file || !file.key) {
      return;
    }

    try {
      let path = this.folderPath;
      let key = file.key;
      let encodedPath = path.replace(/#/g, "%23");
      let encodedKey = key.replace(/#/g, "%23");
      let srId = file.srAssignedFileId || '';
      let userId = this.appSession.userId;
      const token = abp.auth.getToken();

      const downloadUrl = this.apiUrl + "/FileManager/DownloadFile" +
        "?path=" + encodeURIComponent(encodedPath + file.name) +
        "&key=" + encodeURIComponent(encodedKey) +
        "&srAssignedFileId=" + srId +
        "&userId=" + userId +
        "&enc_auth_token=" + encodeURIComponent(token);

      const payload = {
        downloadUrl: downloadUrl,
        fileName: file.key,
        token: token ? `Bearer ${token}` : undefined,
        screenX: (event as MouseEvent).screenX,
        screenY: (event as MouseEvent).screenY
      };

      const base64Data = btoa(unescape(encodeURIComponent(JSON.stringify(payload))));
      const protocolUrl = `escrow-drag://prepare-drag?data=${encodeURIComponent(base64Data)}`;

      let appOpened = false;

      const handler = () => {
        appOpened = true;
        document.removeEventListener("visibilitychange", handler);
      };

      document.addEventListener("visibilitychange", handler);

      const iframe = document.createElement("iframe");
      iframe.style.display = "none";
      iframe.src = protocolUrl;
      document.body.appendChild(iframe);
      setTimeout(() => {
        document.body.removeChild(iframe);
      }, 2000);
      
      // this.onDragEnded.emit();
    } catch (error) {
      console.error('Error in onDragEnd:', error);
    }
  }

  getMimeType(fileName: string): string {
    const ext = fileName.split('.').pop()?.toLowerCase();
    const mimeTypes: { [key: string]: string } = {
      'pdf': 'application/pdf',
      'doc': 'application/msword',
      'docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      'xls': 'application/vnd.ms-excel',
      'xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'png': 'image/png',
      'jpg': 'image/jpeg',
      'jpeg': 'image/jpeg',
      'txt': 'text/plain',
      'msg': 'application/vnd.ms-outlook',
      'eml': 'message/rfc822'
    };
    return mimeTypes[ext] || 'application/octet-stream';
  }
}