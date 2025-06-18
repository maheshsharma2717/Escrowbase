import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FileMainComponent } from './file-main.component';

describe('FileMainComponent', () => {
  let component: FileMainComponent;
  let fixture: ComponentFixture<FileMainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FileMainComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FileMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
