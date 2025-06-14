/**
 * MessageService - Handles all messaging functionality including SignalR integration
 */
class MessageService {
    constructor() {
        this.connection = null;
        this.currentUserId = null;
        this.currentUserType = null;
        this.messageReceivedCallback = null;
        this.messageReadCallback = null;
        this.userConnectedCallback = null;
        this.userDisconnectedCallback = null;
        
        this.initializeSignalR();
    }    /**
     * Initialize SignalR connection
     */
    async initializeSignalR() {
        try {
            // Check if signalR is defined
            if (typeof signalR === 'undefined') {
                console.error("SignalR is not defined. Make sure the SignalR client library is loaded properly.");
                return;
            }
            
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/mesajlasmahub", {
                    withCredentials: true // Ensure cookies are sent with the request
                })
                .withAutomaticReconnect([0, 2000, 5000, 10000, 30000]) // More robust reconnection strategy
                .configureLogging(signalR.LogLevel.Information) // Add logging for better debugging
                .build();

            // Set up event handlers
            this.connection.on("ReceiveMessage", (message) => {
                console.log("Message received:", message);
                if (this.messageReceivedCallback) {
                    this.messageReceivedCallback(message);
                }
            });

            this.connection.on("MessageRead", (data) => {
                console.log("Message read:", data);
                if (this.messageReadCallback) {
                    this.messageReadCallback(data);
                }
            });

            this.connection.on("UserConnected", (userId) => {
                console.log("User connected:", userId);
                if (this.userConnectedCallback) {
                    this.userConnectedCallback(userId);
                }
            });

            this.connection.on("UserDisconnected", (userId) => {
                console.log("User disconnected:", userId);
                if (this.userDisconnectedCallback) {
                    this.userDisconnectedCallback(userId);
                }
            });

            // Add reconnection events
            this.connection.onreconnecting(error => {
                console.log("SignalR reconnecting:", error);
            });
            
            this.connection.onreconnected(connectionId => {
                console.log("SignalR reconnected with connectionId:", connectionId);
                // Rejoin groups after reconnection
                if (this.currentUserId) {
                    this.joinUserGroup(this.currentUserId);
                }
            });
            
            this.connection.onclose(error => {
                console.log("SignalR connection closed:", error);
                // Attempt to restart the connection
                setTimeout(() => this.restartConnection(), 5000);
            });

            // Start connection
            await this.connection.start();
            console.log("SignalR connection established");
        } catch (error) {
            console.error("SignalR connection failed:", error);
            // Attempt to restart the connection after delay
            setTimeout(() => this.restartConnection(), 5000);
        }
    }

    /**
     * Restart SignalR connection if it fails or disconnects
     */
    async restartConnection() {
        if (!this.connection || this.connection.state === signalR?.HubConnectionState?.Disconnected) {
            console.log("Attempting to restart SignalR connection...");
            try {
                await this.initializeSignalR();
                
                // Rejoin user group if we have a current user
                if (this.currentUserId) {
                    this.joinUserGroup(this.currentUserId);
                }
                
                console.log("SignalR connection restarted successfully");
            } catch (error) {
                console.error("Failed to restart SignalR connection:", error);
                // Try again after a delay
                setTimeout(() => this.restartConnection(), 10000);
            }
        }
    }

    /**
     * Set current user information
     */    setCurrentUser(userId, userType) {
        this.currentUserId = userId;
        this.currentUserType = userType;
        
        if (this.connection && this.connection.state === signalR?.HubConnectionState?.Connected) {
            this.joinUserGroup(userId);
        }
    }

    /**
     * Join user-specific group
     */    async joinUserGroup(userId) {
        if (this.connection && this.connection.state === signalR?.HubConnectionState?.Connected) {
            try {
                await this.connection.invoke("JoinGroup", `user-${userId}`);
                console.log(`Joined user group: user-${userId}`);
            } catch (error) {
                console.error("Failed to join user group:", error);
            }
        }
    }

    /**
     * Join conversation group
     */    async joinConversationGroup(userId1, userId2) {
        const groupId = userId1 < userId2 ? 
            `conv-${userId1}-${userId2}` : 
            `conv-${userId2}-${userId1}`;
            
        if (this.connection && this.connection.state === signalR?.HubConnectionState?.Connected) {
            try {
                await this.connection.invoke("JoinGroup", groupId);
                console.log(`Joined conversation group: ${groupId}`);
            } catch (error) {
                console.error("Failed to join conversation group:", error);
            }
        }
    }
    
    /**
     * Get JWT token from cookie
     */
    getJwtToken() {
        // Log all cookies for debugging
        console.log("All cookies:", document.cookie);
        
        const cookies = document.cookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i].trim();
            console.log("Checking cookie:", cookie);
            if (cookie.startsWith('jwt_token=')) {
                const token = cookie.substring('jwt_token='.length, cookie.length);
                console.log("Found JWT token:", token);
                return token;
            }
        }        console.log("JWT token not found in cookies");
        return null;
    }    /**
     * Send message to API
     */
    async sendMessage(gonderenId, gonderenTipi, aliciId, aliciTipi, icerik) {
        try {
            // Validate parameters
            if (!gonderenId || !aliciId || !icerik) {
                throw new Error("Missing required parameters");
            }
            
            // Using only credentials: 'include' since server will now check for the JWT in cookies
            console.log("Sending message with credentials included");
            
            // Log all parameters for debugging
            console.log("Message parameters:", {
                gonderenId, gonderenTipi, aliciId, aliciTipi, icerik
            });
            
            // Ensure proper formatting for GUIDs
            const formatGuid = (guid) => {
                // If it's already a valid GUID string, return as is
                if (typeof guid === 'string' && /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(guid)) {
                    return guid;
                }
                // If it's a string but not formatted as GUID, try to format it
                if (typeof guid === 'string' && guid.length > 30) {
                    // Try to format it if it's a GUID without dashes
                    if (/^[0-9a-f]{32}$/i.test(guid)) {
                        return `${guid.substring(0, 8)}-${guid.substring(8, 12)}-${guid.substring(12, 16)}-${guid.substring(16, 20)}-${guid.substring(20)}`;
                    }
                }
                // Return as is if we can't determine the format
                return guid;
            };
            
            // Prepare the request body
            const requestBody = {
                GonderenId: formatGuid(gonderenId),
                GonderenTipi: gonderenTipi,
                AliciId: formatGuid(aliciId),
                AliciTipi: aliciTipi,
                Icerik: icerik
            };
            
            console.log("Request body:", JSON.stringify(requestBody));
            
            const response = await fetch("/api/Messages", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": this.getAntiForgeryToken()
                },
                credentials: 'include', // Include authentication cookies
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                // Try to get more detailed error information
                let errorDetail = "";
                try {
                    const errorResponse = await response.json();
                    errorDetail = JSON.stringify(errorResponse);
                } catch (e) {
                    errorDetail = await response.text();
                }
                
                throw new Error(`HTTP error! status: ${response.status}, details: ${errorDetail}`);
            }            const result = await response.json();
            console.log("Message sent successfully:", result);
            return result;
        } catch (error) {
            console.error("Failed to send message:", error);
            throw error;
        }
    }
    
    /**
     * Get conversation messages
     */
    async getConversation(user1Id, user1Type, user2Id, user2Type, count = 50) {
        try {
            const url = `/api/Messages/conversation?user1Id=${user1Id}&user1Type=${encodeURIComponent(user1Type)}&user2Id=${user2Id}&user2Type=${encodeURIComponent(user2Type)}&count=${count}`;
            
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: 'include' // Include authentication cookies
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const messages = await response.json();
            console.log("Conversation loaded:", messages);
            return messages;
        } catch (error) {            console.error("Failed to load conversation:", error);
            throw error;
        }
    }
    
    /**
     * Mark message as read
     */
    async markAsRead(mesajId, okuyanId, okuyanTipi) {
        try {
            const response = await fetch(`/api/Messages/${mesajId}/read?okuyanId=${okuyanId}&okuyanTipi=${encodeURIComponent(okuyanTipi)}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: 'include' // Include authentication cookies
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            console.log("Message marked as read");
            return true;
        } catch (error) {            console.error("Failed to mark message as read:", error);
            return false;
        }
    }
    
    /**
     * Get unread messages
     */
    async getUnreadMessages(userId, userType) {
        try {
            const response = await fetch(`/api/Messages/unread?userId=${userId}&userType=${encodeURIComponent(userType)}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: 'include' // Include authentication cookies
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const messages = await response.json();
            return messages;
        } catch (error) {
            console.error("Failed to get unread messages:", error);
            throw error;
        }
    }

    /**
     * Get anti-forgery token for forms
     */
    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    /**
     * Event handler setters
     */
    onMessageReceived(callback) {
        this.messageReceivedCallback = callback;
    }

    onMessageRead(callback) {
        this.messageReadCallback = callback;
    }

    onUserConnected(callback) {
        this.userConnectedCallback = callback;
    }

    onUserDisconnected(callback) {
        this.userDisconnectedCallback = callback;
    }

    /**
     * Utility method to format date/time
     */
    formatTime(dateTime) {
        const date = new Date(dateTime);
        return date.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
    }

    /**
     * Utility method to format date
     */
    formatDate(dateTime) {
        const date = new Date(dateTime);
        const today = new Date();
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);

        if (date.toDateString() === today.toDateString()) {
            return this.formatTime(dateTime);
        } else if (date.toDateString() === yesterday.toDateString()) {
            return 'Dün';
        } else {
            return date.toLocaleDateString('tr-TR');
        }
    }

    /**
     * Cleanup method
     */
    async disconnect() {
        if (this.connection) {
            await this.connection.stop();
        }
    }
}

// Export for global use
window.MessageService = MessageService;
