using System;
using System.Collections.Generic;
using System.Text;

namespace HandiCrafts.Core.Enums
{
    public enum FeeCalculationTypes:byte
    {
        Percent = 1,
        BasedOnCurrencyTransferCommission = 2,
        PriceBasedOnSystemCurrency = 3,
        PriceBasedOnUSD = 4
    }
}
