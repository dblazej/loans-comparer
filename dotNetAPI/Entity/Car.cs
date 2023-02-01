using dotNetAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetAPI.Entity
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public int BasePrice { get; set; }
        public int EnginePower { get; set; }
        public string EnginePowerType { get; set; }
        public int Capacity { get; set; }
        public string Currency { get; set; }
        public Boolean IsRented { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
