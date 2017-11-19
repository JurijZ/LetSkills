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
    public class GetSkillCountController : Controller
    {

        private readonly SkillsContext _context;

        public GetSkillCountController(SkillsContext context)
        {
            _context = context;
        }

        // GET api/getskillcount
        [HttpGet]
        public IEnumerable<SkillCount> Get()
        {
            Console.WriteLine("-- GetSkillCount");
            
            // Makes a Group by Skill name
            var skillCount = from mps in _context.MapProviderSkills
                             join s in _context.Skills on mps.SkillId equals s.Id
                            group mps.ProviderId by s.Name into g
                            select new SkillCount { skill = g.Key, count = g.Count() };
            
            return skillCount;
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

    public class SkillCount
    {
        public string skill { get; set; }
        //public int? skillId { get; set; }
        public int? count { get; set; }
    }
}
