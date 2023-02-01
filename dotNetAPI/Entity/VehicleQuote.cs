using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetAPI.Entity
{
    public class VehicleQuote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public Car car{ get; set; }
        public float price { get; set; }
        public string currency { get; set; }
        public DateTime generatedAt { get; set; }
        public DateTime expiredAt { get; set; }
        public int rentDuration { get; set; }
    }
}
