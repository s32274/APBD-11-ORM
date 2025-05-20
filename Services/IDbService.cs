using Microsoft.AspNetCore.Mvc;
using ORM.DTOs;

namespace ORM.Services;

public interface IDbService
{
    public Task<PatientWithPrescriptonsDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken);

    public Task<int> AddNewPrescriptionByIdsAsync(
        PatientsNewPrescriptionDto newPrescriptionDto,
        CancellationToken cancellationToken
    );
}