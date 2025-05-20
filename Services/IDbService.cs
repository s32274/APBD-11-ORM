using Microsoft.AspNetCore.Mvc;
using ORM.DTOs;

namespace ORM.Services;

public interface IDbService
{
    public Task<PatientWithPrescriptonsDto?> GetPatientByIdAsync(int id);

    public Task<int> AddNewPrescriptionByIdsAsync(
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
    );
}