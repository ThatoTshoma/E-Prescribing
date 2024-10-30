using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Urgent { get; set; }
        public string Note { get; set; }
        public Surgeon Surgeon { get; set; }
        [Display(Name = "Surgoen")]
        public int SurgeonId { get;set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set;}
        public Pharmacist Pharmacist { get; set; }
        [Display(Name = "Pharmacist")]
        public int? PharmacistId { get; set;}
        public Nurse Nurse { get; set; }
        [Display(Name = "Nurse")]
        public int? NurseId { get; set; }




        [NotMapped]
        public List<int> SelectedMedication { get; set; }

        public List<MedicationPrescription> MedicationPrescriptions { get; set; }
        public List<PrescribedMedication> PrescribedMedications { get; set; }
        public List<AdministeredMedication> AdministeredMedications { get; set; }
        public List<RejectedPrescription> RejectedPrescriptions { get; set; }
        public List<IgnorePrescriptionReason> IgnorePrescriptions { get; set; }





    }

}
