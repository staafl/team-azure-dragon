using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;

namespace LearningSystem.Data
{
    public class UoWLearningSystem : UnitOfWork
    {
        public UoWLearningSystem()
            : base(new LearningSystemContext())
        {

        }
    }
}
