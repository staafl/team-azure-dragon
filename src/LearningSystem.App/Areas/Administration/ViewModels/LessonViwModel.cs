using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LearningSystem.Models;
using TeamAzureDragon.Utils;
using TeamAzureDragon.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LearningSystem.App.Areas.Administration.ViewModels
{
    public class LessonViewModel : IViewModel<Lesson>
    {
        [ModelNavigationId]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Lesson name is required!")]
        [StringLength(1000)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int SkillId { get; set; }

        [Required(ErrorMessage = "Skill is required!")]
        public SkillViewModel Skill { get; set; }

        public ICollection<LessonViewModel> Requirements { get; set; }
        
    }

    public class LessonViewModelForExercise : IViewModel<Lesson>
    {
        [ModelNavigationId]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Lesson name is required!")]
        [StringLength(1000)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int SkillId { get; set; }


    }
}