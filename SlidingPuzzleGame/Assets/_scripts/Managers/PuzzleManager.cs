using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public bool GameStarted {  get; private set; }
    public Grid CurrentGrid { get; private set; }
    public Difficult CurrentDifficult {  get; private set; }

    [SerializeField]
    private Grid[] _grids;
    
    [SerializeField]
    private Button[] _selectGridButtons;
    [SerializeField]
    private Dropdown _difficultDropdown;

    private int _iterations;
    private Tile _emptyTile;
    private TileCheckResult _resultInfo;
    private List<Tile> _currentGridTiles = new List<Tile>();
    private List<int> _remainingNumbers = new List<int>();
    private void Awake()
    {
        if (_grids == null)
        {
            Debug.LogError("Grids not assigned on the inspector", this);
            enabled = false;
            return;
        }

        if (_selectGridButtons == null)
        {
            Debug.LogError("Select Grid Buttons not assigned on the inspector", this);
            enabled = false;
            return;
        }

        if (_grids.Length != _selectGridButtons.Length)
        {
            Debug.LogError("Grid Length and Selected Grid Buttons Length are not the same in the Inspector.", this);
            enabled = false;
            return;
        }

        if (_difficultDropdown == null)
        {
            Debug.LogError("Difficult Dropdown not assigned on the inspector", this);
            enabled = false;
            return;
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < _selectGridButtons.Length; i++)
        {
            int index = i;
            _selectGridButtons[i].onClick.AddListener(() => SetGrid(_grids[index]));
        }

        _difficultDropdown.onValueChanged.AddListener((int difficult) => SetDifficult(difficult));
    }
    private void OnDisable()
    {
        if(_grids.Length == _selectGridButtons.Length)
        {
            for (int i = 0; i < _selectGridButtons.Length; i++)
            {
                _selectGridButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
    public void CheckTileMovement(Tile selectedTile, Tile targetTile)
    {
        bool isNeighboringTileValidated = false;
        
        if (targetTile == selectedTile || targetTile == null) //this check is used when the player doesn't drag a tile
        {
            _resultInfo = GetTilesInfo(selectedTile, _emptyTile);

            if(_resultInfo == null)
            {
                return;
            }

            if (!_resultInfo.IsEmptyTileDirectlyAdjacent)
            {
                List<Tile> candidateTiles = selectedTile.CheckAdjacentTiles(_resultInfo.Direction);
                int tilesCount = candidateTiles.Count;

                if (candidateTiles != null)
                {
                    for (int i = 0; i < tilesCount - 1; i++)
                    {
                        if (AreTilesAdjacent(candidateTiles[candidateTiles.Count - 2], candidateTiles[candidateTiles.Count - 1]))
                        {
                            SwapTilesPositions(candidateTiles[candidateTiles.Count - 2], candidateTiles[candidateTiles.Count - 1]);
                            candidateTiles.RemoveAt(candidateTiles.Count - 2);
                        }
                    }
                }
            }

            targetTile = _emptyTile;

            isNeighboringTileValidated = true;
        }

        if (!isNeighboringTileValidated)
        {
            if (targetTile.TileType == TileType.StandardTile)
            {
                return;
            }
            
            if (!AreTilesAdjacent(selectedTile, targetTile))
            {
                return;
            }
        }

        SwapTilesPositions(selectedTile, targetTile);
    }
    private bool AreTilesAdjacent(Tile selectedTile, Tile targetTile)
    {
        if (selectedTile.Coordinates.x == targetTile.Coordinates.x && Mathf.Abs(selectedTile.Coordinates.y - targetTile.Coordinates.y) == 1)
        {
            return true;
        }

        if (selectedTile.Coordinates.y == targetTile.Coordinates.y && Mathf.Abs(selectedTile.Coordinates.x - targetTile.Coordinates.x) == 1)
        {
            return true;
        }

        return false;
    }
    private TileCheckResult GetTilesInfo(Tile selectedTile, Tile emptyTile)
    {
        TileCheckResult result = new TileCheckResult();

        if (selectedTile.Coordinates.x == emptyTile.Coordinates.x)
        {
            switch(Mathf.Abs(selectedTile.Coordinates.y - emptyTile.Coordinates.y))
            {
                case 1:
                    result.IsEmptyTileDirectlyAdjacent = true;
                    return result;
                default:
                    result.IsEmptyTileDirectlyAdjacent = false;
                    if (selectedTile.Coordinates.y < emptyTile.Coordinates.y)
                    {
                        result.Direction = Vector2.right;
                        return result;
                    }
                    else
                    {
                        result.Direction = Vector2.left;
                        return result;
                    }
            }
        }

        if (selectedTile.Coordinates.y == emptyTile.Coordinates.y)
        {
            switch(Mathf.Abs(selectedTile.Coordinates.x - emptyTile.Coordinates.x))
            {
                case 1:
                    result.IsEmptyTileDirectlyAdjacent = true;
                    return result;
                default:
                    result.IsEmptyTileDirectlyAdjacent = false;
                    if(selectedTile.Coordinates.x < emptyTile.Coordinates.x)
                    {
                        result.Direction = Vector2.down;
                        return result;
                    }
                    else
                    {
                        result.Direction= Vector2.up;
                        return result;
                    }
            }
        }

        return null;
    }
    private void SwapTilesPositions(Tile startTile, Tile endTile)
    {
        Vector2 startPosition = startTile.transform.position;
        Vector2 endPosition = endTile.transform.position;

        Vector2 startCoordinates = startTile.Coordinates;
        Vector2 endCoordinates = endTile.Coordinates;

        int startCurrentNumberPosition = startTile.CurrentNumberPosition;
        int endCurrentNumberPosition = endTile.CurrentNumberPosition;

        startTile.SetTilePositionAndCoordinates(endPosition, endCoordinates, endCurrentNumberPosition);
        endTile.SetTilePositionAndCoordinates(startPosition, startCoordinates, startCurrentNumberPosition);
    }
    private void InitializeTilesInOrder()
    {
        _remainingNumbers.Clear();
        _currentGridTiles.Clear();

        for (int i = 0; i < CurrentGrid.transform.childCount; i++)
        {
            _remainingNumbers.Add(i + 1);

            Transform tileTransform = CurrentGrid.transform.GetChild(i);
            if (tileTransform.TryGetComponent<Tile>(out Tile tile))
            {
                tile.SetCurrentNumberPosition(i + 1);
                tile.SetPuzzleManager(this);
                _currentGridTiles.Add(tile);
            }
        }

        for (int i = 0; i < _currentGridTiles.Count; i++)
        {
            _currentGridTiles[i].SetCurrentNumber(i + 1);
        }
    }
    private void ShuffleTilesByValidMoves(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            List<Tile> validNeighbors = new List<Tile>();

            foreach (Tile tile in _currentGridTiles)
            {
                if (AreTilesAdjacent(tile, _emptyTile))
                {
                    validNeighbors.Add(tile);
                }
            }

            if (validNeighbors.Count > 0)
            {
                Tile randomNeighbor = validNeighbors[Random.Range(0, validNeighbors.Count)];
                SwapTilesPositions(randomNeighbor, _emptyTile);
            }
        }
    }
    public void CheckTilesPosition()
    {
        int tilesPositionedCorrectly = 0;

        foreach(Tile tile in _currentGridTiles)
        {
            if(tile.Number == tile.CurrentNumberPosition)
            {
                tilesPositionedCorrectly++;
            }
        }

        if(tilesPositionedCorrectly == _currentGridTiles.Count)
        {
            Debug.Log("won");
        }
    }
    private void SetGrid(Grid currentGrid)
    {
        CurrentGrid = currentGrid;
        _iterations = CurrentGrid.Iterations;
        CurrentGrid.transform.GetChild(CurrentGrid.transform.childCount - 1).TryGetComponent<Tile>(out Tile lastTile);
        _emptyTile = lastTile;
        InitializeTilesInOrder();
        ShuffleTilesByValidMoves(_iterations);
        for(int i = 0; i < _selectGridButtons.Length; i++)
        {
            _selectGridButtons[i].gameObject.SetActive(false);
        }
        CurrentGrid.gameObject.SetActive(true);
        GameStarted = true;
    }
    private void SetDifficult(int difficult)
    {
        switch (difficult)
        {
            case 0:
                CurrentDifficult = Difficult.Easy;
                break;
            case 1:
                CurrentDifficult = Difficult.Medium;
                break;
            case 2:
                CurrentDifficult = Difficult.Hard;
                break;
            default:
                CurrentDifficult = Difficult.Easy;
                break;
        }
    }
}
