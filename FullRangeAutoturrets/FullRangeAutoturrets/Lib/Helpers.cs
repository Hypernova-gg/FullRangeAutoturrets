using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HarmonyTests.Lib
{
    public class Helpers
    {
        /// <summary>
        /// Gets the value of any field in an object using reflection, allowing getting non-public fields.
        /// </summary>
        /// <param name="obj">Object to reflect</param>
        /// <param name="name">Name of the property to fetch</param>
        /// <typeparam name="T">Generic typecast to cast the result to</typeparam>
        /// <returns>Field value casted to generic type</returns>
        public static T GetFieldValue<T>(object obj, string name) {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
        
        /// <summary>
        /// Sets any field in an object using reflection, allowing setting non-public fields.
        /// </summary>
        /// <param name="obj">Object to reflect</param>
        /// <param name="name">Name of the property to change</param>
        /// <param name="value">Value to change the property to</param>
        public static void SetFieldValue(object obj, string name, object value) {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            field?.SetValue(obj, value);
        }
    }
}