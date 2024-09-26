using E_Prescribing.Models;

namespace E_Prescribing.Data.Services
{
    public interface IMedicationService
    {
        Task GetAllAsync(Func<object, object> value);
        Task GetByIdAsync(int id);
        Task<Medication> GetMedicationById(int id);
   
    }
}
