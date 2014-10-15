using UnityEngine;
using System.Collections;

public class Utils {
    private const int currencyValueInFood = 100;
    private const int currencyValueInWood = 100;

    //convert a resource cost into currency cost
    public static Resource ReplaceResourcesByCurrency(Resource resource) {
        int currency = 0;
        //manually rounding up to next int without float conversion (only works for numbers > 0)
        if (resource.wood > 0) { currency += ((resource.wood - 1) / currencyValueInWood) + 1; }
        if (resource.food > 0) { currency += ((resource.food - 1) / currencyValueInFood) + 1; }
        return new Resource(0, 0, currency);
    }
}
