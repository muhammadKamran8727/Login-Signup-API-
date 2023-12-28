using Microsoft.EntityFrameworkCore;
using static ForgetPasswordAPI.Models.User;

namespace ForgetPasswordAPI.Models
{
   public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add additional model configurations if needed
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id); // Assuming Serial_No is the primary key
                                          // Assuming User_ID is the foreign key
               

                entity.ToTable("User");

                //entity.Property(e => e.Student_Id).HasColumnName("Student_Id");

                entity.Property(e => e.Email).HasColumnName("Email");

                entity.Property(e => e.Password).HasColumnName("Password");

            });


        }
    }
}
