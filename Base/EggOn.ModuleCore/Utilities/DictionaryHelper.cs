using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowOptions.EggOn.ModuleCore.Utilities
{
    public static class DictionaryHelper
    {
        /// <summary>
        /// Gets value from dictionary or the default value if key is not found.
        /// </summary>
        /// <param name="key">The key to get the value.</param>
        /// <param name="defaultValue" >The default value to return, the default is the default type value.</param>
        /// <returns>The value in the dictionary.</returns>
        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dic, K key, V defaultValue = default(V))
        {
            V ret;
            
            var found = dic.TryGetValue(key, out ret);
            
            if (found)
                return ret;

            return defaultValue;
        }
    }
}
