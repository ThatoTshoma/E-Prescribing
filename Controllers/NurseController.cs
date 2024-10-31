using E_Prescribing.CollectionModel;
using E_Prescribing.Data;
using E_Prescribing.Data.Services;
using E_Prescribing.Models;
using E_Prescribing.Services;
using iText.Commons.Actions.Contexts;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Security.Claims;
using static E_Prescribing.Areas.Identity.Pages.Account.RegisterModel;

namespace E_Prescribing.Controllers
{
    public class NurseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly Cart _cart;
        private readonly IMedicationService _medicationService;
        private readonly IMyEmailSender _emailSender;




        public NurseController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, Cart cart, IMedicationService medicationService, IMyEmailSender myEmailSender)
        {
            _userManager = userManager;
            _db = db;
            _cart = cart;
            _medicationService = medicationService;
            _emailSender = myEmailSender;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceivePrescription(int prescriptionId)
        {
            var prescription = await _db.Prescriptions.FindAsync(prescriptionId);
            if (prescription != null && prescription.Status == "Dispensed")
            {
                prescription.Status = "Received";
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true, message = "Prescription recieved successfully" });
        }
        public IActionResult MedicationHistoryPage(int id)
        {
            var booking = _db.Bookings
                              .Where(b => b.BookingId == id && b.Status == true && b.DischargeDate ==null)
                              .OrderByDescending(b => b.Date)
                              .FirstOrDefault();

            ViewBag.WardList = booking != null
                ? new SelectList(_db.Wards.Where(w => w.Theatres.Any(t => t.TheatreId == booking.TheatreId)).OrderBy(w => w.WardNumber), "WardId", "WardNumber")
                : new SelectList(_db.Wards.OrderBy(c => c.WardNumber), "WardId", "WardNumber");

            ViewBag.MedicationList = new SelectList(_db.Medications.OrderBy(c => c.Name), "MedicationId", "Name");
            ViewBag.ActiveIngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");
            ViewBag.ConditionList = new SelectList(_db.ConditionDiagnoses.OrderBy(c => c.Name), "ConditionId", "Name");

            var collection = new PatientOnboardingModel
            {
                PatientAllergy = new PatientAllergy { PatientId = id },
                PatientMedication = new PatientMedication { PatientId = id },
                PatientCondition = new PatientCondition { PatientId = id },
                Booking = new Booking { PatientId = id },
                Patients = _db.Patients.ToList(),
                CurrentStep = 1
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MedicationHistoryPage(PatientOnboardingModel model)
        {
            var booking = _db.Bookings
                  .Where(b => b.BookingId == model.PatientAllergy.PatientId && b.Status == true && b.DischargeDate == null)
                  .OrderByDescending(b => b.Date)
                  .FirstOrDefault();
       


            if (booking != null)
            {
                for (int step = model.CurrentStep; step <= 5; step++)
                {
                    switch (step)
                    {
                        case 1:
                            if (model.PatientBed.SelectedBeds != null)
                            {
                                foreach (var selectedBedId in model.PatientBed.SelectedBeds)
                                {
                                    var patientBed = new PatientBed
                                    {
                                        BedId = selectedBedId,
                                        PatientId = booking.PatientId
                                    };

                                    _db.PatientBeds.Add(patientBed);
                                }
                            }
                            break;

                        case 2:
                            if (model.PatientAllergy.SelectedActiveIngredient != null)
                            {
                                foreach (var selectedActiveIngredientId in model.PatientAllergy.SelectedActiveIngredient)
                                {
                                    var patientAllergy = new PatientAllergy
                                    {
                                        ActiveIngredientId = selectedActiveIngredientId,
                                        PatientId = booking.PatientId
                                    };
                                    _db.PatientAllergies.Add(patientAllergy);
                                }
                            }
                            break;

                        case 3:
                            if (model.PatientMedication.SelectedMedication != null)
                            {
                                foreach (var selectedMedicationId in model.PatientMedication.SelectedMedication)
                                {
                                    var patientMedication = new PatientMedication
                                    {
                                        MedicationId = selectedMedicationId,
                                        PatientId = booking.PatientId
                                    };
                                    _db.PatientMedications.Add(patientMedication);
                                }
                            }
                            break;

                        case 4:
                            if (model.PatientCondition.SelectedCondition != null)
                            {
                                foreach (var selectedConditionId in model.PatientCondition.SelectedCondition)
                                {
                                    var patientCondition = new PatientCondition
                                    {
                                        ConditionId = selectedConditionId,
                                        PatientId = booking.PatientId
                                    };
                                    _db.PatientConditions.Add(patientCondition);
                                }
                            }
                            break;

                        case 5:
                            if (model.Booking.AdmissionDate.HasValue)
                            {
                                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                                var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);

                                if (booking != null)
                                {
                                    booking.AdmissionDate = model.Booking.AdmissionDate.Value;
                                    booking.NurseId = nurse?.NurseId; 

                                    _db.Bookings.Update(booking); 
                                }
                            }
                            break;

                    }

                    _db.SaveChanges();
                }

                return RedirectToAction("PatientDetails", new { id = booking.PatientId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }

            ViewBag.MedicationList = new SelectList(_db.Medications.OrderBy(c => c.Name), "MedicationId", "Name");
            ViewBag.ActiveIngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");
            ViewBag.ConditionList = new SelectList(_db.ConditionDiagnoses.OrderBy(c => c.Name), "ConditionId", "Name");
            ViewBag.WardList = booking != null
                ? new SelectList(_db.Wards.Where(w => w.Theatres.Any(t => t.TheatreId == booking.TheatreId)).OrderBy(w => w.WardNumber), "WardId", "WardNumber")
                : new SelectList(_db.Wards.OrderBy(c => c.WardNumber), "WardId", "WardNumber");
            model.Patients = _db.Patients.ToList();
            return View(model);
        }

        public IActionResult ViewMedicationHistory(int id)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientMedicationHistoryViewModel
            {
                Patient = patient,
                Beds = _db.PatientBeds
                    .Where(pb => pb.PatientId == id)
                    .Include(pb => pb.Bed)
                    .ToList(),
                Allergies = _db.PatientAllergies
                    .Where(pa => pa.PatientId == id)
                    .Include(pa => pa.ActiveIngredient)
                    .ToList(),
                Medications = _db.PatientMedications
                    .Where(pm => pm.PatientId == id)
                    .Include(pm => pm.Medication)
                    .ToList(),
                Conditions = _db.PatientConditions
                    .Where(pc => pc.PatientId == id)
                    .Include(pc => pc.Condition)
                    .ToList()
            };

            return View(viewModel);
        }



        public IActionResult LoadSuburbs(int cityId)
        {
            var suburbs = _db.Suburbs.Where(s => s.CityId == cityId).OrderBy(s => s.Name).Select(s => new { suburbID = s.SuburbId, suburbName = s.Name }).ToList();
            return Json(suburbs);
        }
        public JsonResult GetSuburbsByCity(int cityId)
        {

            var suburbs = _db.Suburbs.Where(s => s.CityId == cityId).Select(s => new { s.SuburbId, s.Name }).OrderBy(s => s.Name).ToList();
            return Json(suburbs);
        }



        public IActionResult AddPatient()
        {

            ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c => c.Name), "CityId", "Name");
            ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber");
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPatient(Patient model)
        {

            if (!ValidateDateOfBirth(model.IdNumber, model.DateOfBirth))
            {
                ModelState.AddModelError("DateOfBirth", "Date of birth does not match the ID number.");
                return View();
            }

            var patient = new Patient();

            patient.Name = model.Name;
            patient.Surname = model.Surname;
            patient.IdNumber = model.IdNumber;
            patient.Gender = model.Gender;
            patient.DateOfBirth = model.DateOfBirth;
            patient.ContactNumber = model.ContactNumber;
            patient.EmailAddress = model.EmailAddress;
            patient.AddressLine1 = model.AddressLine1;
            patient.AddressLine2 = model.AddressLine2;
            patient.SuburbId = model.SuburbId;


            ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c => c.Name), "CityId", "Name");
            ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber");
            _db.Patients.Add(patient);
            _db.SaveChanges();


            return RedirectToAction("ListPatient");


        }
        public IActionResult UpdatePatient(int? id)
        {
            var patient = _db.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.CityList = new SelectList(_db.Cities.OrderBy(c => c.Name), "CityId", "Name");
            ViewBag.BedList = new SelectList(_db.Beds, "BedId", "BedNumber");

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePatient(int id, Patient model)
        {
            if (id != model.PatientId)
            {
                return NotFound();
            }


            var patient = _db.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }


            patient.ContactNumber = model.ContactNumber;
            patient.EmailAddress = model.EmailAddress;
            patient.AddressLine1 = model.AddressLine1;
            patient.AddressLine2 = model.AddressLine2;
            patient.SuburbId = model.SuburbId;


            _db.Patients.Update(patient);
            _db.SaveChanges();

            return RedirectToAction("ListAdmittedPatient");
        }
       
        public IActionResult ListAdmittedPatient(string idNumber = null, DateTime? bookingDate = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);

            var query = _db.Bookings
                           .Where(p => p.NurseId == nurse.NurseId)
                           .Include(p => p.Patient)
                           .ToList();


            return View(query);
        }
        [HttpGet]
        public IActionResult GetAdmittedPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);

            var bookings = _db.Bookings
                          .Where(p => p.NurseId == nurse.NurseId)
                          .Include(p => p.Patient)
                          .ToList();


            var viewModel = new
            {
                data = bookings.Select(b => new
                {
         
                    patient = new
                    {
                        patientId = b.PatientId,
                        name = b.Patient.FullName,
                        idNumber = b.Patient.IdNumber,
                        dateOfBirth = b.Patient.DateOfBirth,
                        contactNumber =b.Patient.ContactNumber,
                        email = b.Patient.EmailAddress
                    },
                    bookingId = b.BookingId,
                    admissionDate = b.AdmissionDate?.ToString("dd-MM-yyyy"),
                    dischargeDate = b.DischargeDate?.ToString("dd-MM-yyyy"),
          
                }).ToArray()
            };

            return Json(viewModel);
        }
        [HttpGet]
        public IActionResult DischargePatient(int id)
        {
            var booking = _db.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }


            ViewBag.PatientId = id;
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DischargePatient(int id, Booking model)
        {
            if (id != model.BookingId)
            {
                return NotFound();
            }


            var booking = _db.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }


            booking.DischargeDate = model.DischargeDate;

            _db.Bookings.Update(booking);
            _db.SaveChanges();

            return RedirectToAction("ListAdmittedPatient");
        }

        public IActionResult AddVital(int id)
        {
            var patientVitals = _db.PatientVitals
                .Where(pv => pv.PatientId == id)
                .Include(pv => pv.Vital)
                .ToList();

            var collection = new VitalCollection
            {
                Vital = new VitalRange { PatientId = id },
                Patients = _db.Patients.ToList(),
                SelectedVitals = patientVitals.Select(pv => pv.Vital).ToList() 
            };
            ViewBag.PatientId = id;

            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddVital(VitalCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.Vital.PatientId);

            if (patient != null)
            {
                var outOfRangeVitals = new List<string>(); 

                foreach (var vitalInput in model.VitalInputs)
                {
                    var vital = _db.Vitals.FirstOrDefault(v => v.Name == vitalInput.Key);
                    if (vital != null)
                    {
                        if (double.TryParse(vitalInput.Value, out var vitalValue))
                        {
                            if ((vital.Minimum.HasValue && vitalValue < vital.Minimum.Value) ||
                                (vital.Maximum.HasValue && vitalValue > vital.Maximum.Value))
                            {
                                outOfRangeVitals.Add($"\n{vital.Name}: {vitalValue} (Must range between {vital.Minimum} - {vital.Maximum})");
                            }
                        }
                    }
                }

                if (outOfRangeVitals.Any())
                {
                    TempData["OutOfRangeVitals"] = outOfRangeVitals; 
                    return RedirectToAction("AddVital", new { id = model.Vital.PatientId });
                }

                var vitalRange = new VitalRange
                {
                    PatientId = patient.PatientId,
                    Height = model.VitalInputs.ContainsKey("Height") ? model.VitalInputs["Height"] : string.Empty,
                    Weight = model.VitalInputs.ContainsKey("Weight") ? model.VitalInputs["Weight"] : string.Empty,
                    BloodPressure = model.VitalInputs.ContainsKey("BloodPressure") ? model.VitalInputs["BloodPressure"] : string.Empty,
                    PulseRate = model.VitalInputs.ContainsKey("PulseRate") ? model.VitalInputs["PulseRate"] : string.Empty,
                    Temperature = model.VitalInputs.ContainsKey("Temperature") ? model.VitalInputs["Temperature"] : string.Empty,
                    RespiratoryRate = model.VitalInputs.ContainsKey("RespiratoryRate") ? model.VitalInputs["RespiratoryRate"] : string.Empty,
                    BloodGlucoseLevel = model.VitalInputs.ContainsKey("BloodGlucoseLevel") ? model.VitalInputs["BloodGlucoseLevel"] : string.Empty,
                    BloodOxegenLevel = model.VitalInputs.ContainsKey("BloodOxegenLevel") ? model.VitalInputs["BloodOxegenLevel"] : string.Empty,
                    OxygenSaturation = model.VitalInputs.ContainsKey("OxygenSaturation") ? model.VitalInputs["OxygenSaturation"] : string.Empty,
                    HeartRate = model.VitalInputs.ContainsKey("HeartRate") ? model.VitalInputs["HeartRate"] : string.Empty,
                    BloodpressureSystolic = model.VitalInputs.ContainsKey("BloodpressureSystolic") ? model.VitalInputs["BloodpressureSystolic"] : string.Empty,
                    BMI = model.VitalInputs.ContainsKey("BMI") ? model.VitalInputs["BMI"] : string.Empty,
                    Time = model.Vital.Time
                };

                _db.VitalRanges.Add(vitalRange);
                _db.SaveChanges();

                return RedirectToAction("ViewVitals", new { id = patient.PatientId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }

            model.Patients = _db.Patients.ToList();
            model.SelectedVitals = _db.PatientVitals
                .Where(pv => pv.PatientId == model.Vital.PatientId)
                .Select(pv => pv.Vital)
                .ToList();

            return View(model);
        }


        public IActionResult ViewVitals(int id)
        {
            var patient = _db.Patients
                .Include(p => p.PatientVitals)
                .ThenInclude(pv => pv.Vital)
                .FirstOrDefault(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            var patientVitals = _db.VitalRanges
                .Where(v => v.PatientId == id)
                .ToList();

            var selectedVitalIds = _db.PatientVitals
                .Where(pv => pv.PatientId == id)
                .Select(pv => pv.VitalId)
                .ToList();

            var selectedVitals = _db.Vitals
                .Where(v => selectedVitalIds.Contains(v.VitalId))
                .ToList();

            var model = new VitalCollection
            {
                Patient = patient,
                SelectedVitals = selectedVitals,
                VitalRanges = patientVitals
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetPatientVital(int id)
        {
            var patientVitals = _db.VitalRanges
                .Where(v => v.PatientId == id)
                .ToList();

            var patient = _db.Patients
                .Include(p => p.PatientVitals)
                .ThenInclude(pv => pv.Vital)
                .FirstOrDefault(p => p.PatientId == id);

            var selectedVitalIds = _db.PatientVitals
                .Where(pv => pv.PatientId == id)
                .Select(pv => pv.VitalId)
                .ToList();

            var selectedVitals = _db.Vitals
                .Where(v => selectedVitalIds.Contains(v.VitalId))
                .ToList();

            var result = new
            {
                data = patientVitals.Select(v => new
                {
                    time = v.Time.ToString("hh\\:mm"),
                    height = v.Height,
                    weight = v.Weight,
                    bloodpressure = v.BloodPressure,
                    pulserate = v.PulseRate,
                    temperature = v.Temperature,
                    respiratoryrate = v.RespiratoryRate,
                    bloodglucoselevel = v.BloodGlucoseLevel,
                    heartrate = v.HeartRate,
                    oxygensaturation = v.OxygenSaturation,
                    bloodoxegenlevel = v.BloodOxegenLevel
                }).ToList(),
                vitalNames = selectedVitals.Select(v => v.Name.ToLower()).ToList()
            };



            return Json(result);
        }

        public IActionResult SendVitalsEmail(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Surgeon)
                .Include(b => b.Patient)
                .Include(b => b.Patient.PatientVitals)
                .ThenInclude(pv => pv.Vital)
                .Include(b => b.Patient.Nurse)  
                .FirstOrDefault(b => b.PatientId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var selectedVitalIds = _db.PatientVitals
                .Where(pv => pv.PatientId == id)
                .Select(pv => pv.VitalId)
                .ToList();

            var selectedVitals = _db.Vitals
                .Where(v => selectedVitalIds.Contains(v.VitalId))
                .ToList();

            var patientVitals = _db.VitalRanges
                .Where(v => v.PatientId == id)
                .ToList();

            var emailContent = $"<h2>Vitals Readings for {booking.Patient.FullName}</h2>";
            emailContent += "<table border='1' cellpadding='5' cellspacing='0'>";
            emailContent += "<thead><tr><th>Time</th>";

            foreach (var vital in selectedVitals)
            {
                emailContent += $"<th>{vital.Name}</th>";
            }
            emailContent += "</tr></thead><tbody>";

            foreach (var vitalEntry in patientVitals)
            {
                emailContent += "<tr>";
                emailContent += $"<td>{vitalEntry.Time.ToString("hh\\:mm")}</td>";

                foreach (var vital in selectedVitals)
                {
                    var vitalValue = vitalEntry.GetType().GetProperty(vital.Name)?.GetValue(vitalEntry, null);
                    emailContent += $"<td>{vitalValue ?? " "}</td>";
                }

                emailContent += "</tr>";
            }

            emailContent += "</tbody></table>";

            var nurseName = booking.Patient.Nurse != null
                ? $"{booking.Patient.Nurse.FullName}"
                : "The Nursing Team";

            emailContent += $"<br/><br/>Kind regards,<br/>{nurseName}";

            _emailSender.SendEmail(booking.Surgeon.EmailAddress, "Vitals Readings [GRP-04-21]", emailContent);

            TempData["Success"] = "Email has been sent to the surgeon successfully.";

            return RedirectToAction("ViewVitals", new { id });
        }

        public IActionResult ListVital(int id)
        {
            var patientVital = _db.VitalRanges.Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientVital);
        }
        private bool ValidateDateOfBirth(string idNumber, DateTime dateOfBirth)
        {
            if (idNumber.Length != 13)
            {
                return false;
            }

            int year = int.Parse(idNumber.Substring(0, 2));
            int month = int.Parse(idNumber.Substring(2, 2));
            int day = int.Parse(idNumber.Substring(4, 2));

            if (month > 12)
            {
                year += 2000;
                month -= 20;
            }
            else
            {
                year += 1900;
            }
            if (dateOfBirth.Year == year && dateOfBirth.Month == month && dateOfBirth.Day == day)
            {
                return true;
            }
            else
            {
                return false;
            }

           
        }
        public IActionResult ListPatient(string idNumber = null, DateTime? bookingDate = null)
        {
            var bookings = _db.Bookings
                           .Where(b => b.Status == true)
                           .Include(b => b.Anaesthesiologist)
                           .Include(b => b.Patient)
                           .ThenInclude(p => p.Suburb)
                           .ToList();
            return View(bookings);

        }
        [HttpGet]
        public IActionResult GetPatient()
        {
            var bookings = _db.Bookings
                           .Where(b => b.Status == true)
                           .Include(b => b.Anaesthesiologist)
                           .Include(b => b.Patient)
                           .ThenInclude(p => p.Suburb)
                           .ToList();

            var viewModel = new
            {
                data = bookings.Select(b => new
                {
                    anaesthesiologist = new
                    {
                        name = b.Anaesthesiologist.FullName
                    },
                    patient = new
                    {
                        patientId = b.PatientId,
                        name = b.Patient.FullName,
                        idNumber = b.Patient.IdNumber,
                        dateOfBirth = b.Patient.DateOfBirth,
                        contactNumber = b.Patient.ContactNumber,
                        email = b.Patient.EmailAddress
                    },
                    status = b.Status ? "Approved" : "Pending",
                    date = b.Date.ToString("dd-MM-yyyy"),
                    bookingId = b.BookingId,
                    admissionDate = b.AdmissionDate?.ToString("dd-MM-yyyy"),

                }).ToArray()
            };

            return Json(viewModel);
        }


        public async Task<IActionResult> PatientDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _db.Patients
                .Include(s => s.Suburb)
                .Include(p => p.PatientConditions)
                    .ThenInclude(pc => pc.Condition)
                    .Include(p => p.PatientMedications)
                    .ThenInclude(p => p.Medication)
                    .Include(p => p.PatientVitals)
                    .ThenInclude(p => p.Vital)
                    .Include(p => p.PatientAllergies)
                    .ThenInclude(p => p.ActiveIngredient)
                    .Include(p => p.VitalsRanges)
                            .Include(p => p.Booking) 

                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }
            ViewBag.PatientId = id;
            return View(patient);
        }

        public JsonResult LoadBed(int wardId)
        {
            var beds = _db.Beds.Where(s => s.WardId == wardId).Select(s => new { bedId = s.BedId, bedNumber = s.BedNumber }).ToList();
            return Json(beds);
        }


        public IActionResult ListPatientBed(int id)
        {
            var patientBeds = _db.PatientBeds.Include(pb => pb.Bed)
                                              .ThenInclude(b => b.Ward)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientBeds);
        }



        public IActionResult AddPatientCondition(int id)
        {
            ViewBag.CondtionList = new SelectList(_db.ConditionDiagnoses, "ConditionId", "Name");

            var collection = new PatientConditionCollection
            {
                PatientCondition = new PatientCondition { PatientId = id },
                Patients = _db.Patients.ToList(),
            };
            return View(collection);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPatientCondition(PatientConditionCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.PatientCondition.PatientId);

            if (patient != null)
            {
                foreach (var selectedConditionId in model.PatientCondition.SelectedCondition)
                {
                    var patientCondition = new PatientCondition
                    {
                        ConditionId = selectedConditionId,
                        PatientId = patient.PatientId
                    };

                    _db.PatientConditions.Add(patientCondition);
                }

                _db.SaveChanges();

                return RedirectToAction("ViewChro");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }
            ViewBag.CondtionList = new SelectList(_db.ConditionDiagnoses, "ConditionId", "Name");
            model.Patients = _db.Patients.ToList();
            return View(model);

        }
        public IActionResult ListPatientCondition(int id)
        {
            var patientCondition = _db.PatientConditions.Include(pb => pb.Condition)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientCondition);
        }
        public IActionResult AddPatientVital(int id)
        {
            ViewBag.VitalList = new SelectList(_db.VitalRanges, "VitalId", "Name");

            var collection = new PatinetVitalCollection
            {
                PatientVital = new PatientVital { PatientId = id },
                Patients = _db.Patients.ToList(),
            };
            return View(collection);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPatientVital(PatinetVitalCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.PatientVital.PatientId);

            if (patient != null)
            {
                foreach (var selectedVitalId in model.PatientVital.SelectedVital)
                {
                    var patientVital = new PatientVital
                    {
                        VitalId = selectedVitalId,
                        PatientId = patient.PatientId
                    };

                    _db.PatientVitals.Add(patientVital);
                }

                _db.SaveChanges();

                return RedirectToAction("ViewChro");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }
            ViewBag.VitalList = new SelectList(_db.VitalRanges, "VitalId", "Name");
            model.Patients = _db.Patients.ToList();
            return View(model);

        }
        public IActionResult AddPatientAllergy(int id)
        {
            ViewBag.ActiveIngredientList = new SelectList(_db.ActiveIngredients, "IngredientId", "Name");

            var collection = new PatientAllergyCollection
            {
                PatientAllergy = new PatientAllergy { PatientId = id },
                Patients = _db.Patients.ToList(),
            };
            return View(collection);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPatientAllergy(PatientAllergyCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.PatientAllergy.PatientId);

            if (patient != null)
            {
                foreach (var selectedActiveIngredientId in model.PatientAllergy.SelectedActiveIngredient)
                {
                    var patientAllergy = new PatientAllergy
                    {
                        ActiveIngredientId = selectedActiveIngredientId,
                        PatientId = patient.PatientId
                    };

                    _db.PatientAllergies.Add(patientAllergy);
                }

                _db.SaveChanges();

                return RedirectToAction("ViewChro");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }
            ViewBag.ActiveIngredientList = new SelectList(_db.ActiveIngredients, "IngredientId", "Name");
            model.Patients = _db.Patients.ToList();
            return View(model);

        }
        public IActionResult ListPatientAllergy(int id)
        {
            var patientAllergy = _db.PatientAllergies.Include(pb => pb.ActiveIngredient)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientAllergy);
        }
        public IActionResult AddPatientMedication(int id)
        {
            ViewBag.MedicationList = new SelectList(_db.Medications, "MedicationId", "Name");

            var collection = new PatientMedicationCollection
            {
                PatientMedication = new PatientMedication { PatientId = id },
                Patients = _db.Patients.ToList(),
            };
            return View(collection);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPatientMedication(PatientMedicationCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.PatientMedication.PatientId);

            if (patient != null)
            {
                foreach (var selectedMedicationtId in model.PatientMedication.SelectedMedication)
                {
                    var patientMedication = new PatientMedication
                    {
                        MedicationId = selectedMedicationtId,
                        PatientId = patient.PatientId
                    };

                    _db.PatientMedications.Add(patientMedication);
                }

                _db.SaveChanges();

                return RedirectToAction("ViewChro");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }
            ViewBag.MedicationList = new SelectList(_db.Medications, "MedicationId", "Name");
            model.Patients = _db.Patients.ToList();
            return View(model);

        }
        public IActionResult ListPatientMedication(int id)
        {
            var patientMedication = _db.PatientMedications.Include(pb => pb.Medication)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientMedication);
        }
        public IActionResult ListPatientPrescription(int id)
        {
            var prescription = _db.Prescriptions
                                .Include(p => p.MedicationPrescriptions)
                                .ThenInclude(P => P.Medication)
                                .Include(p => p.Patient)
                                .Include(p => p.Surgeon)
                                .Include(p =>p.Pharmacist)
                                .Where (p => p.PatientId == id)
                                .ToList();
            ViewBag.PatientId = id;
            return View(prescription);
        }
        [HttpGet]
        public IActionResult GetPatientPrescription(int id)
        {

            var prescriptions = _db.Prescriptions
                                  .Include(p => p.MedicationPrescriptions)
                                  .ThenInclude(mp => mp.Medication)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Nurse)
                                  .Include(p => p.Pharmacist)
                                  .Where(p => p.PatientId == id)
                                  .ToList();

            var result = prescriptions.Select(p => new
            {
                Date = p.Date.ToString("dd-MM-yyyy"),
                Medications = string.Join("\n", p.MedicationPrescriptions.Select(mp => mp.Medication.Name)),
                Quantities = string.Join("\n", p.MedicationPrescriptions.Select(mp => mp.Quantity.ToString())),
                p.Status,
                p.Urgent,

                p.PrescriptionId
            });

            return Json(new { data = result });
        }
        public async Task<IActionResult> Prescription(int id)
        {
            var prescription = await _db.Prescriptions
                        .Include(p => p.Patient)
                        .Include(p => p.Surgeon)
                        .Include(p => p.Pharmacist)
                        .Include(p => p.MedicationPrescriptions)
                        .ThenInclude(mp => mp.Medication)
                        .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);


        }
      
        public async Task<IActionResult> AdministerMedication(Dictionary<int, int> quantities)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);
            var order = new Prescription()
            {
                NurseId = nurse.NurseId,
            };
            await _db.Prescriptions.AddAsync(order);
            await _db.SaveChangesAsync();

            if (quantities != null && quantities.Any())
            {
                foreach (var medicationId in quantities.Keys)
                {
                    var quantity = quantities[medicationId];
                    if (quantity > 0)
                    {
                        var pharmacistOrder = new AdministeredMedication
                        {
                            MedicationId = medicationId,
                            PrescriptionId = order.PrescriptionId,
                            Quantity = quantity
                        };

                        _db.AdministeredMedications.Add(pharmacistOrder);
                    }
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ListMedication");
        }

        public async Task<IActionResult> ListMedication()
        {
            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                                .Include(m => m.Medication.DosageForm)

                .Include(m => m.ActiveIngredient)
                .ToListAsync();

            return View(medications);
        }
        public async Task<IActionResult> AddMedicationToCart(int id)
        {
            var medication = await _medicationService.GetMedicationById(id);

            if (medication != null)
            {
                _cart.AddMedication(medication);
            }
            return RedirectToAction(nameof(MedicationCart));
        }
        public async Task<IActionResult> RemoveMedicationFromCart(int id)
        {
            var item = await _medicationService.GetMedicationById(id);

            if (item != null)
            {
                _cart.RemoveMedication(item);
            }
            return RedirectToAction(nameof(MedicationCart));
        }
        public IActionResult MedicationCart()
        {
            var medications = _cart.GetMedicationCart();
            _cart.Medications = medications;

            var response = new CartModel()
            {
                Cart = _cart,
            };

            return View(response);
        }
        public async Task<IActionResult> AdministeredMedication()
        {
            var medications = _cart.GetMedicationCart();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (nurse != null)
            {
                var prescription = await _db.Prescriptions.FirstOrDefaultAsync(p => p.Status == "Prescribed" && p.NurseId == null);

                if (prescription != null)
                {
                    prescription.NurseId = nurse.NurseId;
                    //prescription.Status = "Dispensed";

                    foreach (var medication in medications)
                    {
                        var administeredMedication = new AdministeredMedication
                        {
                            Quantity = medication.Quantity,
                            MedicationId = medication.Medication.MedicationId,
                            PrescriptionId = prescription.PrescriptionId,
                        };

                        _db.AdministeredMedications.Add(administeredMedication);

                    }

                    await _db.SaveChangesAsync();
                    await _cart.ClearCart();

                    return View("CompletedAdministered");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No prescriptions available Administer.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nurse not found.");
            }

            return View("CompletedAdministered");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(int PrescriptionId, Dictionary<int, int> quantities, DateTime administeredTime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (nurse != null)
            {
                var prescription = await _db.Prescriptions
                                            .Include(p => p.MedicationPrescriptions)
                                            .ThenInclude(mp => mp.Medication)
                                            .Include(p => p.AdministeredMedications) 
                                            .FirstOrDefaultAsync(p => p.PrescriptionId == PrescriptionId);

                if (prescription != null)
                {
                    prescription.NurseId = nurse.NurseId;

                    if (quantities != null && quantities.Any())
                    {
                        foreach (var medicationId in quantities.Keys)
                        {
                            var quantityToAdminister = quantities[medicationId];
                            if (quantityToAdminister > 0)
                            {
                                var medicationPrescription = prescription.MedicationPrescriptions
                                    .FirstOrDefault(mp => mp.MedicationId == medicationId);

                                if (medicationPrescription != null)
                                {
                                    var totalAdministered = prescription.AdministeredMedications
                                        .Where(am => am.MedicationId == medicationId)
                                        .Sum(am => am.Quantity);

                                    if (totalAdministered + quantityToAdminister <= medicationPrescription.Quantity)
                                    {
                                        var combinedDateTime = new DateTime(
                                            DateTime.Now.Year,
                                            DateTime.Now.Month,
                                            DateTime.Now.Day,
                                            administeredTime.Hour,
                                            administeredTime.Minute,
                                            0);

                                        var administeredMedication = new AdministeredMedication
                                        {
                                            MedicationId = medicationId,
                                            PrescriptionId = prescription.PrescriptionId,
                                            Time = combinedDateTime,
                                            Quantity = quantityToAdminister
                                        };

                                        _db.Add(administeredMedication);
                                        TempData["success"] = "Medication administered successfully";
                                    }
                                    else
                                    {
                                        TempData["error"] = $"Cannot administer more than prescribed quantity for {medicationPrescription.Medication.Name}.";

                                        return RedirectToAction("ListMedicationToAdminister", new { id = PrescriptionId });


                                    }
                                }
                            }
                        }
                        await _db.SaveChangesAsync();
                    }
                    return RedirectToAction("ListAdministeredMedication", new { id = prescription.PrescriptionId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No prescription found.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nurse not found.");
            }
            return RedirectToAction("ListMedicationToAdminister", new { id =PrescriptionId });
        }


        public async Task<IActionResult> ListMedicationToAdminister(int id)
        {
            var prescription = await _db.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Surgeon)
                    .Include(p => p.Pharmacist)
                    .Include(p => p.MedicationPrescriptions)
                    .ThenInclude(mp => mp.Medication)
                    .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                .Include(m => m.Medication.DosageForm)
                .Include(m => m.ActiveIngredient)
                .ToListAsync();

            var viewModel = new MedicationAdministerViewModel
            {
                Prescription = prescription,
                Patient = prescription.Patient,  
                Medications = medications
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> GetMedicationToAdminister(int id)
        {
            var prescription = await _db.Prescriptions
                 .Include(p => p.Patient)
                 .Include(p => p.Surgeon)
                 .Include(p => p.Pharmacist)
                 .Include(p => p.MedicationPrescriptions)
                 .ThenInclude(mp => mp.Medication)
                 .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            var medicationIds = prescription.MedicationPrescriptions.Select(mp => mp.MedicationId).ToList();

            var medications = await _db.Medications
                .Include(m => m.DosageForm)
                .Where(m => medicationIds.Contains(m.MedicationId))
                .ToListAsync();

            var viewModel = new
            {
                data = medications.Select(m => new
                {
                    medicationId = m.MedicationId,

                    medication = new
                    {
                        name = m.Name,
                        dosageForm = new
                        {
                            name = m.DosageForm.Name
                        }
                    },
       
                    instructions = prescription.MedicationPrescriptions.FirstOrDefault(mp => mp.MedicationId == m.MedicationId)?.Instructions
                })
            };

            return Json(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAdministeredMedication(int id)
        {
            var administeredMedications = await _db.AdministeredMedications
                                                   .Where(am => am.PrescriptionId == id)
                                                   .Select(am => new
                                                   {
                                                       MedicationName = am.Medication.Name,
                                                       DosageForm = am.Medication.DosageForm.Name,
                                                       ActiveIngredients = string.Join(", ", am.Medication.MedicationIngredients.Select(mi => mi.ActiveIngredient.Name)),
                                                       Strength = string.Join(", ", am.Medication.MedicationIngredients.Select(mi => mi.ActiveIngredientStrength)),
                                                       Quantity = am.Quantity,
                                                       Time = am.Time.ToString("yyyy-MM-dd HH:mm")
                                                   })
                                                   .ToListAsync();

            return Json(new { data = administeredMedications });
        }





        public async Task<IActionResult> ListAdministeredMedication(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = await _db.Nurses.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (nurse == null)
            {
                return NotFound("Nurse not found");
            }
            var orders = await _db.Prescriptions
                                 .Include(o => o.AdministeredMedications)
                                     .ThenInclude(am => am.Medication)

                                         .ThenInclude(m => m.DosageForm) 

                                 .Include(o => o.Patient)

                                 .Where(o => o.NurseId == nurse.NurseId && o.PrescriptionId == id)
                                 .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateReport(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = await _db.Nurses.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (nurse == null)
            {
                return NotFound("Nurse not found");
            }

            var prescriptionQuery = _db.Prescriptions
                     .Include(o => o.AdministeredMedications)
                     .ThenInclude(am => am.Medication)
                     .ThenInclude(m => m.DosageForm)
                     .Include(o => o.Patient)
                     .Where(o => o.NurseId == nurse.NurseId);

            if (startDate.HasValue)
            {
                prescriptionQuery = prescriptionQuery
                    .Where(o => o.AdministeredMedications.Any(am => am.Time >= startDate.Value));
            }
            if (endDate.HasValue)
            {
                prescriptionQuery = prescriptionQuery
                    .Where(o => o.AdministeredMedications.Any(am => am.Time <= endDate.Value));
            }

            var prescriptions = await prescriptionQuery.ToListAsync();

            var totalUniquePatients = prescriptions.Select(o => o.Patient?.PatientId).Distinct().Count();

            using (var pdfStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc);

                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

                Paragraph titleParagraph = new Paragraph("MEDICATION REPORT")
                    .SetFont(titleFont)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(titleParagraph);

                Paragraph nurseParagraph = new Paragraph($"{nurse.FullName}")
                    .SetFont(normalFont)
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(nurseParagraph);

                doc.Add(new Paragraph(" "));

                Table headerTable = new Table(2);
                headerTable.SetWidth(UnitValue.CreatePercentValue(100));

                headerTable.AddCell(new Cell().Add(new Paragraph($"Date Range: {startDate?.ToString("dd MMMM yyyy")} – {endDate?.ToString("dd MMMM yyyy")}")
                    .SetFont(normalFont)).SetBorder(Border.NO_BORDER));

                headerTable.AddCell(new Cell().Add(new Paragraph($"Report Generated: {DateTime.Now.ToString("dd MMMM yyyy")}")
                    .SetFont(normalFont))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorder(Border.NO_BORDER));

                doc.Add(headerTable);
                doc.Add(new Paragraph(" "));

                Table table = new Table(new float[] { 25f, 40f, 20f, 10f, 5f });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                table.AddHeaderCell(new Cell().Add(new Paragraph("Date").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Patient").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Medication").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Time").SetFont(boldFont)));

                foreach (var prescription in prescriptions)
                {
                    var filteredMedications = prescription.AdministeredMedications
                        .Where(am => (!startDate.HasValue || am.Time >= startDate.Value)
                                  && (!endDate.HasValue || am.Time <= endDate.Value))
                        .ToList();

                    foreach (var administeredMedication in filteredMedications)
                    {
                        var medicationName = administeredMedication.Medication.Name;
                        var quantity = administeredMedication.Quantity.ToString();

                        table.AddCell(new Cell().Add(new Paragraph(administeredMedication.Time.ToString("dd MMM yyyy")).SetFont(normalFont)));
                        table.AddCell(new Cell().Add(new Paragraph(prescription.Patient?.FullName ?? "N/A").SetFont(normalFont)));
                        table.AddCell(new Cell().Add(new Paragraph(medicationName).SetFont(normalFont)));
                        table.AddCell(new Cell().Add(new Paragraph(quantity).SetFont(normalFont)));
                        table.AddCell(new Cell().Add(new Paragraph(administeredMedication.Time.ToString("HH:mm")).SetFont(normalFont)));
                    }
                }

                table.AddCell(new Cell(1, 5).Add(new Paragraph($"Total Patients: {totalUniquePatients}").SetFont(boldFont)).SetTextAlignment(TextAlignment.LEFT));

                doc.Add(table);

                doc.Add(new Paragraph("SUMMARY PER MEDICINE:").SetFont(boldFont));
                doc.Add(new Paragraph(" "));

                Table summaryTable = new Table(new float[] { 20f, 30f });
                summaryTable.SetWidth(UnitValue.CreatePercentValue(50));

                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Medicine").SetFont(boldFont)));
                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("QTY Administered").SetFont(boldFont)));

                var medicineSummary = prescriptions.SelectMany(o => o.AdministeredMedications)
                    .Where(am => (!startDate.HasValue || am.Time >= startDate.Value)
                              && (!endDate.HasValue || am.Time <= endDate.Value))
                    .GroupBy(mo => mo.Medication.Name)
                    .Select(group => new { Medication = group.Key, TotalQty = group.Sum(mo => mo.Quantity) })
                    .ToList();

                foreach (var med in medicineSummary)
                {
                    summaryTable.AddCell(new Cell().Add(new Paragraph(med.Medication).SetFont(normalFont)));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(med.TotalQty.ToString()).SetFont(normalFont)));
                }

                doc.Add(summaryTable);
                doc.Close();

                using (var finalPdfStream = new MemoryStream())
                {
                    PdfDocument pdfWithPageNumbers = new PdfDocument(new PdfReader(new MemoryStream(pdfStream.ToArray())), new PdfWriter(finalPdfStream));
                    Document docWithPages = new Document(pdfWithPageNumbers);
                    PdfFont normalFonts = PdfFontFactory.CreateFont(StandardFonts.COURIER);

                    int totalPages = pdfWithPageNumbers.GetNumberOfPages();

                    for (int i = 1; i <= totalPages; i++)
                    {
                        docWithPages.ShowTextAligned(
                            new Paragraph($"Page {i} of {totalPages}").SetFont(normalFonts),
                            559f, 20f, i, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
                    }

                    docWithPages.Close();

                    return File(finalPdfStream.ToArray(), "application/pdf", "MedicationReport.pdf");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListAllAdministeredMedication(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = await _db.Nurses.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (nurse == null)
            {
                return NotFound("Nurse not found");
            }

            var prescriptionQuery = _db.Prescriptions
                .Include(o => o.AdministeredMedications)
                    .ThenInclude(am => am.Medication)
                        .ThenInclude(m => m.DosageForm)
                .Include(o => o.Patient)
                .Where(o => o.NurseId == nurse.NurseId);

            if (startDate.HasValue)
            {
                prescriptionQuery = prescriptionQuery
                    .Where(o => o.AdministeredMedications.Any(am => am.Time >= startDate.Value));
            }
            if (endDate.HasValue)
            {
                prescriptionQuery = prescriptionQuery
                    .Where(o => o.AdministeredMedications.Any(am => am.Time <= endDate.Value));
            }

            var prescriptions = await prescriptionQuery.ToListAsync();

            return View(prescriptions);
        }

        [HttpGet]
        public IActionResult GetListAllAdministeredMedication(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = _db.Nurses.SingleOrDefault(c => c.UserId.ToString() == userId);

            var prescriptionQuery = _db.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.NurseId == nurse.NurseId)
                .AsQueryable();

            var administeredMedications = _db.AdministeredMedications
                .Include(am => am.Prescription)
                .ThenInclude(p => p.Patient)
                .Include(am => am.Medication)
                .ThenInclude(m => m.DosageForm)
                .Where(am => prescriptionQuery.Any(p => p.PrescriptionId == am.PrescriptionId))
                .Where(am => (!startDate.HasValue || am.Time >= startDate.Value)
                          && (!endDate.HasValue || am.Time <= endDate.Value))
                .Select(am => new
                {
                    date = am.Time.ToString("yyyy-MM-dd"),
                    prescription = new
                    {
                        patient = new
                        {
                            fullName = am.Prescription.Patient.FullName
                        }
                    },
                    medication = new
                    {
                        name = am.Medication.Name,
                        dosageForm = new
                        {
                            name = am.Medication.DosageForm.Name
                        }
                    },
                    quantity = am.Quantity,
                    timeAdministered = am.Time.ToString("HH:mm")
                })
                .ToList();

            return Json(new { data = administeredMedications });
        }







        public async Task<IActionResult> ListPatientAdministeredMedication(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurse = await _db.Nurses.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (nurse == null)
            {
                return NotFound("Pharmacist not found");
            }

            var orders = await _db.Prescriptions
                             .Include(o => o.AdministeredMedications)
                             .ThenInclude(po => po.Medication)
                             .Where(o => o.NurseId == nurse.NurseId)
                             .Where(p => p.PatientId == id)
                             .ToListAsync();
   
            ViewBag.PatientId = id;

            return View(orders);
        }



    }
}


