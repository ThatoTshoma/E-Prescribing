using E_Prescribing.Models;
using Microsoft.Data.SqlClient;

namespace E_Prescribing.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrder(List<MedicationCart> medications, int userId, int patientId);
        Task StorePrescription(List<MedicationCart> medications, int userId);

        Task<List<Order>> GetOrdersByUserId(int userId);
        Task<List<Prescription>> GetPrescriptionByUserId(int userId);

    }
}
