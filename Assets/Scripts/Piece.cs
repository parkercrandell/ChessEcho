using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int pos;
    public bool isWhite;
    public bool dead = false;
    public float moveTime = 0.5f;

    /// <summary>
    /// Sets up the starting values of a piece
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="isWhite"></param>
    public void Init(Vector2Int newPos, bool isWhite)
    {
        pos = newPos;
        this.isWhite = isWhite;
    }

    /// <summary>
    /// Returns all available moves for that piece
    /// </summary>
    /// <returns></returns>
    public abstract List<Vector2Int> GetAvailableMoves();

    /// <summary>
    /// Changes piece position and animates movement
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="kill"></param>
    public void MovePiece(Vector2Int newPos, bool kill)
    {
        pos = newPos;
        StartCoroutine(MoveRoutine(newPos, kill));

    }

    /// <summary>
    /// Moves piece then plays SFX
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="kill"></param>
    /// <returns></returns>
    IEnumerator MoveRoutine(Vector2Int newPos, bool kill)
    {
        yield return MoveObjectRoutine(transform.position, GameController.instance.grid.GetPosFromGrid(newPos, true), moveTime);
        if (!kill)
        {
            GameController.instance.audioController.PlayMove();
        }
        else
        {
            GameController.instance.audioController.PlayKill();
        }
    }

    /// <summary>
    /// Sets a piece as dead
    /// Could add a better death animation
    /// </summary>
    /// <param name="newPos"></param>
    public void Captured(Vector2Int newPos)
    {
        pos = new Vector2Int(-1, -1);
        dead = true;
        transform.position = GameController.instance.grid.GetPosFromGrid(newPos, true);
    }

    /// <summary>
    /// Useful animation coroutine I typically use in a VisualServices singleton
    /// </summary>
    /// <param name="posStart"></param>
    /// <param name="posEnd"></param>
    /// <param name="animationTime"></param>
    /// <param name="deactivateAtEnd"></param>
    /// <param name="activateAtStart"></param>
    /// <returns></returns>
    public IEnumerator MoveObjectRoutine(Vector3 posStart, Vector3 posEnd, float animationTime, bool deactivateAtEnd = false, bool activateAtStart = false)
    {
        float timer = 0;
        if (activateAtStart)
        {
            transform.gameObject.SetActive(true);
        }

        while (timer <= animationTime)
        {
            yield return timer += Time.deltaTime;
            transform.position = Vector3.Lerp(posStart, posEnd, timer / animationTime);
        }

        if (deactivateAtEnd)
        {
            transform.gameObject.SetActive(false);
        }
    }

}
