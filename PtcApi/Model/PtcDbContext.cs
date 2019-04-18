using Microsoft.EntityFrameworkCore;

namespace PtcApi.Model
{
    public class PtcDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppUserClaim> Claims { get; set; }

        private const string Conn =
                      @"server=.\application;
                            database=ptc-pluralsight;
                            trusted_connection=true;
                            multipleactiveresultsets=true";

        protected override void OnConfiguring(
                    DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Conn);
        }
    }
}


/* Archive 
     // // private const string conn =
    // //               @"server=localhost;
    // //                 database=ptc-pluralsight;
    // //                 trusted_connection=true;
    // //                 multipleactiveresultsets=true";

    // private const string CONN = @"Server=(localdb)\MSSQLLocalDB;
    // Database=PTC-Pluralsight;
    // AttachDbFilename=D:\Samples\SqlData\PTC-Pluralsight.mdf;
    // MultipleActiveResultSets=true";
*/
