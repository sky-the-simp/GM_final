using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEffectManager : MonoBehaviour
{
    public Tilemap tilemap; // Reference to your Tilemap
    public GameObject hoverOverlay; // The overlay GameObject
    public GridSystem gs;

    void Update()
    {
        // Get the mouse position in world coordinates
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mouseWorldPos); // Get the tile position on the grid

        // Check if the tile is valid and set the hover overlay position
        if (tilemap.HasTile(tilePosition))
        {
            hoverOverlay.transform.position = tilemap.GetCellCenterWorld(tilePosition); // Center the overlay on the tile
            hoverOverlay.SetActive(true); // Make the overlay visible
        }
        else
        {
            hoverOverlay.SetActive(false); // Hide the overlay if the mouse is not over a tile
        }
    }

    public void displayWalkableTiles(Character charT){
        foreach (Vector3Int position in charT.reachablePositions.Keys){
            tilemap.SetColor(position, Color.blue);
        }
    }
}
