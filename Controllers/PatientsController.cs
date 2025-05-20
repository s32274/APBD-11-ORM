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
        public async Task<IActionResult> GetPatientByIdAsync(int id, CancellationToken cancellationToken)
        {
            var patientInfo = await _dbService.GetPatientByIdAsync(id, cancellationToken);
            if (patientInfo == null)
                return NotFound();
            
            return Ok(patientInfo);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPrescriptionByIdsAsync(
            PatientsNewPrescriptionDto newPrescriptionDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var newPrescriptionId = await _dbService.AddNewPrescriptionByIdsAsync(
                    newPrescriptionDto,
                    cancellationToken
                );

                return Ok(newPrescriptionId);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return NotFound();
            }
        }
    }
}
