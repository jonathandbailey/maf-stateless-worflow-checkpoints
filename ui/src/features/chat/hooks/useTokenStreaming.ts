import { useCallback, useEffect, useRef, useState } from "react";
import streamingService from "../../../app/api/streaming.api";
import type { ChatResponseDto } from "../api/chat.dto";
import type { Exchange } from "../domain/Exchange";

interface UseTokenStreamingOptions {
    tokenIntervalMs?: number;
}

export const useTokenStreaming = (message: Exchange, options: UseTokenStreamingOptions = {}) => {
    const { tokenIntervalMs = 50 } = options;

    const [currentStream, setCurrentStream] = useState('');

    const tokenQueueRef = useRef<string[]>([]);
    const displayedTokensRef = useRef(0);
    const isStreamingRef = useRef(false);
    const streamIntervalRef = useRef<number | null>(null);

    const processTokenQueue = useCallback(() => {
        if (displayedTokensRef.current < tokenQueueRef.current.length) {
            const tokensToDisplay = tokenQueueRef.current.slice(0, displayedTokensRef.current + 1);
            setCurrentStream(tokensToDisplay.join(' '));
            displayedTokensRef.current++;
        } else {
            // All tokens displayed, stop streaming
            if (streamIntervalRef.current) {
                clearInterval(streamIntervalRef.current);
                streamIntervalRef.current = null;
                isStreamingRef.current = false;
            }
        }
    }, []);

    const startTokenStreaming = useCallback(() => {
        if (!isStreamingRef.current && tokenQueueRef.current.length > 0) {
            isStreamingRef.current = true;
            streamIntervalRef.current = setInterval(processTokenQueue, tokenIntervalMs);
        }
    }, [processTokenQueue, tokenIntervalMs]);

    const addTokensToQueue = useCallback((text: string) => {
        // Split text into words and add to queue
        const newTokens = text.split(/\s+/).filter(token => token.length > 0);
        tokenQueueRef.current.push(...newTokens);
        console.log('Added tokens to queue:', newTokens);
        console.log('Total queue length:', tokenQueueRef.current.length);

        // Start streaming if not already streaming
        startTokenStreaming();
    }, [startTokenStreaming]);

    useEffect(() => {
        const handleUserResponse = (response: ChatResponseDto) => {
            if (!response) return;

            console.log("Received streaming response:", response);

            const messageText = response.message || '';
            if (messageText.trim()) {
                addTokensToQueue(messageText);
            }
        }

        streamingService.on("user", handleUserResponse);

        return () => {
            // Cleanup streaming
            if (streamIntervalRef.current) {
                clearInterval(streamIntervalRef.current);
                streamIntervalRef.current = null;
            }

            // Reset state
            setCurrentStream('');
            tokenQueueRef.current = [];
            displayedTokensRef.current = 0;
            isStreamingRef.current = false;

            streamingService.off("user", handleUserResponse);
        };

    }, [message, addTokensToQueue]);

    return {
        currentStream,
        isStreaming: isStreamingRef.current
    };
};