import { Flex } from "antd";
import type { UIConversationThread } from "../../types/ui/UIConversationThread";
import AssistantMessage from "./AssistantMessage";
import UserMessage from "./UserMessage";

interface ConversationThreadProps {
    thread: UIConversationThread;
}

const Exchange = ({ thread }: ConversationThreadProps) => {
    return (
        <>
            {thread.exchanges.map((exchange, idx) => (
                <div key={idx}>
                    <Flex justify="flex-end" style={{ width: "100%" }}>
                        <UserMessage message={exchange.user} />
                    </Flex>


                    <AssistantMessage message={exchange.assistant} />
                </div>
            ))}



        </>);
}

export default Exchange;