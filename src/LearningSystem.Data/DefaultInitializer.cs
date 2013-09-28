namespace LearningSystem.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TeamAzureDragon.Utils;

    public class DefaultInitializer : DefaultInitializerBase<LearningSystemContext>
    {
        public DefaultInitializer()
        {
        }

        protected override void Seed(LearningSystemContext context)
        {
            // todo

            // context.SaveChanges();
        }

    }
}
