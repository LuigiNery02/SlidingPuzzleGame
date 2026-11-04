using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType TileType
    {
        get { return _tileType; }
    }

    [SerializeField]
    private TileType _tileType;
}
