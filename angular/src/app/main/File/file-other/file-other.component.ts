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
  currentSortField: string = 'name';
  defaultSortPreference: string = 'name';
  ascendingOrder: boolean = true;
  selectedIndex: number | null = null;
  selectedTagIndex2: number | null = null;
  selectedRowIndex: number | null = null;
  isFilterActive: boolean = false;
  selectedFilterTagIds: Set<number> = new Set();
  allFiles: any[] = [];
  globalSearchQuery: string = '';
  viewMode: 'list' | 'tag-drop' = 'list';
  activeHoverTagId: number | null = null;
  draggedFile: any = null;
  tagSearchQuery: string = '';
  isInternalTagDrop: boolean = false;
  expandedTagIds: Set<number> = new Set([-1]);
  isAllTagsExpanded: boolean = true;


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

  private globalScrollListener = (event: Event) => {
    const targetElement = event.target as HTMLElement;
    if (targetElement && targetElement.closest && (targetElement.closest('.radical-context-menu') || targetElement.closest('.context-menu'))) {
      return;
    }
    
    if (this.showContextMenu || this.showContextMenuTags) {
      this.showContextMenu = false;
      this.showContextMenuTags = false;
    }
  };

  ngOnInit(): void {
    window.addEventListener('scroll', this.globalScrollListener, true);
    
    // Restore persisted viewMode selection across page refreshes
    const savedViewMode = localStorage.getItem('fileOtherViewMode') as 'list' | 'tag-drop';
    if (savedViewMode === 'list' || savedViewMode === 'tag-drop') {
      this.viewMode = savedViewMode;
    }

    this.loadUserSortPreferenceFromDb();
    this.getAllFiles();
    this.getAllFileTags();
  }

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.globalScrollListener, true);
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
    this.userTypeFromStorage = userType;
    this.completeEnterprisePathOther = `${companyName}/${subCompanyName}/${EscrowTab}/Other/`

    this
      .getAllFilesApi(companyName, subCompanyName, EscrowTab, this.appSession.user.id.toString())
      .subscribe(
        (response) => {
          this.allFiles = response.result;
          this.files = [...this.allFiles];
          
          this.applyFilters();
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
    if (tag === 'all') {
      this.searchQuery2 = '';
      this.selectedFilterTagIds.clear();
      this.isFilterActive = false;
      this.applyFilters();
      this.showDropdown = false;
    } else {
      if (this.selectedFilterTagIds.has(tag.id)) {
        this.selectedFilterTagIds.delete(tag.id);
      } else {
        this.selectedFilterTagIds.add(tag.id);
      }
      this.isFilterActive = this.selectedFilterTagIds.size > 0;
      this.applyFilters();
      // Keep dropdown open for multi-selection
    }
  }

  getTagCount(tagId: number): number {
    if (!this.allFiles) return 0;
    return this.allFiles.filter(file =>
      Array.isArray(file.escrowFileTags) &&
      file.escrowFileTags.some((t: any) => t.id === tagId)
    ).length;
  }

  getSelectedTagsSummary(): string {
    if (this.selectedFilterTagIds.size === 0) return '';
    if (this.selectedFilterTagIds.size === 1) {
      const id = Array.from(this.selectedFilterTagIds)[0];
      const tag = this.allTagsList.find(t => t.id === id);
      return tag ? tag.tagDescription : '1 Tag';
    }
    return `${this.selectedFilterTagIds.size} Tags`;
  }

  getSelectedTagNames(): string {
    if (this.selectedFilterTagIds.size === 0) return '';
    return this.allTagsList
      .filter(t => this.selectedFilterTagIds.has(t.id))
      .map(t => t.tagDescription)
      .join(', ');
  }

  onGlobalSearch() {
    this.applyFilters();
  }

  clearGlobalSearch() {
    this.globalSearchQuery = '';
    this.applyFilters();
  }

  onTagDeleted(tagId: number) {
    // Remove the tag from all files locally
    this.allFiles.forEach(file => {
      if (Array.isArray(file.escrowFileTags)) {
        file.escrowFileTags = file.escrowFileTags.filter((t: any) => t.id !== tagId);
      }
    });

    // Remove from active filters if it was selected
    if (this.selectedFilterTagIds.has(tagId)) {
      this.selectedFilterTagIds.delete(tagId);
      this.isFilterActive = this.selectedFilterTagIds.size > 0;
    }

    // Refresh tag lists for search dropdowns
    this.getAllFileTags();
    
    // Update the filtered view
    this.applyFilters();
  }

  applyFilters() {
    let filtered = [...this.allFiles];

    // Filter by Tag IDs (if active)
    if (this.isFilterActive && this.selectedFilterTagIds.size > 0) {
      filtered = filtered.filter(file =>
        Array.isArray(file.escrowFileTags) &&
        file.escrowFileTags.some((t: any) => this.selectedFilterTagIds.has(t.id))
      );
    }

    // Filter by Global Search Query
    if (this.globalSearchQuery.trim()) {
      const query = this.globalSearchQuery.toLowerCase().trim();
      filtered = filtered.filter(file =>
        file.name.toLowerCase().includes(query) ||
        (Array.isArray(file.escrowFileTags) &&
          file.escrowFileTags.some((t: any) => t.tagDescription.toLowerCase().includes(query)))
      );
    }

    this.files = filtered;
    this.applySorting();
    this.cdr.detectChanges();
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

    const mouseX = event.clientX;
    const mouseY = event.clientY;

    setTimeout(() => {
      this.contextMenuPosition = { x: mouseX, y: mouseY };
      this.showContextMenu = mode !== 'Tags';
      this.showContextMenuTags = mode === 'Tags';

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

    const mouseX = event.clientX;
    const mouseY = event.clientY;

    setTimeout(() => {
      this.contextMenuPosition = { x: mouseX, y: mouseY };
      
      if (type === "Tags") {
        this.showContextMenu = false;
        this.showContextMenuTags = true;
      } else {
        this.showContextMenuTags = false;
        this.showContextMenu = true;
      }

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

  @HostListener('document:click', ['$event'])
  @HostListener('document:contextmenu', ['$event'])
  onClickOutside(event: Event) {
    const targetElement = event.target as HTMLElement;
    if (targetElement && !targetElement.closest('.context-menu') && !targetElement.closest('.radical-context-menu')) {
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

    else if (fileName.includes('.docx') || fileName.includes('.doc') || fileName.includes('.txt')) {
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

  assignUsersToOther() {
    this.showContextMenu = false;
    this.selectedFile.path = this.completeEnterprisePathOther;
    var obj = {
      selectedFile: this.selectedFile,
      templateRef: "AssignOther",
    }
    this.saveEvent.emit(obj);
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

  getAssignedUsers(key: string): string {
    if (!key) return '';
    const assignedUsers = [];
    const regex = /\{([^}]+)\}/g;
    let match;
    while ((match = regex.exec(key)) !== null) {
      const tagContent = match[1];
      const targetCode = tagContent.indexOf('-') > -1 ? tagContent.split('-')[0] : tagContent;
      if (targetCode && !assignedUsers.includes(targetCode)) {
        assignedUsers.push(targetCode);
      }
    }
    if (assignedUsers.length > 0) {
      return ` <span class="text-muted ml-2 font-weight-normal small">(Assigned: ${assignedUsers.join(', ')})</span>`;
    }
    return '';
  }

  getAssignedUsersText(key: string): string {
    if (!key) return '';
    const assignedUsers = [];
    const regex = /\{([^}]+)\}/g;
    let match;
    while ((match = regex.exec(key)) !== null) {
      const tagContent = match[1];
      const targetCode = tagContent.indexOf('-') > -1 ? tagContent.split('-')[0] : tagContent;
      if (targetCode && !assignedUsers.includes(targetCode)) {
        assignedUsers.push(targetCode);
      }
    }
    if (assignedUsers.length > 0) {
      return ` (Assigned: ${assignedUsers.join(', ')})`;
    }
    return '';
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
      return this.manageTagList;
    } else {
      this.manageTagList = this.allTagsList;
      return this.manageTagList.filter(item =>
        item.tagDescription?.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
    }
  }

  onSearchTags() {
    // Getter filteredItems will handle the update automatically via change detection
  }

  filteredItems2() {
    if (!this.searchQuery2?.trim()) {
      return this.allTagsList;
    }
    return this.allTagsList
      .filter(item =>
        item.tagDescription.toLowerCase().includes(this.searchQuery2.toLowerCase())
      );
  }

  getAllFileTags() {
    debugger;
    this.escrowFileTagsServiceProxy
      .getAll(undefined, undefined, 0, 10000)
      .subscribe(
        (response: any) => {
          this.allTagsList = response.items.map(item => ({
            ...item.escrowFileTags,
            tagColor: (item.escrowFileTags.tagColor || '#6c757d').split(',')[0],
          }));
          this.manageTagList = this.allTagsList;
          
          // Expand General (Untagged) node by default
          const generalTag = this.allTagsList.find(t => t.tagDescription?.toLowerCase() === 'general');
          if (generalTag) {
            this.expandedTagIds.add(generalTag.id);
          }
          this.expandedTagIds.add(-1);
        },
        (error) => {
          console.error('Error fetching tags:', error);
        }
      );
  }

  toggleTagExpand(tagId: number, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    if (this.expandedTagIds.has(tagId)) {
      this.expandedTagIds.delete(tagId);
    } else {
      this.expandedTagIds.add(tagId);
    }
  }

  isTagExpanded(tagId: number): boolean {
    return this.expandedTagIds.has(tagId);
  }

  expandAllTags(): void {
    this.expandedTagIds.add(-1);
    (this.allTagsList || []).forEach(t => this.expandedTagIds.add(t.id));
    this.isAllTagsExpanded = true;
  }

  collapseAllTags(): void {
    this.expandedTagIds.clear();
    this.isAllTagsExpanded = false;
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

  refreshFilesAndTags(savedScrollTop?: number): void {
    const scrollContainer = document.querySelector('.tag-dropzones-scroll-container');
    const scrollTopToRestore = savedScrollTop !== undefined ? savedScrollTop : (scrollContainer ? scrollContainer.scrollTop : 0);

    this.getAllFiles();
    this.getAllFileTags();

    setTimeout(() => {
      if (scrollContainer) {
        scrollContainer.scrollTop = scrollTopToRestore;
      }
      this.cdr.detectChanges();
    }, 50);
  }

  removeTag(id: any, fileName: any) {
    const scrollContainer = document.querySelector('.tag-dropzones-scroll-container');
    const savedScrollTop = scrollContainer ? scrollContainer.scrollTop : 0;

    this.tagsAndFileMapping
      .deleteTagByFileNameAndTagId(id, fileName)
      .subscribe(
        (response: any) => {
          abp.notify.success('Tag removed successfully', 'Success');
          this.refreshFilesAndTags(savedScrollTop);
        });
  }

  trackByTagId(index: number, item: any): any {
    return item ? item.id : index;
  }

  trackByFileName(index: number, item: any): any {
    return item ? (item.srAssignedFileId || item.key || item.name) : index;
  }

  saveUserSortPreferenceToDb(preference: string): void {
    if (!preference) return;
    const url = `${this.apiUrl}/api/services/app/UiCustomizationSettings/ChangeFileSortingPreference?preference=${encodeURIComponent(preference)}`;
    this.http.post(url, {}).subscribe({
      next: () => console.log(`Saved file sorting preference '${preference}' to DB`),
      error: (err) => console.warn('Error saving file sorting preference to DB:', err)
    });
  }

  loadUserSortPreferenceFromDb(): void {
    const url = `${this.apiUrl}/api/services/app/UiCustomizationSettings/GetFileSortingPreference`;
    this.http.get<any>(url).subscribe({
      next: (res) => {
        const val = res?.result;
        if (val) {
          this.defaultSortPreference = val;
          this.currentSortField = val;
          localStorage.setItem('otherSortField', val);
          this.applySorting();
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.warn('Error loading file sorting preference from DB:', err);
      }
    });
  }

  setSortField(field: string, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.defaultSortPreference = field;
    this.currentSortField = field;
    this.ascendingOrder = true;
    localStorage.setItem('otherSortField', this.defaultSortPreference);
    localStorage.setItem('otherSortOrder', 'asc');

    this.saveUserSortPreferenceToDb(field);
    this.applySorting();
  }

  sortTable(field: string) {
    debugger;
    if (this.currentSortField === field) {
      this.ascendingOrder = !this.ascendingOrder;
    } else {
      this.currentSortField = field;
      this.ascendingOrder = true;
    }
    this.applySorting();
  }

  applySorting() {
    this.files.sort((a, b) => {
      let aValue = this.getSortValue(a, this.currentSortField);
      let bValue = this.getSortValue(b, this.currentSortField);

      // Handle nulls (No tags or empty values) - always move to bottom
      if (aValue === null && bValue === null) {
        return a.name.localeCompare(b.name);
      }
      if (aValue === null) return 1;
      if (bValue === null) return -1;

      // Primary sort
      let comparison = 0;
      if (typeof aValue === 'string' && typeof bValue === 'string') {
        comparison = aValue.localeCompare(bValue);
      } else {
        comparison = aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
      }

      // Apply order
      let result = comparison * (this.ascendingOrder ? 1 : -1);

      // Secondary sort by name if primary values are equal
      if (result === 0) {
        return a.name.localeCompare(b.name);
      }

      return result;
    });
  }

  getSortValue(file: any, field: string): any {
    if (field === 'tags') {
      if (!file.escrowFileTags || file.escrowFileTags.length === 0) {
        return null;
      }
      return file.escrowFileTags.map(tag => tag.tagDescription).join(', ');
    }

    const value = file[field];
    return (value === undefined || value === null || value === '') ? null : value;
  }

  switchViewMode(mode: 'list' | 'tag-drop'): void {
    this.viewMode = mode;
    localStorage.setItem('fileOtherViewMode', mode);
    this.cdr.detectChanges();
  }

  get filteredTagListForView(): any[] {
    let tags = this.allTagsList || [];
    if (this.tagSearchQuery?.trim()) {
      const q = this.tagSearchQuery.toLowerCase().trim();
      tags = tags.filter(t => t.tagDescription?.toLowerCase().includes(q));
    }

    const generalTags = tags.filter(t => t.tagDescription?.toLowerCase() === 'general');
    const customTags = tags.filter(t => t.tagDescription?.toLowerCase() !== 'general');

    // Sort custom tags alphabetically A to Z
    customTags.sort((a, b) => {
      const nameA = a.tagDescription || '';
      const nameB = b.tagDescription || '';
      return nameA.localeCompare(nameB, undefined, { sensitivity: 'base' });
    });

    // General tag is placed at the bottom of the tree
    return [...customTags, ...generalTags];
  }

  getFilesForTag(tagId: number): any[] {
    if (!this.allFiles) return [];

    let fileList: any[] = [];
    const tag = (this.allTagsList || []).find(t => t.id === tagId);
    if (tag && tag.tagDescription?.toLowerCase() === 'general') {
      fileList = [...this.allFiles];
    } else {
      fileList = this.allFiles.filter(file =>
        Array.isArray(file.escrowFileTags) &&
        file.escrowFileTags.some((t: any) => t.id === tagId)
      );
    }

    // Sort files alphabetically A to Z
    return fileList.sort((a, b) => (a.name || '').localeCompare(b.name || '', undefined, { sensitivity: 'base' }));
  }

  onTagDragOver(event: DragEvent, tagId: number): void {
    event.preventDefault();
    event.stopPropagation();
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'copy';
    }
    if (this.activeHoverTagId !== tagId) {
      this.activeHoverTagId = tagId;
    }
  }

  onTagDragLeave(event: DragEvent, tagId: number): void {
    event.preventDefault();
    event.stopPropagation();

    // Guard against child element dragleave triggers
    const currentTarget = event.currentTarget as HTMLElement;
    const relatedTarget = event.relatedTarget as Node;
    if (currentTarget && relatedTarget && currentTarget.contains(relatedTarget)) {
      return;
    }

    if (this.activeHoverTagId === tagId) {
      this.activeHoverTagId = null;
    }
  }

  onTagDrop(event: DragEvent, tag: any): void {
    event.preventDefault();
    event.stopPropagation();
    this.activeHoverTagId = null;
    this.isInternalTagDrop = true;

    if (tag && tag.id !== undefined) {
      this.expandedTagIds.add(tag.id);
    }

    if (tag.tagDescription?.toLowerCase() === 'general') {
      return;
    }

    let fileToAssign = this.draggedFile;
    if (!fileToAssign && event.dataTransfer) {
      const dataStr = event.dataTransfer.getData('application/json');
      if (dataStr) {
        try {
          const parsed = JSON.parse(dataStr);
          fileToAssign = (this.allFiles || []).find(f => f.key === parsed.key || f.name === parsed.name);
        } catch (e) {}
      }
    }

    if (!fileToAssign && this.selectedFile) {
      fileToAssign = this.selectedFile;
    }

    if (!fileToAssign) {
      return;
    }

    if (fileToAssign.escrowFileTags && fileToAssign.escrowFileTags.some((t: any) => t.id === tag.id)) {
      abp.notify.warn(`'${fileToAssign.name}' is already assigned to '${tag.tagDescription}'.`, 'Already Assigned');
      return;
    }

    const tagData: CreateOrEditTagsAndFileMappingsDto = {
      tagId: tag.id,
      fileName: fileToAssign.name,
      id: 0,
      init: () => { },
      toJSON: () => ({
        tagId: tag.id,
        fileName: fileToAssign.name,
        id: 0,
      })
    };

    this.tagsAndFileMapping.createOrEdit(tagData).subscribe({
      next: (response: any) => {
        if (response?.success === false) {
          abp.notify.error(response.message || 'Failed to assign tag.', 'Error');
          return;
        }
        abp.notify.success(`Assigned '${tag.tagDescription}' to '${fileToAssign.name}'`, 'Success');
        const scrollContainer = document.querySelector('.tag-dropzones-scroll-container');
        const savedScrollTop = scrollContainer ? scrollContainer.scrollTop : 0;
        this.refreshFilesAndTags(savedScrollTop);
      },
      error: (err) => {
        console.error('Error saving tag:', err);
        abp.notify.error('Tag is already assigned to the file', 'Error');
      }
    });
  }

  onRowClick(index: number) {
    this.selectedRowIndex = index;
  }

  onDragStart(event: DragEvent, file: any) {
    this.draggedFile = file;
    this.isInternalTagDrop = false;
    if (!file || !file.key) {
      return;
    }

    try {
      if (event.dataTransfer) {
        event.dataTransfer.effectAllowed = 'all';

        const dragIcon = document.createElement('div');
        dragIcon.textContent = ` ${file.name}`;
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
        event.dataTransfer.setData('application/json', JSON.stringify({ key: file.key, name: file.name }));

        // Trick Windows File Explorer into accepting the drag (removes the  icon)
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
    this.draggedFile = null;
    this.activeHoverTagId = null;

    if (this.isInternalTagDrop) {
      this.isInternalTagDrop = false;
      return;
    }

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

  canDeleteFile(file: any): boolean {
    console.log('canDeleteFile debug:', file, this.userTypeFromStorage);
    if (!this.userTypeFromStorage) return false;
    let uType = this.userTypeFromStorage.toUpperCase().replace('-READS', '').replace('-READ', '').replace('-SIGN', '').replace('-INPUT', '');
    if (uType.startsWith('EO') || uType.startsWith('EA')) return true;
    if (file && file.uploaderRole) {
      let fType = file.uploaderRole.toUpperCase().replace('-READS', '').replace('-READ', '').replace('-SIGN', '').replace('-INPUT', '');
      if (uType === fType || fType.startsWith(uType) || uType.startsWith(fType)) return true;
    }
    return false;
  }

  deleteSelected() {
    if (this.selectedFiles.size === 0) {
      abp.notify.warn('Please select files to delete');
      return;
    }

    const filesToDelete = this.files.filter(file => {
      const id = file.srAssignedFileId || file.key;
      return this.selectedFiles.has(id) && this.canDeleteFile(file);
    });

    if (filesToDelete.length === 0) {
      abp.notify.warn('You do not have permission to delete the selected files.');
      return;
    }

    var obj = {
      selectedFiles: filesToDelete,
      templateRef: 'deleteAllOther',
      folderPath: this.completeEnterprisePathOther
    }
    this.saveEvent.emit(obj);
  }

  DownloadFile(file?: any) {
    let targetFile = file || this.selectedFile;
    if (!targetFile) return;

    let path = targetFile.path;
    let key = targetFile.key;
    let encodedPath = (path || this.completeEnterprisePathOther).replace(/#/g, "%23");
    let encodedKey = key.replace(/#/g, "%23");
    let srId = targetFile.srAssignedFileId || this.files[0]?.dataItem?.srAssignedFileId || 0;
    let userId = this.appSession.userId;
    const token = abp.auth.getToken();

    const downloadUrl = this.apiUrl + "/FileManager/DownloadFile" +
      "?path=" + encodeURIComponent(encodedPath + targetFile.name) +
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

