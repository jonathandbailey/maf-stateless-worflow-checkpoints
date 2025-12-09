import { useEffect } from "react";
import type { ChatResponseDto } from "../types/dto/chat-response.dto";
import type { UIExchange } from "../types/ui/UIExchange";
import { UIFactory } from "../factories/UIFactory";
import streamingService from "../services/streaming.service";

interface UseChatResponseHandlerProps {
    setActiveExchange: React.Dispatch<React.SetStateAction<UIExchange | null>>;
}

export const useChatResponseHandler = ({ setActiveExchange }: UseChatResponseHandlerProps) => {
    useEffect(() => {
        const handleUserResponse = (response: ChatResponseDto) => {
            console.log('Chat response received:', response);
            if (!response) return;

            // Find the matching exchange and update active exchange
            setActiveExchange(prev => {
                if (prev && prev.assistant.id === response.id) {
                    const updatedAssistant = UIFactory.updateAssistantMessage(
                        prev.assistant,
                        prev.assistant.text + (response.message || ''),
                        false
                    );
                    return {
                        ...prev,
                        assistant: updatedAssistant
                    };
                }
                return prev;
            });
        };

        streamingService.on("user", handleUserResponse);

        return () => {
            streamingService.off("user", handleUserResponse);
        };
    }, [setActiveExchange]);
};