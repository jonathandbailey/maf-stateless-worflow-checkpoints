import type { UIConversation } from "../../types/ui/UIConversation";
import ConversationThread from "./ConversationThreads";

interface ConversationProps {
    conversation: UIConversation;
}

const ConversationComponent = ({ conversation }: ConversationProps) => {
    return (
        <div style={{ padding: "48px", width: 700 }}>
            {conversation.threads.map((thread, idx) => (
                <ConversationThread key={thread.id ?? idx} thread={thread} />
            ))}
        </div>
    );
};

export default ConversationComponent;
