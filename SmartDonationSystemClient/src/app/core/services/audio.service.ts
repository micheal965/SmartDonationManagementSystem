import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  private audioContext: AudioContext | null = null;
  private audioBuffer: AudioBuffer | null = null;
  private isUnlocked = false;
  private lastSound = 0;
  private initPromise: Promise<void> | null = null; // ← store the promise

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    if (isPlatformBrowser(this.platformId)) {
      this.initPromise = this.init(); // ← save it, don't just fire and forget
    }
  }

  private async init() {
    try {
      this.audioContext = new AudioContext();
      const response = await fetch('/assets/audios/notification.mp3');
      const arrayBuffer = await response.arrayBuffer();
      this.audioBuffer = await this.audioContext.decodeAudioData(arrayBuffer);
    } catch (err) {
      console.error('[AudioService] init failed:', err);
    }
  }

  async playNotificationSound() {
    // ← wait for init to finish before doing anything
    if (this.initPromise) {
      await this.initPromise;
    }

    const now = Date.now();
    if (now - this.lastSound < 300) return;
    this.lastSound = now;

    if (!this.audioContext || !this.audioBuffer || !this.isUnlocked) return;

    if (this.audioContext.state === 'suspended') {
      await this.audioContext.resume();
    }

    const source = this.audioContext.createBufferSource();
    source.buffer = this.audioBuffer;

    const gainNode = this.audioContext.createGain();
    const currentTime = this.audioContext.currentTime;

    gainNode.gain.setValueAtTime(0, currentTime);
    gainNode.gain.linearRampToValueAtTime(1, currentTime + 0.01);
    gainNode.gain.setValueAtTime(
      1,
      currentTime + this.audioBuffer.duration - 0.05,
    );
    gainNode.gain.linearRampToValueAtTime(
      0,
      currentTime + this.audioBuffer.duration,
    );

    source.connect(gainNode);
    gainNode.connect(this.audioContext.destination);

    source.onended = () => {
      source.disconnect();
      gainNode.disconnect();
    };

    source.start(currentTime);
  }

  unlockAudioOnFirstInteraction() {
    if (!isPlatformBrowser(this.platformId)) return;

    document.addEventListener(
      'click',
      async () => {
        if (!this.audioContext) return;
        await this.audioContext.resume();
        this.isUnlocked = true;
      },
      { once: true },
    );
  }
}
