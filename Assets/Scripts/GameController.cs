using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    PieceSelection,
    MoveSelection,
    EnemyMovement,
    GameFinished,
    ResetReady
}

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameState gameState = GameState.PieceSelection;

    [Header("Services")]
    public GridModel grid;
    public AudioController audioController;
    public GameEndStuff gameEndStuff;

    private List<Vector2Int> availableMoves;
    private Piece selectedGuy;

    // Used to quickly end the game in editor
    [Header("Debug")]
    [SerializeField] private bool debugEnd = false;

    void Awake()
    {
        // Setting up GC as singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        availableMoves = new List<Vector2Int>();
        // The services script isn't used in this project bc its too small but I alway like to have it set up
        // It means I only need to have one static class in prototype projects
        InitializeServices();
    }

    private void InitializeServices()
    {
        Services.GameController = this;
        grid.GenerateGrid();
    }

    /// <summary>
    /// Called by the Square class. Interprets input from the player then changes the state of the game.
    /// </summary>
    /// <param name="pos"></param>
    public void OnClick(Vector2Int pos)
    {
        // Used to end the game early in scene
        if (debugEnd)
        {

            EndGame(true);
        }

        // When a reset is ready player input resets the game (faster than making a button)
        if (gameState == GameState.ResetReady)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        // If player selects a piece while in piece selection show available moves
        if (gameState == GameState.PieceSelection && grid.pieces[pos.x, pos.y] != null)
        {
            availableMoves.Clear();
            selectedGuy = grid.pieces[pos.x, pos.y];
            availableMoves = selectedGuy.GetAvailableMoves();
            // Color available moves as green
            grid.SetSquaresAsAvailable(availableMoves, true);
            gameState = GameState.MoveSelection;
        }
        else if (gameState == GameState.MoveSelection)
        {
            // If player clicked on an available move them move the piece and move on to enemy turn
            if (availableMoves.Contains(pos))
            {
                grid.MovePiece(pos, selectedGuy);

                gameState = GameState.EnemyMovement;
            }
            // If player clicked anything else just go back to piece selection
            else
            {
                gameState = GameState.PieceSelection;
            }
            // Resets available move squares
            grid.SetSquaresAsAvailable(availableMoves, false);
            availableMoves.Clear();
            selectedGuy = null;

            // Move to enemy turn if needed 
            // It's a coroutine to prevent simultaneous movement
            if (gameState == GameState.EnemyMovement)
            {
                StartCoroutine(MoveEnemyPiece());
            }
        }
    }

    private IEnumerator MoveEnemyPiece()
    {
        // Wait for player movement to finish
        yield return new WaitForSeconds(0.5f);

        // If all enemy pieces are dead the player wins
        if (grid.blackPieces.Count == 0)
        {
            EndGame(true);
        }
        else
        {
            bool goodPieceFound = false;
            Piece selectedPiece = null;
            int loops = 0;
            while (!goodPieceFound && loops < 15)
            {
                selectedPiece = grid.blackPieces[Random.Range(0, grid.blackPieces.Count)];
                if (selectedPiece.GetAvailableMoves().Count != 0)
                {
                    goodPieceFound = true;
                }
                loops++;
            }
            if (selectedPiece != null)
            {
                grid.MovePiece(ChooseOptimalMove(selectedPiece), selectedPiece);
                yield return new WaitForSeconds(selectedPiece.moveTime);
            }

            if (grid.whitePieces.Count == 0)
            {
                EndGame(false);
            }
            else
            {
                gameState = GameState.PieceSelection;
            }
        }
    }

    /// <summary>
    /// I sourced this from DeepSeek. It works really well for how simple it is
    /// If I were to improve it I would check for which piece is optimal to move as well (currently chosen at random)
    /// This is the only thing I sourced from AI. IMO At some point prompt engineering is just as tedious as writing it yourself.
    /// </summary>
    /// <param name="selectedPiece"></param>
    /// <returns></returns>
    private Vector2Int ChooseOptimalMove(Piece selectedPiece)
    {
        List<Vector2Int> availableMoves = selectedPiece.GetAvailableMoves();
        Vector2Int bestMove = availableMoves[0];
        int highestPriority = -1;

        foreach (Vector2Int move in availableMoves)
        {
            // Check if the move captures an opponent piece
            Piece targetPiece = grid.CheckForPiece(move);
            if (targetPiece != null && targetPiece.isWhite)
            {
                // Prioritize capturing opponent pieces
                bestMove = move;
                break;
            }

            // If no capture is available, prioritize moving forward
            int movePriority = move.y; // For black pieces, moving forward means decreasing y
            if (movePriority > highestPriority)
            {
                highestPriority = movePriority;
                bestMove = move;
            }
        }

        return bestMove;
    }

    /// <summary>
    /// Ends the game and plays some animations
    /// </summary>
    /// <param name="win"></param>
    public void EndGame(bool win)
    {
        gameState = GameState.GameFinished;
        if (win)
        {
            StartCoroutine(gameEndStuff.WinRoutine());
        }
        else
        {
            StartCoroutine(gameEndStuff.LoseRoutine());
        }
        StartCoroutine(ReadyReset());
    }

    /// <summary>
    /// Adds a buffer so players cant accidentally reset the game and miss the glorious confetti 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReadyReset()
    {
        yield return new WaitForSeconds(1);
        yield return gameEndStuff.ShowResetText();
        gameState = GameState.ResetReady;
    }
}