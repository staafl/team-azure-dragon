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
    public class QuestionViewModel : IViewModel<Question>
    {
        [ModelNavigationId]
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
        public AnswerType AnswerType { get; set; }

        [StringLength(2000)]
        [Display(Name = "Answer content")]
        [DataType(DataType.MultilineText)]
        public string AnswerContent { get; set; }

        [Display(Name = "Content version")]
        public int AnswerContentVersion { get; set; }
        
        public virtual ExerciseViewModelForQuestions Exercise { get; set; }
    }
}