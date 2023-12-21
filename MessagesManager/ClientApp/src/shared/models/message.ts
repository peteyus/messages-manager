import { Person } from './person';

export interface Message {
  id: number;
  source: string;
  timestamp: Date;
  sender: Person;
  messageText: string;
  messageHtml: string;
  images: any; // photo[]?
  audio: any; // audio[]?
  links: string[];
  videos: any; // video[]?
  sharedUrl: string; // share.Url
  sharedText: string; // share.ShareText
  sharedOwner: string; // share.OriginalContentOwner
  reactions: any; // reaction[]?
  conversationId: number;
}
