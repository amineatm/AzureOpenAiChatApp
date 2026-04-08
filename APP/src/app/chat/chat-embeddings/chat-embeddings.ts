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
import { AuthService } from '../../services/auth.service';
import { RagService } from '../../services/rag.service';
import { DocumentService } from '../../services/document.service';

@Component({
  selector: 'app-chat-embeddings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-embeddings.html',
  styleUrls: ['../chat.scss', './chat-embeddings.scss']
})
export class ChatEmbeddingsComponent implements OnInit, AfterViewChecked {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  messages: Message[] = [];
  input = '';
  fileName: string | null = null;

  showUploadModal = false;
  selectedFile: File | null = null;
  modalPrompt = '';

  sourceDocumentId: string | null = null;

  isLoadingHistory = true;
  loadError = false;

  constructor(
    private auth: AuthService,
    private ragService: RagService,
    private documentService: DocumentService,
    private cdr: ChangeDetectorRef
  ) {}

  async ngOnInit() {
    const userId = this.auth.getUserId()!;
    const doc = await this.documentService.getMyDocument();
    console.log('Got document:', doc);
    this.sourceDocumentId = doc?.id ?? null;
    this.fileName = doc?.fileName ?? null;

    if (!this.sourceDocumentId) {
      this.isLoadingHistory = false;
      return;
    }

    this.isLoadingHistory = true;
    this.loadError = false;

    try {
      const history = await this.ragService.getHistory(userId, this.sourceDocumentId);

      this.messages = history?.length
        ? history.map(h => ({
            from: h.role === 'assistant' ? 'ai' : 'user',
            text: h.message,
            createdAt: h.createdAt
          }))
        : [];
    } catch {
      this.loadError = true;
    }

    this.isLoadingHistory = false;

    this.highlight();
    this.scrollToBottom();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  private highlight() {
    setTimeout(() => {
      Prism.highlightAll();
      this.cdr.detectChanges();
    }, 0);
  }

  private scrollToBottom() {
    setTimeout(() => {
      const el = this.messagesContainer?.nativeElement;
      if (el) {
        el.scrollTo({
          top: el.scrollHeight,
          behavior: 'smooth'
        });
      }
    }, 50);
  }

  private async typeWriterEffect(fullText: string) {
    return new Promise<void>(resolve => {
      let index = 0;
      const interval = setInterval(() => {
        if (index < fullText.length) {
          this.messages[this.messages.length - 1].text += fullText[index];
          index++;
          this.cdr.detectChanges();
          const el = this.messagesContainer?.nativeElement;
          if (el) el.scrollTop = el.scrollHeight;
        } else {
          clearInterval(interval);
          resolve();
        }
      }, 12);
    });
  }
  async send() {
    const trimmed = this.input.trim();
    if (!trimmed) return;

    const userId = this.auth.getUserId()!;
    if (!this.sourceDocumentId) {
      this.messages.push({
        from: 'ai',
        text: 'You must upload a document first.',
        createdAt: new Date().toISOString()
      });
      this.highlight();
      this.scrollToBottom();
      return;
    }

    const userText = this.fileName
      ? `📄 ${this.fileName}\n\n${trimmed}`
      : trimmed;

    this.messages.push({
      from: 'user',
      text: userText,
      createdAt: new Date().toISOString()
    });

    this.input = '';
    this.highlight();
    this.scrollToBottom();

    const loadingMsg: Message = {
      from: 'loading',
      text: '',
      createdAt: new Date().toISOString()
    };
    this.messages.push(loadingMsg);
    this.scrollToBottom();

    const reply = await this.ragService.query(userId, trimmed, this.sourceDocumentId);

    this.messages = this.messages.filter(m => m !== loadingMsg);

    this.messages.push({
      from: 'ai',
      text: '',
      createdAt: new Date().toISOString()
    });

    await this.typeWriterEffect(reply);

    this.highlight();
    this.scrollToBottom();
  }

  async uploadAndSend() {
    if (!this.selectedFile || !this.modalPrompt.trim()) return;

    const userId = this.auth.getUserId()!;
    const prompt = this.modalPrompt.trim();

    await this.documentService.upload(userId, this.selectedFile);

    const doc = await this.documentService.getMyDocument();
    this.sourceDocumentId = doc?.id ?? null;
    this.fileName = doc?.fileName ?? null;

    this.messages.push({
      from: 'user',
      text: `📄 ${this.selectedFile.name}\n\n${prompt}`,
      createdAt: new Date().toISOString()
    });

    this.highlight();
    this.scrollToBottom();

    setTimeout(() => {
      this.closeUploadModal();
      this.cdr.detectChanges();
    }, 50);

    const loadingMsg: Message = {
      from: 'loading',
      text: '',
      createdAt: new Date().toISOString()
    };
    this.messages.push(loadingMsg);
    this.scrollToBottom();

    const reply = await this.ragService.query(userId, prompt, this.sourceDocumentId!);

    this.messages = this.messages.filter(m => m !== loadingMsg);

    this.messages.push({
      from: 'ai',
      text: '',
      createdAt: new Date().toISOString()
    });

    await this.typeWriterEffect(reply);

    this.highlight();
    this.scrollToBottom();
  }

  openUploadModal() {
    this.showUploadModal = true;
    this.selectedFile = null;
    this.modalPrompt = '';
  }

  closeUploadModal() {
    this.showUploadModal = false;
    this.selectedFile = null;
    this.modalPrompt = '';
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0] ?? null;
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
        const cleaned = part.replace(/^[a-zA-Z]+/, '').trim();
        result.push({ type: 'code', lang, content: cleaned });
      } else {
        result.push({ type: 'text', lang: '', content: part });
      }
    }

    return result;
  }

  copy(text: string) {
    navigator.clipboard.writeText(text);
  }
}
