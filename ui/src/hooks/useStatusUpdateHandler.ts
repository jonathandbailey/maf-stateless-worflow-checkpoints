import { useEffect } from "react";
import type { ChatResponseDto } from "../types/dto/chat-response.dto";
import type { Status } from "../types/ui/Status";
import streamingService from "../services/streaming.service";

interface UseStatusUpdateHandlerProps {
    setStatusItems: React.Dispatch<React.SetStateAction<Status[]>>;
}

export const useStatusUpdateHandler = ({ setStatusItems }: UseStatusUpdateHandlerProps) => {
    useEffect(() => {
        const handleStatusUpdate = (response: ChatResponseDto) => {
            console.log('Status update received:', response);
            if (!response) return;

            setStatusItems(prev => {
                const newItems = [
                    ...prev,
                    { message: response.message || '' }
                ];
                console.log('Setting status items:', newItems);
                return newItems;
            });
        };

        streamingService.on("status", handleStatusUpdate);

        return () => {
            streamingService.off("status", handleStatusUpdate);
        };
    }, [setStatusItems]);
};