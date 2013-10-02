using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LearningSystem.App.Areas.Administration.Views.ViewModels
{
    public class ExerciseVM
    {

        [Key]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Exercise name is required!")]
        [StringLength(600)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Order is required!")]
        public int Order { get; set; }
        
        [UIHint("LessonDropdown")]
        public LessonVM Lesson { get; set; }

        [Display(Name = "Lesson")]
        [UIHint("LessonDropdown")]
        public string LessonName { get; set; }
    }
}