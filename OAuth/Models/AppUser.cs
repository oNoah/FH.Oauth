using Microsoft.AspNetCore.Identity;
using System;

namespace OAuth
{
    /// <summary>
    /// 自定义用户类 扩展字段修改主键类型
    /// </summary>
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