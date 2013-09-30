using LearningSystem.App.Models;
using LearningSystem.App.ViewModels;
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
        IUnitOfWork db;
        public SkillController(IUnitOfWork db)
        {
            this.db = db;
        }
        //
        // GET: /Skill/
        [Authorize]
        public ActionResult Index(int skillId)
        {
            var user = db._<User>().All().Single(x => x.UserName == User.Identity.Name);

            var skill = db._<Skill>().All().FirstOrDefault(x => x.SkillId == skillId);

            if (skill != null && skill.Users.Contains(user))
            {
                var lessons = skill.Lessons.ToList();
                List<LessonViewModel> sortedLessons = new List<LessonViewModel>();
                HashSet<Lesson> added = new HashSet<Lesson>();
                var parentlessLessons = lessons.Where(x => x.Requirements.Count == 0).ToList();

                sortedLessons.AddRange(parentlessLessons.ToLessonViewModel());
                parentlessLessons.ForEach(x => added.Add(x));

                int notInPlace = lessons.Count - added.Count;

                while (notInPlace > 0)
                {
                    foreach (var item in lessons)
                    {
                        if (!added.Contains(item))
                        {
                            if (item.Requirements.Count == 0 || RequirementsAlreadyAdded(item.Requirements, added))
                            {
                                sortedLessons.Add(item.ToLessonViewModel());
                                added.Add(item);
                                notInPlace--;
                            }
                        }
                    }

                }


                int a = 5;//debugger is shit;
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


            return View();
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

        
    }
}