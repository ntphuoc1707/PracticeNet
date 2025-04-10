using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.DTO
{
    public class UserRefreshTokenDTO
    {
        public string UserID { get; set; }
        public string RefreshToken { get; set; }
    }
}
