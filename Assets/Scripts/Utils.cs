using UnityEngine;
using System.Collections;

public class Utils {
    private const int currencyValueInFood = 100;
    private const int currencyValueInWood = 100;

    //convert a resource cost into currency cost
    public static int ResourceToCurrency(Resource resource) {
        if (resource.IsEmpty()) { return 0; }
        int currency = 0;

        //manually rounding up to next int witgout float conversion
        currency += ((resource.wood - 1) / currencyValueInWood) + 1;
        currency += ((resource.food - 1) / currencyValueInFood) + 1;
        return currency;
    }
}
