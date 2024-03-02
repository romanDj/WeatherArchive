using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using WeatherArchive.Data;
using WeatherArchive.Models;
using WeatherArchive.Models.ArchiveView;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WeatherArchive.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> ArchiveView(string? month, string? year, int page = 1)
        {
            int pageSize = 15;

            //����������
            IQueryable<Weather> weathers = from s in _context.Weathers
                                     orderby s.Date
                                     select s;
       
            //�� ������
            if (!String.IsNullOrEmpty(month))
            {
                weathers = weathers.Where(p => p.Date.Month == Convert.ToInt32(month));
            }
            //�� ����
            if (!String.IsNullOrEmpty(year))
            {
                weathers = weathers.Where(p => p.Date.Year == Convert.ToInt32(year));
            }

            // ���������
            var count = await weathers.CountAsync();

            // �������� ���������� ����
            IEnumerable<int> years = await _context.Weathers.Select(x => x.Date.Year).Distinct().OrderBy(x => x).ToListAsync();
            // �������� ���������� ������
            IEnumerable<int> months = await _context.Weathers.Select(x => x.Date.Month).Distinct().OrderBy(x => x).ToListAsync();           

            // ��������� ������ �������������
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),                
                FilterViewModel = new FilterViewModel(years, months, year, month),
                Weathers = weathers.Skip((page - 1) * pageSize).Take(pageSize)
            };
            
            return View(viewModel);
        }

        /// <summary>
        /// �������� ��������
        /// </summary>
        /// <returns></returns>
        public IActionResult Load() 
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
