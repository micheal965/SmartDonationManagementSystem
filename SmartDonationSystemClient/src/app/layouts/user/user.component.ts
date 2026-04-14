import { Component } from '@angular/core';
import { UserHeaderComponent } from './user-header/user-header.component';
import { RouterOutlet } from '@angular/router';
import { UserLeftSidebarComponent } from './user-left-sidebar/user-left-sidebar.component';
import { UserRightSidebarComponent } from './user-right-sidebar/user-right-sidebar.component';
import { ChatComponent } from '../../features/chat/chat.component';
import { ConversationsComponent } from '../../features/conversations/conversations.component';
import { MessagingComponent } from "../../features/messaging/messaging.component";

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    UserHeaderComponent,
    RouterOutlet,
    UserLeftSidebarComponent,
    UserRightSidebarComponent,
    MessagingComponent
],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserLayoutComponent {}
