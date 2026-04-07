export interface Agent {
  id: string;
  name: string;
  email: string;
  avatarUrl?: string;
  status: 'Online' | 'Busy' | 'Offline';
}

export interface Contact {
  id: string;
  phoneNumber: string;
  name: string;
  profilePictureUrl?: string;
}

export interface Reaction {
  id: string;
  messageId: string;
  emoji: string;
  fromPhoneNumber: string;
  isFromAgent: boolean;
  createdAt: Date;
}

export interface Message {
  id: string;
  whatsAppMessageId?: string;
  conversationId: string;
  content: string;
  type:
    | 'Text'
    | 'Image'
    | 'Audio'
    | 'Video'
    | 'Document'
    | 'Sticker'
    | 'Reaction'
    | 'Unknown';
  status: 'Pending' | 'Sent' | 'Delivered' | 'Read' | 'Failed';
  mediaUrl?: string;
  isFromAgent: boolean;
  sentByAgentId?: string;
  sentByAgentName?: string;
  quotedMessageId?: string;
  createdAt: Date;
  reactions: Reaction[];
}

export interface Conversation {
  id: string;
  contact: Contact;
  assignedAgent?: Agent;
  status: 'Open' | 'Pending' | 'Resolved' | 'Closed';
  isInQueue: boolean;
  createdAt: Date;
  updatedAt: Date;
  lastMessage?: Message;
}

export interface AuthResponse {
  token: string;
  agent: Agent;
}
