import type { UIMessage } from "./UIMessage";

export interface UIExchange {
    id: string;
    user: UIMessage;
    assistant: UIMessage;
}