namespace dotNetAPI.DTO
{
    public class CarRentalAdminDTO
    {
        public string CarId { get; set; }
        public string RentId { get; set; }
        public DateTime rentDate { get; set; }
        public DateTime returnDate { get; set; }
    }
}
