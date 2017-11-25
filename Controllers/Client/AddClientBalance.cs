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

namespace skillsBackend.Controllers
{
    [Route("[controller]")]
    public class AddClientBalanceController : Controller
    {

        private readonly SkillsContext _context;

        public AddClientBalanceController(SkillsContext context)
        {
            _context = context;
        }

        // POST api/addclientbalance
        [HttpPost]
        [Authorize]
        public ActionResult Post([FromBody] PayPalPayment data)
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            //string userName = "jzilcov@gmail.com"; // This value will be taken from the JWT claim

            string paymentDetails = "Ammount: " + data.ammount + ", PayerID: " + data.payerID + ", PaymentID: " + data.paymentID + ", PaymentToken: " + data.paymentToken;
            Console.WriteLine("--AddClientBalance - " + paymentDetails);

            DateTime currentTime = DateTime.Now;

            var currentBalance = (from u in _context.Users
                                 join w in _context.Wallet on u.Id equals w.UserId
                                 where u.Username == userName
                                 select w).SingleOrDefault();

            if (currentBalance != null)
            {
                Console.WriteLine("Current Balance for the " + userName + " is " + currentBalance.AvailableAmmount);

                // Update Wallet ammount
                // Note - I'm not actually using ammount value from the POST, instead a hardcoded 5 pounds
                currentBalance.AvailableAmmount = currentBalance.AvailableAmmount + 5m;

                // Record change to the WalletHistory table
                var walletHistory = new WalletHistory
                {
                    WalletId = currentBalance.Id,
                    UserId = currentBalance.UserId,
                    AvailableAmountBefore = currentBalance.AvailableAmmount - 5m,   // Im doing a bit reverse logic here
                    BlockedAmountBefore = currentBalance.BlockedAmmount,
                    AvailableAmountAfter = currentBalance.AvailableAmmount,
                    BlockedAmountAfter = currentBalance.BlockedAmmount,
                    TimeStamp = currentTime,
                    Details = paymentDetails
                };
                _context.WalletHistory.Add(walletHistory);

                _context.SaveChanges();

                Console.WriteLine("New Balance for the " + userName + " is " + currentBalance.AvailableAmmount);
            }
            else
            {
                Console.WriteLine("Could not get a balance of the " + userName);
                return NotFound();
            }

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

    public class PayPalPayment
    {
        public string paymentToken { get; set; }
        public string payerID { get; set; }
        public string paymentID { get; set; }
        public decimal ammount { get; set; }
    }
}
