﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearningSystem.App.ViewModels
{
    public class SkillViewModel
    {
        public IEnumerable<IGrouping<int, LessonViewModel>> Lessons { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public bool OwnedByUser { get; set; }

        public int? CompletePercent { get; set; }
    }
}