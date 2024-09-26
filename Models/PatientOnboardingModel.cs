using E_Prescribing.CollectionModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Prescribing.Models
{
    public class PatientOnboardingModel
    {
        public PatientAllergy PatientAllergy { get; set; }
        public PatientMedication PatientMedication { get; set; }
        public PatientCondition PatientCondition { get; set; }
        public PatientBed PatientBed { get; set; }
       // public DateTime? AdmissionDate { get; set; }
        public Patient Patient { get; set; }

        public List<Patient> Patients { get; set; }
        public int CurrentStep { get; set; }
    }
}
