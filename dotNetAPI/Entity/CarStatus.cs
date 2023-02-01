using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetAPI.Entity
{
    public class CarStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Car Car { get; set; }
        public String Action { get; set; }
        public DateTime ActionDateTime { get; set; }
        public String PerformedBy { get; set; } // MicrosoftID
        public string Note { get; set; }
        public CarStatus() { }
    }
}
