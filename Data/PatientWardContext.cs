using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using Tutorial5.Models;

namespace Tutorial5.Data;

public class PatientWardContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription_Medicament> PrescriptionMedicaments { get; set; }
    
    protected PatientWardContext()
    {
    }

    public PatientWardContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Doctor
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");
            
            entity.HasKey(e => e.IdDoctor);
            
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);

            entity.HasMany(e => e.Prescriptions)
                .WithOne(e => e.Doctor)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>()
        {
            new Doctor() { IdDoctor = 1, FirstName = "John", LastName = "Doe", Email = "johndoe@example.com"},
            new Doctor() { IdDoctor = 2, FirstName = "Jan", LastName = "Kowalski", Email = "jankowalski@example.com"}
        });
        
        // Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");
            
            entity.HasKey(e => e.IdPatient);
            
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);

            // One Patient has many Prescriptions, one Prescription has one Patient
            entity.HasMany(e => e.Prescriptions)
                .WithOne(p => p.Patient)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Patient>().HasData(new List<Patient>()
        {
            new Patient() {IdPatient = 1, FirstName = "Jan", LastName = "Nowak", BirthDate = new DateTime(2000, 1 ,30)},
            new Patient() {IdPatient = 2, FirstName = "Janina", LastName = "Kowalska", BirthDate = new DateTime(1850, 9, 25)}
        });
        
        // Prescription
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescription");
            
            entity.HasKey(e => e.IdPrescription);

            // One Prescription has many Prescription_Medicaments
            entity.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(p => p.Prescription)
                .OnDelete(DeleteBehavior.Restrict);

            // One Prescription has one Patient, one Patient has many Prescriptions
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(e => e.IdPatient)
                .OnDelete(DeleteBehavior.Restrict);

            // One Prescription has one Doctor, one Doctor has many Prescriptions
            entity.HasOne(e => e.Doctor)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(e => e.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Prescription>().HasData(new List<Prescription>
        {
            new Prescription()
            {
                IdPrescription = 1, 
                Date = new DateTime(2025, 3, 1), 
                DueDate = new DateTime(2015, 1, 29),
                IdPatient = 1,
                IdDoctor = 1
            },
            new Prescription()
            {
                IdPrescription = 2,
                Date = new DateTime(2025, 5 ,5),
                DueDate = new DateTime(2027,5,4),
                IdPatient = 1,
                IdDoctor = 2
            }
        });
        
        // Medicament
        modelBuilder.Entity<Medicament>(entity =>
        {
            entity.ToTable("Medicament");
            
            entity.HasKey(e => e.IdMedicament);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Medicament)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>
        {
            new Medicament() {IdMedicament = 1, Name = "AAA", Description = "description 1", Type = "type 1"},
            new Medicament() {IdMedicament = 2, Name = "BBB", Description = "description 2", Type = "type 2"}
        });
        
        // Prescription_Medicament
        modelBuilder.Entity<Prescription_Medicament>(entity =>
        {
            entity.ToTable("Prescription_Medicament");
            
            entity.HasKey(e => e.IdMedicament);
            entity.HasKey(e => e.IdPrescription);
            entity.Property(e => e.Details).HasMaxLength(100);
            
            // One Prescription_Medicament has many Medicaments
            entity.HasOne(e => e.Medicament)
                .WithMany(e => e.PrescriptionMedicaments)
                .HasForeignKey(e => e.IdMedicament)
                .OnDelete(DeleteBehavior.Restrict);
            
            // One Prescription_Medicament has many Prescriptions
            entity.HasOne(e => e.Prescription)
                .WithMany(e => e.PrescriptionMedicaments)
                .HasForeignKey(e => e.IdPrescription)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Prescription_Medicament>().HasData(new List<Prescription_Medicament>()
        {
            new Prescription_Medicament()
            {
                IdMedicament = 1,
                IdPrescription = 1,
                Details = "asdaw",
                Dose = 10,
            },
            new Prescription_Medicament()
            {
                IdMedicament = 2,
                IdPrescription = 2,
                Details = "daadadadadadadad",
                Dose = 5,
            }
        });
    }
}