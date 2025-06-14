import * as signalR from "@microsoft/signalr";

export class MessageService {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/mesajlasmahub")
            .withAutomaticReconnect()
            .build();
            
        this.connection.on("ReceiveMessage", (message) => {
            if (this.messageReceivedCallback) {
                this.messageReceivedCallback(message);
            }
        });
        
        this.connection.on("MessageRead", (data) => {
            if (this.messageReadCallback) {
                this.messageReadCallback(data);
            }
        });
        
        this.connection.start().catch(err => console.error(err));
    }
    
    onMessageReceived(callback) {
        this.messageReceivedCallback = callback;
    }
    
    onMessageRead(callback) {
        this.messageReadCallback = callback;
    }
    
    joinUserGroup(userId) {
        return this.connection.invoke("JoinGroup", `user-${userId}`);
    }
    
    joinConversationGroup(userId1, userId2) {
        const groupId = userId1 < userId2 ? 
            `conv-${userId1}-${userId2}` : 
            `conv-${userId2}-${userId1}`;
            
        return this.connection.invoke("JoinGroup", groupId);
    }
    
    // API üzerinden mesaj gönderme
    async sendMessage(gonderenId, gonderenTipi, aliciId, aliciTipi, icerik) {
        try {
            const response = await fetch("/api/Messages", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    gonderenId,
                    gonderenTipi,
                    aliciId,
                    aliciTipi,
                    icerik
                })
            });
            return await response.json();
        } catch (error) {
            console.error("Mesaj gönderilirken hata oluştu:", error);
            throw error;
        }
    }
    
    // Görüşme mesajlarını getirme
    async getConversation(user1Id, user1Type, user2Id, user2Type, count = 50) {
        try {
            const response = await fetch(
                `/api/Messages/conversation?user1Id=${user1Id}&user1Type=${user1Type}&user2Id=${user2Id}&user2Type=${user2Type}&count=${count}`
            );
            return await response.json();
        } catch (error) {
            console.error("Görüşme mesajları alınırken hata oluştu:", error);
            throw error;
        }
    }
    
    // Mesajı okundu olarak işaretleme
    async markAsRead(mesajId, okuyanId, okuyanTipi) {
        try {
            await fetch(
                `/api/Messages/${mesajId}/read?okuyanId=${okuyanId}&okuyanTipi=${okuyanTipi}`,
                { method: "POST" }
            );
            return true;
        } catch (error) {
            console.error("Mesaj okundu işaretlenirken hata oluştu:", error);
            return false;
        }
    }
}

// Singleton olarak tek bir servis örneği
export const messageService = new MessageService();