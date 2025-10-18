using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Manager;
using Mos3ef.DAL.Database;

namespace Mos3ef.Api.Controllers
{
    public class HospitalController :ControllerBase
    {
        private readonly IHospitalManager _hospitalManager;

        public HospitalController (IHospitalManager hospitalManager)
        {
            _hospitalManager = hospitalManager;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var Hospitals = _hospitalManager.GetAll();
            return Ok(Hospitals);
        }

        [HttpGet("Get{id}")]
        public IActionResult Get(int id )
        {
            var Hospital = _hospitalManager.Get( id);
            return Ok(Hospital);
        }

        [HttpPost("Add")]
        public IActionResult Add(HospitalAddDto hospitalAddDto)
        {
            _hospitalManager.Add( hospitalAddDto );
            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update(HospitalUpdateDto hospitalUpdateDto)
        {
            _hospitalManager.Update( hospitalUpdateDto );
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            _hospitalManager.Delete(id);
            return Ok();
        }

        [HttpPost("AddService")]
        public IActionResult AddService(ServicesAddDto servicesAddDto)
        {
            _hospitalManager.AddService(servicesAddDto);
            return Ok();
        }

        [HttpPut("UpdateService")]
        public IActionResult UpdateService(ServicesUpdateDto servicesUpdateDto)
        {
            _hospitalManager.UpdateService(servicesUpdateDto);
            return Ok();
        }

        [HttpDelete("DeleteService")]
        public IActionResult DeleteService(int id)
        {
            _hospitalManager.Delete(id);
            return Ok();
        }

        [HttpGet("GetServicesReviews")]
        public IActionResult GetServicesReviews(int id)
        {
            var Hospital = _hospitalManager.GetServicesReviews(id);
            return Ok(Hospital);
        }

        [HttpGet("GetDashboardStats")]
        public IActionResult GetDashboardStats(int id)
        {
            var Hospital = _hospitalManager.GetDashboardStats(id);
            return Ok(Hospital);
        }
    }
}
