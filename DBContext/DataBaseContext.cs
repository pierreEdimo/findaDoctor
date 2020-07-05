using Microsoft.EntityFrameworkCore;
using findaDoctor.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace findaDoctor.DBContext
{


    public class DatabaseContext : IdentityDbContext

    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DatabaseContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Theme>().HasMany(c => c.Articles).WithOne(a => a.Theme).HasForeignKey(a => a.themeId);

            modelBuilder.Entity<FavoriteArticle>().HasKey(f => new { f.articleId, f.userId });

            modelBuilder.Entity<FavoriteDoctor>().HasKey(f => new { f.doctorId, f.userId });

            modelBuilder.Entity<FavoriteArticle>().HasOne(c => c.Article).WithMany(a => a.FavoriteArticleRef).HasForeignKey(a => a.articleId);

            modelBuilder.Entity<FavoriteDoctor>().HasOne(c => c.Doctor).WithMany(a => a.FavoriteDoctorRef).HasForeignKey(a => a.doctorId);

            modelBuilder.Entity<Doctor>().HasMany(c => c.FavoriteDoctorRef);

            modelBuilder.Entity<Article>().HasMany(c => c.FavoriteArticleRef);

            modelBuilder.Entity<Article>().HasOne(f => f.Theme).WithMany(a => a.Articles);

            modelBuilder.Seed();
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<FavoriteDoctor> FavoriteDoctors { get; set; }
        public DbSet<FavoriteArticle> FavoriteeArticles { get; set; }

    }
}