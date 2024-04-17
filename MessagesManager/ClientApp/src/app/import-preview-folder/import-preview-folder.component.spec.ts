import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportPreviewFolderComponent } from './import-preview-folder.component';

describe('ImportPreviewFolderComponent', () => {
  let component: ImportPreviewFolderComponent;
  let fixture: ComponentFixture<ImportPreviewFolderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ImportPreviewFolderComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ImportPreviewFolderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
