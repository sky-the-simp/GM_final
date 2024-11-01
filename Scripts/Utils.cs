using UnityEngine;

public class Tile
{
    public Vector3Int position; // Position on the grid
    public bool isWalkable; // Whether the tile can be traversed
    public int movementCost; // Movement cost for pathfinding
    public Character character;

    public Tile(Vector3Int pos, bool walkable, int cost)
    {
        position = pos;
        isWalkable = walkable;
        movementCost = cost;
        character = null;
    }
}

public enum eCharacterStatus {waiting, moving, attacking, finishing}
public enum eTurnStatus {playerTurn, enemyTurn, turnTransition}

