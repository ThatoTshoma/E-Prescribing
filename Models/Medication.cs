using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Medication
    {

        [Key]
        public int MedicationId { get; set; }
        public string Name { get; set; }
        public int Schedule { get; set; }
        [Display(Name = "Stock On Hand")]
        public int? StockOnHand { get; set; }
        [Display(Name = "Re-Order Level")]
        public int? ReOrderLevel { get; set; }

        public DosageForm DosageForm { get; set; }
        [Display(Name = "Dosage Form")]
        public int DosageFormId { get; set; }
        public List<MedicationIngredient> MedicationIngredients { get; set; }


    }
}
