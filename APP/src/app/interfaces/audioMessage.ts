interface AudioMessage {
  text: string;
  isUser: boolean;
  audioUrl: string | null;
  createdAt: string;
}
