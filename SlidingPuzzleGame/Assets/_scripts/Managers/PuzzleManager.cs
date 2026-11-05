using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private Tile _emptyTile;
    public void CheckTileMovement(Tile currentTile, Tile neighboringTile)
    {
        bool neighboringChecked = false;

        if(neighboringTile == currentTile || neighboringTile == null)
        {
            if (!CheckCoordinates(currentTile, _emptyTile))
            {
                return;
            }

            neighboringTile = _emptyTile;

            neighboringChecked = true;
        }

        if (!neighboringChecked)
        {
            if (neighboringTile.TileType == TileType.StandardTile)
            {
                return;
            }

            if (!CheckCoordinates(currentTile, neighboringTile))
            {
                return;
            }
        }

        Vector2 startPosition = currentTile.transform.position;
        Vector2 endPosition = neighboringTile.transform.position;

        Vector2 startCoordinates = currentTile.Coordinates;
        Vector2 endCoordinates = neighboringTile.Coordinates;

        currentTile.transform.position = endPosition;
        currentTile.Coordinates = endCoordinates;

        neighboringTile.transform.position = startPosition;
        neighboringTile.Coordinates = startCoordinates;
    }
    private bool CheckCoordinates(Tile currentTile, Tile emptyTile)
    {
        if (currentTile.Coordinates.x == emptyTile.Coordinates.x && Mathf.Abs(currentTile.Coordinates.y - emptyTile.Coordinates.y) == 1)
        {
            return true;
        }

        if (currentTile.Coordinates.y == emptyTile.Coordinates.y && Mathf.Abs(currentTile.Coordinates.x - emptyTile.Coordinates.x) == 1)
        {
            return true;
        }

        return false;
    }
}
