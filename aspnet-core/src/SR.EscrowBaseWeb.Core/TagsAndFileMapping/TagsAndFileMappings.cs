using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace SR.EscrowBaseWeb.TagsAndFileMapping
{
    [Table("TagsAndFileMappingses")]
    public class TagsAndFileMappings : Entity
    {

        public virtual int TagId { get; set; }

        public virtual string FileName { get; set; }

    }
}