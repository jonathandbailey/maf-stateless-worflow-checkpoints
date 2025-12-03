import type { Message } from "./message";

export interface ConversationExchange {
    id: string;
    title: string;
    userMessage: Message;
    assistantMessage: Message;
}