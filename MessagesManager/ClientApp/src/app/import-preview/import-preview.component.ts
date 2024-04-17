import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { MessageSample } from 'src/shared/models/messagesample';
import { ImportPreviewFolderComponent } from '../import-preview-folder/import-preview-folder.component';

@Component({
  selector: 'app-import-preview',
  standalone: false,
  templateUrl: './import-preview.component.html',
  styleUrl: './import-preview.component.css'
})
export class ImportPreviewComponent {
  sample: MessageSample;

  constructor(private router: Router, private location: Location) {
    const data = this.router.getCurrentNavigation().extras.state;
    if (data) {
      this.sample = data as MessageSample;
    }
    else {
      this.location.back();
    }
  }
}
