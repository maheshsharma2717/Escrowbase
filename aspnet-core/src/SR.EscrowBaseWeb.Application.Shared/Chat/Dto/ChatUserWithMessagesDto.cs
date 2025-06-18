using System.Collections.Generic;

namespace SR.EscrowBaseWeb.Chat.Dto
{
    public class ChatUserWithMessagesDto : ChatUserDto
    {
        public List<ChatMessageDto> Messages { get; set; }
    }
}