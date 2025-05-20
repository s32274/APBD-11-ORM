using ORM.DTOs;

namespace ORM.Services;

public interface IDbService
{
    Task<PatientWithPrescriptonsDto?> GetPatientByIdAsync(int id);
}