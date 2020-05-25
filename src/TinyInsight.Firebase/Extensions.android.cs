using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;

namespace TinyInsightsLib.Firebase
{
    internal static class Extensions
    {
        internal static Bundle ToBundle(this Dictionary<string, string> dictionary)
        {
            var bundle = new Bundle();

            foreach(var item in dictionary)
            {
                bundle.PutString(item.Key, item.Value);
            }

            return bundle;
        }
    }
}
