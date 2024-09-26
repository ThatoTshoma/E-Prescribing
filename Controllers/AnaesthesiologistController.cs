using E_Prescribing.CollectionModel;
using E_Prescribing.Data;
using E_Prescribing.Data.Services;
using E_Prescribing.Models;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using System.Security.Claims;






namespace E_Prescribing.Controllers
{
    public class AnaesthesiologistController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IOrdersService _ordersService;


        public AnaesthesiologistController(ApplicationDbContext db, IOrdersService ordersService)
        {
            _db = db;
            _ordersService = ordersService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult AddOrUpdateVital(int? vitalId)
        {
            if (vitalId == null)
            {
                return View(new Vital());
            }
            var vital = _db.Vitals.Find(vitalId);
            if (vital == null)
            {
                return NotFound();
            }
            return View(vital);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdateVital(Vital model)
        {
            if (_db.Vitals.Any(a => a.Name == model.Name && a.VitalId != model.VitalId))
            {
                ModelState.AddModelError("Name", "Vital name already exists");
            }
            if (ModelState.IsValid)
            {
                if (model.VitalId == 0)
                {
                    _db.Vitals.Add(model);
                    TempData["success"] = "Vital added successfully";
                }
                else
                {
                    _db.Vitals.Update(model);
                    TempData["success"] = "Vital updated successfully";
                }
                _db.SaveChanges();

                return RedirectToAction("ListVitalRange");
            }
            return View(model);
        }
        public IActionResult ListVitalRange()
        {
            var vital = _db.Vitals.ToList();
            return View(vital);
        }
        [HttpGet]
        public IActionResult GetVitalRange()
        {
            var vital = _db.Vitals.ToList();
            return Json(new { data = vital });
        }
        [HttpDelete]
        public IActionResult DeleteVitalRange(int id)
        {
            var vital = _db.Vitals.FirstOrDefault(m => m.VitalId == id);
            if (vital == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Vitals.Remove(vital);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        public async Task<IActionResult> ListVitaTo(int id)
        {
            var patient = await _db.Patients
                .Include(o => o.Suburb)
                .FirstOrDefaultAsync(o => o.PatientId == id);


            if (patient == null)
            {
                return NotFound();
            }

            var vitals = await _db.Vitals
                .ToListAsync();

            var viewModel = new VitalViewModel
            {
                Patient = patient,
                Vitals = vitals
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetVitaTo(int id)
        {
            var patient = await _db.Patients
                .Include(o => o.Suburb)
                .FirstOrDefaultAsync(o => o.PatientId == id);


            if (patient == null)
            {
                return NotFound();
            }

            var vitals = await _db.Vitals
                .ToListAsync();


            return Json(new { data = vitals });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessNote(int orderId, Dictionary<int, string> notes)
        {
            var order = await _db.Orders
                .Include(o => o.MedicationOrders)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var note in notes)
            {
                var medicationOrder = await _db.MedicationOrders
                    .FirstOrDefaultAsync(mo => mo.MedicationId == note.Key && mo.OrderId == order.OrderId);

                if (medicationOrder != null)
                {
                    medicationOrder.Note = note.Value;
                    _db.MedicationOrders.Update(medicationOrder);
                    TempData["success"] = "Note added successfully";

                }
            }

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Notes have been successfully saved.";
            return RedirectToAction("ListNotedication", new { id = order.OrderId });
        }


        public async Task<IActionResult> ListMedicationToANote(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Patient)
                .FirstOrDefaultAsync(o => o.OrderId == id);


            if (order == null)
            {
                return NotFound();
            }

            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                .Include(m => m.Medication.DosageForm)
                .Include(m => m.ActiveIngredient)
                .ToListAsync();

            var viewModel = new MedicationNoteViewModel
            {
                Order = order,
                Patient = order.Patient,
                Medications = medications
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicationToNote(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Patient)
                .Include(o => o.MedicationOrders)
                .ThenInclude(mo => mo.Medication)
                .ThenInclude(m => m.DosageForm)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var medicationIds = order.MedicationOrders.Select(mo => mo.MedicationId).ToList();

            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                .Include(m => m.Medication.DosageForm)
                .Include(m => m.ActiveIngredient)
                .Where(m => medicationIds.Contains(m.MedicationId))
                .ToListAsync();

            var viewModel = new
            {
                data = medications.Select(m => new
                {
                    medicationId = m.MedicationId,
                    medication = new
                    {
                        name = m.Medication.Name,
                        dosageForm = new
                        {
                            name = m.Medication.DosageForm.Name
                        }
                    },
                    activeIngredient = new
                    {
                        name = m.ActiveIngredient.Name
                    },
                    activeIngredientStrength = m.ActiveIngredientStrength
                })
            };

            return Json(viewModel);
        }

        public async Task<IActionResult> ListNotedication(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologist = await _db.Anaesthesiologists.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (anaesthesiologist == null)
            {
                return NotFound("Nurse not found");
            }
            var orders = await _db.Orders
                                 .Include(o => o.MedicationOrders)
                                     .ThenInclude(am => am.Medication)

                                         .ThenInclude(m => m.DosageForm)

                                 .Include(o => o.Patient)

                                 .Where(o => o.AnaesthesiologistId == anaesthesiologist.AnaesthesiologistId && o.OrderId == id)
                                 .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> ListMedication(int id)
        {
            ViewBag.PatientId = id;

            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                                .Include(m => m.Medication.DosageForm)

                .Include(m => m.ActiveIngredient)
                .ToListAsync();

            return View(medications);
        }

        //public IActionResult PendingBooking()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);
        //    var date = DateTime.Now.Date;
        //    var booking = _db.Bookings.Include(c => c.Anaesthesiologist).Include(c => c.Patient).Include(c => c.Surgeon).Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId).Where(c => c.Status == false).ToList();
        //    return View(booking);
        //}
        //[HttpGet]
        //public IActionResult GetPendingBooking()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);
        //    var date = DateTime.Now.Date;
        //    var booking = _db.Bookings.Include(c => c.Anaesthesiologist).Include(c => c.Patient).Include(c => c.Surgeon).Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId).Where(c => c.Status == false).ToList();
        //    return Json(new { data = booking });
        //}

        public IActionResult PendingBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);

            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.Surgeon)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId  && b.Status == false)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetPendingBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Surgeon)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId  && b.Status == false)
                .ToList();

            var result = bookings.Select(b => new {
                b.BookingId,
                Surgeon = b.Surgeon.FullName, 
                b.Patient,
                Theatre = b.Theatre.Name,
                Ward = b.Theatre.Ward.WardNumber,
                Date = b.Date.ToString("dd-MM-yyy"),
                b.Status,
                b.Session,
                Treatments = b.PatientTreatments.Select(pt => pt.Treatment.Code).ToList()
            });

            return Json(new { data = result });
        }
        public IActionResult ApprovedBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);

            var bookings = _db.Bookings
                .Include(b => b.Patient)
                 .Include(b => b.Surgeon)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId && b.Status == true)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetApprovedBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                                .Include(b => b.Surgeon)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId && b.Status == true)
                .ToList();

            var result = bookings.Select(b => new {
                b.BookingId,
                Surgeon = b.Surgeon.FullName,
                b.Anaesthesiologist.FullName,
                b.Patient,
                Theatre = b.Theatre.Name,
                Ward = b.Theatre.Ward.WardNumber,
                Date = b.Date.ToString("dd-MM-yyy"),
                b.Status,
                b.Session,
                Treatments = b.PatientTreatments.Select(pt => pt.Treatment.Code).ToList()
            });

            return Json(new { data = result });
        }

        public IActionResult ListBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId && b.PatientId == id)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId && b.PatientId == id)
                .ToList();

            var result = bookings.Select(b => new {
                b.BookingId,
                b.Surgeon,
                Theatre = b.Theatre.Name,
                Ward = b.Theatre.Ward.WardNumber,
                Date = b.Date.ToString("dd-MM-yyy"),
                b.Status,
                b.Session,
                Treatments = b.PatientTreatments.Select(pt => pt.Treatment.Code).ToList()
            });

            return Json(new { data = result });
        }
        //public IActionResult ApprovedBooking()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
        //    var date = DateTime.Now.Date;
        //    var booking = _db.Bookings.Include(c => c.Anaesthesiologist).Include(c => c.Patient).Include(c => c.Surgeon).Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId).Where(c => c.Status == true).ToList();
        //    return View(booking);
        //}
        public IActionResult DetailOfBooking(int id)
        {
            var appointment = _db.Bookings.Include(c => c.Anaesthesiologist).Include(c => c.Patient).Single(c => c.BookingId == id);
            return View(appointment);
        }
        public IActionResult EditBooking(int bookingId)
        {
            var collection = new BookingCollection
            {
                Booking = _db.Bookings.Single(c => c.BookingId == bookingId),
                Patients = _db.Patients.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBooking(int bookingId, BookingCollection model)
        {
            var collection = new BookingCollection
            {
                Booking = model.Booking,
                Patients = _db.Patients.ToList()
            };
            if (model.Booking.Date >= DateTime.Now.Date || model.Booking.Date <= DateTime.Now.Date)
            {
                var appointment = _db.Bookings.Single(c => c.BookingId == bookingId);
               // appointment.PatientId = model.Booking.PatientId;
                appointment.Date = model.Booking.Date;
                appointment.Status = model.Booking.Status;
                _db.SaveChanges();
                if (model.Booking.Status == true)
                {
                    return RedirectToAction("ApprovedBooking");
                }
                else
                {
                    return RedirectToAction("ApprovedBooking");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(collection);
        }
        public IActionResult ListPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
            var patients = _db.Bookings
                                      .Where(b => b.Status == true && b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId)
                                      .Include(b => b.Patient)
                                      .ThenInclude(p => p.Suburb)
                                      .Select(b => b.Patient)
                                      .Distinct()
                                      .ToList();

            return View(patients);
        }
        [HttpGet]
        public IActionResult GetPatient()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
            var patients = _db.Bookings
                                      .Where(b => b.Status == true && b.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId)
                                      .Include(b => b.Patient)
                                      .ThenInclude(p => p.Suburb)
                                      .Select(b => b.Patient)
                                      .Distinct()
                                      .ToList();
            return Json(new { data = patients });
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
                    .Include(p => p.PatientVitals)
                    .ThenInclude(p => p.Vital)
                    .Include(p =>p.PatientAllergies)
                    .ThenInclude(p => p.ActiveIngredient)
                    .Include(p =>p.VitalsRanges)
                         .Include(p => p.PatientMedications)
                    .ThenInclude(p => p.Medication)
                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }
            ViewBag.PatientId = id;
            return View(patient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(Dictionary<int, int> quantities, int patientId, string isUrgent, string reasonForIgnoring = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologist = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);

            var patientAllergies = _db.PatientAllergies
                                     .Where(a => a.PatientId == patientId)
                                     .Select(a => a.ActiveIngredientId)
                                     .ToList();

            var patientConditions = _db.PatientConditions
                                       .Where(pc => pc.PatientId == patientId)
                                       .Select(pc => pc.ConditionId)
                                       .ToList();

            var conflictingAllergens = new List<string>();
            var contraindications = new List<string>();
            var interactionWarnings = new List<string>();

            var activeIngredientsInOrder = new List<int>();

            if (quantities != null && quantities.Any())
            {
                foreach (var medicationId in quantities.Keys)
                {
                    var quantity = quantities[medicationId];
                    if (quantity > 0)
                    {
                        var activeIngredients = _db.MedicationIngredients
                                                   .Where(mi => mi.MedicationId == medicationId)
                                                   .Select(mi => new { mi.ActiveIngredientId, mi.ActiveIngredient.Name })
                                                   .ToList();

                        activeIngredientsInOrder.AddRange(activeIngredients.Select(ai => ai.ActiveIngredientId));

                        var medicationAllergenConflicts = activeIngredients
                            .Where(ai => patientAllergies.Contains(ai.ActiveIngredientId))
                            .Select(ai => ai.Name)
                            .ToList();

                        if (medicationAllergenConflicts.Any())
                        {
                            conflictingAllergens.AddRange(medicationAllergenConflicts);
                        }

                        var medicationContraindications = _db.ContraIndications
                            .Where(ci => activeIngredients.Select(ai => ai.ActiveIngredientId).Contains(ci.ActiveIngredientId)
                                && patientConditions.Contains(ci.ConditionDiagnosisId))
                            .Select(ci => ci.ActiveIngredient.Name)
                            .ToList();

                        if (medicationContraindications.Any())
                        {
                            contraindications.AddRange(medicationContraindications);
                        }
                    }
                }

                foreach (var ingredientId1 in activeIngredientsInOrder)
                {
                    foreach (var ingredientId2 in activeIngredientsInOrder)
                    {
                        if (ingredientId1 != ingredientId2)
                        {
                            var interaction = _db.MedicationInteractions
                                .FirstOrDefault(mi =>
                                    (mi.ActiveIngredient1Id == ingredientId1 && mi.ActiveIngredient2Id == ingredientId2) ||
                                    (mi.ActiveIngredient1Id == ingredientId2 && mi.ActiveIngredient2Id == ingredientId1));

                            if (interaction != null)
                            {
                                interactionWarnings.Add($"{interaction.Description}");
                            }
                        }
                    }
                }
            }

            if (conflictingAllergens.Any() && string.IsNullOrEmpty(reasonForIgnoring))
            {
                return Json(new
                {
                    errorMessage = $"The medications contain Active Ingredients that the patient is allergic to: {string.Join(", ", conflictingAllergens.Distinct())}"
                });
            }

            if (contraindications.Any() && string.IsNullOrEmpty(reasonForIgnoring))
            {
                return Json(new
                {
                    errorMessage = $"The Ordered medications have Contraindications for the patient’s diagnosed conditions: {string.Join(", ", contraindications.Distinct())}"
                });
            }

            if (interactionWarnings.Any() && string.IsNullOrEmpty(reasonForIgnoring))
            {
                return Json(new
                {
                    errorMessage = $"There are Medication Interactions: {string.Join("; ", interactionWarnings.Distinct())}"
                });
            }

            var order = new Order()
            {
                AnaesthesiologistId = anaesthesiologist.AnaesthesiologistId,
                Date = DateTime.Now,
                IsUrgent = isUrgent,
                Status = "Ordered",
                PatientId = patientId,
                IgnoreReason = reasonForIgnoring
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            foreach (var medicationId in quantities.Keys)
            {
                var quantity = quantities[medicationId];
                if (quantity > 0)
                {
                    var medicationOrder = new MedicationOrder
                    {
                        MedicationId = medicationId,
                        OrderId = order.OrderId,
                        Quantity = quantity
                    };

                    _db.Add(medicationOrder);
                }
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Order added successfully";
            return RedirectToAction("ListPatientOrder", new { id = patientId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveOrder(int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null && order.Status == "Dispensed")
            {
                order.Status = "Received";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("ListOrder");
        }
        [HttpGet]
        public async Task<IActionResult> GenerateReport(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologist = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);

            var ordersQuery = _db.Orders
                .Include(o => o.MedicationOrders)
                .ThenInclude(mo => mo.Medication)
                .Include(o => o.Patient)
                .Where(o => o.AnaesthesiologistId == anaesthesiologist.AnaesthesiologistId);

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.Date >= startDate.Value);
            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.Date <= endDate.Value);

            var orders = await ordersQuery.ToListAsync();

            var totalUniquePatients = orders.Select(o => o.Patient?.PatientId).Distinct().Count();

            using (var pdfStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc);

                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

                Paragraph titleParagraph = new Paragraph("ANESTHETIC REPORT")
                    .SetFont(titleFont)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(titleParagraph);

                Paragraph anaesthesiologistParagraph = new Paragraph($"Dr {anaesthesiologist.FullName}")
                    .SetFont(normalFont)
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(anaesthesiologistParagraph);

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

                Table table = new Table(new float[] { 25f, 40f, 20f, 15f });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                table.AddHeaderCell(new Cell().Add(new Paragraph("Date").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Patient").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Medication").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFont(boldFont)));

                foreach (var order in orders)
                {
                    var medications = string.Join("\n", order.MedicationOrders.Select(mo => mo.Medication.Name));
                    var quantities = string.Join("\n", order.MedicationOrders.Select(mo => mo.Quantity.ToString()));

                    table.AddCell(new Cell().Add(new Paragraph(order.Date.ToString("dd MMM yyyy")).SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(order.Patient?.FullName ?? "N/A").SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(medications).SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(quantities).SetFont(normalFont)));
                }

                table.AddCell(new Cell(1, 4).Add(new Paragraph($"Total Patients: {totalUniquePatients}").SetFont(boldFont)).SetTextAlignment(TextAlignment.LEFT));


                doc.Add(table);

                doc.Add(new Paragraph(" ")); 

                doc.Add(new Paragraph("SUMMARY PER MEDICINE:").SetFont(boldFont));
                doc.Add(new Paragraph(" ")); 

                Table summaryTable = new Table(new float[] { 20f, 30f });
                summaryTable.SetWidth(UnitValue.CreatePercentValue(50));

                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Medicine").SetFont(boldFont)));
                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("QTY Ordered").SetFont(boldFont)));

                var medicineSummary = orders.SelectMany(o => o.MedicationOrders)
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

                    return File(finalPdfStream.ToArray(), "application/pdf", "AnestheticReport.pdf");
                }
            }
        }



        public async Task<IActionResult> ListOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
            var orders = await _db.Orders
                .Include(n => n.MedicationOrders)
                .ThenInclude(n => n.Medication)
                .Include(n => n.Anaesthesiologist)
                .Include(n => n.Patient)
                .Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId)
                .ToListAsync();
            ViewBag.PatientId = id;

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrder(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);

            var ordersQuery = _db.Orders
                .Include(o => o.MedicationOrders)
                .ThenInclude(mo => mo.Medication)
                .Include(o => o.Anaesthesiologist)
                .Include(o => o.Patient)
                .Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId);

            // Apply date filtering if provided
            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Date <= endDate.Value);
            }

            var orders = await ordersQuery.ToListAsync();

            var result = orders.Select(o => new
            {
                o.OrderId,
                o.Date,
                Patient = o.Patient != null ? o.Patient.FullName : "N/A",
                Anaesthesiologist = o.Anaesthesiologist.FullName,
                MedicationOrders = o.MedicationOrders.Select(mo => new
                {
                    mo.Medication.Name,
                    mo.Quantity
                }),
                o.Status,
                o.IsUrgent
            });

            return Json(new { data = result });
        }

        public async Task<IActionResult> ListMedicationToOrder(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.Suburb)
                .SingleOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                .Include(m => m.Medication.DosageForm)
                .Include(m => m.ActiveIngredient)
                .ToListAsync();

            var viewModel = new MedicationOrderViewModel
            {
                Patient = patient,
                Medications = medications
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetMedicationToOrder(int id)
        {
            var patient = await _db.Patients
             .Include(p => p.Suburb)
             .SingleOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }
            var medications = await _db.Medications
                .Include(m => m.MedicationIngredients)
                .ThenInclude(mi => mi.ActiveIngredient)
                .Include(m => m.DosageForm)
                .ToListAsync();

            var viewModel = new
            {
                data = medications.Select(m => new
                {
                    medicationId = m.MedicationId,
                    name = m.Name,
                    schedule = m.Schedule,
                    dosageForm = new
                    {
                        name = m.DosageForm.Name
                    },
                    activeIngredients = m.MedicationIngredients.Select(mi => new
                    {
                        name = mi.ActiveIngredient.Name,
                        strength = mi.ActiveIngredientStrength
                    }).ToArray()
                })
            };

            return Json(viewModel);
        }


        public IActionResult AddPatientVital(int id)
        {
            ViewBag.VitalList = new SelectList(_db.Vitals, "VitalId", "Name");

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
                    var existingVital = _db.PatientVitals
                                           .FirstOrDefault(pv => pv.PatientId == patient.PatientId && pv.VitalId == selectedVitalId);

                    if (existingVital == null)
                    {
                        var patientVital = new PatientVital
                        {
                            VitalId = selectedVitalId,
                            PatientId = patient.PatientId
                        };

                        _db.PatientVitals.Add(patientVital);
                    }
                }

                _db.SaveChanges();
                return RedirectToAction("ListPatientVital", new { id = patient.PatientId });


                //return RedirectToAction("ListPatientVital");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient not found.");
            }

            ViewBag.VitalList = new SelectList(_db.Vitals, "VitalId", "Name");
            model.Patients = _db.Patients.ToList();
            return View(model);
        }

        public IActionResult ListPatientVital(int id)
        {
            var patientVital = _db.PatientVitals.Include(pb => pb.Vital)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientVital);
        }
        [HttpGet]
        public IActionResult GetVital(int id)
        {
            var patientVitals = _db.PatientVitals.Include(pb => pb.Vital)
                                                 .Include(pb => pb.Patient)
                                                 .Where(pb => pb.PatientId == id)
                                                 .Select(pb => new
                                                 {
                                                     pb.Vital.Name
                                                 })
                                                 .ToList();
            ViewBag.PatientId = id;
            return Json(new { data = patientVitals });
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
        public IActionResult ListPatientMedication(int id)
        {
            var patientMedication = _db.PatientMedications.Include(pb => pb.Medication)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientMedication);
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
        public IActionResult ListPatientPrescription(int id)
        {
            var prescription = _db.Prescriptions
                              .Include(p => p.MedicationPrescriptions)
                              .ThenInclude(P => P.Medication)
                              .Include(p => p.Patient)
                              .Include(p => p.Surgeon)
                              .Include(p => p.Pharmacist)
                              .Where(p => p.PatientId == id)
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
        //public IActionResult ListVital(int id)
        //{
        //    var patientVital = _db.VitalRanges.Include(pb => pb.Patient)
        //                                      .Where(pb => pb.PatientId == id)
        //                                      .ToList();
        //    ViewBag.PatientId = id;
        //    return View(patientVital);
        //}

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

        public IActionResult ListVital(int id)
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
        public async Task<IActionResult> AddOrderNote(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }



            return View(order);
        }

        public async Task<IActionResult> ListPatientOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
            var orders = await _db.Orders
                .Include(n => n.MedicationOrders)
                .ThenInclude(n => n.Medication)
                .Include(n => n.Anaesthesiologist)
                .Include(n => n.Patient)
                .Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId && c.PatientId == id)
                .ToListAsync();

            ViewBag.PatientId = id;
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
            var orders = await _db.Orders
                                  .Include(o => o.MedicationOrders)
                                  .ThenInclude(mo => mo.Medication)
                                  .Include(o => o.Anaesthesiologist)
                                  .Include(o => o.Patient)
                                  .Where(c => c.AnaesthesiologistId == anaesthesiologists.AnaesthesiologistId && c.PatientId == id)
                                  .ToListAsync();

            var result = orders.Select(o => new
            {
                o.OrderId,
                Patient = o.Patient != null ? o.Patient.FullName : "N/A",
                Anaesthesiologist = o.Anaesthesiologist.FullName,
                MedicationOrders = o.MedicationOrders.Select(mo => new
                {
                    mo.Medication.Name,
                    mo.Quantity
                }),
                o.Status,
                o.IsUrgent
            });

            return Json(new { data = result });
        }
    }
}
