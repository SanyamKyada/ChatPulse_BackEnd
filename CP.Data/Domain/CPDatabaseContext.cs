using CP.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CP.Data.Domain
{
    public class CPDatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public CPDatabaseContext(DbContextOptions<CPDatabaseContext> options) : base(options)
        {

        }
    }
}
