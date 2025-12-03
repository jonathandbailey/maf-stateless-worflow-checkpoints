import type { Message } from "./message";

export interface Exchange {
    id: string;
    user: Message;
    assistant: Message;
}
