using LearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;

namespace LearningSystem.Data
{
    public class UoWLearningSystem : UnitOfWork, IUoWLearningSystem
    {
        public UoWLearningSystem()
            : base(new LearningSystemContext())
        {

        }


        public IRepository<Skill> Skills
        {
            get { return _<Skill>(); }
        }

        public IRepository<Lesson> Lessons
        {
            get { return _<Lesson>(); }
        }

        public IRepository<Exercise> Exercises
        {
            get { return _<Exercise>(); }
        }

        public IRepository<Question> Questions
        {
            get { return _<Question>(); }
        }

        public IRepository<ApplicationUser> Users
        {
            get { return _<ApplicationUser>(); }
        }
    }
}
