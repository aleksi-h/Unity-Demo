
[System.Serializable]
public class Resource {
    public int wood;
    public int food;
    public int currency;

    public Resource(int wood, int food, int currency) {
        this.wood = wood;
        this.food = food;
        this.currency = currency;
    }

    public bool IsEmpty() {
        if (wood == 0 && food == 0 && currency == 0) { return true; }
        else { return false; }
    }

    #region operator overloading
    public static Resource operator +(Resource r1, Resource r2) {
        return new Resource(r1.wood + r2.wood, r1.food + r2.food, r1.currency + r2.currency);
    }

    public static Resource operator -(Resource r1, Resource r2) {
        return new Resource(r1.wood - r2.wood, r1.food - r2.food, r1.currency - r2.currency);
    }

    public static bool operator >=(Resource r1, Resource r2) {
        return (r1.wood >= r2.wood && r1.food >= r2.food && r1.currency >= r2.currency);
    }

    public static bool operator <=(Resource r1, Resource r2) {
        return (r1.wood <= r2.wood && r1.food <= r2.food && r1.currency <= r2.currency);
    }
    #endregion
}
