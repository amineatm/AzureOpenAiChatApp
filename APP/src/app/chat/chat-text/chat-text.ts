import Prism from 'prismjs';
import 'prismjs/components/prism-javascript';
import 'prismjs/components/prism-typescript';
import 'prismjs/components/prism-csharp';
import 'prismjs/components/prism-json';
import 'prismjs/components/prism-java';

import {
  Component,
  ChangeDetectorRef,
  ElementRef,
  ViewChild,
  OnInit,
  AfterViewChecked
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../services/chat.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-chat-text',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-text.html',
  styleUrls: ['../chat.scss', './chat-text.scss']
})
export class ChatTextComponent implements OnInit, AfterViewChecked {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  messages: Message[] = [];
  input = '';

  constructor(
    private chatService: ChatService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  async ngOnInit() {
    const userId = this.auth.getUserId();
    if (!userId) return;

    const history = await this.chatService.getHistory(userId);

    this.messages = history.map(h => ({
      from: h.from === 'assistant' ? 'ai' : 'user',
      text: h.text,
      createdAt: h.createdAt
    }));

    this.highlight();
    this.scrollToBottom();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  private highlight() {
    this.cdr.detectChanges();
    Prism.highlightAll();
  }

  private scrollToBottom() {
    setTimeout(() => {
      const el = this.messagesContainer?.nativeElement;
      if (el) {
        el.scrollTop = el.scrollHeight;
      }
    }, 100);
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

    this.highlight();
    this.scrollToBottom();

    const reply = await this.chatService.sendMessage(userId, trimmed);

    this.messages.push({
      from: 'ai',
      text: reply,
      createdAt: new Date().toISOString()
    });

    this.highlight();
    this.scrollToBottom();
  }

  splitMessage(msg: string) {
    const parts = msg.split(/```/g);
    const result: any[] = [];

    for (let i = 0; i < parts.length; i++) {
      const part = parts[i].trim();
      if (!part) continue;

      if (i % 2 === 1) {
        const firstLine = part.split('\n')[0].trim();
        const lang = /^[a-zA-Z]+$/.test(firstLine) ? firstLine : 'js';

        const cleaned = part
          .replace(/^[a-zA-Z]+\s*\n?/, '')
          .trimStart();

        result.push({ type: 'code', lang, content: cleaned });
      } else {
        const cleanedText = part.replace(/\n+$/, '');
        result.push({ type: 'text', lang: '', content: cleanedText });
      }
    }

    return result;
  }

  copy(text: string) {
    navigator.clipboard.writeText(text);
  }
}
