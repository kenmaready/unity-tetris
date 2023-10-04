using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoes;
    public Piece activePiece { get; private set; }
    public Vector3Int spawnPosition = new Vector3Int(0, 8, 0);
    [SerializeField] public ParticleSystem sparkles;

    public Vector2Int boardSize = new Vector2Int(10, 20);
    public RectInt Bounds { get {
        Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
        return new RectInt (position, this.boardSize);
    } }

    private GameManager gm;

    private void Awake() {
        gm = FindObjectOfType<GameManager>();
        if (gm == null) {
            Debug.Log("GameManager Object not found by Board...");
        }

        this.tilemap = GetComponentInChildren<Tilemap>();
        if (tilemap == null) {
            Debug.LogError("No Tilemap Component Found on Child of Board...");
        }

        this.activePiece = GetComponentInChildren<Piece>();
        if (this.activePiece == null) {
            Debug.LogError("No Piece Component Found on Child of Board...");
        }

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start() {
        // SpawnPiece();
    }

    public void SpawnPiece() {
        int random = UnityEngine.Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];

        this.activePiece.Initialize(this, spawnPosition, data);
        // this.activePiece.Pause();

        if (IsValidPosition(this.activePiece, this.spawnPosition)) {
            Set(activePiece);
            // this.activePiece.Continue();
        } else {
            GameOver();
        }
    }

    public void Set(Piece piece) {
        foreach (Vector3Int cell in piece.cells) {
            Vector3Int tilePosition = cell + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public void CheckAndClearRows() {
        int row = Bounds.yMin;

        while (row < Bounds.yMax) {
            if (IsLineFull(row)) {
                ClearSingleRow(row);
            } else row++;
        }
    }

    private bool IsLineFull(int row) {
        for (int col = Bounds.xMin; col < Bounds.xMax; col++) {
            if (!this.tilemap.HasTile(new Vector3Int(col, row, 0))) return false;
        }
        return true;
    }

    private void ClearSingleRow(int row) {
        sparkles.transform.position = new Vector3(0, row, 0);
        sparkles.Play();
        for (int col = Bounds.xMin; col < Bounds.xMax; col++) {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        while (row < Bounds.yMax) {
            for (int col = Bounds.xMin; col < Bounds.xMax; col++) {
                Vector3Int fromPOS = new Vector3Int(col, row + 1, 0);
                Vector3Int toPOS = new Vector3Int(col, row, 0);
                TileBase fromTile = this.tilemap.GetTile(fromPOS);

                this.tilemap.SetTile(toPOS, fromTile);
            }

            row++;
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position) {
        foreach (Vector3Int cell in piece.cells) {
            Vector3Int tilePosition = cell + position;

            if (this.tilemap.HasTile(tilePosition)) return false;
            if (!Bounds.Contains((Vector2Int)tilePosition)) return false;

        }
        return true; 
    }

    private void GameOver() {
        this.activePiece.Pause();
        gm.GameOver();
        this.tilemap.ClearAllTiles();
    }

    public void StartPlay() {
        SpawnPiece();
    }
}
