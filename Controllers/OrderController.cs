using E_Prescribing.Data;
using E_Prescribing.Data.Services;
using E_Prescribing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Prescribing.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMedicationService _medicationService;
        private readonly IOrdersService _ordersService;
        private readonly Cart _cart;
        private readonly ApplicationDbContext _db;


        public OrderController(IMedicationService medicationService, Cart cart, ApplicationDbContext db, IOrdersService ordersService) 
        {
            _medicationService = medicationService;
            _cart = cart;
            _db = db;
            _ordersService = ordersService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologists = _db.Anaesthesiologists.Single(c => c.UserId.ToString() == userId);
            var orders = await _ordersService.GetOrdersByUserId(anaesthesiologists.AnaesthesiologistId);
            return View(orders);
        }
        public IActionResult MedicationCart(int id)
        {
            var medications = _cart.GetMedicationCart();
            _cart.Medications = medications;

            var response = new CartModel()
            {
                Cart = _cart,
            };
            ViewBag.PatientId = id;

            return View(response);
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
        public async Task<IActionResult> PlaceOrder(int id)
        {
            var patient = await _db.Patients.FindAsync(id);


            var medication = _cart.GetMedicationCart();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anaesthesiologist = _db.Anaesthesiologists.SingleOrDefault(c => c.UserId.ToString() == userId);

            if (anaesthesiologist == null)
            {
                return NotFound("Anaesthesiologist not found");
            }

            await _ordersService.StoreOrder(medication, anaesthesiologist.AnaesthesiologistId, id);
            await _cart.ClearCart();
            ViewBag.PatientId = id;

            return View("CompletedOrder");
        }


        public async Task<IActionResult> PrescribeMedication(int patientId)
        {
            var medication = _cart.GetMedicationCart();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = _db.Pharmacists.Single(c => c.UserId.ToString() == userId);

            await _ordersService.StoreOrder(medication, pharmacist.PharmacistId, patientId);
            await _cart.ClearCart();

            return View("CompletedOrder");
        }

    }
}
