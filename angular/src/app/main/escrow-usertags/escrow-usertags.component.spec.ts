import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EscrowUsertagsComponent } from './escrow-usertags.component';

describe('EscrowUsertagsComponent', () => {
  let component: EscrowUsertagsComponent;
  let fixture: ComponentFixture<EscrowUsertagsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EscrowUsertagsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EscrowUsertagsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
