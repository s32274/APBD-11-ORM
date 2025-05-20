using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ORM.Data;
using ORM.DTOs;

namespace ORM.Services;

public class DbService : IDbService
{
    private readonly PatientWardContext _context;
    
    public DbService(PatientWardContext context)
    {
        _context = context;
    }
    
    public async Task<PatientWithPrescriptonsDto?> GetPatientByIdAsync(int id)
    {
        return await _context.Patients
            .Where(p => p.IdPatient == id)
            .Select(p => new PatientWithPrescriptonsDto
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Birthdate = p.BirthDate,
                PrescriptionWithMedicamentsDto = p.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionWithMedicamentsDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date = pr.Date,
                        DueDate = pr.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName
                        },
                        Medicaments = pr.PrescriptionMedicaments
                            .Select(pm => new MedicamentDto
                            {
                                IdMedicament = pm.Medicament.IdMedicament,
                                Name = pm.Medicament.Name,
                                Dose = pm.Dose,
                                Description = pm.Medicament.Description
                            }).ToList()
                    }).ToList()
            })
        .FirstOrDefaultAsync();
    }
}