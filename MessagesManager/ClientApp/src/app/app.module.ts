import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select'; 
import { MatCheckboxModule } from '@angular/material/checkbox';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ConversationsListComponent } from './conversations/conversations-list.component';
import { ConversationComponent } from './conversations/conversation.component';
import { ImportComponent } from './import/import.component';
import { MessageComponent } from './conversations/messages/message.component';
import { initializeSession } from './providers/sessionInitializer';
import { ImportPreviewComponent } from './import-preview/import-preview.component';
import { ImportPreviewFolderComponent } from './import-preview-folder/import-preview-folder.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ConversationsListComponent,
    ConversationComponent,
    ImportComponent,
    ImportPreviewComponent,
    MessageComponent,
    ImportPreviewFolderComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'conversations', component: ConversationsListComponent, pathMatch: 'full' },
      { path: 'conversation/:id', component: ConversationComponent, pathMatch: 'full' },
      { path: 'import', component: ImportComponent, pathMatch: 'full' },
      { path: 'import/preview', component: ImportPreviewComponent, pathMatch: 'full'}
    ]),
    MatIconModule,
    MatProgressBarModule,
    MatSelectModule,
    MatCheckboxModule
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: initializeSession, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
