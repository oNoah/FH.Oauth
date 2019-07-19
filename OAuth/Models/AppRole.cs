using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth
{
    /// <summary>
    /// 修改主键类型
    /// </summary>
    public class AppRole: IdentityRole<int>
    {
    }
}
