using Microsoft.AspNetCore.Authorization;

namespace CompetenceAssessment.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AssessmentAuthorizeAttribute : Attribute, IAuthorizationRequirement
    {
        public string? Role { get; set; }
        public List<string> Roles { get; set; }
        public bool AllowAnonymous { get; set; }
        
        public AssessmentAuthorizeAttribute() { }
        
        public AssessmentAuthorizeAttribute(string role)
        {
            Role = role;
        }
        
        public AssessmentAuthorizeAttribute(List<string> roles)
        {
            Roles = roles;
        }
    }
}