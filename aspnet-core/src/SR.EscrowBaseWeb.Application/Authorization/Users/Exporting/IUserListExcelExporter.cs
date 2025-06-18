using System.Collections.Generic;
using SR.EscrowBaseWeb.Authorization.Users.Dto;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}