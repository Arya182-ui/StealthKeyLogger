export interface KeystrokeLog {
  id: string;
  content: string;
  timestamp: Date;
  ip: string;
  createdAt?: Date;
  type?: string;
}