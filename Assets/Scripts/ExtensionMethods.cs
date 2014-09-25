using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
       public static T GetInterface<T>(this GameObject obj) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            Debug.LogError(typeof(T).ToString() + ": is not an interface!");
            return null;
        }
        return obj.GetComponents<Component>().OfType<T>().FirstOrDefault();
    }
}
