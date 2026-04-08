import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { firstValueFrom } from 'rxjs';




@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private api = environment.apiUrl + '/chat';

  constructor(private http: HttpClient) {}

  async sendMessage(userId: string, message: string): Promise<string> {
    const body: ChatRequest = { userId, message };

    const res = await firstValueFrom(
      this.http.post<ChatResponse>(this.api, body)
    );

    return res.response;
  }

  async getHistory(userId: string): Promise<ChatMessage[]> {
    return await firstValueFrom(
      this.http.get<ChatMessage[]>(`${this.api}/${userId}`)
    );
  }
}
