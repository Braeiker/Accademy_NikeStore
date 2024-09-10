using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NikeStore.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]

        public ICollection<UserRole> UserRoles { get; set; }

    }
}
