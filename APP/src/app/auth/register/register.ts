import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.scss']
})
export class Register {
  name = '';
  email = '';
  password = '';
  confirmPassword = '';

  nameInvalid = false;
  emailInvalid = false;
  passwordInvalid = false;
  passwordMismatch = false;

  constructor(private auth: AuthService, private router: Router) {}

  async register() {
    this.nameInvalid = this.name.length < 4 || this.name.length > 15;

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    this.emailInvalid = !emailRegex.test(this.email);

    const passRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{10,20}$/;
    this.passwordInvalid = !passRegex.test(this.password);

    this.passwordMismatch = this.password !== this.confirmPassword;

    if (this.nameInvalid || this.emailInvalid || this.passwordInvalid || this.passwordMismatch) {
      return;
    }

    const ok = await this.auth.register(this.name, this.email, this.password);
    if (ok) this.router.navigate(['/login']);
  }
}
