using AspCustomLogin.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspCustomLogin.Common.Attributes
{
    /// <summary>
    /// This attribute checks if the user is logged in and if they have the correct role to access the page.
    /// We're using a static class 'StaticDetails' to store the role names.
    /// Note, the way that HasRole works is that if a user were to have multiple roles, they'd have to be separated by a comma.
    /// I.e: "Admin,User"
    /// Of course you can change this to your own implementation, but this is just a simple example.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }

        public AuthorizedAttribute(string roles = null)
        {
            Roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user id exists in the current session. You can change this to your own variable name.
            var userId = context.HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If the user is not logged in, redirect to the login page.
                context.Result = new RedirectToActionResult("login", "account", null);
                return;
            }

            /// If the user ID is logged in, check if the user exists in your database.
            var uow = context.HttpContext.RequestServices.GetRequiredService(typeof(IUnitOfWork)) as IUnitOfWork);
            var user = uow.Logins.GetFirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                /// This will redirect the user if they are not logged in and are trying to access an administrator page.
                if (Roles == StaticDetails.Role_Admin)
                {
                    context.Result = new RedirectToActionResult("index", "home", null);
                    return;
                }

                /// This will redirect the user if they are not logged in to /account/unauthorized
                context.Result = new RedirectToActionResult("unauthorized", "account", null);
                return;
            }

            /// This redirects the user if their role is not authorized to access the page.
            if (!HasRole(user.Role, Roles))
            {
                context.Result = new RedirectToActionResult("index", "home", null);
                return;
            }
        }

        private bool HasRole(string roles, string roleName)
        {
            if (String.IsNullOrEmpty(roles)) return false;

            return roles.Split(',').Contains(roleName);
        }
    }
}
