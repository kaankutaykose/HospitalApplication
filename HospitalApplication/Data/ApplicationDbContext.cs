using Microsoft.EntityFrameworkCore;

namespace HospitalApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<HospitalApplication.Models.Admin> Admin { get; set; }
        public DbSet<HospitalApplication.Models.Patient> Patient { get; set; }
        public DbSet<HospitalApplication.Models.Report> Report { get; set; }
        public DbSet<HospitalApplication.Models.Doctor> Doctor { get; set; }
        public DbSet<HospitalApplication.Models.Appointment> Appointment { get; set; }

    }
}
