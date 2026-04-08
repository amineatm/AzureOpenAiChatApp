interface Message {
  from: 'user' | 'ai' | 'loading' | 'error';
  text: string;
  createdAt: string;
}