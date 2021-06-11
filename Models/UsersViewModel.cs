using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AuthorizationAndAuthentication.Models
{
    public class UsersViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<IdentityUser> Users { get; set; }
    }
}
