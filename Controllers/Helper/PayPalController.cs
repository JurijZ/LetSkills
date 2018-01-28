using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LetSkillsBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Braintree;
using Microsoft.AspNetCore.Http;

namespace LetSkillsBackend.Controllers
{
    [Route("[controller]")]
    public class PayPalController : Controller
    {

        private readonly SkillsContext _context;
        private BraintreeGateway _gateway;
        private PayPalClientToken _clientToken;

        public PayPalController(SkillsContext context)
        {
            _context = context;
            // Production
            _gateway = new BraintreeGateway("access_token$production$cj3qwh2hs9jf2x7t$caf9e01c1b90d4ac86e0e8e8ec1f32d6");
            _clientToken = new PayPalClientToken();
        }

        // GET api/paypal
        [HttpGet]
        [Authorize]
        public PayPalClientToken Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            _clientToken.clientToken = _gateway.ClientToken.Generate();
            Console.WriteLine("-- PayPal Client Token generated - " + _clientToken.clientToken);
                        
            return _clientToken;
        }

        [HttpPost]
        public ActionResult CreatePurchase(FormCollection collection)
        {
            // yet to be implemented
            string nonce = collection["payment_method_nonce"];
            return Ok();
        }

        // POST api/joblocation
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/joblocation/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/joblocation/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }

    public class PayPalClientToken
    {
        public string clientToken { get; set; }
    }
}
