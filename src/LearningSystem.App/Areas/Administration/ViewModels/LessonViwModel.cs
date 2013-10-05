using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LearningSystem.Models;
using TeamAzureDragon.Utils;
using TeamAzureDragon.Utils.Attributes;

namespace LearningSystem.App.Areas.Administration.ViewModels
{
    public class LessonViewModel : IViewModel<Lesson>
    {
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ModelNavigationId("Skill")]
        [ModelPropertyPath("Skill.SkillId")]
        public int SkillId { get; set; }

        [ModelPropertyPath("Skill.Name")]
        public string SkillName { get; set; }

        [ModelNavigationId("Requirements")]
        [ModelPropertyPath("Requirements.LessonId")]
        public ICollection<int> RequirementsId { get; set; }

        [ModelPropertyPath("Requirements.Name")]
        public ICollection<string> RequirementsName { get; set; }
    }
}