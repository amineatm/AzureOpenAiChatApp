import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { firstValueFrom } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DocumentService {
    private api = environment.apiUrl + '/documents';

    constructor(private http: HttpClient) { }

    async upload(userId: string, file: File): Promise<void> {
        const form = new FormData();
        form.append('userId', userId);
        form.append('file', file);

        await firstValueFrom(
            this.http.post(`${this.api}/upload`, form)
        );
    }

    async getMyDocument(): Promise<any> {
        return await firstValueFrom(
            this.http.get(`${this.api}/me`)
        );
    }
    async deleteMyDocument(): Promise<void> {
        await firstValueFrom(
            this.http.delete(`${this.api}/me`)
        );
    }
}
