using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TeamAzureDragon.Utils;
using TeamAzureDragon.Utils.Attributes;

namespace LearningSystem.App.Areas.Administration.ViewModels
{
    public class SkillViewModel : IViewModel<Skill>
    {
        [ModelNavigationId]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Skill name is required!")]
        [StringLength(600)]
        public string Name { get; set; }
    }
}