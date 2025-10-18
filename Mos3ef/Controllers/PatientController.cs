using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Manager; 


namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _patientManager;
        private readonly IServiceManager _serviceManager;

        public PatientsController(IPatientManager patientManager, IServiceManager serviceManager)
        {
            _patientManager = patientManager;
            _serviceManager = serviceManager;
        }

        [HttpGet("{id}")] 
        public async Task<IActionResult> GetPatientById(int id)
        {

            var patient = await _patientManager.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound(); 
            }

            return Ok(patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientUpdateDto patientUpdateDto)
        {
            var success = await _patientManager.UpdatePatientAsync(id, patientUpdateDto);

            if (!success)
            {
                return NotFound(); 
            }

            return NoContent(); 
        }

        [HttpGet("search")] 
        public async Task<IActionResult> SearchServices([FromQuery] string keyword)
        {
            var services = await _serviceManager.SearchServicesAsync(keyword);

            return Ok(services);
        }
    }
}