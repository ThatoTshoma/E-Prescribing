namespace E_Prescribing.Models
{
    public class MedicationOrderViewModel
    {
        public Patient Patient { get; set; }
        public Order Order { get; set; }
        public Prescription Prescription { get; set; }
        public IEnumerable<MedicationIngredient> Medications { get; set; }

    }
}
