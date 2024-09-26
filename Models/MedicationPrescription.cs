using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class MedicationPrescription
    {
        [Key]
        public int MedicationPrescriptionId { get; set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]
        public int MedicationId { get; set; }
        public Prescription Prescription { get; set; }
        public int PrescriptionId { get;set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
        [NotMapped]
        public List<int> SelectedMedication { get; set; }



    }
}
