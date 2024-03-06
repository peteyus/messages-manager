import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ImportComponent } from './import.component';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { InjectionToken } from '@angular/core';

describe('ImportComponent', () => {
  const BASE_URL = new InjectionToken<string>('base_url/');

  let component: ImportComponent;
  let fixture: ComponentFixture<ImportComponent>;
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [ImportComponent],
      providers: [
        {provide: BASE_URL, useValue: 'base_url/'}
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

  it('should popultate the form on submit', () => {
    const event = {
      target: {
        files: [
          { data: "blah" }
        ]
      }
    }

    component.onFileSelected(event);
    const request = httpTestingController.expectOne('base_url/api/import/upload');
    expect(request.request.method).toEqual('POST');
  })
});
