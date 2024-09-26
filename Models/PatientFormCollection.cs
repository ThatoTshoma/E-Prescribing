namespace E_Prescribing.Models
{
    public class PatientFormCollection
    {
        public int PatientId { get; set; }
        public List<int> SelectedMedications { get; set; }
        public List<int> SelectedAllergies { get; set; }
        public List<int> SelectedConditions { get; set; }
        public List<Patient> Patients { get; set; }
    }
}
