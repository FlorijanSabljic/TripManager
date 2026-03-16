using TripManagerWebApp.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TripManagerData.Models;
using TripManagerWebApp.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TripManagerWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly TripManagerDbContext _context;
        public AccountController(TripManagerDbContext context)
        {
            _context = context;
        }

        #region Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserRegisterVM userRegisterVm)
        {
            try
            {
                if (_context.Users.Any(u => u.Username == userRegisterVm.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(userRegisterVm);
                }


                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(userRegisterVm.Password, salt);

                var user = new User
                {
                    Username = userRegisterVm.Username,
                    Email = userRegisterVm.Email,
                    Password = hash,
                    Salt = salt,
                    FirstName = userRegisterVm.FirstName,
                    LastName = userRegisterVm.LastName,
                    Role = "User",
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                return View(userRegisterVm);
            }
        }

        #endregion

        #region Login
        public IActionResult Login(string returnUrl)
        {
            UserLoginVM userLoginVm = new UserLoginVM()
            {
                ReturnUrl = returnUrl
            };

            return View(userLoginVm);
        }

        [HttpPost]
        public IActionResult Login(UserLoginVM userLoginVm)
        {
            try
            {
                var genericLoginFail = "Invalid username or password.";

                var user = _context.Users.FirstOrDefault(u => u.Username == userLoginVm.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, genericLoginFail);
                    return View();
                }

                var hash = PasswordHashProvider.GetHash(userLoginVm.Password, user.Salt);

                if (user.Password != hash)
                {
                    ModelState.AddModelError(string.Empty, genericLoginFail);
                    return View();
                }
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                var claimIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                var task = Task.Run(() =>
                {
                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimIdentity),
                    authProperties);
                });
                task.GetAwaiter().GetResult();

                if (userLoginVm.ReturnUrl != null)
                {
                    return LocalRedirect(userLoginVm.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Logout
        public IActionResult Logout()
        {
            var task = Task.Run(() =>
            {
                HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            });
            task.GetAwaiter().GetResult();
            return View();
        }

        #endregion

        #region Profile
        public IActionResult Profile()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return NotFound();
                }
                var userDetailsVm = new UserDetailsVM
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };
                return View(userDetailsVm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        public IActionResult SaveProfileChanges(UserDetailsVM newUserData)
        {
            try
            {
                var user = _context.Users.Find(newUserData.Id);
                if (user == null) return NotFound();

                user.Email = newUserData.Email;
                user.FirstName = newUserData.FirstName;
                user.LastName = newUserData.LastName;
                user.PhoneNumber = newUserData.PhoneNumber;
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        #endregion
    }
}
