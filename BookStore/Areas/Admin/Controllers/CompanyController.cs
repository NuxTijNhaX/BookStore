using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            if(id == null)
            {
                return View(company);
            }

            company = _unitOfWork.CompanyRepository.Get(id.GetValueOrDefault());
            if(company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        #region API
        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.CompanyRepository.GetAll();

            return Json(new { data = companies });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id != 0)
                {
                    _unitOfWork.CompanyRepository.Update(company);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Add(company);
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var company = _unitOfWork.CompanyRepository.Get(id);
            if(company == null)
            {
                return Json(new { success = false, message = "Connot delete" });
            }

            _unitOfWork.CompanyRepository.Delete(company);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted" });
        }

        #endregion
    }
}
