using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private Tile _emptyTile;

    private TileCheckResult _resultInfo;
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

        startTile.SetTilePositionAndCoordinates(endPosition, endCoordinates);
        endTile.SetTilePositionAndCoordinates(startPosition, startCoordinates);
    }
}
