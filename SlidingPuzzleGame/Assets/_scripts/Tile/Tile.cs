using TMPro;
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
    public int CurrentNumberPosition { get; private set; }
    public int Number { get; private set; }

    [SerializeField]
    private TileType _tileType;
    [SerializeField]
    private float _raycastDistance = 3f;
    [SerializeField]
    private Vector2 _coordinates;
    [SerializeField]
    private TextMeshProUGUI _numberText;

    private Collider2D _collider2D;
    private PuzzleManager _puzzleManager;
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
    public void SetTilePositionAndCoordinates(Vector2 newPosition, Vector2 newCoordinates, int numberPosition)
    {
        transform.position = newPosition;
        _coordinates = newCoordinates;
        CurrentNumberPosition = numberPosition;

        if(CurrentNumberPosition == Number)
        {
            if(_puzzleManager != null)
            {
                _puzzleManager.CheckTilesPosition();
            }
        }
    }
    public void SetCurrentNumber(int currentNumber)
    {
        Number = currentNumber;
        _numberText.text = Number.ToString();
    }
    public void SetCurrentNumberPosition(int numberPosition)
    {
        CurrentNumberPosition = numberPosition;
    }
    public void SetPuzzleManager(PuzzleManager puzzleManager)
    {
        _puzzleManager = puzzleManager;
    }
}
