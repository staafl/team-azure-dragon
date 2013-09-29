using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningSystem.Models
{
    public class ApplicationUser : User
    {
        public string Email { get; set; }

        public string Facebook { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<Skill> Skills { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<ApplicationUser> Friends { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<Lesson> Lessons { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<Exercise> Exercises { get; set; }

        public ApplicationUser()
        {
            this.Skills = new HashSet<Skill>();
            this.Friends = new HashSet<ApplicationUser>();
            this.Lessons = new HashSet<Lesson>();
            this.Exercises = new HashSet<Exercise>();

        }
    }
}
