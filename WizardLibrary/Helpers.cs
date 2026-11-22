using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WizardLibrary
{
    static public class Helpers
    {
        public static bool IsNull<T> (this T input)
        {
            return input == null;
        }

        public static bool IsNullOrEmpty<T>(this T input) where T : IEnumerable<T>
        {
            return (input == null) || (input.Count() == 0);
        }
    }

}
