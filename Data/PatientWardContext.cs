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
        modelBuilder.Entity<Doctor>(a =>
        {
            a.ToTable("Author");
            
            a.HasKey(e => e.IdDoctor);
            a.Property(e => e.FirstName).HasMaxLength(100);
            a.Property(e => e.LastName).HasMaxLength(100);
            a.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Author>().HasData(new List<Author>()
        {
            new Author() { AuthorId = 1, FirstName = "Jane", LastName = "Doe"},
            new Author() { AuthorId = 2, FirstName = "John", LastName = "Doe"},
        });
        
        modelBuilder.Entity<Book>().HasData(new List<Book>()
        {
            new Book() { BookId = 1, Name = "Book 1", Price = 10.2 },
            new Book() { BookId = 2, Name = "Book 2", Price = 123.2 },
        });
        
        modelBuilder.Entity<BookAuthor>().HasData(new List<BookAuthor>()
        {
            new BookAuthor() { AuthorId = 1, BookId = 1, Notes = "n1" },
            new BookAuthor() { AuthorId = 2, BookId = 1, Notes = "n2" },
        });
    }
}