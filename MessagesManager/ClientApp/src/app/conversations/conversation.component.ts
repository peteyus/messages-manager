import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-conversation-detail',
  templateUrl: './conversation.component.html',
})

export class ConversationComponent {
  conversationId: number;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => { this.conversationId = params['id']; });
  }
}
