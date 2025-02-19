using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The tiles of the chess board
/// </summary>
public class Square : MonoBehaviour
{
    public Vector2Int pos;
    public Color color1;
    public Color color2;

    // The color used to show a possible move for the player (green)
    public Color selectionColor;
    public BoxCollider2D col;

    /// <summary>
    /// Sets up position and color
    /// </summary>
    /// <param name="newPos"></param>
    public void Init(Vector2Int newPos)
    {
        pos = newPos;
        // I was so close yet so far....
        transform.GetComponent<SpriteRenderer>().color = (newPos.x + newPos.y) % 2 == 0 ? color1 : color2;
    }

    /// <summary>
    /// Changes color of square to green or normal color
    /// </summary>
    /// <param name="available"></param>
    public void SetAvailable(bool available)
    {
        if (available)
        {
            transform.GetComponent<SpriteRenderer>().color = selectionColor;
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = (pos.x + pos.y) % 2 == 0 ? color1 : color2;
        }
    }

    /// <summary>
    /// Checks if player input was directed at square
    /// This is definitely not the optimal way to do this but it was the fastest
    /// </summary>
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (col.bounds.Contains(mousePos))
            {
                // Should be an event
                GameController.instance.OnClick(pos);
            }
        }
    }
}
