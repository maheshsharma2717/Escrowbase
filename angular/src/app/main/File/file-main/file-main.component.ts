import { Component, HostListener, Injector, Input, EventEmitter, OnInit, Output, Optional, Inject } from '@angular/core';
import { Observable } from '@node_modules/rxjs';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EscrowFileTagsesServiceProxy, SREscrowFileMastersServiceProxy, API_BASE_URL } from '@shared/service-proxies/service-proxies';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
@Component({
  selector: 'app-file-main',
  templateUrl: './file-main.component.html',
  styleUrls: ['./file-main.component.css']
})
export class FileMainComponent extends AppComponentBase {
  @Input() inputPerson: any;
  @Output() saveEvent = new EventEmitter<any>();
  @Input() onRefresh = new EventEmitter<any>();
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

  ngOnInit(): void {
    this.getAllFiles();
    this.updatePaginatedData();
    this.generatePageNumbers();
  }

  getAllFiles(): void {

    var queryParams = this.inputPerson;
    let Name = this.appSession.user.name + " " + this.appSession.user.surname;
    let subCompanyName = this.validFileName(atob(queryParams['sc']))
    let companyName = this.validFileName(atob(queryParams['c']))
    let EscrowTab = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + EscrowTab);
    this.userTypeFromStorage = userType;
    this.folderPath = `${companyName}/${subCompanyName}/${EscrowTab}/`
    this.parentPath = `${companyName}/${subCompanyName}/${EscrowTab}`
    this
      .getAllFilesApi(companyName, subCompanyName, EscrowTab, this.appSession.user.id.toString(), userType)
      .subscribe(
        (response) => {

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

    const filesToDelete = this.tableData.filter(file => {
      const id = file.srAssignedFileId || file.key;
      return this.selectedFiles.has(id);
    });

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
  onDocumentClick() {
    this.showContextMenu = false;
  }

  onRightClick(event: MouseEvent, index: number, selectedFile: any, mode: string) {
    this.handleShownEvent(selectedFile)
    event.preventDefault();
    this.selectedFile = selectedFile;
    this.selectedIndex = index;
    this.contextMenuPosition = { x: event.clientX, y: event.clientY };
    this.showContextMenu = true;
    const mouseX = event.pageX;
    const mouseY = event.pageY;
    const menuElement = document.getElementById("contextMenu");
    const menuWidth = menuElement?.offsetWidth || 150;
    const menuHeight = menuElement?.offsetHeight || 150;

    // Get screen dimensions
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    const scrollX = window.scrollX || document.documentElement.scrollLeft;
    const scrollY = window.scrollY || document.documentElement.scrollTop;

    // Adjust X position to prevent overflow
    const adjustedX = (mouseX + menuWidth > scrollX + screenWidth)
      ? scrollX + screenWidth - menuWidth
      : mouseX;

    // Adjust Y position to prevent overflow
    const adjustedY = (mouseY + menuHeight > scrollY + screenHeight)
      ? scrollY + screenHeight - menuHeight
      : mouseY;

    // Set context menu position
    this.contextMenuPosition = { x: adjustedX, y: adjustedY };
    this.showContextMenu = true;
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

  handleShownEvent(e) {

    this.readPermission = false;
    this.editPermission = false;

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

    try {
      // Provide visual feedback for the drag without actually putting native file URLs that break Chrome
      if (event.dataTransfer) {
        // A minimal string so Chrome knows a drag is happening
        event.dataTransfer.effectAllowed = 'copy';

        // Optional: Custom drag image if you have a specific icon, otherwise browser uses the clicked row.
        // It's important NOT to preventDefault() immediately if we want Chrome's native ghost image to appear.
        // But removing preventDefault() means Chrome might try to handle it.
        // Let's create a custom ghost element manually or let Chrome handle the visual aspect.
        const dragIcon = document.createElement('div');
        dragIcon.textContent = `📄 ${file.name}`;
        dragIcon.style.position = 'absolute';
        dragIcon.style.top = '-1000px';
        dragIcon.style.backgroundColor = 'white';
        dragIcon.style.padding = '5px 10px';
        dragIcon.style.border = '1px solid #ccc';
        dragIcon.style.borderRadius = '4px';
        dragIcon.style.boxShadow = '0 2px 5px rgba(0,0,0,0.2)';
        dragIcon.style.zIndex = '9999';
        document.body.appendChild(dragIcon);

        event.dataTransfer.setDragImage(dragIcon, 10, 10);

        // Clean up the temporary element shortly after drag starts
        setTimeout(() => {
          if (document.body.contains(dragIcon)) {
            document.body.removeChild(dragIcon);
          }
        }, 100);
      }

      // event.preventDefault() stops the HTML5 drag from finishing natively in Chrome, but we need the native drag to start to get the ghost image.
      // So we do NOT preventDefault() here since we WANT the drag visual to follow the mouse, but we override it via C# `ESC` immediately anyway!
      // event.preventDefault();

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
      fetch('https://localhost:5123/prepare-drag', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          downloadUrl: downloadUrl,
          fileName: file.key,
          token: token ? `Bearer ${token}` : undefined
        })
      }).then(response => {
        if (!response.ok) {
          abp.notify.error('Failed to prepare file for drag. The local drag service returned an error.');
          console.error('Drag service HTTP error:', response);
        }
      }).catch(err => {
        console.error('Drag service error:', err);
        // abp.notify.warn('Local drag service not running.');
        abp.message.confirm(
          'To drag file you need the Escrow Drag Tool installed and running.',
          'Escrow Drag Tool Required',
          async (isConfirmed) => {
            if (isConfirmed) {
              const response = await fetch(this.apiUrl + '/FileManager/DownloadDragDropExeFile', {
                method: 'GET'
              });            
              const blob = await response.blob();
              const url = window.URL.createObjectURL(blob);            
              const a = document.createElement('a');
              a.href = url;
              a.download = 'EscrowDragSetup.exe';
              document.body.appendChild(a);
              a.click();
              a.remove();
            }
          }
        );
      });
    } catch (error) {
      console.error('Error in onDragStart:', error);
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