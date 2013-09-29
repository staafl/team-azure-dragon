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
        [StringLength(60)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public ICollection<Lesson> Lessons { get; set; }

        public Skill()
        {
            Lessons = new HashSet<Lesson>();
        }

    }
}
