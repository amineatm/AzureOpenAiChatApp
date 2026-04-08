export interface ImageHistoryItem {
  from: 'ai' | 'user';
  text: string;
  imageUrl: string;
  createdAt: string;
}