using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using Orca_Gamma.Models.DatabaseModels;

namespace Orca_Gamma.Models
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

		// Added for database
		public Boolean IsDisabled {
			get; set;
		}

		public String FirstName {
			get; set;
		}

		public String LastName {
			get; set;
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

		// Link the DB models to the actual database
		public DbSet<Catagory> Catagories {
			get; set;
		}

		public DbSet<Collaborator> Collaborators {
			get; set;
		}

		public DbSet<Expert> Experts {
			get; set;
		}

		public DbSet<ForumThread> ForumThreads {
			get; set;
		}

		public DbSet<Keyword> Keywords {
			get; set;
		}

		public DbSet<KeywordRelation> KeywordRelations {
			get; set;
		}

		public DbSet<PrivateMessage> PrivateMessages {
			get; set;
		}

		public DbSet<PrivateMessageBetween> PrivateMessagesBetween {
			get; set;
		}

		public DbSet<PrivateMessagePost> PrivateMessagePosts {
			get; set;
		}

		public DbSet<Project> Projects {
			get; set;
		}

		public DbSet<ThreadMessagePost> ThreadMessagePosts {
			get; set;
		}

		/*
		 * Required, these three tables have cyclic references that are non null
		 * So if a single user is deleted, it may cause another user to be deleted
		 * and the database does not like a single delete to cascade onto the same
		 * table twice
		 */ 
		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Project>()
			.HasRequired(c => c.User)
			.WithMany()
			.WillCascadeOnDelete(false);

			modelBuilder.Entity<PrivateMessage>()
			.HasRequired(c => c.User)
			.WithMany()
			.WillCascadeOnDelete(false);

			modelBuilder.Entity<ThreadMessagePost>()
			.HasRequired(c => c.User)
			.WithMany()
			.WillCascadeOnDelete(false);
		}
	}
}