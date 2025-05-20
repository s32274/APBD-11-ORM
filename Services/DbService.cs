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
    public async Task<PatientWithPrescriptonsDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken)
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
                            DoctorFirstName = pr.Doctor.FirstName
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
        .FirstOrDefaultAsync(cancellationToken);
    }

    // Wystawienie nowej recepty. Zwraca nowo utworzone ID recepty.
    public async Task<int> AddNewPrescriptionByIdsAsync(
        PatientsNewPrescriptionDto newPrescriptionDto,
        CancellationToken cancellationToken)
    {
        var idPatient = newPrescriptionDto.Patient.IdPatient;
        var patientFirstName = newPrescriptionDto.Patient.FirstName;
        var patientLastName = newPrescriptionDto.Patient.LastName;
        var patientBirthDate = newPrescriptionDto.Patient.BirthDate;

        var idDoctor = newPrescriptionDto.Doctor.IdDoctor;
        var doctorFirstName = newPrescriptionDto.Doctor.DoctorFirstName;
        var doctorLastName = newPrescriptionDto.Doctor.DoctorLastName;
        var medicamentDtos = newPrescriptionDto.Medicaments;
        var date = newPrescriptionDto.Date;
        var dueDate = newPrescriptionDto.DueDate;
        
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

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        // Sprawdza, czy istnieje podany lekarz
        var doctor = await _context.Doctors
            .FromSqlRaw("SELECT * FROM Doctor WITH (UPDLOCK) WHERE IdDoctor = {0}", idDoctor)
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor == null)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new ArgumentException("Doctor " + doctorFirstName + " " + doctorLastName + " doesn't exist in the database.");
        }
        
        // Sprawdza, czy istnieją podane leki
        foreach (var med in medicamentDtos)
        {
            var medicamentExists = await _context.Medicaments
                .FromSqlRaw("SELECT * FROM Medicament WITH (UPDLOCK) WHERE IdMedicament = {0}", med.IdMedicament)
                // "{0}" zapobiega SQL injection
                .AnyAsync(cancellationToken);

            if (!medicamentExists)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ArgumentException("Medicament with ID " + med.IdMedicament + " does not exist.");
            }
        }
        
        // Sprawdza, czy istnieje podany pacjent
        var patient = await _context.Patients
            .FromSqlRaw("SELECT * FROM Patient WITH (UPDLOCK) WHERE IdPatient = {0}", idPatient)
            .FirstOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            // Jeśli podany pacjent nie istniał w bazie, dodaje go do bazy
            patient = new Patient
            {
                IdPatient = idPatient,
                FirstName = patientFirstName,
                LastName = patientLastName,
                BirthDate = patientBirthDate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        // Dodaje nową receptę do bazy danych
        var newPrescription = new Prescription
        {
            Date = date,
            DueDate = dueDate,
            IdPatient = idPatient,
            IdDoctor = idDoctor
        };
        _context.Prescriptions.Add(newPrescription);
        await _context.SaveChangesAsync(cancellationToken);

        var prescriptionMedicaments = medicamentDtos.Select(m => new Prescription_Medicament()
        {
            IdPrescription = newPrescription.IdPrescription,
            IdMedicament = m.IdMedicament,
            Dose = m.Dose,
            Details = m.Description
        });

        await _context.PrescriptionMedicaments.AddRangeAsync(prescriptionMedicaments);
        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return newPrescription.IdPrescription;
    }

}