using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrenciesApi.Models
{
    /// <summary>
    /// Singleton-объект, храанящий информацию о времени обновления данных 
    /// </summary>
    public class UpdateTime
    {
        /// <summary>
        /// Время последнего запроса, производившего повторный запрос к сайту центробанка
        /// </summary>
        public DateTime LastUpdate;
        /// <summary>
        /// Время последнего обновления информации на сайте центробанка
        /// </summary>
        public DateTime DocTime;
        private static UpdateTime instance;
        private static object syncRoot = new Object();
        private UpdateTime()
        {
            LastUpdate = new();
            DocTime = new();
        }

        /// <summary>
        /// Метод, позволяющий обращение к Singleton-объекту
        /// </summary>
        public static UpdateTime getInstance()
        {
            if (instance == null)
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new UpdateTime();
                }
            return instance;
        }
    }
}
