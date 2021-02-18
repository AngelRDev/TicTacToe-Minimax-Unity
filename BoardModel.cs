using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel : MonoBehaviour
{
    public int[,] positions = new int[3, 3];

    public void SetPosition(int x, int y, int value)
    {
        positions[x, y] = value;
    }

    public int GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public int[,] GetBoard()
    {
        return positions;
    }

    public void CloneBoard(int[,] newBoard)
    {
        newBoard = positions;
    }

    public void ClearBoard()
    {
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                SetPosition(i, j, -1);
            }
        }
    }
    
}
