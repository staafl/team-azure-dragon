using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearningSystem.App.ViewModels
{
    public static class ConvertToViewModel
    {
        public static LessonViewModel ToLessonViewModel(this Lesson lesson)
        {
            var converted =  new LessonViewModel
            {
                Id = lesson.LessonId,
                Description = lesson.Description,
                Name = lesson.Name
            };

            return converted;
        }
        public static IEnumerable<LessonViewModel> ToLessonViewModel(this IEnumerable<Lesson> lesson)
        {
            var converted = lesson.Select(x => x.ToLessonViewModel());

            return converted;
        }
    }
}