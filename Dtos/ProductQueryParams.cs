namespace SupplyChain.DTOs
{
    public class ProductQueryParams
    {
        //public int? TypeId { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; } = "name"; // default
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime FromDate { set;get; } = DateTime.MinValue;
        public DateTime ToDate { set; get; } = DateTime.MinValue;
        public string FromPlace { set; get; } = string.Empty;
        public string ToPlace { set; get; } = string.Empty;

    }
}
