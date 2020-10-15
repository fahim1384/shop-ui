using System;
using System.Collections.Generic;
using System.Text;

namespace HandiCrafts.Core.Enums
{
    public enum TransactionPaymentStatus : byte
    {
        Start = 1,
        Paid = 2,
        PaidBut = 3,
        Failed = 4
    }
}
