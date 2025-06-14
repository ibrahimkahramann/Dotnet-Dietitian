/**
 * ContactsService - Handles the messaging contacts functionality
 */
class ContactsService {
    constructor() {
        this.messageService = null;
        this.currentUserId = null;
        this.currentUserType = null;
        this.contactsCache = new Map();
        this.contactsList = document.querySelector('.contacts-list');
        this.searchInput = document.getElementById('searchContact');
        
        // Initialize event listeners
        this.initEventListeners();
    }
    
    /**
     * Initialize the service with user information and message service
     */
    initialize(userId, userType, messageService) {
        this.currentUserId = userId;
        this.currentUserType = userType;
        this.messageService = messageService;
        
        // Load contacts based on user type
        this.loadContacts();
        
        // Listen for new messages to update contact list
        if (this.messageService) {
            this.messageService.onMessageReceived((message) => {
                this.updateContactWithLatestMessage(message);
            });
        }
        
        console.log(`ContactsService initialized for ${userType} with ID ${userId}`);
    }
    
    /**
     * Initialize event listeners
     */
    initEventListeners() {
        // Handle search input
        if (this.searchInput) {
            this.searchInput.addEventListener('input', (e) => {
                this.filterContacts(e.target.value);
            });
        }
    }
      /**
     * Load contacts based on user type
     */
    async loadContacts() {
        try {
            if (!this.currentUserId || !this.currentUserType) {
                console.error('User information not set');
                return;
            }
            
            // Try both new API endpoints and fallback to old endpoints
            let url = '';
            let fallbackUrl = '';
            const userType = this.currentUserType.toLowerCase();
            
            // First try the dedicated message contacts endpoints
            if (userType === 'diyetisyen' || userType === 'dietitian') {
                url = `/api/Messages/contacts/diyetisyen/${this.currentUserId}`;
                fallbackUrl = `/api/Dietitians/${this.currentUserId}/hastalar`;
            } else if (userType === 'hasta' || userType === 'patient') {
                url = `/api/Messages/contacts/hasta/${this.currentUserId}`;
                fallbackUrl = `/api/Patients/${this.currentUserId}/diyetisyen`;
            } else {
                console.error('Unsupported user type:', this.currentUserType);
                return;
            }
            
            console.log(`Trying to load contacts from ${url}`);
            
            // Try the primary URL first
            let response;
            try {
                response = await fetch(url, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include' // Include authentication cookies
                });
            } catch (primaryError) {
                console.warn(`Primary contacts endpoint failed: ${primaryError.message}`);
                console.log(`Falling back to ${fallbackUrl}`);
                
                // Try fallback URL if primary fails
                response = await fetch(fallbackUrl, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include' // Include authentication cookies
                });
            }
            
            if (!response.ok) {
                throw new Error(`Failed to load contacts: ${response.status}`);
            }
            
            const contacts = await response.json();
            console.log('Loaded contacts:', contacts);
            
            // Cache and display contacts
            this.cacheContacts(contacts);
            this.renderContacts(contacts);
            
            // If the current UI has selected contact, update unread count
            this.updateUnreadCountForAll();
            
        } catch (error) {
            console.error('Error loading contacts:', error);
            // Show error message in contacts list
            if (this.contactsList) {
                this.contactsList.innerHTML = `
                    <div class="text-center p-4">
                        <p class="text-danger mb-0">Kişiler yüklenemedi: ${error.message}</p>
                        <button class="btn btn-sm btn-outline-primary mt-2" onclick="contactsService.loadContacts()">
                            Tekrar Dene
                        </button>
                    </div>
                `;
            }
        }
    }
    
    /**
     * Cache contacts for quick access
     */
    cacheContacts(contacts) {
        if (!Array.isArray(contacts)) {
            // Handle single contact (usually for patient view)
            if (contacts && contacts.id) {
                this.contactsCache.set(contacts.id, contacts);
            }
            return;
        }
        
        contacts.forEach(contact => {
            if (contact && contact.id) {
                this.contactsCache.set(contact.id, contact);
            }
        });
    }
    
    /**
     * Render contacts in the contacts list
     */
    renderContacts(contacts) {
        if (!this.contactsList) {
            console.error('Contacts list element not found');
            return;
        }
        
        // Clear existing contacts
        this.contactsList.innerHTML = '';
        
        if (!contacts || (Array.isArray(contacts) && contacts.length === 0)) {
            // No contacts
            this.contactsList.innerHTML = `
                <div class="text-center p-4">
                    <p class="mb-0 text-muted">Henüz kişi bulunmuyor.</p>
                </div>
            `;
            return;
        }
        
        // Handle single contact object (usually for patient view)
        if (!Array.isArray(contacts) && contacts.id) {
            contacts = [contacts];
        }
        
        // Create and append contact items
        contacts.forEach(contact => {
            if (!contact || !contact.id) return;
            
            const contactElement = this.createContactElement(contact);
            this.contactsList.appendChild(contactElement);
        });
    }
    
    /**
     * Create a contact list item element
     */
    createContactElement(contact) {
        const contactDiv = document.createElement('div');
        contactDiv.className = 'contact-item d-flex align-items-center p-3 border-bottom text-decoration-none text-dark';
        contactDiv.setAttribute('data-id', contact.id);
        contactDiv.setAttribute('role', 'button');
        
        // Determine the name based on contact type
        let contactName = '';
        if (this.currentUserType === 'Diyetisyen') {
            contactName = `${contact.ad || ''} ${contact.soyad || ''}`;
        } else {
            contactName = contact.adSoyad || contact.fullName || 'İsimsiz Diyetisyen';
        }
        
        contactDiv.innerHTML = `
            <div class="position-relative me-3">
                <img src="https://images.unsplash.com/photo-1633332755192-727a05c4013d?w=150&h=150&fit=crop&auto=format" 
                     class="rounded-circle" width="50" height="50" alt="${contactName}">
                <span class="position-absolute bottom-0 end-0 bg-success rounded-circle" style="width: 12px; height: 12px;"></span>
            </div>
            <div class="flex-grow-1">
                <h6 class="mb-1">${contactName}</h6>
                <p class="text-muted small mb-0 last-message">Son mesaj yükleniyor...</p>
            </div>
            <div class="text-end ms-2">
                <small class="text-muted last-message-time"></small>
                <div class="badge bg-danger rounded-pill unread-count d-none">0</div>
            </div>
        `;
        
        // Add click event to select contact
        contactDiv.addEventListener('click', () => {
            this.selectContact(contact.id, contactName);
        });
        
        return contactDiv;
    }
    
    /**
     * Filter contacts based on search text
     */
    filterContacts(searchText) {
        if (!this.contactsList) return;
        
        const contacts = this.contactsList.querySelectorAll('.contact-item');
        const text = searchText.toLowerCase();
        
        contacts.forEach(contact => {
            const name = contact.querySelector('h6').textContent.toLowerCase();
            if (name.includes(text)) {
                contact.style.display = '';
            } else {
                contact.style.display = 'none';
            }
        });
    }
    
    /**
     * Select a contact to start conversation
     */
    selectContact(contactId, contactName) {
        // Update UI to show selected contact
        const contacts = document.querySelectorAll('.contact-item');
        contacts.forEach(contact => {
            contact.classList.remove('active', 'bg-light');
        });
        
        const selectedContact = document.querySelector(`.contact-item[data-id="${contactId}"]`);
        if (selectedContact) {
            selectedContact.classList.add('active', 'bg-light');
        }
        
        // Update chat header
        const chatHeaderName = document.getElementById('chatHeaderName');
        if (chatHeaderName) {
            chatHeaderName.textContent = contactName;
        }
        
        // Hide empty state and show chat area
        const emptyState = document.getElementById('emptyState');
        const chatArea = document.getElementById('chatArea');
        
        if (emptyState) emptyState.classList.add('d-none');
        if (chatArea) {
            chatArea.classList.remove('d-none');
            chatArea.style.display = 'flex';
        }
        
        // Load conversation if message service is available
        if (this.messageService) {
            this.loadConversation(contactId);
        }
        
        // Reset unread count
        this.resetUnreadCount(contactId);
        
        // Trigger custom event for external handlers
        const event = new CustomEvent('contactSelected', {
            detail: {
                contactId,
                contactName
            }
        });
        document.dispatchEvent(event);
    }
    
    /**
     * Load conversation with selected contact
     */
    async loadConversation(contactId) {
        if (!this.messageService) {
            console.error('Message service not initialized');
            return;
        }
        
        const messagesContainer = document.getElementById('messagesContainer');
        if (!messagesContainer) return;
        
        // Show loading state
        messagesContainer.innerHTML = '<div class="text-center p-5"><div class="spinner-border text-primary" role="status"></div><p class="mt-2">Mesajlar yükleniyor...</p></div>';
        
        try {
            // Set the recipient type based on current user type
            let recipientType = this.currentUserType === 'Diyetisyen' ? 'Hasta' : 'Diyetisyen';
            
            // Get conversation messages
            const messages = await this.messageService.getConversation(
                this.currentUserId, 
                this.currentUserType,
                contactId,
                recipientType
            );
            
            console.log('Loaded conversation:', messages);
            
            // Display messages
            this.displayMessages(messages);
            
            // Join conversation group in SignalR
            await this.messageService.joinConversationGroup(this.currentUserId, contactId);
            
            // Mark unread messages as read
            this.markMessagesAsRead(messages);
            
            // Update last seen message for this contact
            this.updateLastMessage(contactId, messages);
            
        } catch (error) {
            console.error('Error loading conversation:', error);
            messagesContainer.innerHTML = `
                <div class="text-center p-4">
                    <p class="text-danger mb-0">Mesajlar yüklenemedi: ${error.message}</p>
                    <button class="btn btn-sm btn-outline-primary mt-2" onclick="contactsService.loadConversation('${contactId}')">
                        Tekrar Dene
                    </button>
                </div>
            `;
        }
    }
    
    /**
     * Display messages in the messages container
     */
    displayMessages(messages) {
        const messagesContainer = document.getElementById('messagesContainer');
        if (!messagesContainer) return;
        
        // Clear messages container
        messagesContainer.innerHTML = '';
        
        if (!messages || messages.length === 0) {
            messagesContainer.innerHTML = `
                <div class="text-center p-4">
                    <p class="text-muted mb-0">Henüz mesaj bulunmuyor. Mesaj göndererek sohbete başlayabilirsiniz.</p>
                </div>
            `;
            return;
        }
        
        // Sort messages by date
        messages.sort((a, b) => new Date(a.gonderimZamani) - new Date(b.gonderimZamani));
        
        let previousDate = null;
        
        // Display each message
        messages.forEach(message => {
            // Check if we need to display a date separator
            const messageDate = new Date(message.gonderimZamani);
            const messageDay = messageDate.toLocaleDateString();
            
            if (previousDate !== messageDay) {
                const dateDiv = document.createElement('div');
                dateDiv.className = 'text-center my-3';
                dateDiv.innerHTML = `<span class="badge bg-light text-dark px-3 py-2">${this.formatDate(messageDate)}</span>`;
                messagesContainer.appendChild(dateDiv);
                previousDate = messageDay;
            }
            
            // Determine if message is sent by current user
            const isSentByMe = message.gonderenId === this.currentUserId && message.gonderenTipi === this.currentUserType;
            
            // Create message element
            const messageDiv = document.createElement('div');
            messageDiv.className = `d-flex mb-3 ${isSentByMe ? 'justify-content-end' : ''}`;
            messageDiv.setAttribute('data-message-id', message.id);
            
            if (isSentByMe) {
                messageDiv.innerHTML = `
                    <div class="bg-primary text-white rounded-3 p-3" style="max-width: 75%;">
                        <p class="mb-0">${this.escapeHtml(message.icerik)}</p>
                        <small class="mt-1 d-block text-end text-white-50">${this.formatTime(messageDate)}</small>
                    </div>
                `;
            } else {
                messageDiv.innerHTML = `
                    <img src="https://images.unsplash.com/photo-1633332755192-727a05c4013d?w=150&h=150&fit=crop&auto=format" 
                         class="rounded-circle align-self-start me-3" width="35" height="35" alt="Contact">
                    <div class="bg-light rounded-3 p-3" style="max-width: 75%;">
                        <p class="mb-0">${this.escapeHtml(message.icerik)}</p>
                        <small class="text-muted mt-1 d-block">${this.formatTime(messageDate)}</small>
                    </div>
                `;
            }
            
            messagesContainer.appendChild(messageDiv);
        });
        
        // Scroll to bottom
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
    
    /**
     * Mark messages as read
     */
    async markMessagesAsRead(messages) {
        if (!this.messageService) return;
        
        // Get unread messages not sent by current user
        const unreadMessages = messages.filter(message => 
            !message.okundu && 
            (message.gonderenId !== this.currentUserId || message.gonderenTipi !== this.currentUserType)
        );
        
        // Mark each message as read
        for (const message of unreadMessages) {
            try {
                await this.messageService.markAsRead(
                    message.id,
                    this.currentUserId,
                    this.currentUserType
                );
            } catch (error) {
                console.error('Error marking message as read:', error);
            }
        }
        
        // Update unread count for the contact
        if (unreadMessages.length > 0) {
            const contactId = unreadMessages[0].gonderenId;
            this.resetUnreadCount(contactId);
        }
    }
    
    /**
     * Update the last message shown for a contact
     */
    updateLastMessage(contactId, messages) {
        if (!messages || messages.length === 0) return;
        
        // Get the latest message
        const latestMessage = messages.sort((a, b) => 
            new Date(b.gonderimZamani) - new Date(a.gonderimZamani)
        )[0];
        
        // Find the contact element
        const contactElement = document.querySelector(`.contact-item[data-id="${contactId}"]`);
        if (!contactElement) return;
        
        // Update last message text and time
        const lastMessageElement = contactElement.querySelector('.last-message');
        const lastTimeElement = contactElement.querySelector('.last-message-time');
        
        if (lastMessageElement) {
            // Truncate message if too long
            const messagePreview = latestMessage.icerik.length > 30 
                ? latestMessage.icerik.substring(0, 27) + '...' 
                : latestMessage.icerik;
                
            lastMessageElement.textContent = messagePreview;
        }
        
        if (lastTimeElement) {
            lastTimeElement.textContent = this.formatTime(new Date(latestMessage.gonderimZamani));
        }
    }
    
    /**
     * Update contact with latest message
     */
    updateContactWithLatestMessage(message) {
        // Determine which ID is the contact (not the current user)
        let contactId;
        if (message.gonderenId === this.currentUserId && message.gonderenTipi === this.currentUserType) {
            contactId = message.aliciId; 
        } else {
            contactId = message.gonderenId;
            
            // Increment unread count if not from current user and not in current conversation
            const selectedContactId = document.querySelector('.contact-item.active')?.dataset.id;
            if (selectedContactId !== contactId) {
                this.incrementUnreadCount(contactId);
            } else {
                // Mark as read immediately if this is the current conversation
                this.messageService.markAsRead(message.id, this.currentUserId, this.currentUserType);
            }
        }
        
        // Find contact element
        const contactElement = document.querySelector(`.contact-item[data-id="${contactId}"]`);
        
        // If contact doesn't exist, we might need to add it
        if (!contactElement && this.contactsCache.has(contactId)) {
            const contact = this.contactsCache.get(contactId);
            const newContactElement = this.createContactElement(contact);
            this.contactsList.insertBefore(newContactElement, this.contactsList.firstChild);
        }
        
        // Get the contact element again (it might have been created)
        const updatedContactElement = document.querySelector(`.contact-item[data-id="${contactId}"]`);
        if (!updatedContactElement) return;
        
        // Update last message text and time
        const lastMessageElement = updatedContactElement.querySelector('.last-message');
        const lastTimeElement = updatedContactElement.querySelector('.last-message-time');
        
        if (lastMessageElement) {
            // Truncate message if too long
            const messagePreview = message.icerik.length > 30 
                ? message.icerik.substring(0, 27) + '...' 
                : message.icerik;
                
            lastMessageElement.textContent = messagePreview;
        }
        
        if (lastTimeElement) {
            lastTimeElement.textContent = this.formatTime(new Date(message.gonderimZamani));
        }
        
        // Move contact to top of list
        if (updatedContactElement.parentElement) {
            updatedContactElement.parentElement.insertBefore(
                updatedContactElement, 
                updatedContactElement.parentElement.firstChild
            );
        }
    }
    
    /**
     * Increment unread count for a contact
     */
    incrementUnreadCount(contactId) {
        const contactElement = document.querySelector(`.contact-item[data-id="${contactId}"]`);
        if (!contactElement) return;
        
        const unreadBadge = contactElement.querySelector('.unread-count');
        if (!unreadBadge) return;
        
        let count = parseInt(unreadBadge.textContent, 10) || 0;
        count++;
        
        unreadBadge.textContent = count;
        unreadBadge.classList.remove('d-none');
    }
    
    /**
     * Reset unread count for a contact
     */
    resetUnreadCount(contactId) {
        const contactElement = document.querySelector(`.contact-item[data-id="${contactId}"]`);
        if (!contactElement) return;
        
        const unreadBadge = contactElement.querySelector('.unread-count');
        if (!unreadBadge) return;
        
        unreadBadge.textContent = '0';
        unreadBadge.classList.add('d-none');
    }
    
    /**
     * Update unread counts for all contacts
     */
    async updateUnreadCountForAll() {
        if (!this.messageService) return;
        
        try {
            const unreadMessages = await this.messageService.getUnreadMessages(
                this.currentUserId,
                this.currentUserType
            );
            
            // Group by sender
            const unreadCounts = {};
            unreadMessages.forEach(message => {
                const senderId = message.gonderenId;
                unreadCounts[senderId] = (unreadCounts[senderId] || 0) + 1;
            });
            
            // Update UI
            Object.keys(unreadCounts).forEach(senderId => {
                const contactElement = document.querySelector(`.contact-item[data-id="${senderId}"]`);
                if (!contactElement) return;
                
                const unreadBadge = contactElement.querySelector('.unread-count');
                if (!unreadBadge) return;
                
                unreadBadge.textContent = unreadCounts[senderId];
                unreadBadge.classList.remove('d-none');
            });
            
        } catch (error) {
            console.error('Error updating unread counts:', error);
        }
    }
    
    /**
     * Format date for messages
     */
    formatDate(date) {
        const today = new Date();
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);
        
        if (date.toDateString() === today.toDateString()) {
            return 'Bugün';
        } else if (date.toDateString() === yesterday.toDateString()) {
            return 'Dün';
        } else {
            return date.toLocaleDateString('tr-TR');
        }
    }
    
    /**
     * Format time for messages
     */
    formatTime(date) {
        return date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
    }
    
    /**
     * Escape HTML to prevent XSS
     */
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
    
    /**
     * Run a diagnostic check on messaging components
     * Returns an object with the diagnostic results
     */
    async runDiagnostics() {
        const results = {
            authStatus: 'Unknown',
            contactsLoaded: false,
            signalRStatus: 'Not checked',
            apiConnectivity: false,
            errors: []
        };
        
        try {
            // Check if the user is authenticated
            const tokenCookie = document.cookie.split('; ').find(row => row.startsWith('jwt_token='));
            const hasCookie = !!tokenCookie;
            
            results.authStatus = hasCookie ? 'Authenticated with JWT cookie' : 'No JWT cookie found';
            
            // Check if contacts were loaded
            results.contactsLoaded = this.contactsCache.size > 0;
            
            // Check SignalR connection
            if (this.messageService?.connection) {
                const connectionState = this.messageService.connection.state;
                results.signalRStatus = `Connection state: ${connectionState}`;
                
                if (connectionState !== signalR?.HubConnectionState?.Connected) {
                    results.errors.push('SignalR not connected. This will prevent real-time message updates.');
                }
            } else {
                results.signalRStatus = 'No SignalR connection found';
                results.errors.push('SignalR connection not initialized.');
            }
            
            // Check API connectivity
            try {
                const testResponse = await fetch('/api/Messages/unread?userId=' + this.currentUserId + '&userType=' + encodeURIComponent(this.currentUserType), {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                });
                
                results.apiConnectivity = testResponse.ok;
                if (!testResponse.ok) {
                    results.errors.push(`API connectivity issue: ${testResponse.status} ${testResponse.statusText}`);
                }
            } catch (error) {
                results.apiConnectivity = false;
                results.errors.push(`API connectivity error: ${error.message}`);
            }
            
            return results;
        } catch (error) {
            results.errors.push(`Diagnostic error: ${error.message}`);
            return results;
        }
    }
}

// Export for global use
window.ContactsService = ContactsService;
