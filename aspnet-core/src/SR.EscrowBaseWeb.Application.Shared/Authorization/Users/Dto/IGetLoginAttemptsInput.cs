using Abp.Application.Services.Dto;

namespace SR.EscrowBaseWeb.Authorization.Users.Dto
{
    public interface IGetLoginAttemptsInput: ISortedResultRequest
    {
        string Filter { get; set; }
    }
}