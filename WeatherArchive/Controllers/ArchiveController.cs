using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Concurrent;
using System.IO;
using WeatherArchive.Data;
using WeatherArchive.Models;
using System.Text;
using System.Globalization;

namespace WeatherArchive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchiveController : ControllerBase
    {
        private IServiceProvider _serviceProvider;
        private readonly AppDbContext _context;
        public ArchiveController(AppDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }


        /// <summary>
        /// Загрузка архива
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                if (Request.Form.Files.Count == 0)
                    return BadRequest("Требуется выбрать минимум 1 файл.");

                //непустые файлы
                var files = Request.Form.Files.Where(x => x.Length > 0);

                //обходим файлы параллельно
                await Parallel.ForEachAsync(files, (file, token) =>
                {
                    using var fileStream = file.OpenReadStream();
                    IWorkbook workbook;

                    if (file.FileName.EndsWith(".xls"))
                        workbook = new HSSFWorkbook(fileStream);
                    else if (file.FileName.EndsWith(".xlsx"))
                        workbook = new XSSFWorkbook(fileStream);
                    else
                        throw new Exception("Неподдерживаемый формат файла");

                    //обходим страницы параллельно
                    Parallel.For(0, workbook.NumberOfSheets, (i) =>
                    {
                        ISheet sheet = workbook.GetSheetAt(i);

                        //получаем отдельный экземпляр для доступа к бд
                        using var _scope = _serviceProvider.CreateScope();
                        using var _ctx = _scope.ServiceProvider.GetService<AppDbContext>();
                    
                        //отключаем автообнаружение изменений
                        _ctx.ChangeTracker.AutoDetectChangesEnabled = false;
                        // отключить отслеживание изменений
                        _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                        //формируем запрос на вставку (SaveChanges работает медленно)
                        StringBuilder rawSqlInsert = new StringBuilder();
                        rawSqlInsert.Append(@"INSERT INTO Weathers (Date, Temp, AirHumidity, DewPoint, AtmPressure, DirWind, SpeedWind, Cloudy, BottomCloudy, HorVisibility, WeatherCond) 
                                SELECT * FROM (VALUES ");

                        for (int row = 4; row <= sheet.LastRowNum; row++)
                        {
                            IRow curRow = sheet.GetRow(row);

                            if (curRow == null)
                                continue;

                            Weather weather = new()
                            {
                                Date = DateTime.ParseExact(curRow.GetCell(0).StringCellValue + " " + curRow.GetCell(1).StringCellValue, "dd.MM.yyyy HH:mm", null),
                                Temp = (double)ConvertNumeric(curRow.GetCell(2)),
                                AirHumidity = (double)ConvertNumeric(curRow.GetCell(3)),
                                DewPoint = (double)ConvertNumeric(curRow.GetCell(4)),
                                AtmPressure = (double)ConvertNumeric(curRow.GetCell(5)),
                                DirWind = curRow.GetCell(6)?.StringCellValue,
                                SpeedWind = ConvertNumeric(curRow.GetCell(7)),
                                Cloudy = ConvertNumeric(curRow.GetCell(8)),
                                BottomCloudy = ConvertNumeric(curRow.GetCell(9)),
                                HorVisibility = ConvertString(curRow.GetCell(10)),
                                WeatherCond = curRow.GetCell(11)?.StringCellValue
                            };                          
                            rawSqlInsert.Append(String.Format(CultureInfo.InvariantCulture, @"('{0:yyyy-MM-dd HH:mm:ss}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})",
                                   weather.Date, weather.Temp, weather.AirHumidity, weather.DewPoint, weather.AtmPressure,
                                   weather.DirWind != null ? $"'{weather.DirWind}'" : "NULL",
                                   weather.SpeedWind != null ? $"'{weather.SpeedWind}'" : "NULL",
                                   weather.Cloudy != null ? $"'{weather.Cloudy}'" : "NULL",
                                   weather.BottomCloudy != null ? $"'{weather.BottomCloudy}'" : "NULL",
                                   weather.HorVisibility != null ? $"'{weather.HorVisibility}'" : "NULL",
                                   weather.WeatherCond != null ? $"'{weather.WeatherCond}'" : "NULL"));

                            if (row != sheet.LastRowNum)
                            {
                                rawSqlInsert.Append(",");
                            }
                        }
                        rawSqlInsert.Append(")  vals(Date, Temp, AirHumidity, DewPoint, AtmPressure, DirWind, SpeedWind, Cloudy, BottomCloudy, HorVisibility, WeatherCond)");                     
                        _ctx.Database.ExecuteSqlRaw(rawSqlInsert.ToString());
                    });
                    workbook.Close();

                    return ValueTask.CompletedTask;
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получение числа из ячейки
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static double? ConvertNumeric(ICell cell)
        {
            if (cell == null)
                return null;

            if (cell.CellType == CellType.Numeric)
            {
                return cell.NumericCellValue;
            }
            else if (cell.StringCellValue.Trim().Length > 0)
            {
                return Double.Parse(cell.StringCellValue);
            }
            return null;
        }
        /// <summary>
        /// Получение строки из ячейки
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string? ConvertString(ICell cell)
        {
            if (cell == null)
                return null;

            if (cell.CellType == CellType.Numeric)
            {
                return cell.NumericCellValue.ToString();
            }
            else if (cell.StringCellValue.Trim().Length > 0)
            {
                return cell.StringCellValue;
            }
            return null;
        }
    }
}
