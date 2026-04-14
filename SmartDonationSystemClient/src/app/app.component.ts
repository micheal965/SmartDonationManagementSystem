import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { NgxSpinnerModule } from 'ngx-spinner';
import { filter } from 'rxjs';
import { AnalyticsService } from './core/services/analytics.service';
import { NotificationService } from './core/services/notification.service';
import { AudioService } from './core/services/audio.service';
import { ChatService } from './core/services/chat.service';

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
