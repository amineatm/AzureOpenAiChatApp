import { Component, OnInit, ElementRef, ViewChild, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpeechService } from '../../services/speech.service';
import { ChatStreamService } from '../../services/chat-stream.service';

interface AudioMessage {
  from: 'user' | 'ai';
  text: string;
  audioUrl: string | null;
  createdAt: string;
}

@Component({
  selector: 'app-chat-speech',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-speech.html',
  styleUrls: ['../chat.scss', './chat-speech.scss']
})
export class ChatSpeechComponent implements OnInit, AfterViewChecked {

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  isRecording = false;
  mediaRecorder!: MediaRecorder;
  audioChunks: BlobPart[] = [];
  fullAiText = '';
  messages: AudioMessage[] = [];

  readonly voiceWaveBars = [
    32, 48, 28, 56, 40, 62, 36, 50, 44, 58, 34, 52, 46, 60, 38, 54, 42, 48, 36, 55, 40, 52, 44, 58, 34, 50, 46, 56
  ];

  playingIndex: number | null = null;
  private activeAudio: HTMLAudioElement | null = null;
  private durationCache: Record<number, number> = {};
  private listeners: Array<{ type: string; fn: any }> = [];

  private mediaStream!: MediaStream;

  constructor(
    private speech: SpeechService,
    private stream: ChatStreamService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.speech.getHistory().subscribe(history => {
      this.messages = history.map(s => ({
        from: s.sessionType === 'transcription' ? 'user' : 'ai',
        text: s.sessionType === 'transcription' ? s.transcribedText : s.generatedText,
        audioUrl: s.sessionType === 'transcription' ? s.sourceBlobUrl : s.resultBlobUrl,
        createdAt: s.completedAt
      }));

      this.preloadVoiceDurations();
      this.forceScrollToBottom();
      this.cdr.detectChanges();
      this.forceScrollToBottom();
    });
  }

  private preloadVoiceDurations() {
    this.messages.forEach((_, i) => this.preloadDurationForIndex(i));
  }

  private preloadDurationForIndex(index: number) {
    const url = this.messages[index]?.audioUrl;
    if (!url) return;

    const a = new Audio();
    a.preload = 'metadata';
    a.src = url;

    a.addEventListener('loadedmetadata', () => {
      if (isFinite(a.duration)) {
        this.durationCache[index] = a.duration;
        this.cdr.detectChanges();
      }
    }, { once: true });
  }

  ngAfterViewChecked() {
    if (this.playingIndex === null) {
      this.forceScrollToBottom();
    }
  }

  private forceScrollToBottom() {
    if (this.playingIndex !== null) return;

    setTimeout(() => {
      const el = this.messagesContainer?.nativeElement;
      if (el) el.scrollTop = el.scrollHeight;
    }, 50);
  }

  private destroyActiveAudio() {
    if (!this.activeAudio) return;

    try {
      this.activeAudio.pause();

      this.listeners.forEach(l => {
        this.activeAudio!.removeEventListener(l.type, l.fn);
      });
      this.listeners = [];

      this.activeAudio.src = '';
      this.activeAudio.load();
    } catch { }

    this.activeAudio = null;
    this.playingIndex = null;
  }

  private addListener(type: string, fn: any) {
    this.activeAudio!.addEventListener(type, fn);
    this.listeners.push({ type, fn });
  }

  async startRecording() {
    this.destroyActiveAudio();

    this.isRecording = true;
    this.audioChunks = [];

    this.mediaStream = await navigator.mediaDevices.getUserMedia({ audio: true });
    this.mediaRecorder = new MediaRecorder(this.mediaStream, { mimeType: 'audio/webm' });

    this.mediaRecorder.ondataavailable = e => this.audioChunks.push(e.data);
    this.mediaRecorder.start();
  }

  async stopAndSend() {
    this.destroyActiveAudio();

    this.isRecording = false;
    this.mediaRecorder.stop();

    this.mediaStream.getTracks().forEach(track => track.stop());

    this.mediaRecorder.onstop = async () => {
      const blob = new Blob(this.audioChunks, { type: 'audio/webm' });

      const audioUrl = URL.createObjectURL(blob);

      const text = await this.speech.speechToText(blob);
      if (!text) return;

      this.messages.push({
        from: 'user',
        text,
        audioUrl,
        createdAt: new Date().toISOString()
      });

      this.cdr.detectChanges();
      this.forceScrollToBottom();

      this.fullAiText = '';

      const aiMessage: AudioMessage = {
        from: 'ai',
        text: '',
        audioUrl: null,
        createdAt: new Date().toISOString()
      };

      this.messages.push(aiMessage);
      this.cdr.detectChanges();
      this.forceScrollToBottom();

      this.stream.stream(text).subscribe({
        next: chunk => {
          this.fullAiText += chunk;
          aiMessage.text = this.fullAiText;

          this.cdr.detectChanges();
          this.forceScrollToBottom();
        },
        complete: async () => {
          this.destroyActiveAudio();

          const audioBlob = await this.speech.textToSpeech(this.fullAiText);
          const url = URL.createObjectURL(audioBlob);

          aiMessage.audioUrl = url;

          const aiIdx = this.messages.indexOf(aiMessage);

          const audio = new Audio(url);
          this.activeAudio = audio;
          this.playingIndex = aiIdx;

          this.addListener('loadedmetadata', () => {
            if (isFinite(audio.duration)) {
              this.durationCache[aiIdx] = audio.duration;
            }
            this.cdr.detectChanges();
          });

          this.addListener('timeupdate', () => {
            this.cdr.detectChanges();
          });

          this.addListener('ended', () => {
            this.destroyActiveAudio();
            this.cdr.detectChanges();
          });

          audio.play().catch(() => {
            this.destroyActiveAudio();
            this.cdr.detectChanges();
          });

          this.cdr.detectChanges();
          this.forceScrollToBottom();
        }

      });
    };
  }

  isVoicePlaying(index: number): boolean {
    return (
      this.playingIndex === index &&
      this.activeAudio !== null &&
      !this.activeAudio.paused &&
      !this.activeAudio.ended
    );
  }

  voiceProgressPercent(index: number): number {
    if (this.playingIndex !== index || !this.activeAudio) return 0;

    const a = this.activeAudio;
    if (!isFinite(a.duration) || a.duration <= 0) return 0;

    return Math.min(100, (100 * a.currentTime) / a.duration);
  }

  toggleVoicePlayback(index: number) {
    const msg = this.messages[index];
    if (!msg?.audioUrl) return;

    if (this.playingIndex === index && this.activeAudio && !this.activeAudio.paused) {
      this.destroyActiveAudio();
      this.cdr.detectChanges();
      return;
    }

    this.destroyActiveAudio();

    const audio = new Audio(msg.audioUrl);
    this.activeAudio = audio;
    this.playingIndex = index;

    this.addListener('loadedmetadata', () => {
      if (isFinite(audio.duration)) {
        this.durationCache[index] = audio.duration;
      }
      this.cdr.detectChanges();
    });

    this.addListener('timeupdate', () => {
      this.cdr.detectChanges();
    });

    this.addListener('ended', () => {
      this.destroyActiveAudio();
      this.cdr.detectChanges();
    });

    audio.play().catch(() => {
      this.destroyActiveAudio();
      this.cdr.detectChanges();
    });
  }

  voiceTimeLabel(index: number): string {
    const msg = this.messages[index];
    if (!msg?.audioUrl) return '';

    if (this.playingIndex === index && this.activeAudio) {
      const left = this.activeAudio.duration - this.activeAudio.currentTime;
      if (isFinite(left) && left >= 0) {
        return this.formatVoiceClock(left);
      }
    }

    const cached = this.durationCache[index];
    if (cached !== undefined && isFinite(cached)) {
      return this.formatVoiceClock(cached);
    }

    return '0:00';
  }

  private formatVoiceClock(seconds: number): string {
    const s = Math.max(0, Math.floor(seconds));
    const m = Math.floor(s / 60);
    const r = s % 60;
    return `${m}:${r.toString().padStart(2, '0')}`;
  }
}