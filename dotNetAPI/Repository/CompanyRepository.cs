using dotNetAPI.Entity;

namespace dotNetAPI.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CarRentalDbContext _db;

        public CompanyRepository(CarRentalDbContext db)
        {
            this._db = db;
        }

        public void AddCompany(Company company)
        {
            _db.Company.Add(company);
            _db.SaveChanges();
        }

        public List<Company> GetCompanies()
        {
            return _db.Company.ToList();
        }

        public Company GetCompanyById(int id)
        {
            return _db.Company.Find(id);
        }
    }
}
