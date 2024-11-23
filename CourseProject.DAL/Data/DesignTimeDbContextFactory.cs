using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CourseProject.DAL.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CourseProjectContext>
    {
        public CourseProjectContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CourseProjectContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=MICRO_DB;Username=postgres;Password=tatarinn");

            return new CourseProjectContext(optionsBuilder.Options);
        }
    }
}
