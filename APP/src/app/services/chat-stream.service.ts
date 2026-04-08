import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ChatStreamService {

  private api = environment.apiUrl + '/chatstream/stream';

  stream(text: string): Observable<string> {
    return new Observable(observer => {

      const url = this.api + '?text=' + encodeURIComponent(text);
      const eventSource = new EventSource(url);

      eventSource.onmessage = e => observer.next(e.data);

      eventSource.onerror = () => {
        eventSource.close();
        observer.complete();
      };

      return () => eventSource.close();
    });
  }
}
