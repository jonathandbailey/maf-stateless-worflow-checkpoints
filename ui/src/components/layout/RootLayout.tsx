import ChatInput from "../chat/ChatInput"
import { Flex } from "antd"
import { useState, useEffect, use } from "react";
import type { UIExchange } from "../../types/ui/UIExchange";
import type { UIMessage } from "../../types/ui/UIMessage";
import { ConversationService } from "../../services/conversation.service";
import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import streamingService from "../../services/streaming.service";
import UserMessage from "../chat/UserMessage";
import AssistantMessage from "../chat/AssistantMessage";

const RootLayout = () => {
    const [sessionId] = useState<string>(crypto.randomUUID());

    const [exchanges, setExchanges] = useState<UIExchange[]>([]);






    streamingService.on("user", (response: ChatResponseDto) => {
        console.log("Streaming chat response received in RootLayout:", response);

        if (!response) return;

        setExchanges(prev => prev.map(exchange => {
            if (exchange.assistant.id === response.id) {
                return {
                    ...exchange,
                    assistant: {
                        ...exchange.assistant,
                        text: exchange.assistant.text + (response.message || ''),
                        isLoading: false
                    }
                };
            }
            return exchange;
        }));
    });

    function handlePrompt(value: string): void {
        console.log("Prompt received in RootLayout:", value);

        const userMessage: UIMessage = {
            id: crypto.randomUUID(),
            text: value,
            role: 'user',
            isLoading: false,
            hasError: false,
            errorMessage: ''
        };

        const assistantMessage: UIMessage = {
            id: crypto.randomUUID(),
            text: '',
            role: 'assistant',
            isLoading: true,
            hasError: false,
            errorMessage: ''
        };

        const newExchange: UIExchange = {
            id: crypto.randomUUID(),
            user: userMessage,
            assistant: assistantMessage
        };

        setExchanges(prev => [...prev, newExchange]);

        const conversationService = new ConversationService();
        conversationService.startConversationExchange(
            value,
            userMessage.id,
            sessionId,
            assistantMessage.id
        ).then(response => {

        }).catch(error => {
            console.error("Error during conversation exchange:", error);

        });
    }

    return <>
        <Flex vertical >
            <div>
                {exchanges.map((exchange, idx) => (
                    <div key={idx}>
                        <Flex justify="flex-end" style={{ width: "100%" }}>
                            <UserMessage message={exchange.user} />
                        </Flex>


                        <AssistantMessage message={exchange.assistant} />
                    </div>
                ))}
            </div>

            <div style={{ width: 700, position: 'sticky', bottom: 0, background: '#fff', zIndex: 10 }}>
                <ChatInput onEnter={handlePrompt} />
            </div>
        </Flex>

    </>

}

export default RootLayout