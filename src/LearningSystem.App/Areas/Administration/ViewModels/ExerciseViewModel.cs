using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TeamAzureDragon.Utils;
using TeamAzureDragon.Utils.Attributes;

namespace LearningSystem.App.Areas.Administration.ViewModels
{
    public class ExerciseViewModel : IViewModel<Exercise>
    {
        [ModelNavigationId]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Exercise name is required!")]
        [StringLength(600)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Order is required!")]
        public int Order { get; set; }

        [Display(Name = "Lesson")]
        public int LessonId { get; set; }

        //[UIHint("LessonDropdown")]
        public virtual LessonViewModelForExercise Lesson { get; set; }
    }

    public class ExerciseViewModelForQuestions : IViewModel<Exercise>
    {
        [ModelNavigationId]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Exercise name is required!")]
        [StringLength(600)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Order is required!")]
        public int Order { get; set; }

        [Display(Name = "Lesson")]
        public int LessonId { get; set; }
    }
}