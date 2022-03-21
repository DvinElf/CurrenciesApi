using CurrenciesApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrenciesApi.Models
{
    /// <summary>
    /// Класс курса валюты
    /// </summary>
    public class Currency : ICurrency
    {
        
        /// <summary>
        /// ID курса валюты
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Циферный код курса валюты
        /// </summary>
        public string NumCode { get; set; }
        /// <summary>
        /// Буквенный код курса валюты
        /// </summary>
        public string CharCode { get; set; }
        /// <summary>
        /// Номинал валюты
        /// </summary>
        public int Nominal { get; set; }
        /// <summary>
        /// Название валюты
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Курс валюты
        /// </summary>
        public float Value { get; set; }
        /// <summary>
        /// Курс валюты за прошлый день
        /// </summary>
        public float Previous { get; set; }
    }
}
