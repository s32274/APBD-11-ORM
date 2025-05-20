using Microsoft.AspNetCore.Mvc;
using ORM.Services;

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
    }
}
