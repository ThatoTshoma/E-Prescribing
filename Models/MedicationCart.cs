using E_Prescribing.Data;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class MedicationCart
    {
        [Key]
        public int MedicationCartId { get; set; }
        public Medication Medication { get; set; }
        public int MedicationId { get; set; }
       // public Cart Cart { get; set; }
        public int CartId { get; set; } 
        public int Quantity { get; set; }

    }
}
