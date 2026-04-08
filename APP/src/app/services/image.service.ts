import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ImageHistoryItem } from '../interfaces/ImageHistoryItem';


@Injectable({ providedIn: 'root' })
export class ImageService {
  private api = '/api/images';

  constructor(private http: HttpClient) {}

  generate(userId: string, prompt: string) {
    const body: ImageGenerationRequest = { userId, prompt };
    return this.http.post<ImageGenerationResponse>(this.api, body);
  }

  getHistory(userId: string) {
    return this.http.get<ImageHistoryItem[]>(`${this.api}/history/${userId}`);
  }
}
