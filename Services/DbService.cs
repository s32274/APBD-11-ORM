using Microsoft.EntityFrameworkCore;
using Tutorial5.Data;
using Tutorial5.DTOs;

namespace Tutorial5.Services;

public class DbService : IDbService
{
    private readonly PatientWardContext _context;
    public DbService(PatientWardContext context)
    {
        _context = context;
    }
    
    public async Task<List<PatientWithPrescriptonsDto>> GetPatientByIdAsync()
    {
        var books = await _context.Patients.Select(e =>
        new PatientWithPrescriptonsDto {
            IdPatient = e.IdPatient,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Birthdate = e.BirthDate,
            PrescriptionWithMedicamentsDto = e.PrescriptionMedicaments.Select(a =>
                new AuthorDto {
                
                }).ToList()
        }).ToListAsync();
        return books;
    }
}