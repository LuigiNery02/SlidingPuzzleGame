using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType TileType
    {
        get { return _tileType; }
    }
    public Vector2 Coordinates
    {
        get { return _coordinates; }
    }

    [SerializeField]
    private TileType _tileType;
    [SerializeField]
    private float _raycastDistance = 3f;
    [SerializeField]
    private Vector2 _coordinates;

    private Collider2D _collider2D;
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();

        if(_collider2D == null)
        {
            Debug.LogError("Collider2D not found in the inspector", this);
            enabled = false;
        }
    }
    public Tile CheckAdjacentTiles(Vector2 direction)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, _raycastDistance, 1 << gameObject.layer);

        for(int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider == _collider2D)
            {
                continue;
            }

            if (hit[i].transform.TryGetComponent<Tile>(out Tile adjacentTile))
            {
                return adjacentTile;
            }
        }

        return null;
    }
    public void SetTilePositionAndCoordinates(Vector2 newPosition, Vector2 newCoordinates)
    {
        transform.position = newPosition;
        _coordinates = newCoordinates;
    }
}
