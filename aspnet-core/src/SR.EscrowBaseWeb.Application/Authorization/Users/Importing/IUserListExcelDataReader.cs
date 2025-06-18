using System.Collections.Generic;
using SR.EscrowBaseWeb.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace SR.EscrowBaseWeb.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
