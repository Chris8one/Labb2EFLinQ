using Labb2EFLinQ.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Labb2EFLinQ.Data
{
    public class SchoolSystemDbContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<SchoolSchedule> SchoolSchedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data source = LAPTOP-MUE17E2K\\SQLEXPRESS; Initial catalog = LabbTwoEFLinQDb; Integrated security = True"); 
        }
    }
}
