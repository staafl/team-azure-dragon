using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LearningSystem.Models
{
    public class Exercise
    {        
        [Key]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Exercise name is required!")]
        [StringLength(600)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Order is required!")]
        public int Order { get; set; }

        [Display(Name = "Lesson")]
        public int LessonId { get; set; }

        //[UIHint("LessonDropdown")]
        public virtual Lesson Lesson { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Exercise()
        {
            this.Questions = new HashSet<Question>();
            this.Users = new HashSet<ApplicationUser>();
        }
    }
}
