using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Authentication
{
    public class ClientCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ICustomerRepository _customerRepository;

        public ClientCookieAuthenticationEvents(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var customer = await _customerRepository.FindAsync(
                context.Principal.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier).Value);

            // User is banned
            if (customer.IsSoftDeleted == true)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
