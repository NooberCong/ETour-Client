using Client.AuthenticationSchemes;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

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
            var result = await HttpContext.AuthenticateAsync(ExternalAuthenticationDefaults.AuthenticationScheme);

            var externalClaims = result.Principal.Claims.ToList();

            var subjectID = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            Customer customer = await _customerRepository.FindAsync(subjectID);

            if (customer == null)
            {
                customer = new Customer
                {
                    ID = subjectID,
                    Name = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                    Email = externalClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    ImgUrl = externalClaims.FirstOrDefault(x => x.Type == "image")?.Value,
                };

                await _customerRepository.AddAsync(customer);
                await _unitOfWork.CommitAsync();
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, customer.ID),
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim("ImgUrl", customer.ImgUrl)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(ExternalAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return LocalRedirect(result.Properties.Items["returnUrl"]);
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }
    }
}
