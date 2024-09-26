using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Order2
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public Pharmacist Pharmacist { get; set; }
        [Display(Name = "Pharmacist")]
        public int PharmacistId { get; set; }
        public List<PharmacistOrder> PharmacistOrders { get; set; }
    }
}
