using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public bool GameStarted {  get; private set; }

    [SerializeField]
    private Transform _grid;
    [SerializeField]
    private Tile _emptyTile;
    [Range(0, 200)]
    [SerializeField]
    private int iterations = 100;

    private TileCheckResult _resultInfo;
    private List<Tile> _currentGridTiles = new List<Tile>();
    private List<int> _remainingNumbers = new List<int>();
    private void Start()
    {
        InitializeTilesInOrder();
        ShuffleTilesByValidMoves(iterations);
        GameStarted = true;
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
                Tile candidateTile = selectedTile.CheckAdjacentTiles(_resultInfo.Direction);

                if(candidateTile != null && AreTilesAdjacent(candidateTile, _emptyTile))
                {
                    SwapTilesPositions(candidateTile, _emptyTile);
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

        for (int i = 0; i < _grid.childCount; i++)
        {
            _remainingNumbers.Add(i + 1);

            Transform tileTransform = _grid.GetChild(i);
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
}
