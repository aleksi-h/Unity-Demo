
[System.Serializable]
public class Resource
{
    public int wood;
    public int food;

    public Resource(int wood, int food)
    {
        this.wood = wood;
        this.food = food;
    }

    #region operator overloading
    public static Resource operator +(Resource r1, Resource r2)
    {
        return new Resource(r1.wood + r2.wood, r1.food + r2.food);
    }

    public static Resource operator -(Resource r1, Resource r2)
    {
        return new Resource(r1.wood - r2.wood, r1.food - r2.food);
    }

    public static bool operator >=(Resource r1, Resource r2)
    {
        return (r1.wood >= r2.wood && r1.food >= r2.food);
    }

    public static bool operator <=(Resource r1, Resource r2)
    {
        return (r1.wood <= r2.wood && r1.food <= r2.food);
    }
    #endregion
}
