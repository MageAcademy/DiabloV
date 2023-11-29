using UnityEngine;
using UnityEngine.UI;

public class CellEntry : MonoBehaviour
{
    public static CellEntry[,] Instances = null;

    public static Vector2Int LastCoordinate = new Vector2Int(-1, -1);

    public static Vector2Int Length = new Vector2Int();

    public Image image = null;

    private Color colorOff = new Color(0f, 0f, 0f, 0.2f);

    private Color colorOn = new Color(1f, 1f, 1f, 0.2f);

    private Vector2Int coordinate = new Vector2Int();


    public void Initialize(Vector2Int coordinate)
    {
        this.coordinate = coordinate;
        transform.position = GetPositionByCoordinate(coordinate);
        ToggleOff();
    }


    public void ToggleOff()
    {
        image.color = colorOff;
    }


    public void ToggleOn()
    {
        if (IsValid(LastCoordinate))
        {
            Instances[LastCoordinate.x, LastCoordinate.y].ToggleOff();
        }

        LastCoordinate = coordinate;
        image.color = colorOn;
    }


    public static Vector2Int GetCoordinateByPosition(Vector3 position)
    {
        return new Vector2Int((int)(position.x + Length.x / 2f), (int)(position.z + Length.y / 2f));
    }


    public static Vector3 GetPositionByCoordinate(Vector2Int coordinate)
    {
        return new Vector3(coordinate.x - (Length.x - 1f) / 2f, 0f, coordinate.y - (Length.y - 1f) / 2f);
    }


    public static bool IsValid(Vector2Int coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < Length.x && coordinate.y >= 0 && coordinate.y < Length.y;
    }


    public static bool IsValid(Vector3 position)
    {
        return IsValid(GetCoordinateByPosition(position));
    }
}