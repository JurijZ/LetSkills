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
    public class GetClientBalanceController : Controller
    {

        private readonly SkillsContext _context;

        public GetClientBalanceController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/clientprofile
        [HttpGet]
        [Authorize]
        public ClientBalance Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            var userName = jwtuser.Value;

            //string userName = "jzilcov@gmail.com"; // This value will be taken from the JWT claim
            //Console.WriteLine("-- GetClientProfile - Hardcoded user name: " + userName);

            var clientBalance = (from u in _context.Users 
                                join w in _context.Wallet on u.Id equals w.UserId
                                where u.Username == userName
                                select new ClientBalance
                                {
                                    availableAmmount = w.AvailableAmmount,
                                    blockedAmmount = w.BlockedAmmount
                                }).SingleOrDefault();
            
            Console.WriteLine("Available Amount: " + clientBalance.availableAmmount);
            
            return clientBalance;
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

    public class ClientBalance
    {
        public decimal availableAmmount { get; set; }
        public decimal blockedAmmount { get; set; }
    }
}
