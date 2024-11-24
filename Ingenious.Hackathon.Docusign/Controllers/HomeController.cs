using System.Security.Claims;
using Ingenious.Hackathon.Docusign.Services;
using Ingenious.Hackathon.Docusign.Sql.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace Ingenious.Hackathon.Docusign.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _usersService;
        public HomeController(UserService userService)
        {
            _usersService = userService;
        }

        public IActionResult Index()
        {
            LoginSuccess();
            return View();
        } 
        
        public async Task LoginSuccess()
        {
            string userEmail = null;

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userClaims = User.Claims;
                var userClaimUserId = ((ClaimsIdentity)User.Identity).HasClaim(c => c.Type == "UserId");
                if (!userClaimUserId) //TODO: This is not working, Adding claims for every call
                {
                    userEmail = userClaims.FirstOrDefault(c => c.Type == "emails")?.Value;

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var user = new Users()
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            EmailId = userEmail,
                            UserUpn = userEmail
                        };
                        var addedUser = await _usersService.Add(user);
                    }
                }
            }
        }

        public async Task AccessDenied()
        {

        }
    }
}
