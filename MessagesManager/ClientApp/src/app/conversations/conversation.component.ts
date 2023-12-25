import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../../shared/models/message';
import { Person } from '../../shared/models/person';

@Component({
  selector: 'app-conversation',
  templateUrl: './conversation.component.html',
})
export class ConversationComponent {
  conversationId: number;
  messages: Message[] = new Array();

  constructor(private route: ActivatedRoute) {
    this.messages.push({
      messageText: "This is a message",
      timestamp: new Date(2023, 12, 20, 20, 41, 20),
      sender: {
        displayName: 'Petey Pie'
      } as Person
    } as Message)

    this.messages.push({
      messageText: "This is a reply.",
      timestamp: new Date(2023, 12, 20, 22, 41, 20),
      sender: {
        displayName: 'Manda Baby'
      } as Person
    } as Message)
  }

  ngOnInit() {
    this.route.params.subscribe(params => { this.conversationId = params['id']; });
  }
}
