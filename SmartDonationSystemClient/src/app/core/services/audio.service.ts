import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

type SoundKey = 'notification' | 'typing' | 'message';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  private audioContext: AudioContext | null = null;
  private isUnlocked = false;
  private lastSound = 0;

  // Per-sound lazy-load promises — each key loads once and is cached
  private bufferCache = new Map<SoundKey, AudioBuffer>();
  private loadPromises = new Map<SoundKey, Promise<AudioBuffer | null>>();

  private readonly soundMap: Record<SoundKey, string> = {
    notification: '/assets/audios/notification.mp3',
    typing: '/assets/audios/typing.mp3',
    message: '/assets/audios/message.mp3',
  };

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    if (isPlatformBrowser(this.platformId)) {
      // Only create the context here — buffers load lazily per sound
      this.audioContext = new AudioContext();
    }
  }

  /**
   * Lazily fetches and decodes a sound buffer, caching the result.
   * Concurrent calls for the same key share the same promise.
   */
  private getBuffer(type: SoundKey): Promise<AudioBuffer | null> {
    // Already cached — return immediately
    if (this.bufferCache.has(type)) {
      return Promise.resolve(this.bufferCache.get(type)!);
    }

    // Already loading — return the in-flight promise
    if (this.loadPromises.has(type)) {
      return this.loadPromises.get(type)!;
    }

    // Start a new load
    const promise = (async (): Promise<AudioBuffer | null> => {
      try {
        const response = await fetch(this.soundMap[type]);
        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const arrayBuffer = await response.arrayBuffer();
        const audioBuffer =
          await this.audioContext!.decodeAudioData(arrayBuffer);

        this.bufferCache.set(type, audioBuffer);
        this.loadPromises.delete(type); // No longer needed — buffer is cached
        return audioBuffer;
      } catch (err) {
        console.error(`[AudioService] Failed to load sound "${type}":`, err);
        this.loadPromises.delete(type); // Allow retry on next call
        return null;
      }
    })();

    this.loadPromises.set(type, promise);
    return promise;
  }

  async playSound(type: SoundKey): Promise<void> {
    if (!this.audioContext) return;

    // Debounce — ignore calls within 300ms of the last played sound
    const now = Date.now();
    if (now - this.lastSound < 300) return;
    this.lastSound = now;

    if (!this.isUnlocked) {
      console.warn(
        '[AudioService] Audio is not unlocked yet. Waiting for user interaction.',
      );
      return;
    }

    // Resume context if suspended (e.g. tab was backgrounded)
    if (this.audioContext.state === 'suspended') {
      await this.audioContext.resume();
    }

    // Load buffer (instant if already cached)
    const buffer = await this.getBuffer(type);
    if (!buffer) return;

    // Build the graph: source → gain → destination
    const source = this.audioContext.createBufferSource();
    source.buffer = buffer;

    const gainNode = this.audioContext.createGain();
    const t = this.audioContext.currentTime;

    // Fade in over 10ms, hold, fade out over 50ms — prevents clicks/pops
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

    source.start(t);
  }

  /**
   * Call this from AppComponent.ngOnInit() so the unlock listener
   * is registered as early as possible, before any user interaction.
   */
  unlockAudioOnFirstInteraction(): void {
    if (!isPlatformBrowser(this.platformId) || !this.audioContext) return;

    document.addEventListener(
      'click',
      async () => {
        try {
          await this.audioContext!.resume();
          this.isUnlocked = true;
        } catch (err) {
          console.error('[AudioService] Failed to resume AudioContext:', err);
        }
      },
      { once: true },
    );
  }

  /**
   * Optional: Pre-warm sounds you know you'll need to avoid
   * any delay on the very first playback.
   */
  preloadSounds(types: SoundKey[] = ['notification']): void {
    if (!this.audioContext) return;
    types.forEach((type) => this.getBuffer(type));
  }
}
