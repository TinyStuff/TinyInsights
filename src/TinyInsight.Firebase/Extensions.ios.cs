using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TinyInsightsLib.Firebase
{
    internal static class Extensions
    {
        internal static NSDictionary<TKey, TValue> ToNsDictionary<TKey,TValue>(this Dictionary<string, string> dictionary) where TKey:NSObject where TValue : NSObject
        {
            var keys = new List<NSString>();
            var values = new List<NSString>();

            foreach (var item in dictionary)
            {
                keys.Add(new NSString(item.Key));
                values.Add(new NSString(item.Value));
            }

#pragma warning disable CS0618 
            return NSDictionary<TKey, TValue>.FromObjectsAndKeys(keys.ToArray(), values.ToArray());
#pragma warning restore CS0618 
        }
    }
}
