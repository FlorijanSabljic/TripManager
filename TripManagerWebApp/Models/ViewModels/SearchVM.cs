namespace TripManagerWebApp.Models.ViewModels
{
    public class SearchVM
    {
        public string? Query { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public string? OrderBy { get; set; }

        public int LastPage { get; set; }
        public int FromPager { get; set; }
        public int ToPager { get; set; }
        public List<TripVM> Trips { get; set; }
        public string Submit { get; set; }
    }
}
