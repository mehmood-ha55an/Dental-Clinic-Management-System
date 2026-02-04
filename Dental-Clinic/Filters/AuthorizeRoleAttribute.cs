namespace Dental_Clinic.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;

            var username = session.GetString("Username");
            var role = session.GetString("Role");

            // Not logged in
            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult(
                    "Login", "Account", null);
                return;
            }

            // Role not allowed
            if (_roles.Any() && !_roles.Contains(role))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
