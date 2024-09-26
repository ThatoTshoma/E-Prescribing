using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PatientAllergyCollection
    {
        public PatientAllergy PatientAllergy { get; set; }
        public IEnumerable<Patient> Patients { get; set;}
        public IEnumerable<ActiveIngredient> ActiveIngredients { get; set; }
    }
}
