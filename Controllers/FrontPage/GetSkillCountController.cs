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
            var skillCounts = from mps in _context.MapProviderSkills
                             join s in _context.Skills on mps.SkillId equals s.Id
                            group mps.ProviderId by s.Name into g
                            select new SkillCount { skill = g.Key, count = g.Count() };

            // Add a value to the current real value
            var skillCount = skillCounts.Select(sc => new SkillCount
            {
                skill = sc.skill,
                count = sc.count + AddValue(sc.skill)
            });

            return skillCount;
        }

    // Add a value to the skill count
    public int AddValue(string skillName)
    {
        if (skillName == "General Skill") return 56;
        if (skillName == "Advertising") return 7;
        if (skillName == "Babysitting") return 4;
        if (skillName == "Beauty") return 23;
        if (skillName == "Food") return 12;
        if (skillName == "Charity") return 4;
        if (skillName == "Cleaning") return 23;
        if (skillName == "Construction") return 34;
        if (skillName == "Consultancy") return 2;
        if (skillName == "Design") return 0;
        if (skillName == "Education") return 12;
        if (skillName == "Engineering") return 3;
        if (skillName == "Farming") return 23;
        if (skillName == "Finances") return 5;
        if (skillName == "Fitness") return 2;
        if (skillName == "Gardening") return 9;
        if (skillName == "Healthcare") return 3;
        if (skillName == "Homecare") return 9;
        if (skillName == "Hospitality") return 10;
        if (skillName == "Housekeeping") return 43;
        if (skillName == "IT and Computers") return 18;
        if (skillName == "Legal") return 2;
        if (skillName == "Logistics") return 54;
        if (skillName == "Repair") return 15;
        if (skillName == "Sales") return 3;
        if (skillName == "Secretarial") return 0;
        if (skillName == "Security") return 0;
        if (skillName == "Social Work") return 9;
        if (skillName == "Transport") return 3;
        return 12;
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
