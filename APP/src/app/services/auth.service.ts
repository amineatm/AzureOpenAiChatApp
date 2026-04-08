import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { firstValueFrom } from 'rxjs';
import { SessionHubService } from './session-hub.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private api = environment.apiUrl + '/auth';

  constructor(private http: HttpClient, private sessionHubService: SessionHubService) {
    window.addEventListener('force-logout', () => {
      this.logout();
    });
  }

  async login(email: string, password: string): Promise<boolean> {
    const res = await firstValueFrom(
      this.http.post<AuthResponse>(`${this.api}/login`, { email, password })
    );

    if (!res) return false;

    localStorage.setItem('accessToken', res.token);
    localStorage.setItem('refreshToken', res.refreshToken);

    this.sessionHubService.connect(res.token);

    return true;
  }

  async register(name: string, email: string, password: string): Promise<boolean> {
    await firstValueFrom(
      this.http.post(`${this.api}/register`, { name, email, password })
    );

    return true;
  }

  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem('refreshToken');

    if (refreshToken) {
      await firstValueFrom(
        this.http.post(`${this.api}/logout`, { refreshToken })
      );
    }

    this.sessionHubService.disconnect();

    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  async logoutAll(): Promise<void> {
    await firstValueFrom(
      this.http.post(`${this.api}/logout-all`, {})
    );

    this.sessionHubService.disconnect();

    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  async refresh(): Promise<string | null> {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) return null;

    const res = await firstValueFrom(
      this.http.post<AuthResponse>(`${this.api}/refresh`, { refreshToken })
    );

    if (!res) return null;

    localStorage.setItem('accessToken', res.token);
    localStorage.setItem('refreshToken', res.refreshToken);

    return res.token;
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getUserId(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;

    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.sub;
  }
    getUserName(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;

    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.name;
  }

}