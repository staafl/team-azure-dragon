using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamAzureDragon.Utils.Attributes;

namespace LearningSystem.App.Areas.Administration.ViewModels
{
    public class LessonViewModel
    {
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ModelNavigationId("Skill")]
        [ModelMapping("Skill.SkillId")]
        public int SkillId { get; set; }

        [ModelMapping("Skill.Name")]
        public string SkillName { get; set; }

        [ModelNavigationId("Requirements")]
        [ModelMapping("Requirements.LessonId")]
        public ICollection<int> RequirementsId { get; set; }

        [ModelMapping("Requirements.Name")]
        public ICollection<string> RequirementsName { get; set; }
    }
}