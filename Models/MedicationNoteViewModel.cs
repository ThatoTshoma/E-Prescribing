namespace E_Prescribing.Models
{
    public class MedicationNoteViewModel
    {
        public Patient Patient { get; set; }

        public Order Order { get; set; }
        public IEnumerable<MedicationIngredient> Medications { get; set; }
        public Dictionary<int, string> Notes { get; set; } = new Dictionary<int, string>();

    }
}
