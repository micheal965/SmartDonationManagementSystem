import { Conversation } from './conversation-model';
import { MessagePayload } from './message-payload-model';

export interface ChatState {
  conversations: Conversation[];
  selectedConversation: Conversation | null;
  messages: MessagePayload[];

  page: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
}
