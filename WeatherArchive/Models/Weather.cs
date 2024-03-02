using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WeatherArchive.Models
{
    [Index(nameof(Date))]
    public class Weather
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// Температура
        /// </summary>
        public double Temp { get; set; }
        /// <summary>
        /// Отн. влажность воздуха
        /// </summary>
        public double AirHumidity { get; set; }
        /// <summary>
        /// Точка росы
        /// </summary>
        public double DewPoint { get; set; }
        /// <summary>
        /// Атм. давление
        /// </summary>
        public double AtmPressure{ get; set; }
        /// <summary>
        /// Направление ветра
        /// </summary>
        public string? DirWind { get; set; }
        /// <summary>
        /// Скорость ветра
        /// </summary>
        public double? SpeedWind { get; set; }
        /// <summary>
        /// Облачность
        /// </summary>
        public double? Cloudy { get; set; }
        /// <summary>
        /// Нижняя граница облачности
        /// </summary>
        public double? BottomCloudy { get; set;}
        /// <summary>
        /// Горизонтальная видимость
        /// </summary>
        public string? HorVisibility { get; set; }
        /// <summary>
        /// Погодные явления
        /// </summary>
        public string? WeatherCond { get; set; }

        /// <summary>
        /// Получение даты
        /// </summary>
        /// <returns>Дата</returns>
        public string GetDate() 
        {
            return Date.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Получение времени
        /// </summary>
        /// <returns>Время</returns>
        public string GetTime() 
        {
            return Date.ToString("HH:mm");
        }
    }
}
