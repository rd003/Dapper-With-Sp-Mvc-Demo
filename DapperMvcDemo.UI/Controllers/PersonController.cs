using DapperMvcDemo.Data.Models.Domain;
using DapperMvcDemo.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DapperMvcDemo.UI.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonRepository _personRepo;

        public PersonController(IPersonRepository personRepo)
        {
            _personRepo = personRepo;
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Person person)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(person);
                bool addPersonResult = await _personRepo.AddAsync(person);
                if(addPersonResult)
                    TempData["msg"] = "Successfully added";
                else
                    TempData["msg"] = "Could not added";
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Could not added";
            }
            return RedirectToAction(nameof(Add));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var person = await _personRepo.GetByIdAsync(id);
            //if (person == null)
            //    return NotFound();
            return View(person);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Person person)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(person);
                var updateResult= await _personRepo.UpdateAsync(person);
                if (updateResult)
                    TempData["msg"] = "Updated succesfully";
                else
                    TempData["msg"] = "Could not updated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Could not updated";
            }
            return View(person);
        }





        public async Task<IActionResult> DisplayAll()
        {
            var people = await _personRepo.GetAllAsync();
            return View(people);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await _personRepo.DeleteAsync(id);
            return RedirectToAction(nameof(DisplayAll));
        } 


    }
}
