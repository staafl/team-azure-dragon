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
        [StringLength(80)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Skill")]
        public int SkillId { get; set; }

        public virtual Skill Skill { get; set; }

        public ICollection<Exercise> Exercises { get; set; }
        public ICollection<Lesson> Requirements { get; set; }

        public Lesson()
        {
            Exercises = new HashSet<Exercise>();
            Requirements = new HashSet<Lesson>();
        }
    }
}
