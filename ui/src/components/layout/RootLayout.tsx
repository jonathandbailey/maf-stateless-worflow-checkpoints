import ChatInput from "../chat/ChatInput"
import { Flex, Timeline } from "antd"
import { useState, useEffect } from "react";
import type { UIExchange } from "../../types/ui/UIExchange";
import { ConversationService } from "../../services/conversation.service";
import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import streamingService from "../../services/streaming.service";
import UserMessage from "../chat/UserMessage";
import AssistantMessage from "../chat/AssistantMessage";
import styles from './RootLayout.module.css';
import { UIFactory } from '../../factories/UIFactory';
import type { Status } from "../../types/ui/Status";
import type { ArtifactStatusDto } from "../../types/dto/artifact-status.dto";

const RootLayout = () => {
    const [sessionId] = useState<string>(crypto.randomUUID());
    const [exchanges, setExchanges] = useState<UIExchange[]>([]);
    const [statusItems, setStatusItems] = useState<Status[]>([]);

    useEffect(() => {
        const handleUserResponse = (response: ChatResponseDto) => {
            if (!response) return;

            setExchanges(prev => prev.map(exchange => {
                if (exchange.assistant.id === response.id) {
                    const updatedAssistant = UIFactory.updateAssistantMessage(
                        exchange.assistant,
                        exchange.assistant.text + (response.message || ''),
                        false
                    );
                    return {
                        ...exchange,
                        assistant: updatedAssistant
                    };
                }
                return exchange;
            }));
        };

        const handleStatusUpdate = (response: ChatResponseDto) => {
            if (!response) return;

            setStatusItems(prev => [
                ...prev,
                { message: response.message || '' }
            ]);
        };

        const handleArtifact = (response: ArtifactStatusDto) => {
            console.log("Artifact status received:", response);

        };

        streamingService.on("user", handleUserResponse);
        streamingService.on("status", handleStatusUpdate);
        streamingService.on("artifact", handleArtifact);

        return () => {
            streamingService.off("user", handleUserResponse);
            streamingService.off("status", handleStatusUpdate);
            streamingService.off("artifact", handleArtifact);
        };
    }, []);

    function handlePrompt(value: string): void {
        const newExchange = UIFactory.createUIExchange(value);

        setExchanges(prev => [...prev, newExchange]);

        const conversationService = new ConversationService();
        conversationService.startConversationExchange(
            value,
            newExchange.user.id,
            sessionId,
            newExchange.assistant.id
        ).then(response => {

        }).catch(error => {
            console.error("Error during conversation exchange:", error);

        });
    }

    return <>

        <Flex gap="large" >

            <div className={styles.container}>
                <Flex vertical className={styles.layout}>
                    <div className={styles.content}>
                        {exchanges.map((exchange, idx) => (
                            <div key={idx}>
                                <Flex justify="flex-end" className={styles.userMessageContainer}>
                                    <UserMessage message={exchange.user} />
                                </Flex>
                                <AssistantMessage message={exchange.assistant} />
                            </div>
                        ))}
                    </div>

                    <div className={styles.chatInputContainer}>
                        <ChatInput onEnter={handlePrompt} />
                    </div>
                </Flex>
            </div>
            <div className={styles.statusContainer}>
                <Timeline>
                    {statusItems.map((status, idx) => (
                        <Timeline.Item key={idx}>{status.message}</Timeline.Item>
                    ))}

                </Timeline>
            </div>

        </Flex>




    </>

}

export default RootLayout