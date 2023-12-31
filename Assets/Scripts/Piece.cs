using UnityEngine;

public class Piece : MonoBehaviour
{

    public Board board { get; private set; }
    public Vector3Int position { get; private set; }
    public TetrominoData data { get; private set; }
    public int rotationIndex { get; private set; }

    public Vector3Int[] cells { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTimer = 0f;
    private float lockTimer = 0f;
    bool active = false;

    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTimer = Time.time + this.stepDelay;
        this.lockTimer = 0f; 

        if (this.cells == null) {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }

        this.active = true;
        this.board.ghostBoard.Continue();
    }

    public void Pause() {
        active = false;
        this.board.ghostBoard.Pause();
    }

    public void Continue() {
        active = true;
        this.board.ghostBoard.Continue();
    }

    private void Update() {
        if (!active) return;

        this.board.Clear(this);
        this.lockTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Rotate(1);
        }


        if (Time.time >= this.stepTimer) {
            Step();
        }

        if (active) this.board.Set(this);
    }

    private bool Move(Vector2Int translation) {
        Vector3Int nextPosition = this.position;
        nextPosition.x += translation.x;
        nextPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, nextPosition);

        if (valid) {
            this.position = nextPosition;
            this.lockTimer = 0f;
        }

        return valid;
    }

    private void Step() {
        this.stepTimer = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        if (this.lockTimer >= this.lockDelay) Lock();
    }

    private void Lock() {
        this.board.Set(this);
        this.board.CheckAndClearRows();
        this.board.SpawnPiece();
    }

    private void HardDrop() {
        while (Move(Vector2Int.down)) {
            continue;
        }
        Lock();
    }

    private void Rotate(int direction) {
        int NextRotationIndex = Wrap(this.rotationIndex + direction, 0, 3);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(NextRotationIndex, direction)) {
            ApplyRotationMatrix(-direction);
        } else {
            this.rotationIndex = NextRotationIndex;
        }
    }

    private void ApplyRotationMatrix(int direction) {
        for (int i = 0; i < this.cells.Length; i++) {
            Vector3 cell = this.cells[i];
            int x, y;

            switch (this.data.tetromino) {
                case Tetromino.I:
                case Tetromino.O:                    
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection) {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++) {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            if (Move(translation)) {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection) {
        int wallKickIndex  = rotationIndex * 2;
        
        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0) - 1);
    }

    private int Wrap(int input, int min, int max) {
        if (input < min) {
            return max;
        } else if (input > max) {
            return min;
        } else return input;
    }
}
