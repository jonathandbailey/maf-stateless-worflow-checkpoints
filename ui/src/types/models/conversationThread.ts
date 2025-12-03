import type { ConversationExchange } from "./conversationExchange";

export interface ConversationThread {
    id: string;
    title: string;
    exchanges: ConversationExchange[];
}