using dotNetAPI.DTO;
using dotNetAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace dotNetAPI.Controllers
{
    [ApiController]
    [Route("company")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("get/all")]
        public List<CompanyDTO> GetCompanies()
        {
            return _companyService.GetCompanies();
        }

        [HttpPost("add")]
        public IActionResult AddCompany(CompanyDTO companyDTO)
        {
            _companyService.AddCompany(companyDTO);

            return StatusCode(200);
        }
    }
}