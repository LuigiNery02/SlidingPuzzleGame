using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private LayerMask _tileLayerMask;

    private InputReader _inputReader;
    private Camera _camera;
    private Vector2 _currentScreenPosition
    {
        get { return _inputReader.PointInput; }
    }

    private Transform _currentTile;
    private void Awake()
    {
        _inputReader = InputReader.Instance;
        _camera = Camera.main;
    }
    private void OnEnable()
    {
        if (_inputReader != null)
        {
            _inputReader.PressInputStarted += IsTileHit;
        }
    }
    private void OnDisable()
    {
        if (_inputReader != null)
        {
            _inputReader.PressInputStarted -= IsTileHit;
        }
    }
    private void IsTileHit()
    {
        Vector3 worldPosition = _camera.ScreenToWorldPoint(_currentScreenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, _tileLayerMask);
        
        if (hit)
        {
            _currentTile = hit.transform;
        }
    }
}
