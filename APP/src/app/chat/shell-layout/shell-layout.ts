import { Component, HostListener } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-shell-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell-layout.html',
  styleUrls: ['./shell-layout.scss']
})
export class ShellLayout {
  collapsed = false;
  navOpen = false;
  pageTitle = 'Chat';

  constructor(private auth: AuthService, private router: Router) {
    this.pageTitle = this.titleForUrl(this.router.url);
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        this.navOpen = false;
        this.pageTitle = this.titleForUrl(this.router.url);
      });
  }

  private titleForUrl(rawUrl: string): string {
    const path = rawUrl.split('?')[0].replace(/\/$/, '') || '/';
    const titles: Record<string, string> = {
      '/': 'Chat',
      '/chat': 'Chat',
      '/embeddings': 'Embeddings',
      '/images': 'Images',
      '/speech': 'Speech'
    };
    return titles[path] ?? 'Chat';
  }

  async logout() {
    await this.auth.logout();
    this.router.navigate(['/login']);
  }
  getUserName(): string | null {
    return this.auth.getUserName();
  }

  toggle() {
    this.collapsed = !this.collapsed;
  }

  openNav() {
    this.navOpen = true;
  }

  closeNav() {
    this.navOpen = false;
  }

  @HostListener('document:keydown.escape')
  onEscape() {
    if (this.navOpen) {
      this.navOpen = false;
    }
  }
}
