﻿using System;
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

namespace LetSkillsBackend.Controllers
{
    [Route("[controller]")]
    public class GetSkillsController : Controller
    {

        private readonly SkillsContext _context;

        public GetSkillsController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/getskills
        [HttpGet]
        [Authorize]
        public AllSkills Get()
        {
            // Users name (it's actually an email) - for this to work in IdentityServer in the ApiClaims must be defined name (and email)
            //var jwtuser = User.Claims.Where(x => x.Type == "name").FirstOrDefault();
            //Console.WriteLine("Authenticated user name is: " + jwtuser.Value); //it's in a {key: value} format
            //var userName = jwtuser.Value;

            Console.WriteLine("Requesting all available Skills from the GetSkillsController");

            // Retrieve all the Skills
            var skillsFullList = (from s in _context.Skills
                                  select s.Name).ToArray();

            var SkillsFullList = new AllSkills
            {
                skills = skillsFullList
            };

            return SkillsFullList;
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

    public class AllSkills
    {
        public string[] skills { get; set; }
    }
}
