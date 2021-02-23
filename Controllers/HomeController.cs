using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginAndRegistration.Models;
using LoginAndRegistration.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginAndRegistration.Controllers
{
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;

        // public HomeController(ILogger<HomeController> logger)
        // {
        //     _logger = logger;
        // }

        private static MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(User user)
        {
            if (ModelState.IsValid)
            {
                // Initializing a PasswordHasher object, providing our User class as its type
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                //Save your user object to the database
                _context.Users.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserID", user.UserID);
                return RedirectToAction("Success");
            } 
            else 
            {
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("NotAuthorized");
            }
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            User user = _context.Users
                .FirstOrDefault(u => u.UserID == userID);
            return View(user);
        }

        [HttpPost("auth")]
        public IActionResult Login(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                User pulledUser = _context.Users.FirstOrDefault(p => p.Email.Contains(user.LoginEmail));
                if (pulledUser == null) 
                {
                    ModelState.AddModelError("LoginEmail", "Email/Password Invalid");
                    return View("Index");
                }
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(user, pulledUser.Password, user.LoginPassword);
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("LoginPassword", "Email/password Invalid");
                    return View("Index");
                }
                else {
                    HttpContext.Session.SetInt32("UserID", pulledUser.UserID);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Index");
            }
        }

        public IActionResult NotAuthorized(LoginUser user)
        {
                return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
