using E_Prescribing.Data;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Patient
    {
        [Key] 
        public int PatientId { get;set; }
        public string Name { get;set; }
        public string Surname { get;set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "ID Number")]
        [Required(ErrorMessage = "ID Number is required.")]
        [StringLength(13, ErrorMessage = "ID Number must be exactly 13 digits.", MinimumLength = 13)]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ID Number must be exactly 13 digits.")]

        public string IdNumber { get;set; }
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get;set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get;set; }
        public string Gender { get; set; }
        [Display(Name= "Date Of Birth")]
        public DateTime DateOfBirth { get;set; }
        [Display(Name = "Addmission Date")]
        public DateTime? AdmissionDate { get;set; }
        [Display(Name = "Discharge Date")]
        public DateTime? DischargeDate { get;set; }
        [Display(Name = "Address Line 1")]
        public string? AddressLine1 { get;set; }
        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get;set; }
        public Suburb Suburb { get; set; }
        [Display(Name = "Suburb Name")]
        public int? SuburbId { get; set; }
        public Nurse Nurse { get; set; }
        [Display(Name = "Nurse")]
        public int? NurseId { get; set; }


        public List<PatientCondition> PatientConditions { get; set; }
        public List<PatientVital> PatientVitals { get; set; }
        public List<PatientAllergy> PatientAllergies { get; set; }
        public List<VitalRange> VitalsRanges { get; set; }
        public List<PatientMedication> PatientMedications { get; set; }
        public List<Booking> Booking { get; set; }






    }
}
