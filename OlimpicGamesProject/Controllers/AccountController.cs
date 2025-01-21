using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace OlimpicGamesProject.Controllers;

public class AccountController : Controller
{
    // GET
    private class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    private static List<User> users = new List<User>
    {
        new User { Email = "admin@wsei.edu.pl", Password = "Admin123@" }
    };
    
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
        if (user != null)
        {
          
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            
            TempData["SuccessMessage"] = "You have logged in successfully!";
            TempData["ReturnUrl"] = returnUrl;

            return RedirectToAction("ListOfAthletes","OlympicGames");
        }

        ViewBag.Error = "Incorrect email or password";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(string email, string password, string confirmPassword, string returnUrl = null)
    {

        var existingUser = users.FirstOrDefault(u => u.Email == email);
        if (existingUser != null)
        {
            ViewBag.Error = "A user with this email address already exists!";
            return View();
        }
        
        if (password != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match!";
            return View();
        }
        if (!IsPasswordValid(password))
        {
            ViewBag.Error = "The password must contain at least one uppercase letter, one special character, and one number.";
            return View();
        }
        users.Add(new User { Email = email, Password = password });

       
        TempData["SuccessMessage"] = "Successfully registered!";
        TempData["ReturnUrl"] = returnUrl;

        return RedirectToAction("Login");
    }
    
    private bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 5)
            return false;

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));
        bool hasDigit = password.Any(char.IsDigit);

        return hasUpperCase && hasSpecialChar && hasDigit;
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
