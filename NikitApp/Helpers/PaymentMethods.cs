using System.Collections.Generic;

namespace NikitApp.Helpers;

public static class PaymentMethods
{
    public static List<string> List = [ CASH, NON_CASH ];

    public const string CASH = "Наличными";
    public const string NON_CASH = "Безнал";
}