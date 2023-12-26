import { Audio } from './audio';
import { Person } from './person';
import { Photo } from './photo';
import { Reaction } from './reaction';
import { Share } from './share';
import { Video } from './video';

export interface Message {
  id: number;
  source: string;
  timestamp: Date;
  sender: Person;
  messageText: string;
  messageHtml: string;
  images: Photo[];
  audio: Audio[];
  links: string[];
  videos: Video[];
  share: Share;
  reactions: Reaction[];
  conversationId: number;
}
