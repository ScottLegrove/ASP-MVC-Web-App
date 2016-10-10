using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SPM.Models
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

        public System.Data.Entity.DbSet<SPM.Models.AddProjectViewModel> AddProjectViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.AddTestViewModel> AddTestViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.AddClassViewModel> AddClassViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.ViewClassesModel> ViewClassesModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.EditTestViewModel> EditTestViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.EditProjectViewModel> EditProjectViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.RegisterAdminViewModel> RegisterAdminViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.AddSchoolViewModel> AddSchoolViewModels { get; set; }

        public System.Data.Entity.DbSet<SPM.Models.AddProgramViewModel> AddProgramViewModels { get; set; }
    }
}