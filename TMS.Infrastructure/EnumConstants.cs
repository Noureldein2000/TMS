using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Infrastructure
{
    class EnumConstants
    {

    }

    public enum BalanceTypes : short
    {
        MomkmBalance = 1,
        CashBalance = 2
    }

    public enum ActiveStatus : short
    {
        True = 1,
        False = 0
    }

    public enum TransactionType 
    {
        Increment = 1,
        Decrement = 2
    }
}
