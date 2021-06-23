using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Client.Controllers
{
    public class BaseController: Controller
    {
        public string UserID { get => User.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier)?.Value; }
    }
}
