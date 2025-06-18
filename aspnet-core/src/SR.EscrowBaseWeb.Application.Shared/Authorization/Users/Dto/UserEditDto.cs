using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using SR.EscrowBaseWeb.SRUserAnswer.Dtos;

namespace SR.EscrowBaseWeb.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    public class UserEditDto : IPassivable
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }

        //[Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

     
        public string Surname { get; set; }

        //[Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        //[Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(UserConsts.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        // Not used "Required" attribute since empty value is used to 'not change password'
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool IsActive { get; set; }
        public string AssociatedUser { get; set; }
        public string SecondaryEmail { get; set; }
        public string UserTitle { get; set; }
        public string Fax { get; set; }
        public string Cell { get; set; }
        public string EmailConfirmationCode { get; set; }
        public virtual bool IsPhoneNumberConfirmed { get; set; }
        public bool ShouldChangePasswordOnNextLogin { get; set; }
        public string UserIP { get; set; }
        public virtual bool IsTwoFactorEnabled { get; set; }
        public DateTime BlockAttemptsTill { get; set; }
        public virtual bool IsLockoutEnabled { get; set; }
        public DateTime SignInTokenExpireTimeUtc { get; set; }
        public string AttemptsCount { get; set; }
        public string PasswordResetCode { get; set; }
        public List<CreateOrEditUserAnswerDto> UserAnswer { get; set; }

    }
}