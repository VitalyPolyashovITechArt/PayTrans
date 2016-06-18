using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace PayTransApi.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        public int Limit { get; set; }

        public virtual ApplicationUser ParentUser { get; set; }

        public virtual ICollection<ApplicationUser> LinkedAccounts { get; set; }

        public virtual LongTermTicket LongTermTicket { get; set; }

        public virtual ActiveTicket ActiveTicket { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<LongTermTicket> LongTermTickets { get; set; }

        public DbSet<ActiveTicket> ActiveTickets { get; set; }
    }
}