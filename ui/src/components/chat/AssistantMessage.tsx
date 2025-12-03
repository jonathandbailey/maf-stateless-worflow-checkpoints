import { Alert, Card, Flex, Spin } from "antd";
import Markdown from "react-markdown";
import type { UIMessage } from "../../types/ui/UIMessage";
import styles from "./AssistantMessage.module.css";

interface AssistantMessageProps {
    message: UIMessage
}

const AssistantMessage = ({ message }: AssistantMessageProps) => {
    return (
        <Card
            className={styles["assistant-message-card"]}
            variant="borderless"

        >
            {message.hasError ? (
                <Alert
                    message={message.errorMessage}
                    description="There was an error processing your request. Please try again."
                    type="error"
                    showIcon
                />
            ) : (
                <Flex vertical>
                    {message.isLoading && (
                        <Spin size="small" className={styles["assistant-spin-left"]} />
                    )}
                    <Markdown>{message.text}</Markdown>
                </Flex>

            )}
        </Card>
    );
};

export default AssistantMessage;