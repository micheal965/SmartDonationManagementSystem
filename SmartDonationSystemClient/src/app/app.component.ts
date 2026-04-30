import { Component, Inject, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AnalyticsService } from './core/services/analytics.service';
import { NotificationService } from './core/services/notification.service';
import { ChatService } from './core/services/chat.service';
import { AudioService } from './core/services/audio.service';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgxSpinnerModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private chatService = inject(ChatService);
  private analyticsService = inject(AnalyticsService);
  private audioService = inject(AudioService);

  ngOnInit(): void {
    this.analyticsService.trackEntrance();

    this.audioService.unlockAudioOnFirstInteraction();

    this.notificationService.startConnection();
    this.chatService.startConnection();
  }
}
