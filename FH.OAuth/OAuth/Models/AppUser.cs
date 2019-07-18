using Microsoft.AspNetCore.Identity;
using System;

namespace OAuth
{
    public class AppUser : IdentityUser<int>
    {
        public int UserType { get; set; } = 1;
        public int Provider { get; set; }
        public int CreatorId { get; set; }
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public int DataState { get; set; }
        public string Name { get; set; }

    }
}