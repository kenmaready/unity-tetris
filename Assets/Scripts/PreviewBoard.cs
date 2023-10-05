using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

public class PreviewBoard : MonoBehaviour
{
    private Tilemap tilemap;
    Vector3Int previewPosition = new Vector3Int(0, -1, 0);

    private void Awake() {
        this.tilemap = GetComponentInChildren<Tilemap>();
            if (tilemap == null) {
                Debug.LogError("No Tilemap Component Found on Child of PreviewBoard...");
            }
        }

    public void Display(TetrominoData previewPiece) {
        for (int i = 0; i < previewPiece.cells.Length; i++) {
            Vector3Int tilePosition = (Vector3Int)previewPiece.cells[i] + previewPosition;
            if (previewPiece.tetromino == Tetromino.I || previewPiece.tetromino == Tetromino.O) {
                Vector3 pos = tilemap.transform.position;
                float newX = pos.x - .125f;
                float newY = (previewPiece.tetromino == Tetromino.I) ? pos.y - 0.125f : pos.y;
                Vector3 shiftedPos = new Vector3(newX, newY, pos.y);
                tilemap.transform.position = shiftedPos;
            }
            this.tilemap.SetTile(tilePosition, previewPiece.tile);
        }
    }

    public void Clear(TetrominoData previewPiece) {
        for (int i = 0; i < previewPiece.cells.Length; i++) {
            Vector3Int tilePosition = (Vector3Int)previewPiece.cells[i] + previewPosition;
            if (previewPiece.tetromino == Tetromino.I || previewPiece.tetromino == Tetromino.O) {
                Vector3 pos = tilemap.transform.position;
                float newX = pos.x + 0.125f;
                float newY = (previewPiece.tetromino == Tetromino.I) ? pos.y + 0.125f : pos.y;
                Vector3 shiftedPos = new Vector3(newX, newY, pos.y);
                tilemap.transform.position = shiftedPos;
            }
            this.tilemap.SetTile(tilePosition, null);
        }
    }
}
