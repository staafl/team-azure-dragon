using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;

namespace LearningSystem.Data
{
    public interface IUoWLearningSystem: IUnitOfWork
    {
        IRepository<Skill> Skills { get; }
        IRepository<Lesson> Lessons { get; }
        IRepository<Exercise> Exercises { get; }
        IRepository<Question> Questions { get; }
        IRepository<ApplicationUser> Users { get; }
    }
}
