import {File} from './file'

export interface Folder {
    name: string;
    path: string;
    folders: Folder[];
    files: File[];
    expanded: boolean;
}