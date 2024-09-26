using E_Prescribing.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Prescribing.Data.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly ApplicationDbContext _db;
        public MedicationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task GetAllAsync(Func<object, object> value)
        {
            throw new NotImplementedException();
        }

        public Task GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Medication> GetMedicationById(int id)
        {
            var medicationDetails = await _db.Medications
                 .Include(c => c.DosageForm)
                 .FirstOrDefaultAsync(n => n.MedicationId == id);

            return medicationDetails;
        }
        public async Task<IEnumerable<Medication>> GetProductByIdAsync()
        {
            return await _db.Medications.ToListAsync();

        }
    }
}
