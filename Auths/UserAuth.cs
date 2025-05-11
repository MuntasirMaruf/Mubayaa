﻿using Mubayaa.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mubayaa.Auth
{
    public class UserAuth : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = (Login)httpContext.Session["User"];
            if (user != null && user.Status.Equals(1)) return true;
            return false;
        }
    }
}