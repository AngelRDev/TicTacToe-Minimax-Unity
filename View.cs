using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    [SerializeField] private Sprite chipX;
    [SerializeField] private Sprite chipO;

    public Text winText;
    public Image []boardChips;
    private Color _startColor;
    private Color _gameColor;



    private void Start()
    {
        _startColor = boardChips[0].color;
        _gameColor = _startColor;
        _gameColor.a = 1f;
    }

    public void DrawBoard(int pos, int turn)
    {
        if (turn == 0) {
            boardChips[pos].sprite = chipX;
            boardChips[pos].color = _gameColor;
        }
        else if (turn == 1) {
            boardChips[pos].sprite = chipO;
            boardChips[pos].color = _gameColor;
        }
    }

    public void SetWinnner(string winner)
    {
        winText.text = winner;
    }

    public void ClearBoard()
    {
        winText.text = "";
        for (int i = 0; i < 9; i++)
        {
            boardChips[i].sprite = null;
            boardChips[i].color = _startColor;
        }
    }

}
