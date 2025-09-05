using Microsoft.AspNetCore.Mvc;

namespace BusServices.Dtos
{
    public class BusQueryParams
    {
       
       public string? Search { set; get; }

       public string? FromPlace { set; get; }

       public string? ToPlace { set; get; }

       public int? BusTypeId { set; get; }

        public int? BusCategoryId { set; get; }

        public string? SortBy { set; get; } = "TravelDate";

        public string SortDirection { set; get; }  = "asc";

        public int Page { set; get; } = 1;

        public int PageSize { set; get; } = 10;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }
}
