using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using LearningSystem.Models;

namespace LearningSystem.Data
{
    public class LearningSystemContext : IdentityDbContextWithCustomUser<ApplicationUser>
    {

    }
}
