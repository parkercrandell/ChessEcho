using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> GetAvailableMoves()
    {
        // Pawns move only move forward so we have to define what that is
        int direction = isWhite ? 1 : -1;

        List<Vector2Int> moves = new List<Vector2Int>();
        // Checking forward direction
        if (GameController.instance.grid.IsPositionWithinBounds(pos + new Vector2Int(0, direction)))
        {
            Piece p = GameController.instance.grid.CheckForPiece(pos + new Vector2Int(0, direction));
            // If there is a piece in the way you can move if its an enemy
            if (p == null)
            {
                moves.Add(pos + new Vector2Int(0, direction));
            }
        }

        // Checking f left
        if (GameController.instance.grid.IsPositionWithinBounds(pos + new Vector2Int(-1, direction)))
        {
            Piece p = GameController.instance.grid.CheckForPiece(pos + new Vector2Int(-1, direction));
            // If there is an enemy piece you can move here
            if (p != null && p.isWhite != isWhite)
            {
                moves.Add(pos + new Vector2Int(-1, direction));
            }
        }

        // Checking f right
        if (GameController.instance.grid.IsPositionWithinBounds(pos + new Vector2Int(1, direction)))
        {
            Piece p = GameController.instance.grid.CheckForPiece(pos + new Vector2Int(1, direction));
            // If there is an enemy piece you can move here
            if (p != null && p.isWhite != isWhite)
            {
                moves.Add(pos + new Vector2Int(1, direction));
            }
        }
        
        return moves;
    }
}
