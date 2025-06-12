using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TT.Core.Enums;

namespace TT.Core.Entities
{
    [Table("TT_SystemUser")]
    public class User
    {
        [Key]
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WHSCode { get; set; }
        public string GroupCode { get; set; }
        public ValueStatus Status { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string RevisedBy { get; set; }
        public DateTime? RevisedDate { get; set; }
        public string FullName => $"{FirstName}, {LastName}";
    }
}
