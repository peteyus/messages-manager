import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ImportComponent } from './import.component';
import { HttpClient, HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { InjectionToken } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';

describe('ImportComponent', () => {
  const BASE_URL = new InjectionToken<string>('base_url/');

  let component: ImportComponent;
  let fixture: ComponentFixture<ImportComponent>;
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, MatCheckboxModule, MatIconModule],
      declarations: [ImportComponent],
      providers: [
        {provide: 'BASE_URL', useValue: BASE_URL}
      ]
    })
    .compileComponents();

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
    
    fixture = TestBed.createComponent(ImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should popultate the sessionId on submit', () => {
    const event = {
      target: {
        files: [
          { data: "blah" }
        ]
      }
    }

    component.onFileSelected(event);
    const request = httpTestingController.expectOne(`${BASE_URL}api/import/uploadfile`);
    expect(request.request.body.has("sessionId")).toBe(true);
  })
});
