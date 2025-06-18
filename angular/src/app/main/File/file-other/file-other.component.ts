import { Component, HostListener, Injector, OnInit, Output, ViewChild, EventEmitter, TemplateRef, Input, Optional, Inject, ElementRef } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EscrowFileTagsesServiceProxy, API_BASE_URL, TagsAndFileMappingsesServiceProxy, CreateOrEditTagsAndFileMappingsDto } from '@shared/service-proxies/service-proxies';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EscrowUsertagsComponent } from '@app/main/escrow-usertags/escrow-usertags.component';
import { SafeHtml } from '@node_modules/@angular/platform-browser/platform-browser';
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
  @Input() inputPerson: any;
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
  sanitizer: any;
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
    @Optional() @Inject(API_BASE_URL) baseUrl?: string

  ) {
    super(injector);
    this.apiUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
  }

  ngOnInit(): void {
    this.getAllFiles();
    this.getAllFileTags();
  }

  getAllFiles(): void {
    var queryParams = this.inputPerson;
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
    return Array.from(tagsSet);  // Convert the Set back to an array
  }

  filterFilesByTag(tag: string): void {
    debugger;
    this.filteredFiles = this.files.filter(file =>
      file.escrowFileTags.some(t => t.tagDescription.toLowerCase() === tag.toLowerCase())
    );
    this.showTagSearch = false;  // Hide the context menu after selection
  }

  getFileIcon(fileName: string): SafeHtml {
    let fileExtension = fileName.split('.').pop()?.toLowerCase(); // Extract file extension
    let iconHtml: string;

    switch (fileExtension) {
      case "pdf":
        iconHtml = "<i class='fas fa-file-pdf' style='color: #e74c3c;'></i>"; // Red PDF
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
    const url = `${this.apiUrl}/FileManager/FileSystem1?company=${company}&subCompany=${subCompany}&escrow=${escrow}&userId=${userId}&arguments=${argumentsString}`;
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
      .slice(0, 8); // Limit the filtered results to 8
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

  toggleFullScreen(fullscreenElement: HTMLElement) {
    this.isFullScreen = !this.isFullScreen;
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
    return item.id; // Assuming each tag has a unique id 
  }

  sortTable(field: string) {
    debugger;
    if (this.currentSortField === field) {
      this.ascendingOrder = !this.ascendingOrder;
    } else {
      this.currentSortField = field;
      this.ascendingOrder = true;
    }

    this.files.sort((a, b) => {
      let aValue = this.getSortValue(a, field);
      let bValue = this.getSortValue(b, field);

      // Normalize null/undefined/empty values
      aValue = (aValue === null || aValue === undefined || aValue === '') ? null : aValue;
      bValue = (bValue === null || bValue === undefined || bValue === '') ? null : bValue;

      // Handle nulls explicitly
      if (aValue === null && bValue === null) return 0;
      if (aValue === null) return this.ascendingOrder ? 1 : -1;
      if (bValue === null) return this.ascendingOrder ? -1 : 1;

      // Standard comparison
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


}
