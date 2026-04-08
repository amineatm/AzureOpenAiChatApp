import { Routes } from '@angular/router';
import { ShellLayout } from './chat/shell-layout/shell-layout';

import { ChatTextComponent } from './chat/chat-text/chat-text';
import { ChatEmbeddingsComponent } from './chat/chat-embeddings/chat-embeddings';
import { ChatImagesComponent } from './chat/chat-images/chat-images';
import { ChatSpeechComponent } from './chat/chat-speech/chat-speech';

import { AuthGuard } from './guards/auth.guard';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  {
    path: '',
    component: ShellLayout,   
    canActivate: [AuthGuard],
    children: [
      { path: 'chat', component: ChatTextComponent },
      { path: 'embeddings', component: ChatEmbeddingsComponent },
      { path: 'images', component: ChatImagesComponent },
      { path: 'speech', component: ChatSpeechComponent },

      { path: '', redirectTo: 'chat', pathMatch: 'full' }
    ]
  },

  { path: '**', redirectTo: 'chat' }
];
