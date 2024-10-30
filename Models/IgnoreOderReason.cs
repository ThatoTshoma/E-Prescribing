using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class IgnoreOderReason
    {
        [Key]
        public int IgnoreOderReasonId { get; set; }
        public string Reason { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; } 
        public Anaesthesiologist Anaesthesiologist { get; set; }
        public int? AnaesthesiologistId { get; set; }
        public Pharmacist Pharmacist {  get; set; }
        public int? PharmacistId { get; set; }
    }
}
