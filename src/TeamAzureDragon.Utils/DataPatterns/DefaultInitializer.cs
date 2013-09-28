namespace TeamAzureDragon.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public class DefaultInitializerBase<T> : IDatabaseInitializer<T> where T : DbContext
    {
        public void InitializeDatabaseWithSetInitializer(T context)
        {
            Database.SetInitializer<T>(this);
            context.Database.Initialize(true);
        }

        public DefaultInitializerBase()
        {
        }

        protected virtual void Seed(T context)
        {
        }

        public void InitializeDatabase(T context)
        {
            if (!context.Database.Exists())
            {
                context.Database.Create();
                Seed(context);
            }
        }
    }
}
