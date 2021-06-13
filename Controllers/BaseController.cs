using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class BaseController: Controller
    {
        public string UserID { get => User.Claims.First(cl => cl.Type == ClaimTypes.NameIdentifier).Value; }
    }
}
