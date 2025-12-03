import type { ConversationThread } from "./conversationThread";

export interface Conversation {
    id: string;
    name: string;
    currentThread: string;
    threads: ConversationThread[];
}