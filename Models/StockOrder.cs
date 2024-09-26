using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class StockOrder
    {
        public int StockOrderId { get; set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]
        public int MedicationId { get; set; }
        public int Quantity { get; set; }
    }
}
