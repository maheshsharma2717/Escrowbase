import { Component, HostListener, Injector, Input, EventEmitter, OnInit, Output, Optional, Inject } from '@angular/core';
import { Observable } from '@node_modules/rxjs';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EscrowFileTagsesServiceProxy, SREscrowFileMastersServiceProxy, API_BASE_URL } from '@shared/service-proxies/service-proxies';
import { HttpClient, HttpHeaders } from '@angular/common/http';
@Component({
  selector: 'app-file-main',
  templateUrl: './file-main.component.html',
  styleUrls: ['./file-main.component.css']
})
export class FileMainComponent extends AppComponentBase {
  @Input() inputPerson: any;
  @Output() saveEvent = new EventEmitter<any>();
  @Input() onRefresh = new EventEmitter<any>();
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
  sanitizer: any;
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
  editPermission: boolean;
  readPermission: boolean;
  viewHistoryPermission: boolean;
  downloadPermission: boolean;
  viewFullNamePermission: boolean;
  renamePermission: boolean;
  renameFileName: boolean;
  reminderPermission: boolean;
  deletePermission: boolean;
  esignPermission: boolean;
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
    debugger;
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
          debugger;
          this.tableData = response.result;
        },
        (error) => {
          console.error('Error fetching files:', error);
        }
      );
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
    debugger;
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
      let srId = this.selectedFile.srAssignedFileId;
      this.downloadname = key;
      const token = 'my JWT';
      const headers = new HttpHeaders().set('authorization', 'Bearer ' + token);
      var userId = abp.session.userId;
      this.http.get(this.apiUrl + "/FileManager/DownloadFile" + "?path=" + encodedPath + this.selectedFile.name + "&key=" + encodedKey + "&srAssignedFileId=" + srId + "&userId=" + userId, { headers, responseType: 'blob' as 'json' }).subscribe((response: any) => {
        let fileRes: any = response;
        let dataType = response.type;
        let binaryData = [];
        binaryData.push(response);
        let downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));

        if (this.downloadname.includes("~")) {
          this.downloadname = this.downloadname.replace("~", "~");
        }

        if (encodedKey) {
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

          }
          if (newResult == 200) {
            alert("File downloaded successfully :)");

          } else {

          }
        } catch (error) {
          alert(error);
        }

      });
    }
  }

  getFileExtension(filename) {
    const extension = filename.split('.').pop();
    return extension;
  }

  onItemClickMain(eventType) {
    debugger;
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
    debugger;
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

  toggleFullScreenMain(fullscreenElement: HTMLElement) {
    debugger;
    this.isFullScreenMain = !this.isFullScreenMain;
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
    debugger;
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

  
}