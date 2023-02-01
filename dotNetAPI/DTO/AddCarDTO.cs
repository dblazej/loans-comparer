namespace dotNetAPI.DTO
{
    public class AddCarDTO
    {
        public string ImageUrl { get; set; }
        public string brandName { get; set; }
        public string modelName { get; set; }
        public int year { get; set; }
        public int enginePower { get; set; }
        public string enginePowerType { get; set; }
        public int capacity { get; set; }
        public string description { get; set; }
        public float price { get; set; }
        public string currency { get; set; }
    }
}
