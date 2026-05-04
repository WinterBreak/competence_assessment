using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CompetenceAssessment.Web.Attributes;

namespace CompetenceAssessment.Web.Handlers
{
    public class AssessmentAuthorizationHandler : AuthorizationHandler<AssessmentAuthorizeAttribute>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssessmentAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ILogger<AssessmentAuthorizationHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            AssessmentAuthorizeAttribute requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            var endpoint = httpContext?.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
            
            if (allowAnonymous != null)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            
            var user = context.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;
            
            if (!isAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(requirement.Role) 
                && (requirement.Roles == null || !requirement.Roles.Any()))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            
            var userRoles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            if (!string.IsNullOrEmpty(requirement.Role))
            {
                var roles = requirement.Role.Split(',');
                var hasRole = roles.Any(role => userRoles.Contains(role.Trim()));
                
                if (!hasRole)
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }

            if (requirement.Roles != null && requirement.Roles.Any())
            {
                var hasRoles = requirement.Roles.Any(role => userRoles.Contains(role.Trim()));
                if (!hasRoles)
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
            
            return Task.CompletedTask;
        }
    }
}