using E_Prescribing.CollectionModel;
using E_Prescribing.Data;
using E_Prescribing.Data.Services;
using E_Prescribing.Models;
using E_Prescribing.Services;
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
using NuGet.Versioning;
using System.Security.Claims;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace E_Prescribing.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMedicationService _medicationService;
        private readonly IOrdersService _ordersService;
        private readonly Cart _cart;
        private readonly IMyEmailSender _emailSender;

        public PharmacistController(IMedicationService medicationService, Cart cart, ApplicationDbContext db, IOrdersService ordersService, IMyEmailSender myEmailSender)
        {
            _medicationService = medicationService;
            _cart = cart;
            _db = db;
            _ordersService = ordersService;
            _emailSender = myEmailSender;
        }

        public IActionResult Index()
        {
            return View();
        }
        //public IActionResult ListMedication()
        //{
        //    var medication = _db.Medications.Include(m => m.DosageForm).ToList();

        //    return View(medication);
        //}
        public async Task<IActionResult> Prescription(int id)
        {
            var prescription = await _db.Prescriptions
                        .Include(p => p.Patient)
                        .Include(p => p.Surgeon)
                        .Include(p => p.MedicationPrescriptions)
                        .ThenInclude(mp => mp.Medication)
                        .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);


        }
        public IActionResult ListPatient()
        {
            var patients = _db.Bookings
                              .Where(b => b.Status == true)
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
            var patients = _db.Bookings
                                      .Where(b => b.Status == true)
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
                    .Include(p => p.PatientAllergies)
                    .ThenInclude(p => p.ActiveIngredient)
                    .Include(p => p.VitalsRanges)
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
                    heartrate = v.HeartRate,
                    oxygensaturation = v.OxygenSaturation,
                    bloodglucoselevel = v.BloodGlucoseLevel,
                    bloodoxegenlevel = v.BloodOxegenLevel
                }).ToList(),
                vitalNames = selectedVitals.Select(v => v.Name.ToLower()).ToList()
            };



            return Json(result);
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
        public IActionResult ListPatientCondition(int id)
        {
            var patientCondition = _db.PatientConditions.Include(pb => pb.Condition)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientCondition);
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
                                .ThenInclude(P=>P.Medication)
                                .Include(p => p.Patient)
                                .Include(p => p.Surgeon)
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
                p.IgnoreReason,
                p.PrescriptionId
            });

            return Json(new { data = result });
        }
        public IActionResult DispensePrescription(int prescriptionId)
        {
            var prescription = _db.Prescriptions.FirstOrDefault(p => p.PrescriptionId == prescriptionId);
            if (prescription != null)
            {
                prescription.Status = "Thato";
                _db.SaveChanges();
            }
            return RedirectToAction("ListPatientPrescription");
        }
        [HttpGet]
        public async Task<IActionResult> GenerateReport(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.Single(c => c.UserId.ToString() == userId);

            var prescriptionQuery = _db.Prescriptions
                                  .Include(p => p.MedicationPrescriptions)
                                  .ThenInclude(mp => mp.Medication)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Nurse)
                                  .Include(p => p.Pharmacist)
                                  .Include(p => p.Surgeon)
                .Where(o => o.PharmacistId == pharmacist.PharmacistId);

            if (startDate.HasValue)
                prescriptionQuery = prescriptionQuery.Where(o => o.Date >= startDate.Value);
            if (endDate.HasValue)
                prescriptionQuery = prescriptionQuery.Where(o => o.Date <= endDate.Value);

            var prescriptions = await prescriptionQuery.ToListAsync();

            var totalScriptsDispensed = prescriptions.Where(p => p.Status == "Dispensed").Count();
            var totalScriptsRejected = prescriptions.Where(p => p.Status == "Rejected").Count();



            using (var pdfStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc);

                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

                Paragraph titleParagraph = new Paragraph("DISPENSARY REPORT")
                    .SetFont(titleFont)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(titleParagraph);

                Paragraph anaesthesiologistParagraph = new Paragraph($"{pharmacist.FullName}")
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

                Table table = new Table(new float[] { 20f, 20f,20f, 20f, 10f, 10f });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                table.AddHeaderCell(new Cell().Add(new Paragraph("Date").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Patient").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Script By").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Medication").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetFont(boldFont)));


                foreach (var order in prescriptionQuery)
                {
                    var medications = string.Join("\n", order.MedicationPrescriptions.Select(mo => mo.Medication.Name));
                    var quantities = string.Join("\n", order.MedicationPrescriptions.Select(mo => mo.Quantity.ToString()));

                    table.AddCell(new Cell().Add(new Paragraph(order.Date.ToString("dd MMM yyyy")).SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(order.Patient?.FullName ?? "N/A").SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(order.Surgeon?.FullName ?? "N/A").SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(medications).SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(quantities).SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(order.Status).SetFont(normalFont)));

                }

                table.AddCell(new Cell(1, 6).Add(new Paragraph($"Total Scripts Dispensed: {totalScriptsDispensed}").SetFont(boldFont)).SetTextAlignment(TextAlignment.LEFT));
                table.AddCell(new Cell(1, 6).Add(new Paragraph($"Total Scripts Rejected: {totalScriptsRejected}").SetFont(boldFont)).SetTextAlignment(TextAlignment.LEFT));



                doc.Add(table);

                doc.Add(new Paragraph(" ")); 

                doc.Add(new Paragraph("SUMMARY PER MEDICINE:").SetFont(boldFont));
                doc.Add(new Paragraph(" ")); 

                Table summaryTable = new Table(new float[] { 20f, 30f });
                summaryTable.SetWidth(UnitValue.CreatePercentValue(50));

                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Medicine").SetFont(boldFont)));
                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("QTY Dispensed").SetFont(boldFont)));
                var medicineSummary = prescriptions
                    .Where(p => p.Status == "Dispensed")
                    .SelectMany(o => o.MedicationPrescriptions)
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

                    return File(finalPdfStream.ToArray(), "application/pdf", "Dispensary.pdf");
                }
            }
        }


        public IActionResult ListPrescription()
        {
            var prescriptions = _db.Prescriptions
                                 .Include(p => p.MedicationPrescriptions)
                                 .ThenInclude(mp => mp.Medication)
                                 .Include(p => p.Patient)
                                 .Include(p => p.Nurse)
                                 .Include(p => p.Pharmacist)
                                 .Include(p => p.Surgeon)
                                 .ToList();
            return View(prescriptions);
        }
        [HttpGet]
        public IActionResult GetPrescription(DateTime? startDate, DateTime? endDate)
        {

            var prescriptions = _db.Prescriptions
                                  .Include(p => p.MedicationPrescriptions)
                                  .ThenInclude(mp => mp.Medication)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Nurse)
                                  .Include(p => p.Pharmacist)
                                  .Include(p => p.Surgeon)
                                  .Where(am => (!startDate.HasValue || am.Date >= startDate.Value)
                                    && (!endDate.HasValue || am.Date <= endDate.Value))
                                   .Select(p => new
                                   {
                                       Date = p.Date.ToString("dd-MM-yyyy"),
                                       Patient = p.Patient.FullName,
                                       Medications = string.Join("\n", p.MedicationPrescriptions.Select(mp => mp.Medication.Name)),
                                       Quantities = string.Join("\n", p.MedicationPrescriptions.Select(mp => mp.Quantity.ToString())),
                                       p.Status,
                                       p.Urgent,
                                       p.IgnoreReason,
                                       p.PrescriptionId
                                   });

            return Json(new { data = prescriptions });
        }
        public async Task<IActionResult> ViewPrescription(int id)
        {
            var prescription = await _db.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Surgeon)
                .Include(p => p.MedicationPrescriptions)
                .ThenInclude(mp => mp.Medication)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
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
        public async Task<IActionResult> DispenseMedication(int prescriptionId, string reasonForIgnoring = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _db.Pharmacists.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (pharmacist != null)
            {
                var prescription = await _db.Prescriptions
                                            .Include(p => p.MedicationPrescriptions)
                                            .ThenInclude(mp => mp.Medication)
                                            .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);

                if (prescription != null)
                {
                    var patientId = prescription.PatientId;

                    var patientAllergies = await _db.PatientAllergies
                                                    .Where(a => a.PatientId == patientId)
                                                    .Select(a => a.ActiveIngredientId)
                                                    .ToListAsync();

                    var patientConditions = await _db.PatientConditions
                                                     .Where(pc => pc.PatientId == patientId)
                                                     .Select(pc => pc.ConditionId)
                                                     .ToListAsync();

                    var conflictingAllergens = new List<string>();
                    var contraindications = new List<string>();
                    var interactionWarnings = new List<string>();
                    var activeIngredientsInPrescription = new List<int>();

                    foreach (var medicationPrescription in prescription.MedicationPrescriptions)
                    {


                        var medicationId = medicationPrescription.MedicationId;
                        var medication = _db.Medications
                                .Where(m => m.MedicationId == medicationId)
                                .Select(m => new { m.Name })
                                .FirstOrDefault();
                        var activeIngredients = await _db.MedicationIngredients
                                                         .Where(mi => mi.MedicationId == medicationId)
                                                         .Select(mi => new { mi.ActiveIngredientId, mi.ActiveIngredient.Name })
                                                         .ToListAsync();

                        activeIngredientsInPrescription.AddRange(activeIngredients.Select(ai => ai.ActiveIngredientId));

                        var medicationAllergenConflicts = activeIngredients
                            .Where(ai => patientAllergies.Contains(ai.ActiveIngredientId))
                            .Select(ai => $"{ai.Name} ({medication.Name})")
                            .ToList();

                        if (medicationAllergenConflicts.Any())
                        {
                            conflictingAllergens.AddRange(medicationAllergenConflicts);
                        }

                        var medicationContraindications = await _db.ContraIndications
                            .Where(ci => activeIngredients.Select(ai => ai.ActiveIngredientId).Contains(ci.ActiveIngredientId)
                                && patientConditions.Contains(ci.ConditionDiagnosisId))
                            .Select(ci => $"{ci.ActiveIngredient.Name} ({medication.Name})")
                            .ToListAsync();

                        if (medicationContraindications.Any())
                        {
                            contraindications.AddRange(medicationContraindications);
                        }
                    }
                    var currentPatientMedications = _db.PatientMedications
                            .Where(pm => pm.PatientId == patientId)
                            .SelectMany(pm => _db.MedicationIngredients
                                .Where(mi => mi.MedicationId == pm.MedicationId)
                                .Select(mi => mi.ActiveIngredientId))
                            .Distinct()
                            .ToList();
                    foreach (var currentIngredientId in currentPatientMedications)
                    {
                        foreach (var newIngredientId in activeIngredientsInPrescription)
                        {
                            if (currentIngredientId != newIngredientId)
                            {
                                var interaction = _db.MedicationInteractions
                                    .FirstOrDefault(mi =>
                                        (mi.ActiveIngredient1Id == currentIngredientId && mi.ActiveIngredient2Id == newIngredientId) ||
                                        (mi.ActiveIngredient1Id == newIngredientId && mi.ActiveIngredient2Id == currentIngredientId));

                                if (interaction != null)
                                {
                                    var currentMedicationName = _db.MedicationIngredients
                                        .Where(mi => mi.ActiveIngredientId == currentIngredientId)
                                        .Select(mi => mi.Medication.Name)
                                        .FirstOrDefault();

                                    var newMedicationName = _db.MedicationIngredients
                                        .Where(mi => mi.ActiveIngredientId == newIngredientId)
                                        .Select(mi => mi.Medication.Name)
                                        .FirstOrDefault();

                                    interactionWarnings.Add($"{interaction.Description} ({currentMedicationName} and {newMedicationName})");
                                }
                            }
                        }
                    }


                    var processedInteractions = new HashSet<(int, int)>();

                    foreach (var ingredientId1 in activeIngredientsInPrescription)
                    {
                        foreach (var ingredientId2 in activeIngredientsInPrescription)
                        {
                            if (ingredientId1 != ingredientId2)
                            {
                                var interactionKey = (Math.Min(ingredientId1, ingredientId2), Math.Max(ingredientId1, ingredientId2));

                                if (!processedInteractions.Contains(interactionKey))
                                {
                                    var interaction = _db.MedicationInteractions
                                        .FirstOrDefault(mi =>
                                            (mi.ActiveIngredient1Id == ingredientId1 && mi.ActiveIngredient2Id == ingredientId2) ||
                                            (mi.ActiveIngredient1Id == ingredientId2 && mi.ActiveIngredient2Id == ingredientId1));

                                    if (interaction != null)
                                    {
                                        var medication1 = _db.MedicationIngredients
                                            .Where(mi => mi.ActiveIngredientId == ingredientId1)
                                            .Select(mi => mi.Medication.Name)
                                            .FirstOrDefault();

                                        var medication2 = _db.MedicationIngredients
                                            .Where(mi => mi.ActiveIngredientId == ingredientId2)
                                            .Select(mi => mi.Medication.Name)
                                            .FirstOrDefault();

                                        interactionWarnings.Add($"{interaction.Description} ({medication1} and {medication2})");

                                        processedInteractions.Add(interactionKey);
                                    }
                                }
                            }
                        }
                    }
                    if (conflictingAllergens.Any() && string.IsNullOrEmpty(reasonForIgnoring))
                    {
                        return Json(new
                        {
                            errorMessage = $"The medications contain active ingredients that the patient is allergic to: {string.Join(", ", conflictingAllergens.Distinct())}"
                        });
                    }

                    if (contraindications.Any() && string.IsNullOrEmpty(reasonForIgnoring))
                    {
                        return Json(new
                        {
                            errorMessage = $"The medications have contraindications for the patient’s diagnosed conditions: {string.Join(", ", contraindications.Distinct())}"
                        });
                    }

                    if (interactionWarnings.Any() && string.IsNullOrEmpty(reasonForIgnoring))
                    {
                        return Json(new
                        {
                            errorMessage = $"There are medication interactions: {string.Join("; ", interactionWarnings.Distinct())}"
                        });
                    }

                    prescription.PharmacistId = pharmacist.PharmacistId;
                    prescription.Status = "Dispensed";
                    prescription.IgnoreReason = reasonForIgnoring;


                    foreach (var medicationPrescription in prescription.MedicationPrescriptions)
                    {
                        var prescribedMedication = new PrescribedMedication
                        {
                            PrescriptionId = prescription.PrescriptionId,
                            MedicationId = medicationPrescription.MedicationId,
                            Quantity = medicationPrescription.Quantity
                        };

                        _db.PrescribedMedications.Add(prescribedMedication);

                        var medication = medicationPrescription.Medication;
                        if (medication != null)
                        {
                            medication.StockOnHand -= medicationPrescription.Quantity;
                        }
                    }

                    await _db.SaveChangesAsync();
                    TempData["success"] = "Medication dispensed successfully";
                    return RedirectToAction("ListDispancedMedication");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No prescriptions available for dispensation.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Pharmacist not found.");
            }

            return View("Index", "Home");
        }


        public async Task<IActionResult> ListDispancedMedication1()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var medications = await _db.PrescribedMedications
                .Include(m => m.Medication)
                                .Include(m => m.Medication.DosageForm)

                .Include(m => m.Prescription)
                                .Include(m => m.Prescription.Patient)

                .Where(c => c.Prescription.PharmacistId == pharmacist.PharmacistId).Where(c => c.Prescription.Status == "Dispensed")
                .ToListAsync();

            return View(medications);
        }

        public IActionResult ListDispancedMedication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var prescription = _db.Prescriptions
                           .Include(p => p.PrescribedMedications)
                           .ThenInclude(P => P.Medication)
                           .Include(p => p.Patient)
                           .Include(p => p.Surgeon)
                           .Where(c => c.PharmacistId == pharmacist.PharmacistId).Where(c => c.Status == "Dispensed")

                           .ToList();
            return View(prescription);
        }
        [HttpGet]
        public IActionResult GetDispancedPrescription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (pharmacist == null)
            {
                return Json(new { data = new List<object>() });
            }

            var prescriptions = _db.Prescriptions
                                  .Include(p => p.PrescribedMedications)
                                  .ThenInclude(mp => mp.Medication)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Surgeon)
                                  .Where(p => p.PharmacistId == pharmacist.PharmacistId && p.Status == "Dispensed")
                                  .ToList();

            var result = prescriptions.Select(p => new
            {
                Date = p.Date.ToString("dd-MM-yyyy"),
                Patient = p.Patient?.FullName,
                Medications = string.Join("\n", p.PrescribedMedications.Select(mp => mp.Medication?.Name)),
                Quantities = string.Join("\n", p.PrescribedMedications.Select(mp => mp.Quantity.ToString())),
                p.Status,
                p.Urgent,
                p.PrescriptionId
            });

            return Json(new { data = result });
        }





        public IActionResult AddMedication()
        {

            ViewBag.DosageFormList = new SelectList(_db.DosageForms.OrderBy(d =>d.Name), "DosageId", "Name");

            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedication(MedicationCollection model)
        {
            ViewBag.DosageFormList = new SelectList(_db.DosageForms.OrderBy(d => d.Name), "DosageId", "Name");
            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");

            if (model.MedicationIngredient.SelectedIngredient == null || !model.MedicationIngredient.SelectedIngredient.Any())
            {
                TempData["error"] = $"Active Ingredient for {model.Medication.Name} is required";

                return View(model);
            }

            _db.Add(model.Medication);
            TempData["success"] = "Medication added successfully";
            await _db.SaveChangesAsync();

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

            return RedirectToAction("ListMedication");
        }

        public async Task<IActionResult> UpdateMedication(int medicationId)
        {
            var medication = await _db.Medications.Include(m => m.MedicationIngredients)
                                                  .FirstOrDefaultAsync(m => m.MedicationId == medicationId);
            if (medication == null)
            {
                return NotFound();
            }

            var model = new MedicationCollection
            {
                Medication = medication,
                MedicationIngredient = new MedicationIngredient
                {
                    SelectedIngredient = medication.MedicationIngredients.Select(mi => mi.ActiveIngredientId).ToList()
                },
                Strengths = medication.MedicationIngredients.ToDictionary(mi => mi.ActiveIngredientId, mi => mi.ActiveIngredientStrength)
            };

            ViewBag.DosageFormList = new SelectList(_db.DosageForms.OrderBy(d => d.Name), "DosageId", "Name", medication.DosageFormId);
            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMedication(MedicationCollection model)
        {
            ViewBag.DosageFormList = new SelectList(_db.DosageForms.OrderBy(d => d.Name), "DosageId", "Name", model.Medication.DosageFormId);
            ViewBag.IngredientList = new SelectList(_db.ActiveIngredients.OrderBy(a => a.Name), "IngredientId", "Name");

            if (model.MedicationIngredient.SelectedIngredient == null || !model.MedicationIngredient.SelectedIngredient.Any())
            {
                TempData["error"] = $"Active Ingredient for {model.Medication.Name} is required";
                return View(model);
            }

            var medication = await _db.Medications.FindAsync(model.Medication.MedicationId);
            if (medication == null)
            {
                return NotFound();
            }

            medication.Name = model.Medication.Name;
            medication.DosageFormId = model.Medication.DosageFormId;
            medication.Schedule = model.Medication.Schedule;
            medication.ReOrderLevel = model.Medication.ReOrderLevel;
            _db.Update(medication);

            var existingIngredients = _db.MedicationIngredients
                                         .Where(mi => mi.MedicationId == medication.MedicationId)
                                         .ToList();

            foreach (var selectedIngredient in model.MedicationIngredient.SelectedIngredient)
            {
                var existingIngredient = existingIngredients
                                         .FirstOrDefault(mi => mi.ActiveIngredientId == selectedIngredient);

                if (existingIngredient != null)
                {
                    existingIngredient.ActiveIngredientStrength = model.Strengths[selectedIngredient];
                    _db.Update(existingIngredient);
                }
                else
                {
                    var newIngredient = new MedicationIngredient
                    {
                        ActiveIngredientId = selectedIngredient,
                        MedicationId = model.Medication.MedicationId,
                        ActiveIngredientStrength = model.Strengths[selectedIngredient],
                    };
                    _db.Add(newIngredient);
                }
            }

            await _db.SaveChangesAsync();

            TempData["success"] = "Medication updated successfully";
            return RedirectToAction("ListMedication");
        }



        public async Task<IActionResult> ListMedication()
        {
            var medications = await _db.Medications
                .Include(m => m.MedicationIngredients)
                .ThenInclude(m=>m.ActiveIngredient)
                .Include(m=> m.DosageForm)
                .ToListAsync();

            return View(medications);
        }
        [HttpGet]
        public async Task<IActionResult> GetMedication()
        {
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

        [HttpDelete]
        public IActionResult DeleteMedication(int id)
        {
            var suburbs = _db.Medications.FirstOrDefault(m => m.MedicationId == id);
            if (suburbs == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Medications.Remove(suburbs);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }


        public IActionResult RejectPrescription(int id)
        {
            var prescription = _db.Prescriptions.Find(id);
            if (prescription == null)
            {
                return NotFound();
            }

            var rejectedPrescription = new RejectedPrescription
            {
                PrescriptionId = prescription.PrescriptionId
            };

            return View(rejectedPrescription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPrescription(int prescriptionId, string rejectedReason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (pharmacist != null)
            {
                var prescription = await _db.Prescriptions.FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);

                if (prescription != null && prescription.Status == "Prescribed")
                {
                    prescription.PharmacistId = pharmacist.PharmacistId;
                    prescription.Status = "Rejected";

                    var rejectedMedication = new RejectedPrescription
                    {
                        RejectedReason = rejectedReason,
                        PrescriptionId = prescription.PrescriptionId
                    };

                    _db.RejectedPrescriptions.Add(rejectedMedication);

                    await _db.SaveChangesAsync();

                    TempData["success"] = "Prescription rejected successfully";

                    return RedirectToAction("ListPatientRejectedPrescriptions", new { id = prescription.PrescriptionId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Prescription not found or is not in 'Prescribed' status.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Pharmacist not found.");
            }

            return RedirectToAction("ListRejectedPrescriptions");
        }


        public IActionResult ListRejectedPrescriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var prescriptions = _db.Prescriptions
                                   .Include(p => p.RejectedPrescriptions)
                                   .Include(p => p.Patient)
                                   .Include(p => p.Surgeon)
                                   .Where(p => p.PharmacistId == pharmacist.PharmacistId && p.Status == "Rejected")
                                   .ToList();
            return View(prescriptions);
        }
        [HttpGet]
        public IActionResult GetRejectedPrescription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (pharmacist == null)
            {
                return Json(new { data = new List<object>() });
            }

            var prescriptions = _db.Prescriptions
                                  .Include(p => p.RejectedPrescriptions)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Surgeon)
                                  .Where(p => p.PharmacistId == pharmacist.PharmacistId && p.Status == "Rejected")
                                  .ToList();

            var result = prescriptions.Select(p => new
            {
                Date = p.Date.ToString("dd-MM-yyyy"),
                Patient = p.Patient?.FullName,
                Reason = p.RejectedPrescriptions.Select(mp => mp.RejectedReason?.ToString()),
                p.Status,
                p.Urgent,
                p.PrescriptionId
            });

            return Json(new { data = result });
        }
        public IActionResult ListPatientRejectedPrescriptions(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var prescriptions = _db.Prescriptions
                                   .Include(p => p.RejectedPrescriptions)
                                   .Include(p => p.Patient)
                                   .Include(p => p.Surgeon)
                                   .Where(p => p.PharmacistId == pharmacist.PharmacistId && p.PrescriptionId == id  && p.Status == "Rejected" )
                                   .ToList();
            return View(prescriptions);
        }
        public IActionResult AddMedicationOrder()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(Dictionary<int, int> quantities)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var hospital = _db.Hospitals.SingleOrDefault();

            var order = new Order2()
            {
                PharmacistId = pharmacist.PharmacistId,
                Date = DateTime.Now,
                Status = "Delivered"
            };
            await _db.order2s.AddAsync(order);
            TempData["success"] = "Order added successfully";

            await _db.SaveChangesAsync();

            if (quantities != null && quantities.Any())
            {
                var medicationDetails = new StringBuilder();
                medicationDetails.Append("<table><tr><th>Medication Name</th><th>Quantity</th></tr>");

                foreach (var medicationId in quantities.Keys)
                {
                    var quantity = quantities[medicationId];
                    if (quantity > 0)
                    {
                        var pharmacistOrder = new PharmacistOrder
                        {
                            MedicationId = medicationId,
                            OrderId = order.OrderId,
                            Quantity = quantity
                        };

                        _db.Add(pharmacistOrder);
                        var medication = _db.Medications.SingleOrDefault(m => m.MedicationId == medicationId);

                        medicationDetails.Append($"<tr><td>{medication.Name}</td><td>{quantity}</td></tr>");
                    }
                }

                medicationDetails.Append("</table>");
                await _db.SaveChangesAsync();

                string emailBody = $"Dear Purchase Manager,<br/><br/><br/> You have a new order with the following details:<br/><br/>{medicationDetails.ToString()}<br/>Kind regards,<br/>{pharmacist.FullName}";

                _emailSender.SendEmail(hospital.PurchaseManagerEmailAddress, "Order Notification [GRP-04-21]", emailBody);
            }

            return RedirectToAction("ListMedicationOrder");
        }


        public async Task<IActionResult> ListMedicationOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _db.Pharmacists.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (pharmacist == null)
            {
                return NotFound("Pharmacist not found");
            }

            var orders = await _db.order2s
                                 .Where(o => o.PharmacistId == pharmacist.PharmacistId)
                                 .Include(o => o.PharmacistOrders)
                                 .ThenInclude(po => po.Medication)
                                 .ThenInclude(po => po.DosageForm)
                                 .ToListAsync();

            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> GetMedicationOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _db.Pharmacists.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (pharmacist == null)
            {
                return NotFound("Pharmacist not found");
            }

            var orders = await _db.order2s
                .Where(o => o.PharmacistId == pharmacist.PharmacistId)
                .Select(o => new
                {
                    o.OrderId,
                    Date = o.Date.ToString("dd-MM-yyyy"),
                    Medications = string.Join("\n", o.PharmacistOrders.Select(po => po.Medication.Name)),
                    DosageForms = string.Join("\n", o.PharmacistOrders.Select(po => po.Medication.DosageForm.Name)),
                    Quantities = string.Join("\n", o.PharmacistOrders.Select(po => po.Quantity.ToString())),
                    o.Status
                })
                .ToListAsync();

            return Json(new { data = orders });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _db.Pharmacists.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (pharmacist == null)
            {
                return NotFound("Pharmacist not found");
            }

            var order = await _db.order2s
                                 .Include(o => o.PharmacistOrders)
                                 .ThenInclude(po => po.Medication)
                                 .SingleOrDefaultAsync(o => o.OrderId == orderId && o.PharmacistId == pharmacist.PharmacistId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            order.Status = "Received";

            foreach (var pharmacistOrder in order.PharmacistOrders)
            {
                var medication = pharmacistOrder.Medication;
                medication.StockOnHand = (medication.StockOnHand ?? 0) + pharmacistOrder.Quantity;
            }
            TempData["success"] = "Order Recieved successfully";

            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Order recieved successfully" });
        }






        public async Task<IActionResult> ListMedicationStock()
        {
            var medications = await _db.MedicationIngredients
                .Include(m => m.Medication)
                                .Include(m => m.Medication.DosageForm)

                .Include(m => m.ActiveIngredient)
                .ToListAsync();

            return View(medications);
        }
        [HttpGet]
        public async Task<IActionResult> GetMedicationStock()
        {
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
                    stockOnHand = m.StockOnHand,
                    reOrderLevel = m.ReOrderLevel,

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
        public async Task<IActionResult> ListOrder()
        {
            var orders = await _db.Orders.Include(n => n.MedicationOrders).ThenInclude(n => n.Medication).Include(n => n.Anaesthesiologist).Include(n => n.Patient).ToListAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            var orders = await _db.Orders
                                  .Include(o => o.MedicationOrders)
                                      .ThenInclude(mo => mo.Medication)
                                  .Include(o => o.Anaesthesiologist)
                                  .Include(o => o.Patient)
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
                o.IgnoreReason,
                o.IsUrgent 
            });

            return Json(new { data = result });
        }
        public async Task<IActionResult> DispenseMedicationOrder(int orderId, string ignoreReason = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _db.Pharmacists.SingleOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (pharmacist != null)
            {
                var order = await _db.Orders
                                     .Include(o => o.MedicationOrders)
                                     .ThenInclude(mo => mo.Medication)
                                     .FirstOrDefaultAsync(o => o.OrderId == orderId && o.PharmacistId == null);

                if (order != null)
                {
                    var patientId = order.PatientId;

                    var patientAllergies = await _db.PatientAllergies
                                                    .Where(a => a.PatientId == patientId)
                                                    .Select(a => a.ActiveIngredientId)
                                                    .ToListAsync();

                    var patientConditions = await _db.PatientConditions
                                                     .Where(pc => pc.PatientId == patientId)
                                                     .Select(pc => pc.ConditionId)
                                                     .ToListAsync();

                    var conflictingAllergens = new List<string>();
                    var contraindications = new List<string>();
                    var interactionWarnings = new List<string>();
                    var activeIngredientsInOrder = new List<int>();

                    foreach (var medicationOrder in order.MedicationOrders)
                    {
                        var medicationId = medicationOrder.MedicationId;
                        var medication = _db.Medications
                          .Where(m => m.MedicationId == medicationId)
                          .Select(m => new { m.Name })
                          .FirstOrDefault();

                        var activeIngredients = await _db.MedicationIngredients
                                                         .Where(mi => mi.MedicationId == medicationId)
                                                         .Select(mi => new { mi.ActiveIngredientId, mi.ActiveIngredient.Name })
                                                         .ToListAsync();

                        activeIngredientsInOrder.AddRange(activeIngredients.Select(ai => ai.ActiveIngredientId));

                        var medicationAllergenConflicts = activeIngredients
                            .Where(ai => patientAllergies.Contains(ai.ActiveIngredientId))
                            .Select(ai => $"{ai.Name} ({medication.Name})")
                            .ToList();

                        if (medicationAllergenConflicts.Any())
                        {
                            conflictingAllergens.AddRange(medicationAllergenConflicts);
                        }

                        var medicationContraindications = await _db.ContraIndications
                            .Where(ci => activeIngredients.Select(ai => ai.ActiveIngredientId).Contains(ci.ActiveIngredientId)
                                && patientConditions.Contains(ci.ConditionDiagnosisId))
                            .Select(ci => $"{ci.ActiveIngredient.Name} ({medication.Name})")
                            .ToListAsync();

                        if (medicationContraindications.Any())
                        {
                            contraindications.AddRange(medicationContraindications);
                        }
                    }

                    var currentPatientMedications = _db.PatientMedications
                        .Where(pm => pm.PatientId == patientId)
                        .SelectMany(pm => _db.MedicationIngredients
                            .Where(mi => mi.MedicationId == pm.MedicationId)
                            .Select(mi => mi.ActiveIngredientId))
                        .Distinct()
                        .ToList();

                    foreach (var currentIngredientId in currentPatientMedications)
                    {
                        foreach (var newIngredientId in activeIngredientsInOrder)
                        {
                            if (currentIngredientId != newIngredientId)
                            {
                                var interaction = _db.MedicationInteractions
                                    .FirstOrDefault(mi =>
                                        (mi.ActiveIngredient1Id == currentIngredientId && mi.ActiveIngredient2Id == newIngredientId) ||
                                        (mi.ActiveIngredient1Id == newIngredientId && mi.ActiveIngredient2Id == currentIngredientId));

                                if (interaction != null)
                                {
                                    var currentMedicationName = _db.MedicationIngredients
                                        .Where(mi => mi.ActiveIngredientId == currentIngredientId)
                                        .Select(mi => mi.Medication.Name)
                                        .FirstOrDefault();

                                    var newMedicationName = _db.MedicationIngredients
                                        .Where(mi => mi.ActiveIngredientId == newIngredientId)
                                        .Select(mi => mi.Medication.Name)
                                        .FirstOrDefault();

                                    interactionWarnings.Add($"{interaction.Description} ({currentMedicationName} and {newMedicationName})");
                                }
                            }
                        }
                    }


                    var processedInteractions = new HashSet<(int, int)>();

                    foreach (var ingredientId1 in activeIngredientsInOrder)
                    {
                        foreach (var ingredientId2 in activeIngredientsInOrder)
                        {
                            if (ingredientId1 != ingredientId2)
                            {
                                var interactionKey = (Math.Min(ingredientId1, ingredientId2), Math.Max(ingredientId1, ingredientId2));

                                if (!processedInteractions.Contains(interactionKey))
                                {
                                    var interaction = _db.MedicationInteractions
                                        .FirstOrDefault(mi =>
                                            (mi.ActiveIngredient1Id == ingredientId1 && mi.ActiveIngredient2Id == ingredientId2) ||
                                            (mi.ActiveIngredient1Id == ingredientId2 && mi.ActiveIngredient2Id == ingredientId1));

                                    if (interaction != null)
                                    {
                                        var medication1 = _db.MedicationIngredients
                                            .Where(mi => mi.ActiveIngredientId == ingredientId1)
                                            .Select(mi => mi.Medication.Name)
                                            .FirstOrDefault();

                                        var medication2 = _db.MedicationIngredients
                                            .Where(mi => mi.ActiveIngredientId == ingredientId2)
                                            .Select(mi => mi.Medication.Name)
                                            .FirstOrDefault();

                                        interactionWarnings.Add($"{interaction.Description} ({medication1} and {medication2})");

                                        processedInteractions.Add(interactionKey);
                                    }
                                }
                            }
                        }
                    }

                    if (conflictingAllergens.Any() && string.IsNullOrEmpty(ignoreReason))
                    {
                        return Json(new
                        {
                            errorMessage = $"The medications contain active ingredients that the patient is allergic to: {string.Join(", ", conflictingAllergens.Distinct())}"
                        });
                    }

                    if (contraindications.Any() && string.IsNullOrEmpty(ignoreReason))
                    {
                        return Json(new
                        {
                            errorMessage = $"The medications have contraindications for the patient’s diagnosed conditions: {string.Join(", ", contraindications.Distinct())}"
                        });
                    }

                    if (interactionWarnings.Any() && string.IsNullOrEmpty(ignoreReason))
                    {
                        return Json(new
                        {
                            errorMessage = $"There are medication interactions: {string.Join("; ", interactionWarnings.Distinct())}"
                        });
                    }

                    order.PharmacistId = pharmacist.PharmacistId;
                    order.Status = "Dispensed";
                    order.IgnoreReason = ignoreReason;

                    foreach (var medicationOrder in order.MedicationOrders)
                    {
                        var prescribedMedication = new PrescribedMedication
                        {
                            OrderId = order.OrderId,
                            MedicationId = medicationOrder.MedicationId,
                            Quantity = medicationOrder.Quantity
                        };

                        _db.PrescribedMedications.Add(prescribedMedication);

                        var medication = medicationOrder.Medication;
                        if (medication != null)
                        {
                            medication.StockOnHand -= medicationOrder.Quantity;
                        }
                    }

                    await _db.SaveChangesAsync();
                    TempData["success"] = "Medication dispensed successfully";
                    return RedirectToAction("ListDispancedMedicationOrder");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No orders available for dispensation.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Pharmacist not found.");
            }

            return RedirectToAction("ListDispancedMedicationOrder");
        }


        public IActionResult ListDispancedMedicationOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);
            var order = _db.Orders
                           .Include(p => p.PrescribedMedications)
                           .ThenInclude(P => P.Medication)
                           .Include(p => p.Patient)
                           .Include(p => p.Anaesthesiologist)
                           .Where(c => c.PharmacistId == pharmacist.PharmacistId).Where(c => c.Status == "Dispensed")

                           .ToList();
            return View(order);
        }
        [HttpGet]
        public IActionResult GetDispancedMedicationOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (pharmacist == null)
            {
                return Json(new { data = new List<object>() });
            }

            var prescriptions = _db.Orders
                                  .Include(p => p.PrescribedMedications)
                                  .ThenInclude(mp => mp.Medication)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Anaesthesiologist)
                                  .Where(p => p.PharmacistId == pharmacist.PharmacistId && p.Status == "Dispensed")
                                  .ToList();

            var result = prescriptions.Select(p => new
            {
                Date = p.Date.ToString("dd-MM-yyyy"),
                Patient = p.Patient?.FullName,
                Anaesthesiologist = p.Anaesthesiologist?.FullName,
                Medications = string.Join("\n", p.PrescribedMedications.Select(mp => mp.Medication?.Name)),
                Quantities = string.Join("\n", p.PrescribedMedications.Select(mp => mp.Quantity.ToString())),
                p.Status,
                p.OrderId
            });

            return Json(new { data = result });
        }

        public async Task<IActionResult> ListPatientOrder(int id)
        {
 
            var orders = await _db.Orders
                .Include(n => n.MedicationOrders)
                .ThenInclude(n => n.Medication)
                .Include(n => n.Anaesthesiologist)
                .Include(n => n.Patient)
                .Where(c => c.PatientId == id)
                .ToListAsync();



            ViewBag.PatientId = id;
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientOrder(int id)
        {
    
            var orders = await _db.Orders
                                  .Include(o => o.MedicationOrders)
                                  .ThenInclude(mo => mo.Medication)
                                  .Include(o => o.Anaesthesiologist)
                                  .Include(o => o.Patient)
                                  .Where(c => c.PatientId == id)
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
                o.IgnoreReason,
                o.IsUrgent
            });

            return Json(new { data = result });
        }

    }
}
