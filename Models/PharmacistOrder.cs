using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PharmacistOrder
    {
        [Key]
        public int PharmacistOrderId { get; set; }
        public int Quantity { get; set; }
        public Order2 Order { get; set; }
        [Display(Name = "Order Number")]
        public int OrderId { get; set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]
        public int MedicationId { get; set; }
        public int StockOnHand { get; set; }
        [NotMapped]
        public List<int> SelectedMedication { get; set; }

    }
}
