using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.Utils
{
    public static class ParaUtils
    {
        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null || str.Length == 0)
            {
                return false;
            }
            return true;
        }
    }
}
