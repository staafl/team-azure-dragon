using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearningSystem.App.ViewModels
{
    public class ExcerciseViewModel
    {
        public int ExerciseId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}