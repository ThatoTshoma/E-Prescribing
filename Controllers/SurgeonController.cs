using E_Prescribing.CollectionModel;
using E_Prescribing.Data;
using E_Prescribing.Models;
using E_Prescribing.Services;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

namespace E_Prescribing.Controllers
{
    public class SurgeonController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMyEmailSender _emailSender;


        public SurgeonController(ApplicationDbContext db, IMyEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult LoadTheatre(int wardId)
        {
            var theatre = _db.Theatres.Where(s => s.WardId == wardId).Select(s => new { theatreId = s.TheatreId, theatreName = s.Name }).ToList();
            return Json(theatre);
        }

        public IActionResult AddBooking(int id)
        {
            ViewBag.TreatmentList = new SelectList(_db.Treatments, "TreatmentId", "DisplayText");

            var collection = new BookingCollection
            {
                Booking = new Booking { PatientId = id },
                Patients = _db.Patients.ToList(),
                Anaesthesiologists = _db.Anaesthesiologists.ToList(),
                Theatres = _db.Theatres.ToList(),
                Treatments = _db.Treatments.ToList() 

            };

            return View(collection);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBooking(BookingCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.Booking.PatientId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var collection = new BookingCollection
            {
                Booking = model.Booking,
                Patients = _db.Patients.ToList(),
                Theatres = _db.Theatres.ToList(),
                Anaesthesiologists = _db.Anaesthesiologists.ToList(),
                Treatments = _db.Treatments.ToList()
            };

            if (model.Booking.Date >= DateTime.Now.Date)
            {
                var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
                var booking = new Booking
                {
                    SurgeonId = surgeon.SurgeonId,
                    AnaesthesiologistId = model.Booking.AnaesthesiologistId,
                    TheatreId = model.Booking.TheatreId,
                    PatientId = patient.PatientId,
                    Date = model.Booking.Date,
                    Status = false,
                    Session = model.Booking.Session
                };

                _db.Bookings.Add(booking);
                _db.SaveChanges();

                foreach (var selectedTreatmentId in model.PatientTreatment.SelectedTreaments)
                {
                    var patientTreatment = new PatientTreatment
                    {
                        TreatmentId = selectedTreatmentId,
                        PatientId = patient.PatientId,
                        BookingId = booking.BookingId
                    };

                    _db.PatientTreatments.Add(patientTreatment);
                }

                _db.SaveChanges();

                var anaesthesiologist = _db.Anaesthesiologists.FirstOrDefault(a => a.AnaesthesiologistId == model.Booking.AnaesthesiologistId);
                if (anaesthesiologist != null)
                {
                    string patientEmailMessage = $"Dear {patient.FullName},<br><br>" +
                                                 "You have a new booking scheduled with Anaesthesiologist " +
                                                 $"{anaesthesiologist.FullName} on {booking.Date.ToShortDateString()}.<br><br>" +
                                                 "Kind regards,<br>" +
                                                 $"{surgeon.FullName}";

                    _emailSender.SendEmail(patient.EmailAddress, "Booking Notification [GRP-04-21]", patientEmailMessage);

                    string anaesthesiologistEmailMessage = $"Dear {anaesthesiologist.FullName},<br><br>" +
                                                           "You have a new booking scheduled for patient " +
                                                           $"{patient.FullName} on {booking.Date.ToShortDateString()}.<br><br>" +
                                                           "Kind regards,<br>" +
                                                           $"{surgeon.FullName}";

                    _emailSender.SendEmail(anaesthesiologist.EmailAddress, "Booking Notification [GRP-04-21]", anaesthesiologistEmailMessage);
                }




                TempData["success"] = "Booking added successfully";
                return RedirectToAction("ListBooking", new { id = patient.PatientId });
            }

            ViewBag.TreatmentList = new SelectList(_db.Treatments, "TreatmentId", "DisplayText");
            ViewBag.Message = "Please Enter the Date greater than or equal to today!";

            return View(collection);
        }

        public IActionResult PendingBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);

            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.SurgeonId == surgeon.SurgeonId && b.Status == false)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetPendingBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.SurgeonId == surgeon.SurgeonId && b.Status == false)
                .ToList();

            var viewModel = new
            {
                data = bookings.Select(b => new
                {
                    bookingId = b.BookingId,
                    anaesthesiologist = new
                    {
                        name = b.Anaesthesiologist.FullName
                    },
                    patient = new
                    {
                        name = b.Patient.FullName,
                    },
                    theatre = new
                    {
                        name = b.Theatre.Name
                    },
                    ward = new
                    {
                        number = b.Theatre.Ward.WardNumber
                    },
                    date = b.Date.ToString("dd-MM-yyyy"),
                    status = b.Status,
                    session = b.Session,
                    treatments = b.PatientTreatments.Select(pt => new
                    {
                        name = pt.Treatment.DisplayText
                    }).ToArray()
                }).ToArray()
            };

            return Json(viewModel);
        }

        public IActionResult ApprovedBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);

            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.SurgeonId == surgeon.SurgeonId && b.Status == true)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetApprovedBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.SurgeonId == surgeon.SurgeonId && b.Status == true)
                .ToList();

            var viewModel = new
            {
                data = bookings.Select(b => new
                {
                    bookingId = b.BookingId,
                    anaesthesiologist = new
                    {
                        name = b.Anaesthesiologist.FullName
                    },
                    patient = new
                    {
                        name = b.Patient.FullName,
                    },
                    theatre = new
                    {
                        name = b.Theatre.Name
                    },
                    ward = new
                    {
                        number = b.Theatre.Ward.WardNumber
                    },
                    date = b.Date.ToString("dd-MM-yyyy"),
                    status = b.Status,
                    session = b.Session,
                    treatments = b.PatientTreatments.Select(pt => new
                    {
                        name = pt.Treatment.DisplayText
                    }).ToArray()
                }).ToArray()
            };

            return Json(viewModel);
        }

        public IActionResult ListBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments) 
                .ThenInclude(pt => pt.Treatment) 
                .Where(b => b.SurgeonId == surgeon.SurgeonId && b.PatientId == id)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward) 
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments) 
                .ThenInclude(pt => pt.Treatment) 
                .Where(b => b.SurgeonId == surgeon.SurgeonId && b.PatientId == id)
                .ToList();

            var result = bookings.Select(b => new {
                b.BookingId,
                b.Anaesthesiologist.FullName,
                Theatre = b.Theatre.Name,
                Ward = b.Theatre.Ward.WardNumber, 
                Date = b.Date.ToString("dd-MM-yyy"), 
                b.Status,
                b.Session,
                Treatments = b.PatientTreatments.Select(pt => pt.Treatment.DisplayText).ToList()
            });

            return Json(new { data = result });
        }
        public IActionResult EditBooking(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.Theatre)
                .Include(b => b.PatientTreatments)
                .SingleOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var collection = new BookingCollection
            {
                Booking = booking,
                Patients = _db.Patients.ToList(),
                Wards = _db.Wards.ToList(),
                Anaesthesiologists = _db.Anaesthesiologists.ToList(),
          
            };

            ViewBag.TreatmentList = new SelectList(_db.Treatments, "TreatmentId", "Code");

            return View(collection);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBooking(BookingCollection model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TreatmentList = new SelectList(_db.Treatments, "TreatmentId", "Code");
                return View(model);
            }

            var booking = _db.Bookings
                .Include(b => b.Patient)
                .SingleOrDefault(b => b.BookingId == model.Booking.BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            booking.AnaesthesiologistId = model.Booking.AnaesthesiologistId;
            booking.TheatreId = model.Booking.TheatreId;
            booking.Date = model.Booking.Date;
            booking.Status = model.Booking.Status;
            booking.Session = model.Booking.Session;

            _db.Bookings.Update(booking);

            var existingPatientTreatments = _db.PatientTreatments
                .Where(pt => pt.BookingId == model.Booking.BookingId)
                .ToList();

            _db.PatientTreatments.RemoveRange(existingPatientTreatments);

            foreach (var selectedTreatmentId in model.PatientTreatment.SelectedTreaments)
            {
                var patientTreatment = new PatientTreatment
                {
                    TreatmentId = selectedTreatmentId,
                    PatientId = booking.PatientId,
                    BookingId = booking.BookingId,
                };

                _db.PatientTreatments.Add(patientTreatment);
            }

            _db.SaveChanges();

            var patient = _db.Patients.SingleOrDefault(p => p.PatientId == booking.PatientId);
            if (patient != null)
            {
                _emailSender.SendEmail(patient.EmailAddress, "Booking Update", "Your booking has been updated.");
            }

            var anaesthesiologist = _db.Anaesthesiologists.SingleOrDefault(a => a.AnaesthesiologistId == booking.AnaesthesiologistId);
            if (anaesthesiologist != null)
            {
                _emailSender.SendEmail(anaesthesiologist.EmailAddress, "Booking Update", "A booking you are assigned to has been updated.");
            }

            return RedirectToAction("ListBooking");
        }

        [HttpDelete]
        public IActionResult DeleteBooking(int id)
        {
            var theatre = _db.Bookings.FirstOrDefault(m => m.BookingId == id);
            if (theatre == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Bookings.Remove(theatre);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
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
        public IActionResult AddPatient()
        {

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPatient(Patient model)
        {

            if (_db.Patients.Any(a => a.IdNumber == model.IdNumber && a.PatientId != model.PatientId))
            {
                ModelState.AddModelError("IdNumber", "ID Number already exists");
                return View(model);

            }

            if (!ValidateDateOfBirth(model.IdNumber, model.DateOfBirth))
            {
                ModelState.AddModelError("DateOfBirth", "Date of birth does not match the ID number.");
                return View(model);
            }
      
            var patient = new Patient();

            patient.Name = model.Name;
            patient.Surname = model.Surname;
            patient.FullName = model.Name + " " + model.Surname;
            patient.IdNumber = model.IdNumber;
            patient.Gender = model.Gender;
            patient.DateOfBirth = model.DateOfBirth;
            patient.ContactNumber = null;
            patient.EmailAddress = model.EmailAddress;
            patient.AddressLine1 = null;
            patient.AddressLine2 = null;
            patient.SuburbId = null;
            patient.AdmissionDate = null;
            patient.DischargeDate = null;

            _db.Patients.Add(patient);
            TempData["success"] = "Patient added successfully";

            _db.SaveChanges();
            return RedirectToAction("ListPatient");


        }

        public IActionResult LoadSuburbs(int cityId)
        {
            var suburbs = _db.Suburbs.Where(s => s.CityId == cityId).Select(s => new { suburbID = s.SuburbId, suburbName = s.Name }).ToList();
            return Json(suburbs);
        }
        public JsonResult GetSuburbsByCity(int cityId)
        {

            var suburbs = _db.Suburbs.Where(s => s.CityId == cityId).Select(s => new { s.SuburbId, s.Name }).OrderBy(s => s.Name).ToList();
            return Json(suburbs);
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
        public IActionResult ListPatient(string idNumber)
        {
            var patients = _db.Patients.ToList();

            return View(patients);
        }
        [HttpGet]
        public IActionResult GetPatient()
        {
            var patients = _db.Patients.ToList();
            return Json(new { data = patients });
        }

        [HttpDelete]
        public IActionResult DeletePatient(int id)
        {
            var patient = _db.Patients.FirstOrDefault(m => m.PatientId == id);
            if (patient == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Patients.Remove(patient);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        [HttpGet]
        public async Task<IActionResult> GenerateReport(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.Single(c => c.UserId.ToString() == userId);

            var bookingsQuery = _db.Bookings
              .Include(b => b.Patient)
              .Include(b => b.Theatre)
              .ThenInclude(t => t.Ward)
              .Include(b => b.Anaesthesiologist)
              .Include(b => b.PatientTreatments)
              .ThenInclude(pt => pt.Treatment)
              .Where(b => b.SurgeonId == surgeon.SurgeonId);
            if (startDate.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(o => o.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(o => o.Date <= endDate.Value);
            }
            bookingsQuery = bookingsQuery.OrderBy(o => o.Date);

            var bookings = bookingsQuery.ToList();

            var totalUniquePatients = bookings.Select(o => o.Patient?.PatientId).Distinct().Count();

            using (var pdfStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfStream);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc);

                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

                Paragraph titleParagraph = new Paragraph("SURGERY REPORT")
                    .SetFont(titleFont)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(titleParagraph);

                Paragraph anaesthesiologistParagraph = new Paragraph($"Dr {surgeon.FullName}")
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

                Table table = new Table(new float[] { 20f, 40f, 40f });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                table.AddHeaderCell(new Cell().Add(new Paragraph("Date").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Patient").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Treatment Code").SetFont(boldFont)));

                foreach (var order in bookings)
                {
                    var medications = string.Join("\n", order.PatientTreatments.Select(mo => mo.Treatment.DisplayText));

                    table.AddCell(new Cell().Add(new Paragraph(order.Date.ToString("dd MMM yyyy")).SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(order.Patient?.FullName ?? "N/A").SetFont(normalFont)));
                    table.AddCell(new Cell().Add(new Paragraph(medications).SetFont(normalFont)));
                }

                table.AddCell(new Cell(1, 4).Add(new Paragraph($"Total Patients: {totalUniquePatients}").SetFont(boldFont)).SetTextAlignment(TextAlignment.LEFT));


                doc.Add(table);

                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph("SUMMARY PER TREATMENT CODE:").SetFont(boldFont));
                doc.Add(new Paragraph(" ")); 

                Table summaryTable = new Table(new float[] { 20f, 30f });
                summaryTable.SetWidth(UnitValue.CreatePercentValue(50));

                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Treatment Code").SetFont(boldFont)));
                summaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Total Surgeries").SetFont(boldFont)));

                var medicineSummary = bookings.SelectMany(o => o.PatientTreatments)
                    .GroupBy(pt => pt.Treatment.DisplayText)
                    .Select(group => new
                    {
                        TreatmentCode = group.Key,
                        TotalSurgeries = group.Count()
                    })
                    .ToList();

                foreach (var med in medicineSummary)
                {
                    summaryTable.AddCell(new Cell().Add(new Paragraph(med.TreatmentCode).SetFont(normalFont)));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(med.TotalSurgeries.ToString()).SetFont(normalFont)));
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

                    return File(finalPdfStream.ToArray(), "application/pdf", "SurgeryReport.pdf");
                }
            }
        }

        public IActionResult ListBookedPatient(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var bookings = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                .ThenInclude(pt => pt.Treatment)
                .Where(b => b.SurgeonId == surgeon.SurgeonId)
                .ToList();

            ViewBag.PatientId = id;

            return View(bookings);
        }
        [HttpGet]
        public IActionResult GetBookedPatient(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);

            var bookingsQuery = _db.Bookings
                .Include(b => b.Patient)
                .Include(b => b.Theatre)
                    .ThenInclude(t => t.Ward)
                .Include(b => b.Anaesthesiologist)
                .Include(b => b.PatientTreatments)
                    .ThenInclude(pt => pt.Treatment)
                .Where(b => b.SurgeonId == surgeon.SurgeonId);

            if (startDate.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(o => o.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(o => o.Date <= endDate.Value);
            }

            var bookings = bookingsQuery.ToList();

            var viewModel = new
            {
                data = bookings.Select(b => new
                {
                    bookingId = b.BookingId,
                    anaesthesiologist = new
                    {
                        name = b.Anaesthesiologist.FullName
                    },
                    patient = new
                    {
                        name = b.Patient.FullName,
                    },
                    theatre = new
                    {
                        name = b.Theatre.Name
                    },
                    ward = new
                    {
                        number = b.Theatre.Ward.WardNumber
                    },
                    date = b.Date.ToString("dd-MM-yyyy"), 
                    status = b.Status,
                    session = b.Session,
                    treatments = b.PatientTreatments.Select(pt => new
                    {
                        name = pt.Treatment.DisplayText
                    }).ToArray() 
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


        public IActionResult AddPrescription(int id)
        {
            var collection = new PrescriptionCollection
            {
                Prescription = new Prescription { PatientId = id },

                Medications = _db.PharmacistOrders.Select(po => po.Medication).Distinct().ToList(),
                Patients = _db.Patients.ToList()
            };
            return View(collection);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription(PrescriptionCollection model)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == model.Prescription.PatientId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (patient != null && surgeon != null)
            {
                var prescription = new Prescription
                {
                    PatientId = patient.PatientId,
                    SurgeonId = surgeon.SurgeonId,
                    PharmacistId = null,
                    Urgent = model.Prescription.Urgent,
                    Date = DateTime.Now,
                    Status = "Prescribed",
                    Note = model.Prescription.Note,
                };

                _db.Prescriptions.Add(prescription);
                await _db.SaveChangesAsync();

                foreach (var selectedMedication in model.MedicationPrescription.SelectedMedication)
                {
                    var pharmacistOrder = _db.PharmacistOrders.FirstOrDefault(po => po.MedicationId == selectedMedication);
                    if (pharmacistOrder != null)
                    {
                        var medicationPrescription = new MedicationPrescription
                        {
                            MedicationId = selectedMedication,
                            PrescriptionId = prescription.PrescriptionId,
                            Instructions = model.Instructions[selectedMedication],
                            Quantity = model.Quantities[selectedMedication],
                        };
                        _db.Add(medicationPrescription);
                    }
                }
                await _db.SaveChangesAsync();

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Selected patient or surgeon not found.");
            }

            model.Patients = _db.Patients.ToList();
            model.Medications = _db.PharmacistOrders.Select(po => po.Medication).Distinct().ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPrescription(Dictionary<int, int> quantities,int patientId,string isUrgent,string note,Dictionary<int, string> instructions,string reasonForIgnoring = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);

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

            var activeIngredientsInPrescription = new List<int>();

            if (quantities != null && quantities.Any())
            {
                foreach (var medicationId in quantities.Keys)
                {
                    var quantity = quantities[medicationId];
                    if (quantity > 0)
                    {
                        var medication = _db.Medications
                                   .Where(m => m.MedicationId == medicationId)
                                   .Select(m => new { m.Name })
                                   .FirstOrDefault();

                        var activeIngredients = _db.MedicationIngredients
                                                   .Where(ma => ma.MedicationId == medicationId)
                                                   .Select(ma => new { ma.ActiveIngredientId, ma.ActiveIngredient.Name })
                                                   .ToList();

                        activeIngredientsInPrescription.AddRange(activeIngredients.Select(ai => ai.ActiveIngredientId));

                        var medicationAllergenConflicts = activeIngredients
                            .Where(ai => patientAllergies.Contains(ai.ActiveIngredientId))
                            .Select(ai => $"{ai.Name} ({medication.Name})")  
                            .ToList();

                        if (medicationAllergenConflicts.Any())
                        {
                            conflictingAllergens.AddRange(medicationAllergenConflicts);
                        }
                        var medicationContraindications = _db.ContraIndications
                            .Where(ci => activeIngredients.Select(ai => ai.ActiveIngredientId).Contains(ci.ActiveIngredientId)
                                && patientConditions.Contains(ci.ConditionDiagnosisId))
                            .Select(ci => $"{ci.ActiveIngredient.Name} ({medication.Name})")
                            .ToList();

                        if (medicationContraindications.Any())
                        {
                            contraindications.AddRange(medicationContraindications);
                        }
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

            }

            if (conflictingAllergens.Any() && string.IsNullOrEmpty(reasonForIgnoring))
            {
                return Json(new
                {
                    errorMessage = $"The prescribed medications contain Active Ingredients the patient is allergic to: {string.Join(", ", conflictingAllergens.Distinct())}"
                });
            }

            if (contraindications.Any() && string.IsNullOrEmpty(reasonForIgnoring))
            {
                return Json(new
                {
                    errorMessage = $"The prescribed medications have Contraindications for the patient’s diagnosed conditions: {string.Join(", ", contraindications.Distinct())}"
                });
            }

            if (interactionWarnings.Any() && string.IsNullOrEmpty(reasonForIgnoring))
            {
                return Json(new
                {
                    errorMessage = $"{string.Join("; ", interactionWarnings.Distinct())}"
                });
            }

            var prescription = new Prescription()
            {
                SurgeonId = surgeon.SurgeonId,
                Date = DateTime.Now,
                PharmacistId = null,
                Urgent = isUrgent,
                Status = "Prescribed",
                Note = note,
                PatientId = patientId,
                IgnoreReason = reasonForIgnoring 
            };

            await _db.Prescriptions.AddAsync(prescription);
            await _db.SaveChangesAsync();

            foreach (var medicationId in quantities.Keys)
            {
                var quantity = quantities[medicationId];
                if (quantity > 0)
                {
                    var prescribedMedication = new MedicationPrescription
                    {
                        MedicationId = medicationId,
                        PrescriptionId = prescription.PrescriptionId,
                        Instructions = instructions.ContainsKey(medicationId) ? instructions[medicationId] : null,
                        Quantity = quantity
                    };

                    _db.Add(prescribedMedication);
                }
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Prescription added successfully";
            return RedirectToAction("ListPatientPrescription", new { id = patientId });
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

        public async Task<IActionResult> ListMedication(int id)
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

            var viewModel = new MedicationPrescriptionViewModel
            {
                Patient = patient,
                Medications = medications
            };


            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetMedication(int id)
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


        public IActionResult ListPatientTreatment(int id)
        {
            var patientTreatment = _db.PatientTreatments.Include(pb => pb.Treatment)
                                              .Include(pb => pb.Patient)
                                              .Where(pb => pb.PatientId == id)
                                              .ToList();
            ViewBag.PatientId = id;
            return View(patientTreatment);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var prescription = _db.Prescriptions
                                 .Include(p => p.MedicationPrescriptions)
                                 .ThenInclude(P => P.Medication)
                                 .Include(p => p.Patient)
                                 .Include(P =>P.Nurse)
                                 .Include(p => p.Pharmacist)
                                 .Where(p => p.PatientId == id && p.SurgeonId == surgeon.SurgeonId)
                                .ToList();
            ViewBag.PatientId = id;
            return View(prescription);
        }
        [HttpGet]
        public IActionResult GetPatientPrescription(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surgeon = _db.Surgeons.SingleOrDefault(c => c.UserId.ToString() == userId);
            var prescriptions = _db.Prescriptions
                                  .Include(p => p.MedicationPrescriptions)
                                  .ThenInclude(mp => mp.Medication)
                                  .Include(p => p.Patient)
                                  .Include(p => p.Nurse)
                                  .Include(p => p.Pharmacist)
                                  .Where(p => p.PatientId == id && p.SurgeonId == surgeon.SurgeonId)
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



        [HttpDelete]
        public IActionResult DeletePatientPrescription(int id)
        {
            var prescription = _db.Prescriptions.FirstOrDefault(m => m.PrescriptionId == id);
            if (prescription == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _db.Prescriptions.Remove(prescription);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }
        //public IActionResult ListVital(int id)
        //{
        //    var patientVital = _db.VitalRanges.Include(pb => pb.Patient)
        //                                      .Where(pb => pb.PatientId == id)
        //                                      .ToList();
        //    ViewBag.PatientId = id;
        //    return View(patientVital);
        //}
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
                    heartrate = v.HeartRate,
                    oxygensaturation = v.OxygenSaturation,
                    bloodglucoselevel = v.BloodGlucoseLevel,
                    bloodoxegenlevel = v.BloodOxegenLevel
                }).ToList(),
                vitalNames = selectedVitals.Select(v => v.Name.ToLower()).ToList()
            };



            return Json(result);
        }

    }
}
