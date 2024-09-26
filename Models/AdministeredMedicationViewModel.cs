namespace E_Prescribing.Models
{
    public class AdministeredMedicationViewModel
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; }
        public string DosageForm { get; set; }
        public List<string> ActiveIngredients { get; set; }
        public string Strength { get; set; }
        public int Quantity { get; set; }
        public DateTime Time { get; set; }
    }
}
