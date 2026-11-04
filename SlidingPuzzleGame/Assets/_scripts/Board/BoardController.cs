using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private PuzzleManager _puzzleManager;

    [SerializeField]
    private LayerMask _tileLayerMask;

    private InputReader _inputReader;
    private Camera _camera;
    private Vector2 _currentScreenPosition
    {
        get { return _inputReader.PointInput; }
    }
    private Tile _hit
    {
        get { return Hit(); }
    }

    private Tile _currentTile;
    private void Awake()
    {
        _inputReader = InputReader.Instance;
        _camera = Camera.main;

        if(_puzzleManager == null)
        {
            Debug.LogError("PuzzleManager not defined in the inspector", this);
            enabled = false;
        }
    }
    private void OnEnable()
    {
        if (_inputReader != null)
        {
            _inputReader.PressInputStarted += IsTileHit;
            _inputReader.PressInputCanceled += RequestTileMovement;
        }
    }
    private void OnDisable()
    {
        if (_inputReader != null)
        {
            _inputReader.PressInputStarted -= IsTileHit;
            _inputReader.PressInputCanceled -= RequestTileMovement;
        }
    }
    private void IsTileHit()
    {
        if (_hit == null)
        {
            return;
        }

        if (_hit.TileType == TileType.EmptyTile)
        {
            return;
        }

        _currentTile = _hit;
    }
    private void RequestTileMovement()
    {
        if(_currentTile == null)
        {
            return;
        }

        if(_hit == null)
        {
            return;
        }

        if (_hit.transform == _currentTile)
        {
            return;
        }

        _puzzleManager.CheckTileMovement(_currentTile, _hit);
    }
    private Tile Hit()
    {
        Vector3 worldPosition = _camera.ScreenToWorldPoint(_currentScreenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, _tileLayerMask);

        if(hit == null)
        {
            return null;
        }

        if (hit.TryGetComponent<Tile>(out Tile tile))
        {
            return tile;
        }

        return null;
    }
}
