import { HttpClient } from '@angular/common/http';
import { Component, Input, Inject } from '@angular/core';
import { Subscription } from 'rxjs';

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

  onFileSelected(event: Event) {
    let fileList: FileList | null = (event.target as HTMLInputElement)?.files;
    if (!fileList) {
      return;
    }

    const file: File = fileList[0];
    if (file) {
      this.fileName = file.name;
    }
    const formData = new FormData();
    formData.append(file.name, file); 
    formData.append('sessionId', localStorage.getItem('sessionId'))
    const upload$ = this.http.post(this.baseUrl + "api/import/uploadfile", formData);
    upload$.subscribe(); // TODO PRJ: Use observable instead of subscribe
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
