using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WeatherArchive.Models.ArchiveView
{
    /// <summary>
    /// Фильтры на странице просмотра архивов
    /// </summary>
    public class FilterViewModel
    {

        public FilterViewModel(IEnumerable<int>  _years, IEnumerable<int> _months, string? _year, string? _month)
        {

            Years = new List<SelectListItem>() {
                new SelectListItem() { Text= "--", Value = "" }
            };
            Years.AddRange(_years.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList());            
            Months = new List<SelectListItem>() {
                new SelectListItem() { Text= "--", Value = "" }
            };
            Months.AddRange(_months.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList());                      
            SelectedMonth = _month;
            SelectedYear = _year;
        }

        public List<SelectListItem> Years { get; private set; }
        public List<SelectListItem> Months { get; private set; }

        /// <summary>
        /// Выбранный месяц
        /// </summary>
        public string? SelectedMonth { get; private set; }
        /// <summary>
        /// Выбранный год
        /// </summary>
        public string? SelectedYear { get; private set; }
    }
}
