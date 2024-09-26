using E_Prescribing.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Prescribing.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly ApplicationDbContext _context;
        public OrdersService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders.Include(n => n.MedicationOrders).ThenInclude(n => n.Medication).Include(n => n.Anaesthesiologist).Include(n => n.Patient).ToListAsync();

            return orders;
        }
        public async Task<List<Prescription>> GetPrescriptionByUserId(int userId)
        {
            var prescribe = await _context.Prescriptions.Include(n => n.MedicationPrescriptions).ThenInclude(n => n.Medication).Include(n => n.Pharmacist).ToListAsync();

            return prescribe;
        }

        public async Task StoreOrder(List<MedicationCart> medications, int userId, int patientdId)
        {


            var order = new Order()
            {
                AnaesthesiologistId = userId,
                Date = DateTime.Now,
                PatientId = patientdId,
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            foreach (var medication in medications)
            {
                var medi = await _context.Medications
                    .FirstOrDefaultAsync(po => po.MedicationId == medication.Medication.MedicationId);

                if (medi != null && medi.StockOnHand >= medication.Quantity)
                {
                    var medicationOrder = new MedicationOrder()
                    {
                        Quantity = medication.Quantity,
                        MedicationId = medication.Medication.MedicationId,
                        OrderId = order.OrderId,
                    };
                    await _context.MedicationOrders.AddAsync(medicationOrder);

                    medi.StockOnHand -= medication.Quantity;
                    _context.Medications.Update(medi);
                }
            
            }

            await _context.SaveChangesAsync();

        }

        public async Task StorePrescription(List<MedicationCart> medications, int userId)
        {


            var prescription = new Prescription()
            {
                PharmacistId = userId,
            };
            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();

            foreach (var medication in medications)
            {
                var prescribedMedications = new PrescribedMedication()
                {
                    Quantity = medication.Quantity,
                    MedicationId = medication.Medication.MedicationId,
                    PrescriptionId = prescription.PrescriptionId,
                };
                await _context.PrescribedMedications.AddAsync(prescribedMedications);
            }
            await _context.SaveChangesAsync();

        }


    }
}
