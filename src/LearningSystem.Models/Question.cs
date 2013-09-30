using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LearningSystem.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Question statement is required!")]
        [StringLength(3000)]
        //[StringNotContains("<script>")]
        public string Statement { get; set; }

        [Required(ErrorMessage = "Order is required!")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Points are required!")]
        public int Points { get; set; }

        [Required(ErrorMessage = "Answer Type is required!")]
        [Display(Name = "Answer type")]
        public AnswerType AnswerType { get; set; }

        [StringLength(2000)]
        [Display(Name="Answer content")]
        public string AnswerContent { get; set; }

        [Display(Name = "Exercise")]
        public int ExerciseId { get; set; }

        public virtual Exercise Exercise { get; set; }

    }
}
