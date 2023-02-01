namespace dotNetAPI
{
    public class CarDTO
    {
        public string id { get; set; }
        public string ImageUrl { get; set; }
        public string brandName { get; set; }
        public string modelName { get; set; }
        public int year { get; set; }
        public int enginePower { get; set; }
        public string enginePowerType { get; set; }
        public int capacity { get; set; }
        public string description { get; set; }
        public string currency { get; set; }
        public string company { get; set; }

        public CarDTO()
        {

        }
    }
}
