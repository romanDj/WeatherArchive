namespace WeatherArchive.Models.ArchiveView
{
    public class IndexViewModel
    {
        public IQueryable<Weather> Weathers { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}
