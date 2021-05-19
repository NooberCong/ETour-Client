using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class AuthController : Controller
    {
        // Display sign in page with a button for authenticating using Google
        // Return View()
        public IActionResult Index()
        {
            return View();
        }

        // This action is called when sign in with google button is clicked
        // Return Challenge result with google authentication scheme
        public IActionResult SignInWithGoogle(string returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("SignInWithGoogleCallback"),
                Items = { { "returnUrl", returnUrl } }
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        // This action is called when Google finishes authenticating user
        // Check if the user is already in database, if not add user to database
        // Redirect to returnUrl item param on completion
        public async Task<IActionResult> SignInWithGoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync();
            // Todo: Add user to database if user is new
            return LocalRedirect(result.Properties.Items["returnUrl"]);
        }
    }
}
