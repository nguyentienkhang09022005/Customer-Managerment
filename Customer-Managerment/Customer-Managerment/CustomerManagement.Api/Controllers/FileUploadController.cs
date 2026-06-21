using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly LeadHandler _leadHandler;
        private readonly CustomerHandler _customerHandler;

        public FileUploadController(LeadHandler leadHandler, CustomerHandler customerHandler)
        {
            _leadHandler = leadHandler;
            _customerHandler = customerHandler;
        }

        [HttpPost("lead")]
        public async Task<IActionResult> ImportLeadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File không được để trống!" });

            try
            {
                var result = await _leadHandler.ImportLeadExcelAsync(file);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("customer")]
        public async Task<IActionResult> ImportCustomerExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File không được để trống!" });

            try
            {
                var result = await _customerHandler.ImportCustomerExcelAsync(file);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}