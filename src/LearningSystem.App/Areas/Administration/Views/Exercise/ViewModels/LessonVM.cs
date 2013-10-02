using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LearningSystem.App.Areas.Administration.Views.ViewModels
{
    public class LessonVM
    {
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Lesson name is required!")]
        [StringLength(1000)]
        public string Name { get; set; }
    }
}