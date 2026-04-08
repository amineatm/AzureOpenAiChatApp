import {
  Component,
  ElementRef,
  ViewChild,
  ChangeDetectorRef,
  OnInit,
  AfterViewChecked
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ImageService } from '../../services/image.service';
import { AuthService } from '../../services/auth.service';
import { ImageHistoryItem } from '../../interfaces/ImageHistoryItem';

interface ImageMessage {
  from: 'user' | 'ai';
  text: string;
  imageUrl?: string;
  createdAt: string;
}

@Component({
  selector: 'app-chat-images',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-images.html',
  styleUrls: ['../chat.scss', './chat-images.scss']
})
export class ChatImagesComponent implements OnInit, AfterViewChecked {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  input = '';
  messages: ImageMessage[] = [];

  constructor(
    private imageService: ImageService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  async ngOnInit() {
    const userId = this.auth.getUserId();
    if (!userId) return;

    const history = await this.imageService.getHistory(userId).toPromise();

    this.messages = (history ?? []).map((h: ImageHistoryItem) => ({
      from: h.from,
      text: h.text,
      imageUrl: h.imageUrl,
      createdAt: h.createdAt
    }));

    this.scrollToBottom();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  private scrollToBottom() {
    setTimeout(() => {
      const el = this.messagesContainer?.nativeElement;
      if (el) el.scrollTop = el.scrollHeight;
    }, 0);
  }

  async send() {
    const trimmed = this.input.trim();
    if (!trimmed) return;

    const userId = this.auth.getUserId();
    if (!userId) return;

    this.messages.push({
      from: 'user',
      text: trimmed,
      createdAt: new Date().toISOString()
    });

    this.input = '';
    this.scrollToBottom();
    this.cdr.detectChanges();

    const result = await this.imageService.generate(userId, trimmed).toPromise();

    this.messages.push({
      from: 'ai',
      text: trimmed,
      imageUrl: result?.imageUrl,
      createdAt: result?.createdAt ?? new Date().toISOString()
    });

    this.scrollToBottom();
  }
}
