using Domain.Enums;

namespace Application.UseCases.Product
{
    public class ProductWithUOMDto
    {
        public string CustomerCode { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string UomName { get; set; } = null!;
        public decimal LengthInternal { get; set; }
        public decimal WidthInternal { get; set; }
        public decimal HeightInternal { get; set; }
        public decimal NetWeightInternal { get; set; }
        public decimal GrossWeightInternal { get; set; }
        public byte Status { get; set; }
        public byte IsCpart { get; set; }
        public bool IsMixed { get; set; }
        public int FloorStackability { get; set; }
        public int TruckStackability { get; set; }
    }
}
