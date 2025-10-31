using CemSys2.Models;
using CemSys2.Enumerable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CemSys2.ValidacionAnotations
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly RolUsuario[] _allowedRoles;

        public AuthorizeRoleAttribute(params RolUsuario[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // Verificar autenticación
            var isAuthenticated = session.GetString("IsAuthenticated");
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            // Verificar roles
            var userRole = session.GetInt32("Rol");
            if (userRole == null || !_allowedRoles.Contains((RolUsuario)userRole))
            {
                context.Result = new RedirectResult("~/");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
