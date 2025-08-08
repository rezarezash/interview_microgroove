using GetInitialFunctions.Entitites;
using Microsoft.EntityFrameworkCore;

namespace Interv.API.DAL
{
    public class InitialsContext(DbContextOptions<InitialsContext> options) : DbContext(options)
    {
        public DbSet<Initials> Initials { get; set; }      
    }
}
