using Microsoft.EntityFrameworkCore;

namespace Lab3
{
    public class ModelDB:DbContext
    {
        public ModelDB(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Tariff>? Tariffs { get; set; }
        public DbSet<Patient>? Patients { get; set; }
        public DbSet<User>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    Age = 19, FullName = "Елизавета Рыбова Олег", Gender = "жен.",
                    Days = 3, DiagnosisCode = "К41", DiagnosisName = "Одностороння или неуточнённая бедренная грыжа с гангреной", 
                    PlaceOfResidence = "Находка", Cost = 5000d, TariffId = 1
                });
            modelBuilder.Entity<Tariff>().HasData(
                new Tariff
                {
                    Id = 1,
                    Name = "Рыбнинск",
                    DiagnosisCode = "К41",
                    DiagnosisName = "Одностороння или неуточнённая бедренная грыжа с гангреной",
                    Cost = 5000d
                });

            modelBuilder.Entity<User>().HasData(
                new User { id = 1, EMail = "somes@mail.ru", Password = "123456" }
                );
        }
    }
}
