using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace GIZ.API.Models
{

    public class GIZContext : DbContext
    {
        public GIZContext(string connectionStringOrName = null): base(connectionStringOrName ?? "name=GIZContext")
        {
            QueryFilterManager.InitilizeGlobalFilter(this);
        }


        protected virtual void OnPreSave()
        {
            foreach(var timestamp in ChangeTracker.Entries<ITimestamp>())
            {
                var entity = timestamp.Entity as ITimestamp;

                if(timestamp.State == EntityState.Added)
                    entity.DateCreated = DateTime.UtcNow;

                if (timestamp.State == EntityState.Added || timestamp.State == EntityState.Modified)
                    entity.LastUpdated = DateTime.UtcNow;
            }

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(typeof(GIZContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            //  On pre save
            OnPreSave();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            //  On pre save
            OnPreSave();

            return base.SaveChangesAsync();
        }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<CompanyInfo> Companies { get; set; }

        public DbSet<CompanyBranch> CompanyBranches { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductView> ProductViews { get; set; }

        public DbSet<ProductLifeCycle> LifeCycles { get; set; }

        public DbSet<LifeCycleTag> LifeCycleTags { get; set; }

        public DbSet<ProductProperty> ProductProperties { get; set; }

        public DbSet<ProductSolution> Solutions { get; set; }

        public DbSet<Media> Media { get; set; }
    }

}