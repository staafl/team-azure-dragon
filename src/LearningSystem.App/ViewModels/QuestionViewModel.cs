using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearningSystem.App.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }

        public string Statement { get; set; }

        public string AnswerType { get; set; }

        public string AnswerContent { get; set; }

        public string InputHtml { get; set; }

        public int ExerciseId { get; set; }
    }
}