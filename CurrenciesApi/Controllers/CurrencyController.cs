using CurrenciesApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrenciesApi.Controllers
{

    /// <summary>
    /// Главный контролер. Работает с Get-запросами, способен возвращать список курсов валют либо курс конкретной валюты.
    /// </summary>
    [ApiController]
    public class CurrencyController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;   
        /// <summary>
        /// В конструкторе контроллера передает HTTPClientFactory используемая для работы с HTTPClient через Dependency Injection
        /// </summary>
        /// <param name="clientFactory"></param>
        public CurrencyController (IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Обработчик Get-запроса, возвращающий курсы валют по данным центробанка. Поддерживает пагинацию.
        /// </summary>
        /// <param name="pagination">Параметр, указывающий, нужна ли пагинация</param>
        /// <param name="Page">Текущая страница при применении пагинации</param>
        /// <param name="PageSize">Количество записей на странице</param>
        /// <returns>
        /// Возвращает курсы валют в формате JSON. В заголовках помещаются данные о последнем обновлении и пагинации.
        /// </returns>
        [HttpGet]
        [Route("api/currencies")]
        public async Task<IActionResult> CurrenciesGet([FromQuery] bool pagination=false, int Page=1, int PageSize = 5)
        {


            await CheckCurrenciesAsunc();

            Response.Headers.Append("Updated", UpdateTime.getInstance().LastUpdate.ToString());
            Response.Headers.Append("DocData", UpdateTime.getInstance().DocTime.ToString());
            if (pagination)
            {
                Response.Headers.Append("PageNumber", Page.ToString());
                Response.Headers.Append("PageSize", PageSize.ToString());
                Response.Headers.Append("Pages", Currencies.getInstance().Pages(PageSize).ToString());
                return new JsonResult(Currencies.getInstance().Pagination(Page, PageSize));
            }
            else
                return new JsonResult(Currencies.getInstance().GetCurrencies());

        }

        /// <summary>
        /// Возвращает курс конкретной валюты.
        /// </summary>
        /// <param name="Code">CharCode необходимой валюты</param>
        /// <returns>Возвращает курс выбранной валюты</returns>
        [HttpGet]
        [Route("api/currency/")]
        public async Task<IActionResult> CurrencyAsync([FromQuery] string Code="USD")
        {
            await CheckCurrenciesAsunc();
            Response.Headers.Append("Updated", UpdateTime.getInstance().LastUpdate.ToString());
            Response.Headers.Append("DocData", UpdateTime.getInstance().DocTime.ToString());
            return new JsonResult (Currencies.getInstance().GetCurrency(Code));
        }


        /// <summary>
        /// Метод, обращающийся к сайту центробанка и сохраняющий полученную информацию по курсам валют в singleton-объект
        /// </summary>
        /// /// <remarks>
        /// Для экономии времени при повторных обращениях, а также ограниченной подстраховки на случай недоступности сайта центробанка, 
        /// повторный запрос к центробанку происходит только раз в день, после обновления курсов, либо при запуске Api. 
        /// </remarks>
        [NonAction]
        public async Task CheckCurrenciesAsunc ()
        {

            DateTime tDocTime = UpdateTime.getInstance().DocTime;
            DateTime tNow = DateTime.UtcNow.AddHours(3);
            if ((!Currencies.getInstance().GetCurrencies().Any()) || (((tNow.Hour > 11) || (tNow.Hour > 10 && tNow.Minute > 30)) &&
                ((tDocTime.Day < tNow.Day) ||
                (tDocTime.Month < tNow.Month) ||
                (tDocTime.Year < tNow.Year))))
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
            "https://www.cbr-xml-daily.ru/daily_json.js");

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);


                var responseStream = await response.Content.ReadAsStreamAsync();

                byte[] responseBytes = new byte[responseStream.Length];
                await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                using JsonDocument valuteDoc = JsonDocument.Parse(Encoding.UTF8.GetString(responseBytes));
                UpdateTime.getInstance().DocTime = DateTime.Parse(valuteDoc.RootElement.GetProperty("Date").GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                UpdateTime.getInstance().LastUpdate = DateTime.UtcNow.AddHours(3);
                List<Currency> currencies = new();
                JObject jObj = (JObject)JsonConvert.DeserializeObject(valuteDoc.RootElement.GetProperty("Valute").ToString());

                foreach (JProperty f in jObj.Properties().ToList())
                {
                    Currency currency = JsonConvert.DeserializeObject<Currency>(f.Value.ToString());
                    currencies.Add(currency);
                }
                Currencies.getInstance().SetCurrencies(currencies);
            }
        }

    }
}
