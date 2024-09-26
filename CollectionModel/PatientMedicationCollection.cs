using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PatientMedicationCollection
    {
        public PatientMedication PatientMedication { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Medication> Medications { get; set; }
    }
}
