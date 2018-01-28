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
    public class PayPalDevController : Controller
    {

        private readonly SkillsContext _context;
        private BraintreeGateway _gateway;
        private PayPalDevClientToken _clientToken;

        public PayPalDevController(SkillsContext context)
        {
            _context = context;
            // Sandbox
            _gateway = new BraintreeGateway("access_token$sandbox$k7qv4rtj3x2sjwn2$e6106c8ddfc8a0241077b2220ed98b1f");
            _clientToken = new PayPalDevClientToken();
        }

        // GET api/PayPalDev
        [HttpGet]
        //[Authorize]
        public PayPalDevClientToken Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            //var userName = jwtuser.Value;

            _clientToken.clientToken = _gateway.ClientToken.Generate();
            Console.WriteLine("-- PayPal Client Token generated - " + _clientToken.clientToken);
                        
            return _clientToken;
        }

        [HttpPost]
        public ActionResult CreatePurchase(FormCollection collection)
        {
            // yet to be implemented
            string nonce = collection["payment_method_nonce"];

            var request = new TransactionRequest
            {
                Amount = 1.00M,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = _gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Console.WriteLine(result.Transaction.Id);
                // Add the amount to the persons Wallet
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
            
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

    public class PayPalDevClientToken
    {
        public string clientToken { get; set; }
    }
}
