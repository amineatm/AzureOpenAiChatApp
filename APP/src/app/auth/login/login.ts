import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class Login {
  email = '';
  password = '';

  constructor(private auth: AuthService, private router: Router) {}

  async login() {
    const ok = await this.auth.login(this.email, this.password);
    if (ok) this.router.navigate(['/chat']);
  }
  
}