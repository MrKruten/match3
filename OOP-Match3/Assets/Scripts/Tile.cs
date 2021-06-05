using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    [SerializeField]
    private int color;
    private Board board;
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;

    public void SetBoard(Board board)
    {
        this.board = board;
    }

    public void SetXY(int y, int x)
    {
        this.x = x;
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetColor(int color)
    {
        this.color = color;
    }

    public int GetColor()
    {
        return color;
    }

    void OnMouseDown()
    {
        board.SetTile(this);
    }

    public void Move(float x, float y, Action timeHandler)
    {
        Vector2 point = new Vector2(x, y);
        StartCoroutine(ReplaceAnimation(point, 0.75f, timeHandler));
    }

    private IEnumerator ReplaceAnimation(Vector2 point, float time, Action timeHandler)
    {
        Vector2 start = transform.position;
        float timer = 0;
        while ((Vector2)transform.position != point)
        {
            transform.position = Vector2.Lerp(start, point, timer);
            timer += Time.deltaTime / time;
            yield return null;
        }
        timeHandler?.Invoke();
    }
}
