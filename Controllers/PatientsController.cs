using Microsoft.AspNetCore.Mvc;
using ORM.Services;
using ORM.DTOs;

namespace ORM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public PatientsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientByIdAsync(int id)
        {
            var result = await _dbService.GetPatientByIdAsync(id);
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPrescriptionByIdsAsync(
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
            var result = await _dbService.AddNewPrescriptionByIdsAsync(
                idPatient,
                patientFirstName,
                patientLastName,
                patientBirthDate,
                idDoctor,
                doctorFirstName,
                doctorLastName,
                doctorEmail,
                medicamentDtos,
                date,
                dueDate
            );

            return Ok(idPatient);
        }

    }
}
