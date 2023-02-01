using dotNetAPI.DTO;

namespace dotNetAPI.Service
{
    public interface ICompanyService
    {
        void AddCompany(CompanyDTO companyDTO);
        List<CompanyDTO> GetCompanies();
    }
}
