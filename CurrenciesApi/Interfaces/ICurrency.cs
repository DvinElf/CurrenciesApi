using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrenciesApi.Interfaces
{
    interface ICurrency
    {
        
        string ID { get; set; }
       string NumCode { get; set; }
       string CharCode { get; set; }
       int Nominal { get; set; }
       string Name { get; set; }
        float Value { get; set; }
        float Previous { get; set; }
    }
}
