using Microsoft.AspNetCore.Identity;

namespace NikeStore.Models
{
    public class Role : IdentityRole
    {
        public ICollection<UserRole> UserRoles { get; set; }

    }
}