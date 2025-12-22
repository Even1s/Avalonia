using System.Collections.Generic;

namespace CafeApp.Helpers;

public static class UserStatuses
{
    public static List<string> List = [ USER_WORKING, USER_FIRED ];
    
    public const string USER_WORKING = "Работает";
    public const string USER_FIRED = "Уволен";
}