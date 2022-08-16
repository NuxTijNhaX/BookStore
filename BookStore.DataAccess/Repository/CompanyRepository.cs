using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            var oldCompany = _db.Companies.Find(company.Id);
            
            if(oldCompany != null)
            {
                oldCompany.Name = company.Name;
                oldCompany.PhoneNumber = company.PhoneNumber;
                oldCompany.StreetAddress = company.StreetAddress;
                oldCompany.City = company.City;
                oldCompany.State = company.State;
                oldCompany.PostalCode = company.PostalCode;
                oldCompany.IsAuthorizedCompany = company.IsAuthorizedCompany;
            }
        }
    }
}
