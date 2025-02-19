using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // Forward
        moves.AddRange(GetMovesInDirection(Vector2Int.up));
        // Forward Right
        moves.AddRange(GetMovesInDirection(new Vector2Int(1, 1)));
        // Right
        moves.AddRange(GetMovesInDirection(Vector2Int.right));
        // Down Right
        moves.AddRange(GetMovesInDirection(new Vector2Int(1, -1)));
        // Down 
        moves.AddRange(GetMovesInDirection(new Vector2Int(0, -1)));
        // Down Left
        moves.AddRange(GetMovesInDirection(new Vector2Int(-1, -1)));
        // Left
        moves.AddRange(GetMovesInDirection(new Vector2Int(-1, 0)));
        // Forward Left
        moves.AddRange(GetMovesInDirection(new Vector2Int(-1, 1)));

        return moves;
    }

    /// <summary>
    /// Uses a while loop to add all available moves until it hits an obstacle
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public List<Vector2Int> GetMovesInDirection(Vector2Int direction)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        bool moveEnded = false;
        // Use the direction vector once to get starting position
        Vector2Int newPos = pos + direction;

        // Add moves until an obstacle is hit
        while (!moveEnded)
        {
            // If the position is out of bounds end the loop and don't add pos to list
            if (GameController.instance.grid.IsPositionWithinBounds(newPos))
            {
                // If the position is in bounds but has a piece in it...
                Piece p = GameController.instance.grid.CheckForPiece(newPos);
                if (p != null)
                {
                    // If its an enemy move to it then stop
                    if (p.isWhite != isWhite)
                    {
                        moves.Add(newPos);
                    }
                    // Otherwise treat it as an obstacle and stop
                    moveEnded = true;
                }
                else
                {
                    moves.Add(newPos);
                }
            }
            else
            {
                moveEnded = true;
            }
            // Try a new position further in the direction
            newPos += direction;
        }

        return moves;
    }
}
