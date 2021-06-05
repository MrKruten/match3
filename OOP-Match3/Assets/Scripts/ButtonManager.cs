using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private Board board;
    [SerializeField]
    private int xSize, ySize;
    [SerializeField]
    private int countColor;
    [SerializeField]
    private Text xText;
    [SerializeField]
    private Text yText;
    [SerializeField]
    private Text countColorText;
    [SerializeField]
    private Image menu;
    [SerializeField]
    private Text score;
    [SerializeField]
    private Image result;
    [SerializeField]
    private Text resultText;
    [SerializeField]
    private Button playAgain;
    [SerializeField]
    private Button endGame;
    [SerializeField]
    private GameObject border;


    private void Awake()
    {
        xSize = 3;
        ySize = 3;
        countColor = 3;

        board.endMenu += EndGame;
    }

    public void OnXsizeChanged(float newValue)
    {
        xSize = (int)newValue;
        xText.text = xSize.ToString();
    }

    public void OnYsizeChanged(float newValue)
    {
        ySize = (int)newValue;
        yText.text = ySize.ToString();
    }

    public void OnColorCountChanged(float newValue)
    {
        countColor = (int)newValue;
        countColorText.text = countColor.ToString();
    }

    public void Go()
    {
        board.SetValues(ySize, xSize, countColor);
        endGame.gameObject.SetActive(true);
        playAgain.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
        
    }

    public void EndGame()
    {
        border.SetActive(false);
        resultText.text = score.text;
        endGame.gameObject.SetActive(false);
        playAgain.gameObject.SetActive(false);
        result.gameObject.SetActive(true);
    }

    public void AgainGame()
    {
        border.SetActive(false);
        result.gameObject.SetActive(false);
        endGame.gameObject.SetActive(false);
        playAgain.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        board.EndHandler();
    }

    public void ButtonExit()
    {
        Application.Quit();
    }
}
