using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PharmacistOrderCollection
    {
        public PharmacistOrder PharmacistOrder { get; set; }
        public IEnumerable<Medication> Medications { get; set; }
    }
}
