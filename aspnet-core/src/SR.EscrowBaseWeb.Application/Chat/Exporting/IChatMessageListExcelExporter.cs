using System.Collections.Generic;
using Abp;
using SR.EscrowBaseWeb.Chat.Dto;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}
