using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using skillsBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Braintree;
using Microsoft.AspNetCore.Http;

namespace skillsBackend.Controllers
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
            _gateway = new BraintreeGateway("access_token$sandbox$k7qv4rtj3x2sjwn2$e6106c8ddfc8a0241077b2220ed98b1f");
            _clientToken = new PayPalClientToken();
        }

        // GET api/getclienttoken
        [HttpGet]
        [Authorize]
        public PayPalClientToken Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + userName.Value); //it's in a JSON format - name: value
            
            //string userName = "jz@gmail.com"; // This value will be taken from the JWT claim
            
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
