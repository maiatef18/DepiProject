using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Auth
{
    public class AssignRoleDto
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string RoleName { get; set; } = null!;
    }
}
