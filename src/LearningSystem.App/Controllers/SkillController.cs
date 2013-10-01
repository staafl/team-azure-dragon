using LearningSystem.App.ViewModels;
using LearningSystem.Data;
using LearningSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeamAzureDragon.Utils;

namespace LearningSystem.App.Controllers
{
    public class SkillController : Controller
    {
        IUoWLearningSystem db;
        public SkillController(IUoWLearningSystem db)
        {
            this.db = db;
        }
        //
        // GET: /Skill/
        [Authorize]
        public ActionResult Index(int skillId)
        {
            var user = db.Users.All().Single(x => x.UserName == User.Identity.Name);

            var skill = db.Skills.All().FirstOrDefault(x => x.SkillId == skillId);
            List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
            IEnumerable<Lesson> learnedLessons;

            if (skill != null && skill.Users.Contains(user))
            {
                var lessons = skill.Lessons.ToList();
                HashSet<Lesson> added = new HashSet<Lesson>();
                var parentlessLessons = lessons.Where(x => x.Requirements.Count == 0).ToList();

                learnedLessons  = user.Lessons.Where(x => x.SkillId == skill.SkillId && lessons.Any(y => y.LessonId == x.LessonId));

                sortedLessons.AddRange(parentlessLessons.ToLessonViewModel(0));
                parentlessLessons.ForEach(x => added.Add(x));

                int levelInSkillTree = 1;
                int notInPlace = lessons.Count - added.Count;

                while (notInPlace > 0)
                {
                    foreach (var item in lessons)
                    {
                        if (!added.Contains(item))
                        {
                            if (item.Requirements.Count == 0 || RequirementsAlreadyAdded(item.Requirements, added))
                            {
                                sortedLessons.Add(item.ToLessonViewModel(levelInSkillTree));
                                added.Add(item);
                                notInPlace--;
                            }
                        }
                    }
                    levelInSkillTree++;
                }
            }
            else
            {
                return View("Error", new ErrorModel
                {
                    ActionName = "Index",
                    ControllerName = "Skill",
                    Exception = new InvalidOperationException("Skill doesn't exist or User is not learning this skill")
                });
            }

            SkillViewModel vm = new SkillViewModel();

            foreach (var item in sortedLessons)
            {
                if(learnedLessons.Any(x => x.LessonId == item.Id))
                {
                    item.IsLearned = true;
                }
            }

            vm.SkillName = skill.Name;
            vm.SkillId = skill.SkillId;
            vm.Lessons = sortedLessons.GroupBy(x => x.LevelInSkillTree);
            return View(vm);
        }

        private bool RequirementsAlreadyAdded(ICollection<Lesson> requirements, HashSet<Lesson> added)
        {
            foreach (var item in requirements)
            {
                if (!added.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        [Authorize]
        public ActionResult SignUpForSkill(int skillId)
        {
            var user = db.Users.All().Single(x => x.UserName == User.Identity.Name);

            var skill = db.Skills.All().FirstOrDefault(x => x.SkillId == skillId);



            if (!user.Skills.Contains(skill))
            {
                user.Skills.Add(skill);
                db.SaveChanges();
                ViewBag.Message = "Skill successfully assigned!";
                ViewBag.Success = true;
            }
            else
            {
                ViewBag.Message = "You already learn this skill!";
                ViewBag.Success = false;
            }

            return PartialView("_SignUpForSkill");
        }

        
    }
}