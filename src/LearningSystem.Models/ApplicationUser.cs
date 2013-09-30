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
        [RegularExpression(@"^(?i)(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$",
                ErrorMessage = "You must enter a valid e-mail address.")]
        public string Email { get; set; }

        [RegularExpression(@"(www\.|http?://(www\.))?facebook\.com/(profile\.php\?id=)?([^/#?]+)(\n|$)",
                ErrorMessage = "You must enter a valid Facebok profile link.")]
        public string Facebook { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<Skill> Skills { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<ApplicationUser> Friends { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<Lesson> Lessons { get; set; }

        [ScaffoldColumn(false)]
        public virtual ICollection<Exercise> Exercises { get; set; }

        public bool? IsConfirmed { get; set; }

        public ApplicationUser()
        {
            this.Skills = new HashSet<Skill>();
            this.Friends = new HashSet<ApplicationUser>();
            this.Lessons = new HashSet<Lesson>();
            this.Exercises = new HashSet<Exercise>();

        }
    }
}
