public enum HexagonDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {
    public static HexagonDirection Opposite (this HexagonDirection direction) 
    {
        return (int)direction < 3 ? direction + 3 : direction - 3;
    }
}