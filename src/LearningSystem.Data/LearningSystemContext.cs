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
        public LearningSystemContext() : base() {

        }

        public IDbSet<Skill> Skills { get; set; }
        public IDbSet<Lesson> Lessons { get; set; }
        public IDbSet<Exercise> Exercises { get; set; }
        public IDbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Friends).WithMany();
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Exercises).WithMany(x => x.Users);
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Lessons).WithMany(x => x.Users);
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Skills).WithMany(x => x.Users);
            modelBuilder.Entity<Lesson>().HasMany(x => x.Requirements).WithMany();
            base.OnModelCreating(modelBuilder);
        }
    }
}
