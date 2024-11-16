using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADXAPI.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresClaimAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _claimName;
        private readonly string _claimsValue;

        public RequiresClaimAttribute(string claimName, string claimsValue)
        {
            _claimName = claimName;
            _claimsValue = claimsValue;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.HasClaim(_claimName, _claimsValue))
            {
                context.Result = new ForbidResult();
            }
    
        }
    }
}
