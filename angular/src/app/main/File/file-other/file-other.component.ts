import { Component, HostListener, Injector, OnInit, Output, ViewChild, EventEmitter, TemplateRef, Input, Optional, Inject, ElementRef, ChangeDetectorRef } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EscrowFileTagsesServiceProxy, API_BASE_URL, TagsAndFileMappingsesServiceProxy, CreateOrEditTagsAndFileMappingsDto } from '@shared/service-proxies/service-proxies';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
declare var Swal: any;
import { EscrowUsertagsComponent } from '@app/main/escrow-usertags/escrow-usertags.component';
import { SafeHtml } from '@angular/platform-browser';
import { FileMainComponent } from '../file-main/file-main.component';

@Component({
  selector: 'app-file-other',
  templateUrl: './file-other.component.html',
  styleUrls: ['./file-other.component.css']
})

export class FileOtherComponent extends AppComponentBase {
  file: any;
  openContextMenu($event: any, arg1: any) {
    throw new Error('Method not implemented.');
  }
  @ViewChild('escrowUsertagsComponent', { static: true }) escrowUsertagsComponent: EscrowUsertagsComponent;
  @ViewChild('dropdownWrapper', { static: false }) dropdownWrapper!: ElementRef;
  @Output() saveEvent = new EventEmitter<any>();
  @Output() onDragEnded = new EventEmitter<void>();
  @Input() inputPerson: any;
  @Input() deletePermission: boolean;
  @Input() isThunderbirdMode: boolean = false;
  selectedFiles: Set<string> = new Set();
  isAllSelected: boolean = false;
  apiUrl: string = "";
  files: any = [];
  showContextMenuTags: boolean = false;
  showContextMenu = false;
  contextMenuPosition = { x: 0, y: 0 };
  selectedFile: any = null;
  searchQuery: string = '';
  selectedFileIndex: number | null = null;
  manageTagList: any[] = [];
  manageTagList2: any[] = [];
  allTagsList: any[] = [];
  fileTagService: any;
  isCollapsed: boolean = false;
  isFullScreen: boolean = false;
  // sanitizer: any; // Removed as it is now injected
  filteredFiles = [...this.files];
  availableTags: string[] = [];
  showTagSearch = false;
  showDropdown = false;
  searchQuery2: string = '';
  tags = [];
  userTypeFromStorage: string;
  folderPath: string;
  parentPath: string;
  tableData: any;
  completeEnterprisePathOther: string;
  editPermissionOtherArea: boolean;
  readPermissionOtherArea: boolean;
  editPermissionDocOtherArea: boolean;
  downloadPermission: boolean = false;
  currentSortField: string = '';
  ascendingOrder: boolean = true;
  selectedIndex: number | null = null;
  selectedTagIndex2: number | null = null;
  selectedRowIndex: number | null = null;


  constructor(
    private escrowFileTagsServiceProxy: EscrowFileTagsesServiceProxy,
    private http: HttpClient,
    injector: Injector,
    private tagsAndFileMapping: TagsAndFileMappingsesServiceProxy,
    public sanitizer: DomSanitizer,
    public cdr: ChangeDetectorRef,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string

  ) {
    super(injector);
    this.apiUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
  }

  ngOnInit(): void {
    this.getAllFiles();
    this.getAllFileTags();
  }

  getAllFiles(person?: any): void {
    this.selectedFiles.clear();
    this.isAllSelected = false;
    var queryParams = person || this.inputPerson;
    if (!queryParams) {
      console.error("getAllFiles: inputPerson is null or undefined");
      return;
    }
    let Name = this.appSession.user.name + " " + this.appSession.user.surname;
    let subCompanyName = this.validFileName(atob(queryParams['sc']))
    let companyName = this.validFileName(atob(queryParams['c']))
    let EscrowTab = localStorage.getItem("activeTab")
    let userType = localStorage.getItem("accessTYpe" + EscrowTab);
    this.completeEnterprisePathOther = `${companyName}/${subCompanyName}/${EscrowTab}/Other/`

    this
      .getAllFilesApi(companyName, subCompanyName, EscrowTab, this.appSession.user.id.toString())
      .subscribe(
        (response) => {
          debugger;
          this.files = response.result;
          const savedField = localStorage.getItem('otherSortField');
          const savedOrder = localStorage.getItem('otherSortOrder');

          if (savedField) {
            this.currentSortField = savedField;
            this.ascendingOrder = savedOrder === 'asc';
            this.applySorting();
          }

          // Force change detection to ensure view updates
          this.cdr.detectChanges();
        },

        (error) => {
          console.error('Error fetching files:', error);
        }
      );
  }

  @HostListener('click', ['$event'])
  onGlobalClick(event: MouseEvent) {
    console.log('Global click detected'); // Debugging to confirm click capture
  }

  toggleDropdown(): void {
    debugger;
    this.showDropdown = !this.showDropdown;
  }

  filteredTags(): string[] {
    debugger;
    return this.tags.filter(tag => tag.toLowerCase().includes(this.searchQuery.toLowerCase()));
  }

  selectTag3(tag: any): void {
    debugger;
    if (tag === 'all') {
      this.getAllFiles();
    } else if (tag.escrowFileTags !== null) {
      this.getAllFilesApi("Enterprise", "Enterprise", "ESCROW", "689")
        .subscribe(
          (response) => {
            debugger;
            this.files = response.result.filter((file: any) =>
              Array.isArray(file.escrowFileTags) &&
              file.escrowFileTags.some((t: any) => t.id === tag.id)
            );
          },
          (error) => {
            console.error("Error fetching files:", error);
          }
        );
    }
    this.showDropdown = false;
  }

  getUniqueTags(): string[] {
    const tagsSet = new Set<string>();
    this.files.forEach(file => {
      file.escrowFileTags.forEach(tag => {
        tagsSet.add(tag.tagDescription);
      });
    });
    return Array.from(tagsSet);
  }

  filterFilesByTag(tag: string): void {
    debugger;
    this.filteredFiles = this.files.filter(file =>
      file.escrowFileTags.some(t => t.tagDescription.toLowerCase() === tag.toLowerCase())
    );
    this.showTagSearch = false;
  }

  getFileIcon(fileName: string): SafeHtml {
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

  @Output() onFileDropped = new EventEmitter<any>();
  isDragOver = false;
  dragCounter = 0;

  @HostListener('dragenter', ['$event'])
  onDragEnter(event: DragEvent) {
    if (!this.isThunderbirdMode) {
      if (this.isValidDragEvent(event)) {
        event.preventDefault();
        event.stopPropagation();
        this.dragCounter++;
        this.isDragOver = true;
      }
    }
  }

  @HostListener('dragleave', ['$event'])
  onDragLeave(event: DragEvent) {
    if (!this.isThunderbirdMode) {
      event.preventDefault();
      event.stopPropagation();
      this.dragCounter--;
      if (this.dragCounter === 0) {
        this.isDragOver = false;
      }
    }
  }

  @HostListener('dragover', ['$event'])
  onDragOver(event: DragEvent) {
    if (!this.isThunderbirdMode) {
      event.preventDefault();
      event.stopPropagation();
      if (event.dataTransfer) {
        event.dataTransfer.dropEffect = 'copy';
      }
    }
  }

  @HostListener('drop', ['$event'])
  onDrop(event: DragEvent) {
    if (!this.isThunderbirdMode) {
      event.preventDefault();
      event.stopPropagation();
      this.dragCounter = 0;
      this.isDragOver = false;
    }
  }

  onOverlayDragOver(event: DragEvent) {
    if (!this.isThunderbirdMode) {
      event.preventDefault();
      event.stopPropagation();
      if (event.dataTransfer) {
        event.dataTransfer.dropEffect = 'copy';
      }
    }
  }

  onOverlayDrop(event: DragEvent) {
    if (!this.isThunderbirdMode) {
      event.preventDefault();
      event.stopPropagation();
      this.dragCounter = 0;
      this.isDragOver = false;

      if (event.dataTransfer && event.dataTransfer.files && event.dataTransfer.files.length > 0) {
        this.onFileDropped.emit(event.dataTransfer.files);
      }
    }
  }

  isValidDragEvent(event: DragEvent): boolean {
    if (event.dataTransfer && event.dataTransfer.types) {
      for (let i = 0; i < event.dataTransfer.types.length; i++) {
        if (event.dataTransfer.types[i] === 'Files') {
          return true;
        }
      }
    }
    return false;
  }
  onRightClick(event: MouseEvent, index: number, selectedFile: any, mode: any) {
    event.preventDefault();
    this.handleShownEventOtherArea(selectedFile);

    this.selectedFile = selectedFile;
    this.selectedIndex = index;

    const mouseX = event.pageX;
    const mouseY = event.pageY;

    this.showContextMenu = mode !== 'Tags';
    this.showContextMenuTags = mode === 'Tags';

    setTimeout(() => {
      const menuElement = document.getElementById("contextMenu");
      const container = document.querySelector('.table-container') as HTMLElement;

      if (!menuElement || !container) return;

      const menuWidth = menuElement.offsetWidth || 150;
      const menuHeight = menuElement.offsetHeight || 170;

      const containerRect = container.getBoundingClientRect();

      // Calculate relative to container
      let adjustedX = mouseX - containerRect.left + container.scrollLeft;
      let adjustedY = mouseY - containerRect.top + container.scrollTop;

      // Prevent overflow (right and bottom)
      const maxX = container.scrollWidth - menuWidth - 10;
      const maxY = container.scrollHeight - menuHeight - 10;

      adjustedX = Math.min(adjustedX, maxX);
      adjustedY = Math.min(adjustedY, maxY);

      // Avoid negative values
      adjustedX = Math.max(0, adjustedX);
      adjustedY = Math.max(0, adjustedY);

      this.contextMenuPosition = { x: adjustedX, y: adjustedY };
    }, 50);
  }

  onRightClick2(event: MouseEvent, index: number | null, file: any | null, type: string): void {
    if (event.button === 2) {
      event.preventDefault();
      console.log("Right-click detected");
    } else {
      console.log("Left-click detected");
    }

    this.selectedFileIndex = index;
    this.selectedFile = file;
    this.selectedTagIndex2 = index;

    const mouseX = event.pageX;
    const mouseY = event.pageY;

    const menuElement = document.getElementById("contextMenu");
    const menuWidth = menuElement?.offsetWidth || 150;
    const menuHeight = menuElement?.offsetHeight || 150;
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    const scrollX = window.scrollX || document.documentElement.scrollLeft;
    const scrollY = window.scrollY || document.documentElement.scrollTop;

    const adjustedX = (mouseX + menuWidth > scrollX + screenWidth)
      ? scrollX + screenWidth - menuWidth
      : mouseX;

    const adjustedY = (mouseY + menuHeight > scrollY + screenHeight)
      ? scrollY + screenHeight - menuHeight
      : mouseY;

    this.contextMenuPosition = { x: adjustedX, y: adjustedY };

    if (type === "Tags") {
      this.showContextMenu = false;
      this.showContextMenuTags = true;
    } else {
      this.showContextMenuTags = false;
      this.showContextMenu = true;
    }
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    const targetElement = event.target as HTMLElement;
    if (targetElement && !targetElement.closest('.context-menu')) {
      this.showContextMenu = false;
      this.showContextMenuTags = false;
    }
    if (
      this.showDropdown &&
      this.dropdownWrapper &&
      !this.dropdownWrapper.nativeElement.contains(targetElement)
    ) {
      this.showDropdown = false;
    }
  }

  onItemClickMain(event) {
    debugger;
    var obj = {
      selectedFile: this.selectedFile,
      templateRef: event,
      folderPath: this.completeEnterprisePathOther
    }
    this.saveEvent.emit(obj);
    this.showContextMenu = false;
  }

  handleShownEventOtherArea(e) {
    let fileName = e.name
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

  moveToMain($event) {
    this.showContextMenu = false;
    this.selectedFile.path = this.completeEnterprisePathOther;
    var obj = {
      selectedFile: this.selectedFile,
      templateRef: "Move",
    }
    this.saveEvent.emit(obj);
    this.getAllFiles();
  }

  deleteFile(event) {
    debugger;
    this.selectedFile.path = this.completeEnterprisePathOther;
    var obj = {
      selectedFile: this.selectedFile,
      templateRef: "delete1",
    }
    this.saveEvent.emit(obj);
  }

  Opentags() {
    this.escrowUsertagsComponent.show();
    this.showContextMenuTags = false;
  }

  validFileName(folderName) {
    let newString = folderName.replace("<", "(").replace(">", ")").replace(":", ";").replace("*", "'").replace("/", "-").replace("?", "+").replace("|", "_").replace("*", ".").replace("\/", "=");
    let str = newString.charAt(newString.length - 1)
    if (str == ".") {
      newString = newString.replace(str, "");
    }
    return newString
  }

  getAllFilesApi(company: string, subCompany: string, escrow: string, userId: string): Observable<any> {
    debugger;
    let argumentsData: any = {
      pathInfo: [
        { key: company, name: company },
        { key: subCompany, name: subCompany },
        { key: escrow, name: escrow },
        { key: 'Other', name: 'Other' },
      ],
    };

    const argumentsString = JSON.stringify(argumentsData);
    const url = `${this.apiUrl}/FileManager/FileSystem1?company=${company}&subCompany=${subCompany}&escrow=${escrow}&userId=${userId}&arguments=${argumentsString}&_ts=${Date.now()}`;
    return this.http.get(url);
  }

  items: string[] = [];
  selectTag(tag: any) {
    debugger;
    if (this.selectedFileIndex !== null) {
      if (!this.files[this.selectedFileIndex].escrowFileTags) {
        this.files[this.selectedFileIndex].escrowFileTags = [];
      }
      this.files[this.selectedFileIndex].escrowFileTags.push(tag);
      this.showContextMenuTags = false;
    }
  }

  get filteredItems() {
    if (!this.searchQuery) {
      return this.manageTagList.slice(0, 8);
    } else {
      this.manageTagList = this.allTagsList;
      return this.manageTagList.filter(item =>
        item.tagDescription?.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
    }
  }

  onSearchTags() {
    debugger;
    this.getAllFileTags();
    if (!this.searchQuery) {
      return this.manageTagList;
    }
    return this.allTagsList.filter(item =>
      item.escrowFileTags?.tagDescription?.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  filteredItems2() {
    if (!this.searchQuery2?.trim()) {
      return this.allTagsList.slice(0, 8);
    }
    return this.allTagsList
      .filter(item =>
        item.tagDescription.toLowerCase().includes(this.searchQuery2.toLowerCase())
      )
      .slice(0, 8);
  }

  getAllFileTags() {
    debugger;
    this.escrowFileTagsServiceProxy
      .getAll(undefined, undefined, 0, 10000)
      .subscribe(
        (response: any) => {
          this.allTagsList = response.items.map(item => ({
            ...item.escrowFileTags,
            tagColor: item.escrowFileTags.tagColor.split(',')[0],
          }));
          this.manageTagList = this.allTagsList;
        },
        (error) => {
          console.error('Error fetching tags:', error);
        }
      );
  }

  selectTag2(item: any) {
    debugger;
    const tagData: CreateOrEditTagsAndFileMappingsDto = {
      tagId: item.id,
      fileName: this.selectedFile.name,
      id: 0,
      init: () => { },
      toJSON: () => ({
        tagId: item.id,
        fileName: this.selectedFile.name,
        id: 0,
      })
    };
    this.tagsAndFileMapping.createOrEdit(tagData).subscribe({
      next: (response: any) => {
        if (response?.success === false) {
          abp.notify.error(response.message || 'Failed to assign tag.', 'Error');
          return;
        }

        abp.notify.success(response?.message || 'Tag added to file', 'Success');
        if (this.selectedFile?.escrowFileTags) {
          this.selectedFile.escrowFileTags.push({
            id: tagData.tagId,
            name: item.name,
            bgColor: item.bgColor || '#ffffff',
            fontColor: item.fontColor || '#000000'
          });
        }

        this.getAllFiles();
        this.getAllFileTags();
      },
      error: (err) => {
        console.error('Error saving tag:', err);
        abp.notify.error('This tag is already assigned to the file', 'Error');
      }
    });
    this.showContextMenuTags = false;
  }

  toggleCollapse() {
    debugger
    this.isCollapsed = !this.isCollapsed;
  }

  @Output() fullScreenToggled = new EventEmitter<boolean>();

  toggleFullScreen(fullscreenElement: HTMLElement) {
    this.isFullScreen = !this.isFullScreen;
    this.fullScreenToggled.emit(this.isFullScreen);
  }

  onChildModalClose(): void {
    this.getAllFileTags();
  }

  removeTag(id: any, fileName: any) {
    debugger;
    this.tagsAndFileMapping
      .deleteTagByFileNameAndTagId(id, fileName)
      .subscribe(
        (response: any) => {
          abp.notify.success('Tag removed successfully', 'Success');
          this.getAllFiles();
          this.getAllFileTags();
        })
  }

  trackByTagId(index: number, item: any): any {
    return item.id;
  }

  // sortTable(field: string) {
  //   debugger;
  //   if (this.currentSortField === field) {
  //     this.ascendingOrder = !this.ascendingOrder;
  //   } else {
  //     this.currentSortField = field;
  //     this.ascendingOrder = true;
  //   }

  //   this.files.sort((a, b) => {
  //     let aValue = this.getSortValue(a, field);
  //     let bValue = this.getSortValue(b, field);

  //     // Normalize null/undefined/empty values
  //     aValue = (aValue === null || aValue === undefined || aValue === '') ? null : aValue;
  //     bValue = (bValue === null || bValue === undefined || bValue === '') ? null : bValue;

  //     // Handle nulls explicitly
  //     if (aValue === null && bValue === null) return 0;
  //     if (aValue === null) return this.ascendingOrder ? 1 : -1;
  //     if (bValue === null) return this.ascendingOrder ? -1 : 1;

  //     // Standard comparison
  //     if (aValue === bValue) return 0;
  //     return (aValue > bValue ? 1 : -1) * (this.ascendingOrder ? 1 : -1);
  //   });
  // }

  sortTable(field: string) {
    debugger;
    if (this.currentSortField === field) {
      this.ascendingOrder = !this.ascendingOrder;
    } else {
      this.currentSortField = field;
      this.ascendingOrder = true;
    }
    localStorage.setItem('otherSortField', this.currentSortField);
    localStorage.setItem('otherSortOrder', this.ascendingOrder ? 'asc' : 'desc');
    this.applySorting();
  }

  applySorting() {
    this.files.sort((a, b) => {
      let aValue = this.getSortValue(a, this.currentSortField);
      let bValue = this.getSortValue(b, this.currentSortField);
      if (aValue === bValue) return 0;
      return (aValue > bValue ? 1 : -1) * (this.ascendingOrder ? 1 : -1);
    });
  }

  getSortValue(file: any, field: string): string | null {
    if (field === 'tags') {
      const tagString = file.escrowFileTags?.map(tag => tag.tagDescription).join(', ');
      return tagString || null;
    }

    const value = file[field];
    return (value === undefined || value === null || value === '') ? null : value;
  }

  onRowClick(index: number) {
    this.selectedRowIndex = index;
  }

  onDragStart(event: DragEvent, file: any) {
    if (!file || !file.key) {
      return;
    }

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

        // To start a drag, we must add some valid data format or the drag will abort in Chromium.
        // We use invisible zero-width strings so Gmail accepts the drop (removes no-drop cursor)
        // without pasting huge text strings natively.
        event.dataTransfer.setData('text/plain', '\u200B');
        event.dataTransfer.setData('text/html', '<span style="display:none;">\u200B</span>');
        event.dataTransfer.setData('application/x-escrow-file', file.name);

        // Trick Windows File Explorer into accepting the drag (removes the 🚫 icon)
        // Only apply if not in Thunderbird mode to prevent blue links/0-byte attachments
        if (!this.isThunderbirdMode) {
          event.dataTransfer.setData('text/uri-list', '\\\\.\\NUL');
        }
      }
    } catch (error) {
      console.error('Error in onDragStart:', error);
    }
  }

  onDragEnd(event: DragEvent, file: any) {
    // Instantly ping the C# background service to delete dummy shortcuts
    fetch('http://localhost:5123/cleanup', { method: 'DELETE' }).catch(e => console.log('Cleanup fetch failed:', e));

    if (!file || !file.key) {
      return;
    }

    try {
      let path = file.path;
      let key = file.key;
      let encodedPath = (path || this.completeEnterprisePathOther).replace(/#/g, "%23");
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

      const iframe = document.createElement('iframe');
      iframe.style.display = 'none';
      iframe.src = protocolUrl;
      document.body.appendChild(iframe);
      setTimeout(() => document.body.removeChild(iframe), 2000);

    } catch (error) {
      console.error('Error in onDragEnd:', error);
    }

    // this.onDragEnded.emit();
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

  toggleSelectAll(event: any) {
    this.isAllSelected = event.target.checked;
    if (this.isAllSelected) {
      this.files.forEach(file => {
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
    this.isAllSelected = this.selectedFiles.size === this.files.length && this.files.length > 0;
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

    this.files.forEach(file => {
      const id = file.srAssignedFileId || file.key;
      if (this.selectedFiles.has(id)) {
        this.DownloadFile(file);
      }
    });
  }

  downloadAsZip() {
    const filesToDownload = this.files.filter(file => {
      const id = file.srAssignedFileId || file.key;
      return this.selectedFiles.has(id);
    }).map(file => ({
      key: file.key,
      name: file.name
    }));

    const input = {
      parentPath: this.completeEnterprisePathOther,
      files: filesToDownload,
      userId: this.appSession.user.id
    };

    this.notify.info('Compressing and downloading files...');

    this.http.post(`${this.apiUrl}/FileManager/DownloadZip`, input, { responseType: 'blob' })
      .subscribe((blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `OtherDocuments_${new Date().getTime()}.zip`;
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

    const filesToDelete = this.files.filter(file => {
      const id = file.srAssignedFileId || file.key;
      return this.selectedFiles.has(id);
    });

    var obj = {
      selectedFiles: filesToDelete,
      templateRef: 'deleteAllOther',
      folderPath: this.completeEnterprisePathOther
    }
    this.saveEvent.emit(obj);
  }

  DownloadFile(file: any) {
    // Trigger the download manually since onDragStart prepares it but might not execute it directly without a drag event context fully utilizing it for click
    // Actually, looking at onDragStart, it constructs the URL but doesn't trigger a window.open or link click for the user to download immediately apart from drag data.
    // We need to implement actual download logic here similar to what standard download does.
    // Re-using logic from FileMain's DownloadFile or constructing it here.

    let path = file.path;
    let key = file.key;
    let encodedPath = (path || this.completeEnterprisePathOther).replace(/#/g, "%23");
    let encodedKey = key.replace(/#/g, "%23");
    let srId = file.srAssignedFileId || this.files[0]?.dataItem?.srAssignedFileId || 0;
    let userId = this.appSession.userId;
    const token = abp.auth.getToken();

    const downloadUrl = this.apiUrl + "/FileManager/DownloadFile" +
      "?path=" + encodeURIComponent(encodedPath + file.name) +
      "&key=" + encodeURIComponent(encodedKey) +
      "&srAssignedFileId=" + srId +
      "&userId=" + userId +
      "&enc_auth_token=" + encodeURIComponent(token);

    // window.open(downloadUrl, '_blank');
    const iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    iframe.src = downloadUrl;
    document.body.appendChild(iframe);

    // Clean up the iframe after a delay
    setTimeout(() => {
      document.body.removeChild(iframe);
    }, 60000);
  }

}

