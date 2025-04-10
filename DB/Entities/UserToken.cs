using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DB.Entities
{
    [Table("UserToken")]
    public class UserToken
    {
        [Key]
        public int Id { get; set; }

        [NotNull]
        public string UserID { get; set; }

        [AllowNull]
        public string RefreshToken { get; set; }

        [AllowNull]
        public DateTime DateCreated { get; set; }
    }
}
