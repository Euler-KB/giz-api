namespace GIZ.API.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GIZ.API.Models.GIZContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.GIZContext context)
        {
            try
            {
                //  Seed users & products
                DbInitializer.RunSeed(context);
            }
            catch(Exception ex)
            {
                File.WriteAllText("c:\\error.txt", ex.ToString());
            }
        }
    }
}
