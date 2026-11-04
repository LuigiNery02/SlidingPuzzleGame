using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public void CheckTileMovement(Tile currentTile, Tile neighboringTile)
    {
        if(neighboringTile == null)
        {
            return;
            //check neighboring tiles of current tile
        }

        //check if current tile and neighboring tile are neighbors

        if(neighboringTile.TileType == TileType.StandardTile)
        {
            return;
        }

        Vector2 startPosition = currentTile.transform.position;
        Vector2 endPosition = neighboringTile.transform.position;

        currentTile.transform.position = endPosition;
        neighboringTile.transform.position = startPosition;
    }
}
