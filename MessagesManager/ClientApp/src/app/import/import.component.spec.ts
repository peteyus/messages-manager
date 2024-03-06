import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ImportComponent } from './import.component';
import { HttpClient, HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { InjectionToken } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { asyncData, asyncError } from '../testing/observable-testing-helper';

describe('ImportComponent', () => {
  const BASE_URL = new InjectionToken<string>('base_url/');

  let component: ImportComponent;
  let fixture: ComponentFixture<ImportComponent>;
  let httpClientSpy = jasmine.createSpyObj('HttpClient', ['post']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, MatCheckboxModule, MatIconModule],
      declarations: [ImportComponent],
      providers: [
        {provide: 'BASE_URL', useValue: BASE_URL},
        {provide: HttpClient, useValue: httpClientSpy }
      ]
    })
    .compileComponents();
    
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
    httpClientSpy.post.and.returnValue(asyncData({}));
    component.onFileSelected(event);
    expect(httpClientSpy.post.calls.count()).toBe(1);
  })
});
