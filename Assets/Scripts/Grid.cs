using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridModel : MonoBehaviour
{
    public GameObject squarePrefab;
    public GameObject whitePawnPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject blackPawnPrefab;
    public GameObject blackQueenPrefab;
    public List<Piece> blackPieces = new List<Piece>();
    public List<Piece> whitePieces = new List<Piece>();
    public List<Piece> blackLostPieces = new List<Piece>();
    public List<Piece> whiteLostPieces = new List<Piece>();
    public Square[,] squares;
    public Piece[,] pieces;

    public int gridSize = 6;


    /// <summary>
    /// Generates the grid and places all the pieces and fills all the piece lists
    /// </summary>
    public void GenerateGrid()
    {
        squares = new Square[gridSize, gridSize];
        pieces = new Piece[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Generate squares and adds them to the 2D array
                GameObject s = Instantiate(squarePrefab, GameController.instance.transform);
                s.transform.position = GetPosFromGrid(new Vector2Int(x, y));
                Square sq = s.GetComponent<Square>();
                sq.Init(new Vector2Int(x, y));
                squares[x, y] = sq;

                // Adds the white queens
                if (y == 0)
                {
                    GameObject p = Instantiate(whiteQueenPrefab, transform);
                    // Sets up piece
                    p.transform.position = GetPosFromGrid(new Vector2Int(x, y), true);
                    Queen ps = p.GetComponent<Queen>();
                    ps.Init(new Vector2Int(x, y), true);
                    // Places piece in model
                    pieces[x, y] = ps as Piece;
                    // Adds piece to a list of pieces that the EnemyAI can use
                    whitePieces.Add(ps);
                }

                // Adds the white pawns
                if (y == 1)
                {
                    GameObject p = Instantiate(whitePawnPrefab, transform);
                    p.transform.position = GetPosFromGrid(new Vector2Int(x, y), true);
                    Pawn ps = p.GetComponent<Pawn>();
                    ps.Init(new Vector2Int(x, y), true);
                    pieces[x, y] = ps as Piece;
                    whitePieces.Add(ps);
                }

                // Adds the black pawns
                if (y == gridSize - 2)
                {
                    GameObject p = Instantiate(blackPawnPrefab, transform);
                    p.transform.position = GetPosFromGrid(new Vector2Int(x, y), true);
                    Pawn ps = p.GetComponent<Pawn>();
                    ps.Init(new Vector2Int(x, y), false);
                    pieces[x, y] = ps as Piece;
                    blackPieces.Add(ps);
                }

                // Adds the black queens
                if (y == gridSize - 1)
                {
                    GameObject p = Instantiate(blackQueenPrefab, transform);
                    p.transform.position = GetPosFromGrid(new Vector2Int(x, y), true);
                    Queen ps = p.GetComponent<Queen>();
                    ps.Init(new Vector2Int(x, y), false);
                    pieces[x, y] = ps as Piece;
                    blackPieces.Add(ps);
                }
            }
        }
    }

    /// <summary>
    /// Used to find world position of objects
    /// since the squares are only one unit in size this is pretty simple
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    public Vector3 GetPosFromGrid(Vector2Int gridPos, bool piece = false)
    {
        // Pieces need to appear above square sprites
        int zPos = piece ? -1 : 0;
        return new Vector3(gridPos.x, gridPos.y, zPos);
    }

    /// <summary>
    /// Moves the piece within the model and tells the piece to animate after the model is updated 
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="selectedGuy"></param>
    public void MovePiece(Vector2Int newPos, Piece selectedGuy)
    {
        Piece doomedPiece = CheckForPiece(newPos);
        if (doomedPiece != null)
        {
            if (doomedPiece.isWhite)
            {
                whiteLostPieces.Add(doomedPiece);
                whitePieces.Remove(doomedPiece);
                doomedPiece.Captured(new Vector2Int(-(whiteLostPieces.Count - 1) / gridSize - 1, (whiteLostPieces.Count - 1) % gridSize));

            }
            else
            {
                blackLostPieces.Add(doomedPiece);
                blackPieces.Remove(doomedPiece);
                doomedPiece.Captured(new Vector2Int((blackLostPieces.Count - 1) / gridSize + gridSize, -((blackLostPieces.Count - 1) % gridSize) + gridSize - 1));
            }
        }
        Vector2Int oldPos = selectedGuy.pos;
        selectedGuy.MovePiece(newPos, doomedPiece != null);
        pieces[oldPos.x, oldPos.y] = null;
        pieces[newPos.x, newPos.y] = selectedGuy;

    }

    /// <summary>
    /// Checks if there is a piece at position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Piece CheckForPiece(Vector2Int pos)
    {
        if (!IsPositionWithinBounds(pos))
        {
            return null;
        }
        return pieces[pos.x, pos.y];
    }

    /// <summary>
    /// Changes color of squares to green or their normal color
    /// </summary>
    /// <param name="moves"></param>
    /// <param name="available"></param>
    public void SetSquaresAsAvailable(List<Vector2Int> moves, bool available)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            squares[moves[i].x, moves[i].y].SetAvailable(available);
        }
    }

    /// <summary>
    /// Checks if position is in the bounds of the game 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsPositionWithinBounds(Vector2Int pos)
    {
        return pos.x < gridSize && pos.y < gridSize && pos.x >= 0 & pos.y >= 0;
    }

}
