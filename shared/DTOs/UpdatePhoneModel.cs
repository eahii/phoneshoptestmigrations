namespace Shared.DTOs
{
    public class UpdatePhoneModel
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public int? StockQuantity { get; set; }
    }
}