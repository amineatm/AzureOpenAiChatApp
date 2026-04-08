import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class SpeechService {
  private api = environment.apiUrl + '/speech';
  private token = localStorage.getItem('accessToken');

  constructor(private http: HttpClient) {}

  async speechToText(audioBlob: Blob): Promise<string> {
    const form = new FormData();
    form.append('AudioFile', audioBlob, 'audio.wav');


    const res = await firstValueFrom(
      this.http.post<{ text: string }>(
        `${this.api}/stt`,
        form,
        {
          headers: { Authorization: `Bearer ${this.token}` }
        }
      )
    );

    return res.text;
  }

  async textToSpeech(text: string): Promise<Blob> {

    return await firstValueFrom(
      this.http.post(
        `${this.api}/tts`,
        { text },
        {
          responseType: 'blob',
          headers: { Authorization: `Bearer ${this.token}` }
        }
      )
    );
  }

  getHistory() {
    return this.http.get<any[]>(
      `${this.api}/history`,
      { headers: { Authorization: `Bearer ${this.token}` } }
    );
  }
}
