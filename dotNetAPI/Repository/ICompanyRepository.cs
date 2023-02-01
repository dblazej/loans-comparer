using dotNetAPI.Entity;

namespace dotNetAPI.Repository
{
    public interface ICompanyRepository
    {
        void AddCompany(Company company);
        List<Company> GetCompanies();
        Company GetCompanyById(int id);
    }
}
