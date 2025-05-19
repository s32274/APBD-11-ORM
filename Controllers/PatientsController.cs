using Microsoft.AspNetCore.Mvc;
using Tutorial5.Services;

namespace Tutorial5.Controllers
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
        
        [HttpGet]
        public async Task<IActionResult> GetPatientByIdAsync()
        {
            var result = await _dbService.GetPatientByIdAsync();
            return Ok(result);
        }
    }
}
