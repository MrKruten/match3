using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private int xSize, ySize;
    private int countColor;
    private Tile[,] tiles;
    private enum State
    {
        Input = 0,
        Animation = 1
    }
    private State state;
    private int score;
    private List<Color> colors;
    private Tile nowTile;
    private Tile previousTile;
    private bool isCreating;
    public Action endMenu;

    [SerializeField]
    private Tile tileGo;
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private SpriteRenderer border;

    private void Awake()
    {
        colors = new List<Color>();

        colors.Add(new Color(1, 1, 1, 1));
        colors.Add(new Color(0.8584f, 0.1579f, 0.1579f, 1));
        colors.Add(new Color(0.9545f, 1, 0, 1));
        colors.Add(new Color(0.3079f, 0.9245f, 0.1003f, 1));
        colors.Add(new Color(0.1003f, 0.9245f, 0.5767f, 1));
        colors.Add(new Color(0.1003f, 0.4366f, 0.9245f, 1));
        colors.Add(new Color(0.4715f, 0.1003f, 0.9245f, 1));
        colors.Add(new Color(0.7933f, 0.1003f, 0.9245f, 1));
        colors.Add(new Color(0.9245f, 0.1003f, 0.4146f, 1));
        colors.Add(new Color(0.5943f, 0.2106f, 0.0532f, 1));
    }

    private void BorderHandler(Tile tile, bool isShow)
    {
        if (isShow)
        {
            border.transform.position = tile.transform.position;
            border.gameObject.SetActive(true);
        }
        else
        {
            border.gameObject.SetActive(false);
        }
    }

    public void SetValues(int ySize, int xSize, int countColor)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.countColor = countColor;
        CreateBoard();
    }

    private void CreateBoard()
    {
        isCreating = true;
        score = 0;
        scoreText.text = score.ToString();

        state = State.Animation;
        tiles = new Tile[ySize, xSize];

        float xPos = transform.position.x;
        float yPos = transform.position.y;
        Vector2 tileSize = tileGo.spriteRenderer.bounds.size;
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                Tile newTile = Instantiate(tileGo, transform.position, Quaternion.identity);
                newTile.transform.position = new Vector3( yPos + (tileSize.y * j), xPos + (tileSize.x * i), 0);
                newTile.transform.parent = transform;
                int color = Random.Range(0, countColor);
                newTile.GetComponent<SpriteRenderer>().color = colors[color];
                newTile.SetColor(color);
                newTile.SetBoard(this);
                newTile.SetXY(i, j);

                tiles[i, j] = newTile;
            }
        }
        float a = xPos + (tiles.GetLength(0)) * tileSize.x / 2;
        float b = yPos + (tiles.GetLength(1)) * tileSize.y / 2;
        camera.transform.position = new Vector3(b - tileSize.y / 2, a - tileSize.x / 2, camera.transform.position.z);
        camera.orthographicSize = a > b ? a : b;

        CheckMatch();
    }

    public void SetTile(Tile tile)
    {
        if (state == State.Input)
        {
            if (previousTile == null)
            {
                BorderHandler(tile, true);
                previousTile = tile;
            }
            else
            {
                if (previousTile == tile)
                {
                    BorderHandler(null, false);
                    previousTile = null;
                }
                else
                {
                    nowTile = tile;
                    if ((Math.Abs(nowTile.GetX() - previousTile.GetX()) <= 1) && (Math.Abs(nowTile.GetY() - previousTile.GetY()) <= 1) && !((Math.Abs(nowTile.GetX() - previousTile.GetX()) == 1) && (Math.Abs(nowTile.GetY() - previousTile.GetY()) == 1)))
                    {
                        
                        BorderHandler(null, false);
                        state = State.Animation;
                        float positionX = nowTile.transform.position.x;
                        float positionY = nowTile.transform.position.y;
                        float position1X = previousTile.transform.position.x;
                        float position2Y = previousTile.transform.position.y;
                        nowTile.Move(position1X, position2Y, null);
                        previousTile.Move(positionX, positionY, CheckUserMatch);
                    }
                    else
                    {
                        BorderHandler(tile, true);
                        previousTile = tile;
                        nowTile = null;
                    }
                }
            }
        }
    }

    private void ReplaceTiles()
    {
        int bufX = previousTile.GetX();
        int bufY = previousTile.GetY();
        int bufX1 = nowTile.GetX();
        int bufY1 = nowTile.GetY();

        Tile buf = tiles[previousTile.GetY(), previousTile.GetX()];
        tiles[previousTile.GetY(), previousTile.GetX()] = tiles[nowTile.GetY(), nowTile.GetX()];
        tiles[nowTile.GetY(), nowTile.GetX()] = buf;
        nowTile.SetXY(bufY, bufX);
        previousTile.SetXY(bufY1, bufX1);
        previousTile = null;
        nowTile = null;
        state = State.Input;
    }

    private void CheckUserMatch()
    {
        int bufX = previousTile.GetX();
        int bufY = previousTile.GetY();
        int bufX1 = nowTile.GetX();
        int bufY1 = nowTile.GetY();

        Tile buf = tiles[previousTile.GetY(), previousTile.GetX()];
        tiles[previousTile.GetY(), previousTile.GetX()] = tiles[nowTile.GetY(), nowTile.GetX()];
        tiles[nowTile.GetY(), nowTile.GetX()] = buf;
        nowTile.SetXY(bufY, bufX);
        previousTile.SetXY(bufY1, bufX1);

        List<Tile> match1 = FindMatch(nowTile.GetY(), nowTile.GetX());
        List<Tile> match2 = FindMatch(previousTile.GetY(), previousTile.GetX());
        
        if (match1.Count > 2 || match2.Count > 2)
        {
            CountScore(match1);
            CountScore(match2);

            match1.AddRange(match2);
            DestroyTiles(match1);

            previousTile = null;
            nowTile = null;
        }
        else
        {
            float positionX = nowTile.transform.position.x;
            float positionY = nowTile.transform.position.y;
            float position1X = previousTile.transform.position.x;
            float position2Y = previousTile.transform.position.y;
            nowTile.Move(position1X, position2Y, null);
            previousTile.Move(positionX, positionY, ReplaceTiles);
        }
    }

    private void CountScore(List<Tile> match)
    {
        if (match.Count > 3)
        {
            for (int i = 0; i < match.Count; i ++)
                score += 3;
        }
        else
        {
            score += 6;
        }
        scoreText.text = score.ToString();
    }

    private void DestroyTiles(List<Tile> match)
    {
        Vector2 tileSize = tileGo.spriteRenderer.bounds.size;
        for (int i = 0; i < match.Count; i++)
        {
            if ((match.Count - i) == 1)
            {
                match[i].Move(match[i].transform.position.x, -1 * (tileSize.x * tiles.GetLength(1)), NewTiles);
                Destroy(match[i].gameObject, 0.85f);
                tiles[match[i].GetY(), match[i].GetX()] = null;
                
            }
            else
            {
                match[i].Move(match[i].transform.position.x, -1 * (tileSize.x * tiles.GetLength(1)), null);
                Destroy(match[i].gameObject, 0.8f);
                tiles[match[i].GetY(), match[i].GetX()] = null;
            }
            
        }
    }

    private void NewTiles()
    {
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        Vector2 tileSize = tileGo.spriteRenderer.bounds.size;

        int lastColumn = -1;
        int lastRow = -1;
        for (int j = 0; j < tiles.GetLength(1); j++)
        {
            int countTiles = 0;
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                if (tiles[i, j] != null)
                {
                    if (i != countTiles)
                    {
                        tiles[i, j].Move(yPos + (tileSize.y * j), xPos + (tileSize.x * countTiles), null);
                        tiles[countTiles, j] = tiles[i, j];
                        tiles[i, j] = null;
                        tiles[countTiles, j].SetXY(countTiles, j);
                    }
                    countTiles++;
                }
            }
        }

        for (int j = tiles.GetLength(1)-1; j >= 0; j--)
        {
            int i;
            for (i = 0; i < tiles.GetLength(0); i++)
            {
                if (tiles[i, j] == null)
                {
                    lastColumn = j;
                    lastRow = i;
                    break;
                }
            }
            if (i == tiles.GetLength(0))
                i--;
            if (tiles[i, j] == null)
            {
                break;
            }
        }

        for (int j = 0; j <= lastColumn; j++)
        {
            for (int k = tiles.GetLength(0)-1; k >= 0; k--)
            {
                if (tiles[k, j] != null)
                    continue;
                Tile newTile = Instantiate(tileGo, transform.position, Quaternion.identity);
                newTile.transform.position = new Vector3(yPos + (tileSize.y * j), xPos + (tileSize.x * tiles.GetLength(0)), 0);
                newTile.transform.parent = transform;
                int color = Random.Range(0, countColor);
                newTile.GetComponent<SpriteRenderer>().color = colors[color];
                newTile.SetColor(color);
                newTile.SetBoard(this);
                newTile.SetXY(k, j);

                tiles[k, j] = newTile;
                if (j == lastColumn && k == lastRow)
                {
                    tiles[k, j].Move(yPos + (tileSize.y * j), xPos + (tileSize.x * k), CheckMatch);
                }
                else
                {
                    tiles[k, j].Move(yPos + (tileSize.y * j), xPos + (tileSize.x * k), null);
                }
            }
        }
    }

    private void CheckMatch()
    {
        List<Tile> destroyTiles = new List<Tile>();
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                List<Tile> checkMatchTile = FindMatch(i, j);
                if (checkMatchTile.Count > 2)
                {
                    CountScore(checkMatchTile);
                    for(int k = 0; k < checkMatchTile.Count; k++)
                    {
                        if (!destroyTiles.Contains(checkMatchTile[k]))
                        {
                            destroyTiles.Add(checkMatchTile[k]);
                        }
                    }
                }
            }
        }

        if(destroyTiles.Count > 2)
        {
            DestroyTiles(destroyTiles);
            return;
        }

        if (CheckMove() == false)
        {
            if(isCreating == true)
            {
                EndHandler();
                CreateBoard();
            }
            else
            {
                endMenu();
            }  
        }
        else
        {
            state = State.Input;
            isCreating = false;
        }
    }

    private bool CheckMove()
    {
        for (int i = 0; i < (tiles.GetLength(0) - 1); i++)
        {
            for (int j = 0; j < (tiles.GetLength(1) - 1); j++)
            {
                List<Tile> match = FindMatch(i, j, i + 1, j);
                if (match.Count > 2)
                    return true;

                match = FindMatch(i, j, i, j + 1);
                if (match.Count > 2)
                    return true;

                match = FindMatch(i + 1, j, i, j);
                if (match.Count > 2)
                    return true;

                match = FindMatch(i, j + 1, i, j);
                if (match.Count > 2)
                    return true;
            }
        }
        int topRow = tiles.GetLength(0) - 1;
        int rightColumn = tiles.GetLength(1) - 1;
        for (int j = 0; j < (tiles.GetLength(1) - 1); j++)
        {
            List<Tile> match = FindMatch(topRow, j, topRow, j + 1);
            if (match.Count > 2)
                return true;

            match = FindMatch(topRow, j + 1, topRow, j);
            if (match.Count > 2)
                return true;
        }
        for (int i = 0; i < (tiles.GetLength(0) - 1); i++)
        {
            List<Tile> match = FindMatch(i, rightColumn, i + 1, rightColumn);
            if (match.Count > 2)
                return true;

            match = FindMatch(i + 1, rightColumn, i, rightColumn);
            if (match.Count > 2)
                return true;
        }
        return false;
    }

    private List<Tile> FindMatch(int yPrev, int xPrev, int yPos, int xPos)
    {
        List<Tile> match = new List<Tile>();
        match.Add(tiles[yPrev, xPrev]);
        int color = tiles[yPrev, xPrev].GetColor();
        int count = 0;
        // go bottom
        for (int i = yPos - 1; i >= 0; i--)
        {
            if (i == yPrev)
                continue;
            if (tiles[i, xPos].GetColor() != color)
                break;

            count++;
            match.Add(tiles[i, xPos]);
        }
        //go top
        for (int i = yPos + 1; i < tiles.GetLength(0); i++)
        {
            if (i == yPrev)
                continue;
            if (tiles[i, xPos].GetColor() != color)
                break;

            count++;
            match.Add(tiles[i, xPos]);
        }
        if (count < 2)
            match.Clear();

        count = 0;
        List<Tile> matchLine = new List<Tile>();
        // go left
        for (int i = xPos - 1; i >= 0; i--)
        {
            if (i == xPrev)
                continue;
            if (tiles[yPos, i].GetColor() != color)
                break;

            count++;
            matchLine.Add(tiles[yPos, i]);
        }
        //go right
        for (int i = xPos + 1; i < tiles.GetLength(1); i++)
        {
            if (i == xPrev)
                continue;
            if (tiles[yPos, i].GetColor() != color)
                break;

            count++;
            matchLine.Add(tiles[yPos, i]);
        }

        if (count >= 2)
        {
            if (match.Count == 0)
            {
                match.Add(tiles[yPrev, xPrev]);
            }
            match.AddRange(matchLine);
        }
        return match;
    }

    private List<Tile> FindMatch(int y, int x)
    {
        List<Tile> match = new List<Tile>();
        match.Add(tiles[y, x]);
        int color = tiles[y, x].GetColor();
        int count = 0;
        // go bottom
        for (int i = y - 1; i >= 0; i--)
        {
            if (tiles[i, x].GetColor() != color)
                break;

            count++;
            match.Add(tiles[i, x]);
        }
        //go top
        for (int i = y + 1; i < tiles.GetLength(0); i++)
        {
            if (tiles[i, x].GetColor() != color)
                break;

            count++;
            match.Add(tiles[i, x]);
        }
        if (count < 2)
            match.Clear();

        count = 0;
        List<Tile> matchLine = new List<Tile>();
        // go left
        for (int i = x - 1; i >= 0; i--)
        {
            if (tiles[y, i].GetColor() != color)
                break;

            count++;
            matchLine.Add(tiles[y, i]);
        }
        //go right
        for (int i = x + 1; i < tiles.GetLength(1); i++)
        {
            if (tiles[y, i].GetColor() != color)
                break;

            count++;
            matchLine.Add(tiles[y, i]);
        }

        if (count >= 2)
        {
            if (match.Count == 0)
            {
                match.Add(tiles[y, x]);
            }
            match.AddRange(matchLine);
        }
        return match;
    }

    public void EndHandler()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
