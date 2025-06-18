using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Chat.Dto;

namespace SR.EscrowBaseWeb.Chat
{
    public interface IChatAppService : IApplicationService
    {
        GetUserChatFriendsWithSettingsOutput GetUserChatFriendsWithSettings();

        Task<ListResultDto<ChatMessageDto>> GetUserChatMessages(GetUserChatMessagesInput input);

        Task MarkAllUnreadMessagesOfUserAsRead(MarkAllUnreadMessagesOfUserAsReadInput input);
    }
}
