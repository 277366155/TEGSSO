using Microsoft.AspNetCore.Mvc;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.WebAPI.Controllers
{
    public class BaseController:ControllerBase
    {
        /// <summary>
        /// 当前用户信息
        /// </summary>
        public TokenUserInfo CurrentUser;

    }
}
