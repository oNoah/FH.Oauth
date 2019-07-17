using Microsoft.AspNetCore.Identity;

namespace OAuth
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }

    }
}