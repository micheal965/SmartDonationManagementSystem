import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

type SoundKey = 'notification' | 'typing' | 'message'|'sendMessage';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  private audioContext: AudioContext | null = null;
  private isUnlocked = false;
  private lastSound = 0;

  private bufferCache = new Map<SoundKey, AudioBuffer>();
  private loadPromises = new Map<SoundKey, Promise<AudioBuffer | null>>();

  private readonly soundMap: Record<SoundKey, string> = {
    notification: '/assets/audios/notification.mp3',
    typing: '/assets/audios/typing.mp3',
    message: '/assets/audios/message.mp3',
    sendMessage: '/assets/audios/send-message.mp3',
  };

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  // -----------------------------
  // INIT / UNLOCK
  // -----------------------------
  unlockAudioOnFirstInteraction(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const unlock = async () => {
      try {
        if (!this.audioContext) {
          this.audioContext = new AudioContext();
        }

        await this.audioContext.resume();
        this.isUnlocked = true;

        console.log('[AudioService] Audio unlocked');
      } catch (err) {
        console.error('[AudioService] Failed to unlock audio:', err);
      }
    };

    document.addEventListener('click', unlock, { once: true });
    document.addEventListener('keydown', unlock, { once: true });
    document.addEventListener('touchstart', unlock, { once: true });
  }

  // -----------------------------
  // BUFFER LOADING (LAZY + CACHE)
  // -----------------------------
  private async getBuffer(type: SoundKey): Promise<AudioBuffer | null> {
    if (this.bufferCache.has(type)) {
      return this.bufferCache.get(type)!;
    }

    if (this.loadPromises.has(type)) {
      return this.loadPromises.get(type)!;
    }

    const promise = (async (): Promise<AudioBuffer | null> => {
      try {
        if (!this.audioContext) return null;

        const response = await fetch(this.soundMap[type]);
        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const arrayBuffer = await response.arrayBuffer();
        const audioBuffer =
          await this.audioContext.decodeAudioData(arrayBuffer);

        this.bufferCache.set(type, audioBuffer);
        this.loadPromises.delete(type);

        return audioBuffer;
      } catch (err) {
        console.error(`[AudioService] Failed loading sound "${type}"`, err);
        this.loadPromises.delete(type);
        return null;
      }
    })();

    this.loadPromises.set(type, promise);
    return promise;
  }

  // -----------------------------
  // PLAY SOUND
  // -----------------------------
  async playSound(type: SoundKey): Promise<void> {
    if (!isPlatformBrowser(this.platformId)) return;

    // Debounce
    const now = Date.now();
    if (now - this.lastSound < 300) return;
    this.lastSound = now;

    // Ensure AudioContext exists
    if (!this.audioContext) {
      this.audioContext = new AudioContext();
    }

    // Always try resume (important fix for Chrome suspend)
    if (this.audioContext.state !== 'running') {
      try {
        await this.audioContext.resume();
      } catch {}
    }

    // Block if not unlocked yet
    if (!this.isUnlocked) {
      console.warn('[AudioService] Audio is not unlocked yet');
      return;
    }

    const buffer = await this.getBuffer(type);
    if (!buffer) return;

    const source = this.audioContext.createBufferSource();
    source.buffer = buffer;

    const gainNode = this.audioContext.createGain();
    const t = this.audioContext.currentTime;

    // Smooth fade (prevents click/pop)
    gainNode.gain.setValueAtTime(0, t);
    gainNode.gain.linearRampToValueAtTime(1, t + 0.01);
    gainNode.gain.setValueAtTime(1, t + buffer.duration - 0.05);
    gainNode.gain.linearRampToValueAtTime(0, t + buffer.duration);

    source.connect(gainNode);
    gainNode.connect(this.audioContext.destination);

    source.onended = () => {
      source.disconnect();
      gainNode.disconnect();
    };

    source.start();
  }

  // -----------------------------
  // OPTIONAL PRELOAD
  // -----------------------------
  preloadSounds(types: SoundKey[] = ['notification']): void {
    if (!isPlatformBrowser(this.platformId)) return;
    types.forEach((t) => this.getBuffer(t));
  }
}
