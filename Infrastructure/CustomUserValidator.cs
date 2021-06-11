using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAndAuthentication.Infrastructure
{
    public class CustomUserValidator : UserValidator<IdentityUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);
            List<IdentityError> lstIdentityErrors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            if(!user.Email.EndsWith("@PsDevs.com"))
            {
                lstIdentityErrors.Add(new IdentityError()
                {
                    Code = "OutOfDomain",
                    Description = "Email must end with @PsDevs.com"
                });
            }

            return lstIdentityErrors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(lstIdentityErrors.ToArray());
        }
    }
}
