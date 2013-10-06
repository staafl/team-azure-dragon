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
        [ModelNavigationId]
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public SkillViewModel Skill { get; set; }

        public ICollection<LessonViewModel> Requirements { get; set; }

        public class SkillViewModel : IViewModel<Skill>
        {
            public string Name { get; set; }

            [ModelNavigationId]
            public int SkillId { get; set; }
        }
    }
}