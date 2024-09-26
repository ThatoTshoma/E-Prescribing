using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class MedicationOrder
    {
        [Key]
        public int MedicationOrderId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        [Display(Name = "Order Number")]
        public int OrderId { get; set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]
        public int MedicationId { get; set; }
        public string? Note { get; set; }

    }
}
