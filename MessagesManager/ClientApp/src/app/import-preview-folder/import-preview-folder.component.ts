import { Component, Input } from '@angular/core';
import { Folder } from 'src/shared/models/folder';
import { File } from 'src/shared/models/file';

@Component({
  selector: 'app-import-preview-folder',
  templateUrl: './import-preview-folder.component.html',
  styleUrl: './import-preview-folder.component.css'
})
export class ImportPreviewFolderComponent {
  @Input() folder: Folder;

  updateFileSelected() {
    // TODO: Update event service with selected file
  }
}
