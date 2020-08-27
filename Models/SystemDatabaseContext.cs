using Microsoft.EntityFrameworkCore;

namespace TCU.English.Models
{
    // https://xuanthulab.net/ef-core-tao-migration-trong-entityframework-voi-c-csharp.html
    // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-3.1
    // dotnet tool install --global dotnet-ef
    // dotnet ef migrations add InitialCreate.Version.1
    // dotnet ef database update InitialCreate.Version.1 
    // dotnet ef database drop -f (Lệnh xóa database - cực kỳ nguy hiển)
    // dotnet ef migrations remove
    public class SystemDatabaseContext : DbContext
    {
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserTypeUser> UserTypeUser { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }
        public virtual DbSet<TestCategory> TestCategories { get; set; }
        public virtual DbSet<ReadingPartOne> ReadingPartOnes { get; set; }
        public virtual DbSet<ReadingPartTwo> ReadingPartTwos { get; set; }

        public SystemDatabaseContext(DbContextOptions<SystemDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Cấu hình đặc trưng cho từng bản
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("user_types");
            });

            modelBuilder.Entity<UserTypeUser>(entity =>
            {
                entity.ToTable("user_type_users");
            });

            modelBuilder.Entity<TestCategory>(entity =>
            {
                entity.ToTable("test_categories");
            });

            modelBuilder.Entity<ReadingPartOne>(entity =>
            {
                entity.ToTable("reading_part_1");
            });
            
            modelBuilder.Entity<ReadingPartTwo>(entity =>
            {
                entity.ToTable("reading_part_2");
            });
            #endregion
        }
    }
}
