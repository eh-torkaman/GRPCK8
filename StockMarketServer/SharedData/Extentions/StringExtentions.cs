using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Extentions
{
    public static class StringExtentions
    {
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }
    }
}
