using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Board board;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Tilemap gameOverTilemap;
    bool playing = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start() {
        GameOver();
    }


    public void GameOver() {
        gameOverTilemap.GetComponent<Renderer>().enabled = true;
        uiCanvas.gameObject.SetActive(true);
        playing = false;
    }

    public void PlayAgain() {
        gameOverTilemap.GetComponent<Renderer>().enabled = false;
        uiCanvas.gameObject.SetActive(false);
        board.StartPlay();
        playing = true;
    }

}
