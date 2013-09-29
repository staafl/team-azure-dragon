using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningSystem.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Skill name is required!")]
        [StringLength(600)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Skill()
        {
            this.Lessons = new HashSet<Lesson>();
            this.Users = new HashSet<ApplicationUser>();
        }

    }
}
