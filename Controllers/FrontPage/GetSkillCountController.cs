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

namespace LetSkillsBackend.Controllers
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

            var startingSkillValues = GetStartingSkillValues();

            // Makes a Group by Skill name
            var skillCounts = from mps in _context.MapProviderSkills
                             join s in _context.Skills on mps.SkillId equals s.Id
                            group mps.ProviderId by s.Name into g
                            select new SkillCount { skill = g.Key, count = g.Count() };

            // Project real values into starting 
            var skillCount = from ssv in startingSkillValues
                             join sc in skillCounts on ssv.skill equals sc.skill into sc
                             from jsc in sc.DefaultIfEmpty()
                             select new SkillCount
                             {
                                  skill = ssv.skill,
                                  count = ssv.count + (jsc == null ? 0 : jsc.count)
                             };

            
            // Add a real value to the starting value
            //var skillCount = skillCounts.Select(sc => new SkillCount
            //{
            //    skill = sc.skill,
            //    count = sc.count + AddValue(sc.skill)
            //});

            return skillCount;
        }

        // Generate starting values
        public IEnumerable<SkillCount> GetStartingSkillValues()
        {
            List<SkillCount> list = new List<SkillCount>();

            list.Add(new SkillCount { skill = "General Skill", count = 54 });
            list.Add(new SkillCount { skill = "Advertising", count = 4 });
            list.Add(new SkillCount { skill = "Babysitting", count = 12 });
            list.Add(new SkillCount { skill = "Beauty", count = 23 });
            list.Add(new SkillCount { skill = "Food", count = 7});
            list.Add(new SkillCount { skill = "Charity", count = 5});
            list.Add(new SkillCount { skill = "Cleaning", count = 43});
            list.Add(new SkillCount { skill = "Construction", count = 32});
            list.Add(new SkillCount { skill = "Consultancy", count = 2});
            list.Add(new SkillCount { skill = "Design", count = 1});
            list.Add(new SkillCount { skill = "Education", count = 7});
            list.Add(new SkillCount { skill = "Engineering", count = 2});
            list.Add(new SkillCount { skill = "Farming", count = 17});
            list.Add(new SkillCount { skill = "Finances", count = 2});
            list.Add(new SkillCount { skill = "Fitness", count = 7});
            list.Add(new SkillCount { skill = "Gardening", count = 31});
            list.Add(new SkillCount { skill = "Healthcare", count = 28});
            list.Add(new SkillCount { skill = "Homecare", count = 34});
            list.Add(new SkillCount { skill = "Hospitality", count = 3});
            list.Add(new SkillCount { skill = "Housekeeping", count = 10});
            list.Add(new SkillCount { skill = "IT and Computers", count = 5});
            list.Add(new SkillCount { skill = "Legal", count = 0});
            list.Add(new SkillCount { skill = "Logistics", count = 9});
            list.Add(new SkillCount { skill = "Repair", count = 13});
            list.Add(new SkillCount { skill = "Sales", count = 34});
            list.Add(new SkillCount { skill = "Secretarial", count = 1});
            list.Add(new SkillCount { skill = "Security", count = 0});
            list.Add(new SkillCount { skill = "Social Work", count = 6});
            list.Add(new SkillCount { skill = "Transport", count = 33});
            IEnumerable<SkillCount> s = list.AsEnumerable();

            return s;
        }

        // Add a value to the skill count (NOT IN USE)
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
