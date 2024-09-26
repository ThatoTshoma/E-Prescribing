namespace E_Prescribing.Models
{
    public class PatientMedicationHistoryViewModel
    {
        public Patient Patient { get; set; }
        public List<PatientBed> Beds { get; set; }
        public List<PatientAllergy> Allergies { get; set; }
        public List<PatientMedication> Medications { get; set; }
        public List<PatientCondition> Conditions { get; set; }
        public List<PatientTreatment> Treatments { get; set; }

    }
}
