using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningSystem.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Lesson name is required!")]
        [StringLength(200)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Skill")]
        public int SkillId { get; set; }

        public virtual Skill Skill { get; set; }

        public virtual ICollection<Exercise> Exercises { get; set; }
        public virtual ICollection<Lesson> Requirements { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Lesson()
        {
            this.Exercises = new HashSet<Exercise>();
            this.Requirements = new HashSet<Lesson>();
            this.Users = new HashSet<ApplicationUser>();
        }
    }
}
