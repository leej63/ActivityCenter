using Microsoft.EntityFrameworkCore;

namespace CsharpBeltExam.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Activityy> Activities {get;set;}
        public DbSet<Participant> Participants {get;set;}
    }
}