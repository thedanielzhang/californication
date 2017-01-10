using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SpotiFind.BusinessLogic;
using SpotiFind.Models;

namespace SpotiFind.Controllers
{

    public class AuthenticationController : ApiController
    {
        private BusinessLogic.BusinessLogic businessLogic = new BusinessLogic.BusinessLogic();

        // GET: api/Authentication
        public string GetAuthentication()
        {
            string authentication = businessLogic.GetAccessToken();
            return authentication;
        }

    }
}
