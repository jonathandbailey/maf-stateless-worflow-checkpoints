import { useEffect } from "react";
import type { ChatResponseDto } from "../types/dto/chat-response.dto";
import streamingService from "../services/streaming.service";
import type { TravelPlanDto } from "../types/dto/travel-plan.dto";
import { ConversationService } from "../services/conversation.service";

interface UseExchangeStatusUpdateHandlerProps {
    sessionId: string;
    setTravelPlan: React.Dispatch<React.SetStateAction<TravelPlanDto | null>>;
}

export const useTravelPlanUpdateHandler = ({ sessionId, setTravelPlan }: UseExchangeStatusUpdateHandlerProps) => {
    useEffect(() => {

        const handleExchangeStatusUpdate = () => {

            console.log('Requesting travel plan for sessionId:', sessionId);
            const conversationService = new ConversationService();
            conversationService.getTravelPlan(sessionId)

                .then((travelPlan: TravelPlanDto) => {
                    console.log('Travel plan received:', travelPlan);
                    setTravelPlan(travelPlan);
                })

        };

        streamingService.on("travelPlan", handleExchangeStatusUpdate);

        return () => {

            streamingService.off("travelPlan", handleExchangeStatusUpdate);
        };
    }, [setTravelPlan]);
};