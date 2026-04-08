import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}
  private notificationAudio: HTMLAudioElement | null = null;
  playNotificationSound() {
    const audio = this.getAudio();
    if (!audio) return;

    audio.pause();
    audio.currentTime = 0;

    const playPromise = audio.play();

    if (playPromise !== undefined) {
      playPromise.catch((err) => {
        console.warn('Audio blocked until user interaction', err);
      });
    }
  }
  unlockAudio() {
    const audio = this.getAudio();
    if (!audio) return;

    audio.volume = 0;

    audio
      .play()
      .then(() => {
        audio.pause();
        audio.currentTime = 0;
        audio.volume = 1;
      })
      .catch(() => {});
  }
  private getAudio() {
    if (typeof window === 'undefined') return null;

    if (!this.notificationAudio) {
      this.notificationAudio = new Audio('/assets/audios/notification.mp3');
      this.notificationAudio.preload = 'auto';
    }

    return this.notificationAudio;
  }

  unlockAudioOnFirstInteraction() {
    if (!isPlatformBrowser(this.platformId)) return;

    document.addEventListener(
      'click',
      () => {
        this.unlockAudio();
      },
      { once: true },
    );
  }
}
