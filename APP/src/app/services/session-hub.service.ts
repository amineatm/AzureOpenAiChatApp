import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SessionHubService {
  private hub?: HubConnection;

  connect(token: string) {
    if (!token) return;

    this.hub = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/session`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hub.start().catch(err => console.error('SignalR error:', err));

    this.hub.on('logout', () => {
      window.dispatchEvent(new Event('force-logout'));
    });
  }

  disconnect() { this.hub?.stop(); }
}
