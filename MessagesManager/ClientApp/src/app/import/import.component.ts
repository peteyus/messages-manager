import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, Input, Inject } from '@angular/core';
import { Subscription, finalize } from 'rxjs';

@Component({
  selector: 'app-import',
  templateUrl: './import.component.html',
  styleUrls: ['./import.component.css']
})
export class ImportComponent {
  @Input()
  fileName = '';
  uploadProgress: number;
  uploadSub: Subscription;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }
  
  onFileSelected(event) {
    const file:File = event.target.files[0];
  
    if (file) {
        this.fileName = file.name;
        const formData = new FormData();
        formData.append("thumbnail", file);

        const upload$ = this.http.post(`${this.baseUrl}api/import/uploadfile`, formData, {
            reportProgress: true,
            observe: 'events'
        })
        .pipe(
            finalize(() => this.reset())
        );
      
        this.uploadSub = upload$.subscribe(event => {
          if (event.type == HttpEventType.UploadProgress) {
            this.uploadProgress = Math.round(100 * (event.loaded / event.total));
          }
        })
      }
  }

  cancelUpload() {
    this.uploadSub.unsubscribe();
    this.reset();
  }

  reset() {
    this.uploadProgress = null;
    this.uploadSub = null;
  }
}
