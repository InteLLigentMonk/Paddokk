using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.User
{
    public class UpdateUserRequest
    {
        [StringLength(100)]
        public string? DisplayName { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
