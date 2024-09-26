using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class AdministeredMedication
    {
        public int AdministeredMedicationId { get; set; }
        public Medication Medication { get; set; }
        [Display(Name = "Medication Name")]

        public int MedicationId { get; set; }
        public Prescription Prescription { get; set; }
        public int PrescriptionId { get; set; }
        public int Quantity { get; set; }
        public DateTime Time { get; set; }


    }
}
