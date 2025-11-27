using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mos3ef.DAL.Enums;

namespace Mos3ef.BLL.Dtos.Auth
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;

        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }

        public string? Name { get; set; }
        public UserType? UserType { get; set; }
    }
}
