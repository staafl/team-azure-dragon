using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using TeamAzureDragon.Utils;

namespace LearningSystem.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Question statement is required!")]
        [DataType(DataType.MultilineText)]
        [StringLength(3000)]
        //[StringNotContains("<script>")]
        public string Statement { get; set; }

        [Required(ErrorMessage = "Order is required!")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Points are required!")]
        public int Points { get; set; }

        [Required(ErrorMessage = "Answer Type is required!")]
        [Display(Name = "Answer type")]
        public string AnswerType { get; set; }

        [StringLength(2000)]
        [Display(Name = "Answer content")]
        [DataType(DataType.MultilineText)]
        public string AnswerContent { get; set; }

        [Display(Name = "Content version")]
        public int AnswerContentVersion { get; set; }

        [Display(Name = "Exercise")]
        public int ExerciseId { get; set; }

        public virtual Exercise Exercise { get; set; }

    }
}
