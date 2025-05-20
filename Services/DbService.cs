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
    
    // Wyświetlenie wszystkich danych na temat konkretnego pacjenta, wraz z listę recept i leków, które pobrał.
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

    // Wystawienie nowej recepty. Zwraca nowo utworzone ID recepty.
    public async Task<int> AddNewPrescriptionByIdsAsync(
        int idPatient,
        string patientFirstName,
        string patientLastName,
        DateTime patientBirthDate,
        
        int idDoctor,
        string doctorFirstName,
        string doctorLastName,
        string doctorEmail,
        
        ICollection<MedicamentDto> medicamentDtos,
        
        DateTime date,
        DateTime dueDate
        )
    {
        // Sprawdza, czy due date nie jest wcześniej, niż date
        if (dueDate < date)
        {
            throw new ArgumentException("Prescription due date cannot be lower than the date of creating it.");
        }

        // Sprawdza, czy nie ma ponad 10 leków na nowej recepcie
        if (medicamentDtos.Count > 10)
        {
            throw new ArgumentException("A prescription cannot contain more than 10 medicaments");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        
        // Sprawdza, czy istnieje podany lekarz
        var doctorExistence = await _context.Doctors
            .Where(d => d.IdDoctor == idDoctor)
            .Select(d => d.IdDoctor)
            .ToListAsync();

        if (doctorExistence.Count == 0)
        {
            throw new ArgumentException("Doctor " + doctorFirstName + " " + doctorLastName +
                                        " doesn't exist in the database");
        }
        
        // Sprawdza, czy istnieją podane leki
        var medicamentIds = medicamentDtos.Select(m => m.IdMedicament).ToList();
        var databaseMedicamentIds = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        if (databaseMedicamentIds.Count != medicamentIds.Count)
        {
            throw new ArgumentException("Not all input medicaments exist in the database.");
        }
        
        // Sprawdza, czy istnieje podany pacjent
        var patientExistence = await _context.Patients
            .Where(p => p.IdPatient == idPatient)
            .Select(p => p.IdPatient)
            .ToListAsync();

        if (patientExistence.Count == 0)
        {
            // Jeśli podany pacjent nie istniał w bazie, dodaje go do bazy
            var newPatient = new Patient()
            {
                IdPatient = idPatient,
                FirstName = patientFirstName,
                LastName = patientLastName,
                BirthDate = patientBirthDate
            };
            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();
        }

        var newPrescription = new Prescription
        {
            Date = date,
            DueDate = dueDate,
            IdPatient = idPatient,
            IdDoctor = idDoctor
        };
        _context.Prescriptions.Add(newPrescription);
        await _context.SaveChangesAsync();

        var prescriptionMedicaments = medicamentDtos.Select(m => new Prescription_Medicament()
        {
            IdPrescription = newPrescription.IdPrescription,
            IdMedicament = m.IdMedicament,
            Dose = m.Dose,
            Details = m.Description
        });

        await _context.PrescriptionMedicaments.AddRangeAsync(prescriptionMedicaments);
        await _context.SaveChangesAsync();

        return newPrescription.IdPrescription;
    }

}