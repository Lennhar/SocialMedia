using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SocialMedia.Core.Entities;
using SocialMedia.Infraestructure.Data.Configurations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SocialMedia.Infraestructure.Data
{
    public partial class SocialMediaContext : DbContext
    {
        public SocialMediaContext()
        {
        }

        public SocialMediaContext(DbContextOptions<SocialMediaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<User> Users { get; set; }

        //Se configura cada una de las entidades y sus campos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CommentConfiguration());

            modelBuilder.ApplyConfiguration(new PostConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
