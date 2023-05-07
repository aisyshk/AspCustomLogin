using AspCustomLogin.Models;
using AspCustomLogin.Services;
using AspCustomLogin.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspCustomLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly LoginHandler _login;

        public AccountController(IUnitOfWork uow)
        {
            _uow = uow;
            _login = new LoginHandler(_uow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            var hashedPass = _login.HashPassword(user.PasswordHash);
            var loginStatus = _login.Login(user.Username, hashedPass);

            if (loginStatus.Item1 <= 0 || !(loginStatus.Item2))
                return RedirectToAction("register");

            /// Sets the session's UserId and UserRole to be matching that of the logged in user.
            HttpContext.Session.SetInt32("UserId", loginStatus.Item1);
            HttpContext.Session.SetString("UserRole", loginStatus.Item3);

            return RedirectToAction("index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> register(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Id == 0)
                {
                    _uow.Logins.Add(new User
                    {
                        UserId = _login.UniqueId(),
                        Username = user.Username,
                        PasswordHash = _login.HashPassword(user.PasswordHash),
                        Role = $"{StaticDetails.Role_Customer}"
                    });
                }
                else { _uow.Logins.Update(user); }

                await _uow.SaveAsync();

                return RedirectToAction(nameof(index));
            }
            else
            {
                if (user.Id != 0)
                {
                    user = _uow.SiteUsers.GetAll(x => x.UserId == user.UserId).FirstOrDefault();
                }
            }

            return View(user);
        }

        /// Resets the session for that user.
        public IActionResult logout()
        {
            HttpContext.Session.SetInt32("UserId", -1);
            HttpContext.Session.SetString("UserRole", String.Empty);

            return RedirectToAction("index", "home");
        }

        /// Redirect when user has no access.
        public IActionResult unauthorized()
        {
            return View();
        }
    }
}