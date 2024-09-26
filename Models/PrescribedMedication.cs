using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PrescribedMedication
    {
        [Key]
        public int PrescribedMedicationId { get; set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]
        public int MedicationId { get; set; }
        public Prescription Prescription { get; set; }
        public int? PrescriptionId { get; set; }
        public Order Order { get; set; }
        [Display(Name = "Order Number")]
        public int? OrderId { get; set; }
        public int Quantity { get; set; }
        //public List<PrescribedMedication> PrescribedMedications { get; set; }




    }
}
