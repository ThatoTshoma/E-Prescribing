using E_Prescribing.Areas.Identity.Pages.Account;
using E_Prescribing.CollectionModel;
using E_Prescribing.Data;
using E_Prescribing.Models;
using E_Prescribing.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace E_Prescribing.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        //private readonly IEmailSender _emailSender;
        private readonly IMyEmailSender _myEmailSender;



        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IMyEmailSender myEmailSender)
        {
            _userManager = userManager;
            _db = db;
           // _emailSender = emailSender;
            _myEmailSender = myEmailSender;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddOrUpdateDosageForm(int? dosageId)
        {

            if (dosageId == null)
            {
                return View(new DosageForm());
            }
            var city = _db.DosageForms.FirstOrDefault(i => i.DosageId == dosageId);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateDosageForm(DosageForm model)
        {
            if (_db.DosageForms.Any(a => a.Name == model.Name && a.DosageId != model.DosageId))
            {
                ModelState.AddModelError("Name", "Dosage form name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.DosageId == 0)
                {
                    _db.DosageForms.Add(model);
                    TempData["success"] = "Dosage form added successfully";
                }
                else
                {
                    _db.DosageForms.Update(model);
                    TempData["success"] = "Dosage form updated successfully";
                }
                _db.SaveChanges();
                return RedirectToAction("ListDosageForm");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult GetDosageForm()
        {
            var dosageForm = _db.DosageForms.ToList();
            return Json(new { data = dosageForm });
        }
        [HttpDelete]
        public IActionResult DeleteDosageForm(int id)
        {
            var dosageForm = _db.DosageForms.FirstOrDefault(m => m.DosageId == id);
            if (dosageForm == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.DosageForms.Remove(dosageForm);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult ListDosageForm()
        {
            var dosageForm = _db.DosageForms.ToList();
            return View(dosageForm);
        }
        public IActionResult AddOrUpdateTreatment(int? treatmentId)
        {
            if (treatmentId == null)
            {
                return View(new Treatment());
            }
            var treatment = _db.Treatments.Find(treatmentId);
            if (treatment == null)
            {
                return NotFound();
            }
            return View(treatment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateTreatment(Treatment model)
        {

            if (_db.Treatments.Any(a => a.Code == model.Code && a.TreatmentId != model.TreatmentId))

            {
                ModelState.AddModelError("Name", "Treatment code already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.TreatmentId == 0)
                {
                    _db.Treatments.Add(model);
                    TempData["success"] = "Treatment added successfully";
                }
                else
                {
                    _db.Treatments.Update(model);
                    TempData["success"] = "Treatment updated successfully";
                }

                _db.SaveChanges();

                return RedirectToAction("ListTreatment");
            }
            return View(model);


        }

        [HttpGet]
        public IActionResult GetTreatment()
        {
            var treatment = _db.Treatments.ToList();
            return Json(new { data = treatment });
        }
        [HttpDelete]
        public IActionResult DeleteTreatment(int id)
        {
            var treatment = _db.Treatments.FirstOrDefault(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Treatments.Remove(treatment);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult ListTreatment()
        {
            var treatments = _db.Treatments.ToList();
            return View(treatments);
        }


        public IActionResult AddOrUpdateActiveIngredient(int? ingredientId)
        {
            if (ingredientId == null)
            {
                return View(new ActiveIngredient());
            }
            var activeIngredient = _db.ActiveIngredients.Find(ingredientId);
            if (activeIngredient == null)
            {
                return NotFound();
            }
            return View(activeIngredient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateActiveIngredient(ActiveIngredient model)
        {
            if (_db.ActiveIngredients.Any(a => a.Name == model.Name && a.IngredientId != model.IngredientId))
            {
                ModelState.AddModelError("Name", "Active Ingredient name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.IngredientId == 0)
                {
                    _db.ActiveIngredients.Add(model);
                    TempData["success"] = "Active Ingredient added successfully";
                }
                else
                {
                    _db.ActiveIngredients.Update(model);
                    TempData["success"] = "Active Ingredient updated successfully";
                }
                _db.SaveChanges();

                return RedirectToAction("ListActiveIngredient");
            }
            return View(model);
        }
        public IActionResult UpdateActiveIngredient(int? id)
        {
            if (id == null)
            {
                return View(new ActiveIngredient());
            }
            var activeIngredient = _db.ActiveIngredients.FirstOrDefault(i => i.IngredientId == id);
            if (activeIngredient == null)
            {
                return NotFound();
            }
            return View(activeIngredient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateActiveIngredient(int? id, ActiveIngredient model)
        {
            if (_db.ActiveIngredients.Any(a => a.Name == model.Name && a.IngredientId != model.IngredientId))
            {
                ModelState.AddModelError("Name", "Active Ingredient name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.IngredientId == 0)
                {
                    _db.ActiveIngredients.Add(model);
                    TempData["success"] = "Active Ingredient added successfully";
                }
                else
                {
                    _db.ActiveIngredients.Update(model);
                    TempData["success"] = "Active Ingredient updated successfully";
                }
                _db.SaveChanges();

                return RedirectToAction("ListActiveIngredient");
            }
            return View(model);
        }
        public IActionResult ListActiveIngredient()
        {
            var activeIngredient = _db.ActiveIngredients.ToList();
            return View(activeIngredient);
        }
        [HttpGet]
        public IActionResult GetActiveIngredient()
        {
            var activeIngredient = _db.ActiveIngredients.ToList();
            return Json(new { data = activeIngredient });
        }

        [HttpDelete]
        public IActionResult DeleteActiveIngredient(int id)
        {
            var ingredient = _db.ActiveIngredients.FirstOrDefault(m => m.IngredientId == id);
            if (ingredient == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.ActiveIngredients.Remove(ingredient);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult AddDosageForm()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDosageForm(DosageForm model)
        {
            if (_db.DosageForms.Any(d => d.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Dosage Form Already exist");
            }
            if (ModelState.IsValid)
            {
                _db.DosageForms.Add(model);
                _db.SaveChanges();
                return RedirectToAction("ListDosageForm");
            }
            return View(model);
        }
        public IActionResult AddOrUpdateMedicationInteraction(int? interactionId)
        {
            ViewBag.ListIngredient = new SelectList(_db.ActiveIngredients.OrderBy(c => c.Name), "IngredientId", "Name");

            if (interactionId == null)
            {
                return View(new MedicationInteraction());
            }

            var medicationInteraction = _db.MedicationInteractions.Find(interactionId);
            if (medicationInteraction == null)
            {
                return NotFound();
            }

            return View(medicationInteraction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateMedicationInteraction(MedicationInteraction model)
        {
            ViewBag.ListIngredient = new SelectList(_db.ActiveIngredients.OrderBy(c => c.Name), "IngredientId", "Name");

            model.Description = InteractionDescription(model.ActiveIngredient1Id, model.ActiveIngredient2Id);
            if (_db.MedicationInteractions.Any(a => a.Description == model.Description && a.InteractionId != model.InteractionId))
            {
                ModelState.AddModelError("Name", "Medication Interaction already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.InteractionId == 0)
                {
                    _db.MedicationInteractions.Add(model);
                    TempData["success"] = "Medication Interaction added successfully";
                }
                else
                {
                    _db.MedicationInteractions.Update(model);
                    TempData["success"] = "Medication Interaction updated successfully";
                }
                _db.SaveChanges();
                return RedirectToAction("ListMedicationInteraction");

            }
            return View(model);

        }

        private string InteractionDescription(int ingredientId1, int ingredientId2)
        {
            var ingredient1 = _db.ActiveIngredients.FirstOrDefault(a => a.IngredientId == ingredientId1);
            var ingredient2 = _db.ActiveIngredients.FirstOrDefault(a => a.IngredientId == ingredientId2);

            if (ingredient1 != null && ingredient2 != null)
            {
                return $"{ingredient1.Name} interacts with {ingredient2.Name} and should not be taken at the same time.";
            }

            return "Not valid";
        }

        public IActionResult ListMedicationInteraction()
        {
            var medicationInteraction = _db.MedicationInteractions.Include(m => m.ActiveIngredient1).Include(m => m.ActiveIngredient2).ToList();
            return View(medicationInteraction);
        }
        [HttpGet]
        public IActionResult GetMedicationInteraction()
        {
            var medicationInteraction = _db.MedicationInteractions.Include(m => m.ActiveIngredient1).Include(m => m.ActiveIngredient2).ToList();
            return Json(new { data = medicationInteraction });
        }
        [HttpDelete]
        public IActionResult DeleteMedicationInteraction(int id)
        {
            var interaction = _db.MedicationInteractions.FirstOrDefault(m => m.InteractionId == id);
            if (interaction == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.MedicationInteractions.Remove(interaction);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        [HttpPost]
        public IActionResult DeleteMedicationInteraction(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                ModelState.AddModelError("", "No Medication Interaction selected for deletion.");
                var medicationInteraction = _db.MedicationInteractions.ToList();
                return View("ListMedicationInteraction", medicationInteraction);
            }

            var medicationInteractionToDelete = _db.MedicationInteractions.Where(t => ids.Contains(t.InteractionId)).ToList();

            if (medicationInteractionToDelete.Any())
            {
                _db.MedicationInteractions.RemoveRange(medicationInteractionToDelete);
                _db.SaveChanges();
            }

            return RedirectToAction("ListMedicationInteraction");
        }
        public IActionResult AddOrUpdateContraIndication(int? contraIndicationId)
        {
            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a =>a.Name), "IngredientId", "Name");
            ViewBag.ConditionList = new SelectList(_db.ConditionDiagnoses.OrderBy(c =>c.Name), "ConditionId", "Name");

            if (contraIndicationId == null)
            {
                return View(new ContraIndication());
            }
            var contraIndication = _db.ContraIndications.Find(contraIndicationId);
            if (contraIndication == null)
            {
                return NotFound();
            }
            return View(contraIndication);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateContraIndication(ContraIndication model)
        {
            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");
            ViewBag.ConditionList = new SelectList(_db.ConditionDiagnoses.OrderBy(c => c.Name), "ConditionId", "Name");

            if (model.ContraIndicationId == 0)
            {
                _db.ContraIndications.Add(model);
                TempData["success"] = "ContraIndications added successfully";
            }
            else
            {
                _db.ContraIndications.Update(model);
                TempData["success"] = "ContraIndications updated successfully";
            }
            _db.SaveChanges();

            return RedirectToAction("ListContraIndication");



        }
        public IActionResult ListContraIndication()
        {
            var contraIndication = _db.ContraIndications.Include(c => c.ActiveIngredient).Include(c => c.ConditionDiagnosis).ToList();
            return View(contraIndication);
        }
        [HttpGet]
        public IActionResult GetContraIndication()
        {
            var contraIndication = _db.ContraIndications.Include(c => c.ActiveIngredient).Include(c => c.ConditionDiagnosis).ToList();
            return Json(new { data = contraIndication });
        }
        [HttpDelete]
        public IActionResult DeleteAContraIndication(int id)
        {
            var contraIndications = _db.ContraIndications.FirstOrDefault(m => m.ContraIndicationId == id);
            if (contraIndications == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.ContraIndications.Remove(contraIndications);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        [HttpPost]
        public IActionResult DeleteContraIndication(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                ModelState.AddModelError("", "No Contra-Indication selected for deletion.");
                var contraIndication = _db.ContraIndications.ToList();
                return View("ListContraIndication", contraIndication);
            }

            var contraIndicationToDelete = _db.ContraIndications.Where(t => ids.Contains(t.ContraIndicationId)).ToList();

            if (contraIndicationToDelete.Any())
            {
                _db.ContraIndications.RemoveRange(contraIndicationToDelete);
                _db.SaveChanges();
            }

            return RedirectToAction("ListContraIndication");
        }
        public IActionResult AddOrUpdateConditionDiagnosis(int? conditionId)
        {
            if (conditionId == null)
            {
                return View(new ConditionDiagnosis());
            }
            var conditionDiagnoses = _db.ConditionDiagnoses.Find(conditionId);
            if (conditionDiagnoses == null)
            {
                return NotFound();
            }
            return View(conditionDiagnoses);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateConditionDiagnosis(ConditionDiagnosis model)
        {

            if (_db.ConditionDiagnoses.Any(a => a.Name == model.Name && a.ConditionId != model.ConditionId))
            {
                ModelState.AddModelError("Name", "Chronic Condition name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.ConditionId == 0)
                {
                    _db.ConditionDiagnoses.Add(model);
                    TempData["success"] = "Chronic Condition added successfully";
                }
                else
                {
                    _db.ConditionDiagnoses.Update(model);
                    TempData["success"] = "Chronic Condition updated successfully";
                }
                _db.SaveChanges();

                return RedirectToAction("ListConditionDiagnosis");
            }
            return View(model);
        }
        public IActionResult ListConditionDiagnosis()
        {
            var conditionDiagnosis = _db.ConditionDiagnoses.ToList();
            return View(conditionDiagnosis);
        }
        [HttpGet]
        public IActionResult GetConditionDiagnosis()
        {
            var conditionDiagnose = _db.ConditionDiagnoses.ToList();
            return Json(new { data = conditionDiagnose });
        }
        [HttpDelete]
        public IActionResult DeleteConditionDiagnosis(int id)
        {
            var conditionDiagnoses = _db.ConditionDiagnoses.FirstOrDefault(m => m.ConditionId == id);
            if (conditionDiagnoses == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.ConditionDiagnoses.Remove(conditionDiagnoses);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        [HttpPost]
        public IActionResult DeleteConditionDiagnosis(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                ModelState.AddModelError("", "No Condition Diagnosis selected for deletion.");
                var conditionDiagnosis = _db.ConditionDiagnoses.ToList();
                return View("ListConditionDiagnosis", conditionDiagnosis);
            }

            var conditionDiagnosisToDelete = _db.ConditionDiagnoses.Where(t => ids.Contains(t.ConditionId)).ToList();

            if (conditionDiagnosisToDelete.Any())
            {
                _db.ConditionDiagnoses.RemoveRange(conditionDiagnosisToDelete);
                _db.SaveChanges();
            }

            return RedirectToAction("ListConditionDiagnosis");
        }
        public IActionResult AddOrUpdateTheatre(int? theatreId)
        {
            ViewBag.WardList = new SelectList(_db.Wards.OrderBy(w => w.WardNumber), "WardId", "WardNumber");
            if (theatreId == null)
            {
                return View(new Theatre());
            }
            var theatre = _db.Theatres.Find(theatreId);
            if (theatre == null)
            {
                return NotFound();
            }
            return View(theatre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateTheatre(Theatre model)
        {

            if (_db.Theatres.Any(a => a.Name == model.Name && a.TheatreId != model.TheatreId))

            {
                ModelState.AddModelError("Name", "Theatre name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.TheatreId == 0)
                {
                    _db.Theatres.Add(model);
                    ViewBag.WardList = new SelectList(_db.Wards.OrderBy(w => w.WardNumber), "WardId", "WardNumber");
                }
                else
                {
                    _db.Theatres.Update(model);
                    TempData["success"] = "Theatres updated successfully";
                }
                ViewBag.WardList = new SelectList(_db.Wards, "WardId", "WardNumber");

                _db.SaveChanges();

                return RedirectToAction("ListTheatre");
            }
            return View(model);


        }
        public IActionResult ListTheatre()
        {
            var theatre = _db.Theatres.Include(t =>t.Ward).ToList();
            return View(theatre);
        }
        [HttpGet]
        public IActionResult GetTheatre()
        {
            var theatre = _db.Theatres.Include(t => t.Ward).ToList();
            return Json(new { data = theatre });
        }
        [HttpDelete]
        public IActionResult DeleteTheatre(int id)
        {
            var theatre = _db.Theatres.FirstOrDefault(m => m.TheatreId == id);
            if (theatre == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Theatres.Remove(theatre);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult AddOrUpdateProvince(int? provinceId)
        {
            if (provinceId == null)
            {
                return View(new Province());
            }
            var city = _db.Provinces.FirstOrDefault(i => i.ProvinceId == provinceId);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateProvince(Province model)
        {
            if (_db.Provinces.Any(a => a.Name == model.Name && a.ProvinceId != model.ProvinceId))
            {
                ModelState.AddModelError("Name", "Province name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.ProvinceId == 0)
                {
                    _db.Provinces.Add(model);
                    TempData["success"] = "Province added successfully";
                }
                else
                {
                    _db.Provinces.Update(model);
                    TempData["success"] = "Province updated successfully";
                }
                _db.SaveChanges();

                return RedirectToAction("ListProvince");
            }
            return View(model);
        }
        public IActionResult ListProvince()
        {
            var province = _db.Provinces.ToList();
            return View(province);
        }
        [HttpGet]
        public IActionResult GetProvince()
        {
            var province = _db.Provinces.ToList();
            return Json(new { data = province });
        }

        [HttpDelete]
        public IActionResult DeleteProvince(int id)
        {
            var province = _db.Provinces.FirstOrDefault(m => m.ProvinceId == id);
            if (province == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Provinces.Remove(province);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }


        public IActionResult AddOrUpdateCity(int? cityId)
        {

            ViewBag.ProvinceList = new SelectList(_db.Provinces.OrderBy(p => p.Name), "ProvinceId", "Name");

            if (cityId == null)
            {
                return View(new City());
            }
            var city = _db.Cities.FirstOrDefault(i => i.CityId == cityId);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateCity(City model)
        {
            if (_db.Cities.Any(a => a.Name == model.Name && a.CityId != model.CityId))
            {
                ModelState.AddModelError("Name", "City name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.CityId == 0)
                {
                    _db.Cities.Add(model);
                    TempData["success"] = "City added successfully";
                }
                else
                {
                    _db.Cities.Update(model);
                    TempData["success"] = "City updated successfully";
                }
                _db.SaveChanges();
                ViewBag.ProvinceList = new SelectList(_db.Provinces.OrderBy(p => p.Name), "ProvinceId", "Name");
                return RedirectToAction("ListCity");
            }
            return View(model);
        }
        public IActionResult ListCity()
        {
            var city = _db.Cities.Include(p => p.Province).ToList();
            return View(city);
        }
        [HttpGet]
        public IActionResult GetCity()
        {
            var city = _db.Cities.Include(p => p.Province).ToList();
            return Json(new { data = city });
        }

        [HttpDelete]
        public IActionResult DeleteCity(int id)
        {
            var city = _db.Cities.FirstOrDefault(m => m.CityId == id);
            if (city == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Cities.Remove(city);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }

        public IActionResult AddOrUpdateSuburb(int? suburbId)
        {
            ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c =>c.Name), "CityId", "Name");

            if (suburbId == null)
            {
                return View(new Suburb());
            }
            var suburb = _db.Suburbs.FirstOrDefault(i => i.SuburbId == suburbId);
            if (suburb == null)
            {
                return NotFound();
            }
            return View(suburb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateSuburb(Suburb model)
        {
            if (_db.Suburbs.Any(a => a.Name == model.Name && a.SuburbId != model.SuburbId))
            {
                ModelState.AddModelError("Name", "Suburb name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.SuburbId == 0)
                {
                    _db.Suburbs.Add(model);
                    TempData["success"] = "Suburb added successfully";
                }
                else
                {
                    _db.Suburbs.Update(model);
                    TempData["success"] = "Suburb updated successfully";
                }
                _db.SaveChanges();
                ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c => c.Name), "CityId", "Name");

                return RedirectToAction("ListSuburb");
            }
            return View(model);
        }
        public IActionResult ListSuburb()
        {
            var suburb = _db.Suburbs.Include(s =>s.City).ToList();
            return View(suburb);
        }
        [HttpGet]
        public IActionResult GetSuburb()
        {
            var suburb = _db.Suburbs.Include(s => s.City).ToList();
            return Json(new { data = suburb });
        }

        [HttpDelete]
        public IActionResult DeleteSuburb(int id)
        {
            var suburbs = _db.Suburbs.FirstOrDefault(m => m.SuburbId == id);
            if (suburbs == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Suburbs.Remove(suburbs);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult AddOrUpdateWard(int? wardId)
        {
            if (wardId == null)
            {
                return View(new Ward());
            }
            var ward = _db.Wards.Find(wardId);
            if (ward == null)
            {
                return NotFound();
            }
            return View(ward);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateWard(Ward model)
        {
            if (_db.Wards.Any(a => a.WardNumber == model.WardNumber && a.WardId != model.WardId))
            {
                ModelState.AddModelError("Name", "Ward name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.WardId == 0)
                {
                    _db.Wards.Add(model);
                    TempData["success"] = "Ward added successfully";
                }
                else
                {
                    _db.Wards.Update(model);
                    TempData["success"] = "Ward updated successfully";
                }
                _db.SaveChanges();

                return RedirectToAction("ListWard");
            }
            return View(model);

        }

        public IActionResult ListWard()
        {
            var ward = _db.Wards.ToList();
            return View(ward);
        }
        [HttpGet]
        public IActionResult GetWad()
        {
            var ward = _db.Wards.ToList();
            return Json(new { data = ward });
        }
        [HttpDelete]
        public IActionResult DeleteWard(int id)
        {
            var ward = _db.Wards.FirstOrDefault(m => m.WardId == id);
            if (ward == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Wards.Remove(ward);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }

        public IActionResult AddMedication()
        {

            ViewBag.DosageFormList = new SelectList(_db.DosageForms, "DosageId", "Name");

            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients, "IngredientId", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedication(MedicationCollection model)
        {

            ViewBag.DosageFormList = new SelectList(_db.DosageForms, "DosageId", "Name");

            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients, "IngredientId", "Name");

            _db.Add(model.Medication);
            await _db.SaveChangesAsync();


            if (model.MedicationIngredient.SelectedIngredient != null)
            {
                foreach (var selectedIngredient in model.MedicationIngredient.SelectedIngredient)
                {
                    var medicationIngredient = new MedicationIngredient
                    {
                        ActiveIngredientId = selectedIngredient,
                        MedicationId = model.Medication.MedicationId,
                        ActiveIngredientStrength = model.Strengths[selectedIngredient],

                    };
                    _db.Add(medicationIngredient);
                }
                await _db.SaveChangesAsync();

            }
            model.Medications = _db.Medications.ToList();
            return View(model);
        }
   
        public IActionResult AddOrUpdateBed(int? bedId)
        {
            ViewBag.WardList = new SelectList(_db.Wards, "WardId", "WardNumber");
            if (bedId == null)
            {
                return View(new Bed());
            }
            var bed = _db.Beds.Find(bedId);
            if (bed == null)
            {
                return NotFound();
            }
            return View(bed);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateBed(Bed model)
        {

            if (_db.Beds.Any(a => a.BedNumber == model.BedNumber && a.BedId != model.BedId))

            {
                ModelState.AddModelError("Name", "Bed number already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.BedId == 0)
                {
                    _db.Beds.Add(model);
                    TempData["success"] = "Bed added successfully";
                }
                else
                {
                    _db.Beds.Update(model);
                    TempData["success"] = "Bed updated successfully";
                }

                _db.SaveChanges();
                ViewBag.WardList = new SelectList(_db.Wards, "WardId", "WardNumber");

                return RedirectToAction("ListBed");
            }
            return View(model);


        }
        public IActionResult ListBed()
        {
            var bed = _db.Beds.Include(t => t.Ward).ToList();
            return View(bed);
        }
        [HttpGet]
        public IActionResult GetBed()
        {
            var bed = _db.Beds.Include(t => t.Ward).ToList();
            return Json(new { data = bed });
        }
        [HttpDelete]
        public IActionResult DeleteBed(int id)
        {
            var bed = _db.Beds.FirstOrDefault(m => m.BedId == id);
            if (bed == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Beds.Remove(bed);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult AddUser()
        {
            var userCollection = new UserCollection();

            return View(userCollection);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserCollection model)
        {
            var generatedPassword = GenerateRandomPassword();


            var user = new ApplicationUser
            {
                UserName = GenerateUserName(model.ApplicationUser.Email),
                Email = model.ApplicationUser.Email,
                UserRole = model.ApplicationUser.UserRole
            };

            var result = await _userManager.CreateAsync(user, generatedPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, user.UserRole);

                _myEmailSender.SendEmail(user.Email, "Account Credentials", $"Username: {user.UserName}<br/>Password: {generatedPassword}");


                if (user.UserRole == "Nurse")
                {
                    var nurse = new Nurse
                    {
                        Name = model.Nurse.Name,
                        Surname = model.Nurse.Surname,
                        FullName = model.Nurse.Name + " " + model.Nurse.Surname,
                        ContactNumber = model.Nurse.ContactNumber,
                        EmailAddress = model.ApplicationUser.Email,
                        RegistrationNumber = model.Nurse.RegistrationNumber,
                        UserId = user.Id,
                    };
                    _db.Nurses.Add(nurse);
                }
                else if (user.UserRole == "Pharmacist")
                {
                    var pharmacist = new Pharmacist
                    {
                        Name = model.Pharmacist.Name,
                        Surname = model.Pharmacist.Surname,
                        FullName = model.Pharmacist.Name + " " + model.Pharmacist.Surname,
                        ContactNumber = model.Pharmacist.ContactNumber,
                        EmailAddress = model.ApplicationUser.Email,
                        RegistrationNumber = model.Pharmacist.RegistrationNumber,
                        UserId = user.Id,
                    };
                    _db.Pharmacists.Add(pharmacist);
                }
                else if (user.UserRole == "Surgeon")
                {
                    var surgeon = new Surgeon
                    {
                        Name = model.Surgeon.Name,
                        Surname = model.Surgeon.Surname,
                        FullName = model.Surgeon.Name + " " + model.Surgeon.Surname,
                        ContactNumber = model.Surgeon.ContactNumber,
                        EmailAddress = model.ApplicationUser.Email,
                        RegistrationNumber = model.Surgeon.RegistrationNumber,
                        UserId = user.Id,
                    };
                    _db.Surgeons.Add(surgeon);
                }
                else if (user.UserRole == "Anaesthesiologist")
                {
                    var anaesthesiologist = new Anaesthesiologist
                    {
                        Name = model.Anaesthesiologist.Name,
                        Surname = model.Anaesthesiologist.Surname,
                        FullName = model.Anaesthesiologist.Name + " " + model.Anaesthesiologist.Surname,
                        ContactNumber = model.Anaesthesiologist.ContactNumber,
                        EmailAddress = model.ApplicationUser.Email,
                        RegistrationNumber = model.Anaesthesiologist.RegistrationNumber,
                        UserId = user.Id,
                    };
                    _db.Anaesthesiologists.Add(anaesthesiologist);
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("ListUser");


                //switch (user.UserRole)
                //{
                //    case "Nurse":
                //        return RedirectToAction("ListNurse");
                //    case "Pharmacist":
                //        return RedirectToAction("ListPharmacist");
                //    case "Surgeon":
                //        return RedirectToAction("ListSurgeon");
                //    case "Anaesthesiologist":
                //        return RedirectToAction("ListAnaesthesiologist");
                //    default:
                //        return RedirectToAction("Index", "Admin");
                //}
            }
            return View(model);
        }


        private string GenerateUserName(string email)
        {

            return email.Split('@')[0];
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] { "ABCDEFGHJKLMNOPQRSTUVWXYZ", "abcdefghijkmnopqrstuvwxyz", "0123456789", "!@$?_-" };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        public IActionResult ListNurse()
        {
            var nurse = _db.Nurses.ToList();
            return View(nurse);
        }
        public IActionResult AddOrUpdateHospital(int? hospitalId)
        {
            ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c => c.Name), "CityId", "Name");

            if (hospitalId == null)
            {
                return View(new Hospital());
            }
            var suburb = _db.Hospitals.FirstOrDefault(i => i.HospitalId == hospitalId);
            if (suburb == null)
            {
                return NotFound();
            }

            return View(suburb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateHospital(Hospital model)
        {

            if (_db.Hospitals.Any(a => a.Name == model.Name && a.HospitalId != model.HospitalId))
            {
                ModelState.AddModelError("Name", "Hospital name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.HospitalId == 0)
                {
                    _db.Hospitals.Add(model);
                    TempData["success"] = "Hospital added successfully";
                }
                else
                {
                    _db.Hospitals.Update(model);
                    TempData["success"] = "Hospital updated successfully";
                }
                _db.SaveChanges();
                ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c => c.Name), "CityId", "Name");

                return RedirectToAction("ListHospital");
            }
            return View(model);
        }
        public IActionResult ListHospital()
        {
            var hospital = _db.Hospitals.Include(s => s.Suburb).ToList();
            return View(hospital);
        }
        [HttpGet]
        public IActionResult GetHospital()
        {
            var hospital = _db.Hospitals.Include(s => s.Suburb).ToList();
            return Json(new { data = hospital });
        }

        [HttpDelete]
        public IActionResult DeleteHospital(int id)
        {
            var hospital = _db.Hospitals.FirstOrDefault(m => m.HospitalId == id);
            if (hospital == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Hospitals.Remove(hospital);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public IActionResult LoadSuburbs(int cityId)
        {
            var suburbs = _db.Suburbs.Where(s => s.CityId == cityId).Select(s => new { suburbID = s.SuburbId, suburbName = s.Name }).OrderBy(s => s.suburbName).ToList();
            return Json(suburbs);
        }
        public JsonResult GetSuburbsByCity(int cityId)
        {

            var suburbs = _db.Suburbs.Where(s => s.CityId == cityId).Select(s => new { s.SuburbId, s.Name }).OrderBy(s => s.Name).ToList();
            return Json(suburbs);
        }
        public async Task<IActionResult> ListUser()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                if (userRole == "Nurse")
                {
                    var nurse = await _db.Nurses.FirstOrDefaultAsync(n => n.UserId == user.Id);
                    if (nurse != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = nurse.FullName,
                            ContactNumber = nurse.ContactNumber,
                            RegistrationNumber = nurse.RegistrationNumber
                        });
                    }
                }
                else if (userRole == "Pharmacist")
                {
                    var pharmacist = await _db.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (pharmacist != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = pharmacist.FullName,
                            ContactNumber = pharmacist.ContactNumber,
                            RegistrationNumber = pharmacist.RegistrationNumber
                        });
                    }
                }
                else if (userRole == "Surgeon")
                {
                    var surgeon = await _db.Surgeons.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (surgeon != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = surgeon.FullName,
                            ContactNumber = surgeon.ContactNumber,
                            RegistrationNumber = surgeon.RegistrationNumber
                        });
                    }
                }
                else if (userRole == "Anaesthesiologist")
                {
                    var anaesthesiologist = await _db.Anaesthesiologists.FirstOrDefaultAsync(a => a.UserId == user.Id);
                    if (anaesthesiologist != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = anaesthesiologist.FullName,
                            ContactNumber = anaesthesiologist.ContactNumber,
                            RegistrationNumber = anaesthesiologist.RegistrationNumber
                        });
                    }
                }
            }

            return View(userList);
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                if (userRole == "Nurse")
                {
                    var nurse = await _db.Nurses.FirstOrDefaultAsync(n => n.UserId == user.Id);
                    if (nurse != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = nurse.FullName,
                            ContactNumber = nurse.ContactNumber,
                            RegistrationNumber = nurse.RegistrationNumber
                        });
                    }
                }
                else if (userRole == "Pharmacist")
                {
                    var pharmacist = await _db.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (pharmacist != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = pharmacist.FullName,
                            ContactNumber = pharmacist.ContactNumber,
                            RegistrationNumber = pharmacist.RegistrationNumber
                        });
                    }
                }
                else if (userRole == "Surgeon")
                {
                    var surgeon = await _db.Surgeons.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (surgeon != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = surgeon.FullName,
                            ContactNumber = surgeon.ContactNumber,
                            RegistrationNumber = surgeon.RegistrationNumber
                        });
                    }
                }
                else if (userRole == "Anaesthesiologist")
                {
                    var anaesthesiologist = await _db.Anaesthesiologists.FirstOrDefaultAsync(a => a.UserId == user.Id);
                    if (anaesthesiologist != null)
                    {
                        userList.Add(new UserViewModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            UserRole = userRole,
                            FullName = anaesthesiologist.FullName,
                            ContactNumber = anaesthesiologist.ContactNumber,
                            RegistrationNumber = anaesthesiologist.RegistrationNumber
                        });
                    }
                }
            }

            return Json(new { data = userList });
        }
        [HttpGet]
        public async Task<IActionResult> AddOrUpdateUser(int? userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            //if (user == null)
            //{
            //    return NotFound();
            //}

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var model = new UserViewModel
            {
               // UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                UserRole = userRole
            };

            switch (userRole)
            {
                case "Nurse":
                    var nurse = await _db.Nurses.FirstOrDefaultAsync(n => n.UserId == user.Id);
                    if (nurse != null)
                    {
                        model.FullName = nurse.FullName;
                        model.ContactNumber = nurse.ContactNumber;
                        model.RegistrationNumber = nurse.RegistrationNumber;
                    }
                    break;
                case "Pharmacist":
                    var pharmacist = await _db.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (pharmacist != null)
                    {
                        model.FullName = pharmacist.FullName;
                        model.ContactNumber = pharmacist.ContactNumber;
                        model.RegistrationNumber = pharmacist.RegistrationNumber;
                    }
                    break;
                case "Surgeon":
                    var surgeon = await _db.Surgeons.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (surgeon != null)
                    {
                        model.FullName = surgeon.FullName;
                        model.ContactNumber = surgeon.ContactNumber;
                        model.RegistrationNumber = surgeon.RegistrationNumber;
                    }
                    break;
                case "Anaesthesiologist":
                    var anaesthesiologist = await _db.Anaesthesiologists.FirstOrDefaultAsync(a => a.UserId == user.Id);
                    if (anaesthesiologist != null)
                    {
                        model.FullName = anaesthesiologist.FullName;
                        model.ContactNumber = anaesthesiologist.ContactNumber;
                        model.RegistrationNumber = anaesthesiologist.RegistrationNumber;
                    }
                    break;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            //if (user == null)
            //{
            //    return NotFound();
            //}

            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            switch (userRole)
            {
                case "Nurse":
                    var nurse = await _db.Nurses.FirstOrDefaultAsync(n => n.UserId == user.Id);
                    if (nurse != null)
                    {
                        nurse.FullName = model.FullName;
                        nurse.ContactNumber = model.ContactNumber;
                        nurse.RegistrationNumber = model.RegistrationNumber;
                    }
                    break;
                case "Pharmacist":
                    var pharmacist = await _db.Pharmacists.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (pharmacist != null)
                    {
                        pharmacist.FullName = model.FullName;
                        pharmacist.ContactNumber = model.ContactNumber;
                        pharmacist.RegistrationNumber = model.RegistrationNumber;
                    }
                    break;
                case "Surgeon":
                    var surgeon = await _db.Surgeons.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (surgeon != null)
                    {
                        surgeon.FullName = model.FullName;
                        surgeon.ContactNumber = model.ContactNumber;
                        surgeon.RegistrationNumber = model.RegistrationNumber;
                    }
                    break;
                case "Anaesthesiologist":
                    var anaesthesiologist = await _db.Anaesthesiologists.FirstOrDefaultAsync(a => a.UserId == user.Id);
                    if (anaesthesiologist != null)
                    {
                        anaesthesiologist.FullName = model.FullName;
                        anaesthesiologist.ContactNumber = model.ContactNumber;
                        anaesthesiologist.RegistrationNumber = model.RegistrationNumber;
                    }
                    break;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("ViewAllUsers");
        }


    }
}
