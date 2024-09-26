namespace E_Prescribing.Models
{
    public class MedicationPrescriptionViewModel
    {
        public Patient Patient { get; set; }

        public Prescription Prescription { get; set; }
        public IEnumerable<MedicationIngredient> Medications { get; set; }
    }
}
