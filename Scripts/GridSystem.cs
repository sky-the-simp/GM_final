using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;


public class GridSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[,] gridData;
    private BoundsInt bounds;

    void Start(){
        InitializeGrid();
    }

    void InitializeGrid(){
        bounds = tilemap.cellBounds;
        gridData = new Tile[bounds.size.x, bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++){
            for (int y = bounds.yMin; y < bounds.yMax; y++){
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(cellPosition)){
                    // Example for different terrain types
                    bool walkable = true;
                    int movementCost = 1;

                    // Example: Make water tiles impassable
                    if (tilemap.GetTile(cellPosition).name == "Water"){
                        walkable = false;
                        movementCost = int.MaxValue;
                    }

                    gridData[x - bounds.xMin, y - bounds.yMin] = new Tile(cellPosition, walkable, movementCost);
                }
            }
        }
    }
    public Vector3Int WorldToGridPosition(Vector3 worldPosition){
        return tilemap.WorldToCell(worldPosition);
    }
    public Vector3 GridToWorldPosition(Vector3Int gridPosition){
        return tilemap.CellToWorld(gridPosition) + tilemap.tileAnchor;
    }
    private void OnDrawGizmos(){
        if (gridData == null) return;

        foreach (var tile in gridData){
            if (tile == null) continue;

            Vector3 worldPos = GridToWorldPosition(tile.position);

            Gizmos.color = tile.isWalkable ? Color.green : Color.red;

            Gizmos.DrawCube(worldPos, Vector3.one * 0.5f); // Adjust size if needed

            // Optionally, display the movement cost as text
            UnityEditor.Handles.Label(worldPos - Vector3.up*0.3f, tile.movementCost.ToString());
        }
    }

    public Dictionary<Vector3Int, Vector3Int> GetReachablePositions(Vector3Int startPos, int maxMovementRange){
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, int> movementCosts = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, Vector3Int> reachablePositions = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> parentPositions = new Dictionary<Vector3Int, Vector3Int>();
        queue.Enqueue(startPos);
        movementCosts[startPos] = 0;

        while (queue.Count > 0)
        {
            Vector3Int currentPos = queue.Dequeue();
            int currentCost = movementCosts[currentPos];
            Tile currentTile = gridData[currentPos.x, currentPos.y];

            if (currentCost <= maxMovementRange && currentTile.isWalkable)
            {
                reachablePositions[currentPos] = parentPositions[currentPos];

                foreach (var neighborPos in GetNeighborsPos(currentPos))
                {
                    Tile neighbor = gridData[neighborPos.x, currentPos.y];
                    int newCost = currentCost + neighbor.movementCost;
                    
                    if (newCost <= maxMovementRange && !movementCosts.ContainsKey(neighborPos))
                    {
                        queue.Enqueue(neighborPos);
                        movementCosts[neighborPos] = newCost;
                        parentPositions[neighborPos] = currentPos;
                    }
                }
            }
        }

        return reachablePositions;
    }
    private IEnumerable<Vector3Int> GetNeighborsPos(Vector3Int pos){
        // Define grid directions (e.g., top, bottom, left, right)
        Vector3Int[] directions = {Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right};

        foreach (var dir in directions)
        {
            Tile neighbor = GetTileAt(pos + dir);
            if (neighbor != null)
            {
                yield return neighbor.position;
            }
        }
    }
    public Tile GetTileAt(Vector3Int position){
        // Calculate the relative grid position
        int relativeX = position.x - bounds.xMin;
        int relativeY = position.y - bounds.yMin;

        // Check bounds to avoid IndexOutOfRangeException
        if (relativeX < 0 || relativeX >= gridData.GetLength(0) || 
            relativeY < 0 || relativeY >= gridData.GetLength(1))
        {
            Debug.LogWarning($"GetTileAt: Position {position} is out of bounds.");
            return null; // Handle out-of-bounds access
        }

        return gridData[relativeX, relativeY]; // Return the tile at the specified position
    }

}
