using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AcademyERP.Persistence.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
    "Server=localhost,1433;Database=AcademyERPDb;User Id=sa;Password=AcademyERP@123;TrustServerCertificate=True;Encrypt=False;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}