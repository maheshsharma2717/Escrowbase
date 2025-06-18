using System.Collections.Generic;
using SR.EscrowBaseWeb.Authorization.Users.Importing.Dto;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
