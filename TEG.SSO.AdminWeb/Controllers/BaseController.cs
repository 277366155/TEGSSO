using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TEG.SSO.AdminService;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEG.SSO.AdminWeb.Controllers
{
    public class BaseController : Controller
    {
        IServiceProvider _svp;
 
        public BaseController()
        {
            HttpContext.EnableCookie();
        }
        public BaseController(IServiceProvider svp)
        {
            HttpContext.EnableCookie();
            _svp = svp; 
        }

 
    }
}
