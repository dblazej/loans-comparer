using dotNetAPI.DTO;
using dotNetAPI.Entity;
using dotNetAPI.Repository;

namespace dotNetAPI.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public void AddCompany(CompanyDTO companyDTO)
        {
            Company company = new Company();
            company.Name = companyDTO.Name;
            _companyRepository.AddCompany(company);
        }

        public List<CompanyDTO> GetCompanies()
        {
            List<Company> companies = _companyRepository.GetCompanies();

            //configure autoMapper instead
            List<CompanyDTO> companyDTOs = new List<CompanyDTO>();
            foreach (Company company in companies)
            {
                CompanyDTO companyDTO = new CompanyDTO();
                companyDTO.Id = company.Id;
                companyDTO.Name = company.Name;
                companyDTOs.Add(companyDTO);
            }

            return companyDTOs;

        }
    }
}
