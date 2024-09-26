using E_Prescribing.Data;

namespace E_Prescribing.Models
{
    public class CartModel
    {
        public Cart Cart { get; set; }
        public double CartTotal { get; set; }

        //public Patient Patient { get; set; }  // Add this line


         public List<Patient> Patient { get; set; }
    }
}
