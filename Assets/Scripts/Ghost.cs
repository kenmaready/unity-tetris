using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile ghostTile;
    public Piece trackedPiece;
    public Board gameboard;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private bool active = false;

    private void Awake() {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void LateUpdate() {
        if (!active) return;

        Clear();
        Copy();
        Drop();
        Set();
    }

    public void Pause() {
        active = false;
    }

    public void Continue() {
        active = true;
    }

    private void Clear() {
        foreach (Vector3Int cell in this.cells) {
            Vector3Int tilePosition = cell + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy() {
        for (int i = 0; i < this.cells.Length; i++) {
            this.cells[i] = trackedPiece.cells[i];
        }
    }

    private void Drop() {
        Vector3Int testPosition = trackedPiece.position;
        int startingRow = testPosition.y;
        int bottomRow = this.gameboard.Bounds.yMin;

        // clear trackedpiece temporarily to allow for testing for
        // valid position of ghost piece:
        this.gameboard.Clear(this.trackedPiece);

        for (int row = startingRow; row >= bottomRow; row--) {
            testPosition.y = row;

            if (this.gameboard.IsValidPosition(trackedPiece, testPosition)) {
                this.position = testPosition;
            } else break;
        }

        this.gameboard.Set(this.trackedPiece);
    }

    private void Set() {
        foreach (Vector3Int cell in this.cells) {
            Vector3Int tilePosition = cell + this.position;
            this.tilemap.SetTile(tilePosition, this.ghostTile);
        }
    }

}
