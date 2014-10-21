using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods {

    //UNTESTED
    public static bool ImplementsInterface<T>(this GameObject obj) where T : class {
        /*if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an interface!");
            return false;
        }*/
        //return obj.GetComponents<Component>().OfType<T>().FirstOrDefault() != null;
        return GetInterface<T>(obj) != null;
    }

    public static T GetInterface<T>(this GameObject obj) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an interface!");
            return null;
        }
        return obj.GetComponents<Component>().OfType<T>().FirstOrDefault();
    }
}
