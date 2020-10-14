using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TulipWpfUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        private readonly IConfiguration _config;

        public ConfigHelper(IConfiguration config)
        {
            _config = config;
        }
        public decimal GetTaxRate()
        {

            string rateText = ConfigurationManager.AppSettings["taxRate"]; // WPF UI
            if (rateText == null)
            {
                rateText = _config.GetValue<string>("taxRate"); // This is if Blazor the UI
            }

            bool IsValidTaxRate = Decimal.TryParse(rateText, out decimal output);

            if (IsValidTaxRate == false)
            {
                throw new ConfigurationErrorsException("The tax rate is not set up properly");
            }

            return output;
        }
    }
}
