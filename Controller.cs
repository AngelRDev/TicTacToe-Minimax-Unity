using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public BoardModel boardModel;
    public View view;

    public int turn = -1;
    private int indexX;
    private int indexY;

    public int AITurn = -2;
    public int freeSpaces = 9;

    public int playerTurn = 0;

    public float[] scores = new float[] { -10f, 10f, 0f }; // If player Player, AI, Draw (0,1,2) values for minimax() lowest for player, highest for AI, 0 for draw

    void Start()
    {
        boardModel.ClearBoard();
        view.ClearBoard();
    }

    private void Update()
    {
        AIMove();
    }

    public void SetPosition(int pos)
    {
        if (turn == -1) { // if player hasn't selected turn but already clicks on board, it sets PLAYER to play first, AI second
            turn = 0;
            AITurn = 1;
        }

        CalculateCoordinates(pos); // calculates indexes of [,] from a single position
        

        if (boardModel.GetPosition(indexX, indexY) != -1 || freeSpaces <= 0) // If position is not valid (free)
            return;

        boardModel.SetPosition(indexX, indexY, turn);
        view.DrawBoard(pos, turn);

        freeSpaces--; // recude available slots on board
        CheckWin(true);
        // Switch turns
        if (turn == 0) turn = 1;
        else if (turn == 1) turn = 0;
    }


    private void AIMove()
    {
        if(turn == AITurn) {
            BestMove();
        }
    }

    public void SelectChip(int _turn)
    {
        if (_turn == 0) {
            AITurn = 1;
            playerTurn = turn;
        }
        else {
            AITurn = 0;
            playerTurn = 1;
            // switch minimax values in case AI plays first
            scores[0] = 10;
            scores[1] = -10;
        }
        turn = 0;
    }

    // Access 2-dimensional array from an index
    private void CalculateCoordinates(int index)
    {
        for (var i = 0; i < 3; i++)
        {
            //check if the index parameter is in the row
            if (index < (3 * i) + 3 && index >= 3 * i)
            {
                indexY = index - 3 * i;
                indexX = i;
            }
        }
    }

    // Finds best move for AI
    void BestMove()
    {
        float bestScore = -Mathf.Infinity;
        Vector2 move = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardModel.GetPosition(i, j) == -1)
                {
                    // set new position AI
                    boardModel.SetPosition(i, j, AITurn);
                    freeSpaces--;
                    float score = Minimax(boardModel.GetBoard(), 0, false);
                    
                    // reset to previous
                    boardModel.SetPosition(i, j, -1);
                    freeSpaces++;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        move = new Vector2(i, j);
                    }
                }
            }
        }
        SetPosition((3 * (int)move.x) + (int)move.y);
    }
  
    // Recursive search of best move for AI
    float Minimax(int[,] board, int depth, bool isMaximizing)
    {
        int result = CheckWin(false);

        if (result != -1)
        {
            return scores[result];
        }

        if (isMaximizing)
        {
            // AI TURN
            float bestScore = -Mathf.Infinity;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardModel.GetPosition(i, j) == -1)
                    {
                        boardModel.SetPosition(i, j, AITurn);
                        freeSpaces--;

                        float score = Minimax(board, depth + 1, false);

                        boardModel.SetPosition(i, j, -1);
                        freeSpaces++;
                        bestScore = Mathf.Max(score, bestScore);                    
                    }
                }
            }

            return bestScore;
        }
        else
        {
            // PLAYER turn
            float bestScore = Mathf.Infinity;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardModel.GetPosition(i, j) == -1)
                    {
                        boardModel.SetPosition(i, j, playerTurn);
                        freeSpaces--;
                        float score = Minimax(board, depth + 1, true);             
                        boardModel.SetPosition(i, j, -1);
                        freeSpaces++;
                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }
    
    // Check if game is finished, PLAYER/AI win or DRAW
    int CheckWin(bool realcheck)
    {
        int winner = -1; // no result

        // horizontal
        for (int i = 0; i < 3; i++)
        {
            if (boardModel.GetPosition(i, 0) == boardModel.GetPosition(i, 1) && boardModel.GetPosition(i, 0) == boardModel.GetPosition(i, 2) && boardModel.GetPosition(i, 0) != -1)
            {
                winner = boardModel.GetPosition(i, 0);
            }
        }

        // vertical
        for (int i = 0; i < 3; i++)
        {
            if (boardModel.GetPosition(0, i) == boardModel.GetPosition(1, i) && boardModel.GetPosition(0, i) == boardModel.GetPosition(2, i) && boardModel.GetPosition(0, i) != -1)
            {
                winner = boardModel.GetPosition(0, i);
            }
        }
        // Diagonal
        if (boardModel.GetPosition(0, 0) == boardModel.GetPosition(1, 1) && boardModel.GetPosition(0, 0) == boardModel.GetPosition(2, 2) && boardModel.GetPosition(0, 0) != -1)
        {
            winner = boardModel.GetPosition(0, 0);
        }
        if (boardModel.GetPosition(2, 0) == boardModel.GetPosition(1, 1) && boardModel.GetPosition(2, 0) == boardModel.GetPosition(0, 2) && boardModel.GetPosition(2, 0) != -1)
        {
            winner = boardModel.GetPosition(2, 0);
        }

        if (winner == -1 && freeSpaces <= 0)
        {
            if(realcheck) view.SetWinnner("Draw");
            return 2;
        }
        else
        {
            if(realcheck) {
                if (winner == AITurn) view.SetWinnner("AI wins");
                else if(winner == playerTurn) view.SetWinnner("Player wins");
            }
            return winner;
        }
    }

    // Restart game
    public void Restart()
    {
        turn = -1;
        AITurn = -2;
        playerTurn = 0;
        freeSpaces = 9;

        // reset minimax values in case AI played first
        scores[0] = -10;
        scores[1] = 10;
        boardModel.ClearBoard();
        view.ClearBoard();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
}