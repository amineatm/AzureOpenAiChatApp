import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RagService {
  private api = environment.apiUrl + '/rag';

  constructor(private http: HttpClient) {}

  async query(userId: string, question: string, sourceDocumentId: string): Promise<string> {
    const body = { userId, question, sourceDocumentId };

    const res = await firstValueFrom(
      this.http.post<{ answer: string }>(`${this.api}/query`, body)
    );

    return res.answer;
  }

  async getHistory(userId: string, documentId: string) {
    return await firstValueFrom(
      this.http.get<any[]>(`${this.api}/history/${userId}`)
    );
  }
}
