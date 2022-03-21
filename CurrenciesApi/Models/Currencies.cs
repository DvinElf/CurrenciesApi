using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrenciesApi.Models
{
    /// <summary>
    /// Singleton-объект, содержащий в себе список курсов валют, а также необходимые для работы с ним методы.
    /// </summary>
    public class Currencies
    {

        private static Currencies instance;
        private static object syncRoot = new Object();

        private List<Currency> currencies { get; set; }

        private Currencies()
        {
            currencies = new();
        }

        /// <summary>
        /// Метод, позволяющий обращение к Singleton-объекту
        /// </summary>
        public static Currencies getInstance()
        {
            if (instance == null)
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new Currencies();
                }
            return instance;
        }
        /// <summary>
        /// Метод, возвращающий список курсов валют
        /// </summary>
        /// <returns>Спикос курсов валют</returns>
        public List<Currency> GetCurrencies ()
        {
            return currencies;
        }
        /// <summary>
        /// Метод, возвращающий курс конкретной валюты по CharCode
        /// </summary>
        /// <param name="Code">CharCode валюты</param>
        /// <returns>Курс выбранной валюты</returns>
        public Currency GetCurrency(string Code)
        {
            return currencies.Where(i => i.CharCode == Code).FirstOrDefault(); ;
        }

        /// <summary>
        /// Метод, задающий список курсов валют на основании JSon-файла
        /// </summary>
        /// <param name="tcurrencies">Список курсов валют</param>
        public void SetCurrencies(List<Currency> tcurrencies)
        {
            

            currencies = tcurrencies;
        }

        /// <summary>
        /// Метод, реализующий пагинацию списка курсов валют
        /// </summary>
        /// <param name="Page">Выбранная страница</param>
        /// <param name="PageSize">Число объектов на странице</param>
        /// <returns>Страницу с выбранным количеством курсов валют</returns>
        public List<Currency> Pagination (int Page, int PageSize)
        {
            return currencies.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
        }

        /// <summary>
        /// Метод, возвращающий общее число страниц
        /// </summary>
        /// <param name="PageSize">Число объектов на странице</param>
        /// <returns>Число страниц</returns>
        public int Pages(int PageSize) 
        {
            return (int) Math.Ceiling((currencies.Count() / (decimal)PageSize));
        }

    }
}
