using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Game : MonoBehaviour
{
    // Reference na prefab Chesspiece
    public GameObject chesspiece;
    // Doèasný chesspiece
    private GameObject tempPiece;
    // Reference na controller
    public GameObject controller;
    // Reference na ChessMove
    public GameObject chessMove;
    // Všechny pozice na šachovnici
    private GameObject[,] positions = new GameObject[8, 8];
    // Všechny èerné figurky
    private GameObject[] playerB = new GameObject[16];
    // Všechny bíle figurky
    private GameObject[] playerW = new GameObject[16];
    // Poèet kol
    private int turnCount = 1;

    // Kdo je na øadì?
    private bool player = true;
    // Skonèila hra?
    private bool gameOver = false;
    // Simulate game end
    private bool simulatedEnd = false;
    // Obtížnost AI
    public static int difficulty = 0;
    // Start se zavolá pøed prvním snímkem
    void Start()
    {

        playerW = new GameObject[]
        {
            Create("w_pawn", 0,1),
            Create("w_pawn", 1,1),
            Create("w_pawn", 2,1),
            Create("w_pawn", 3,1),
            Create("w_pawn", 4,1),
            Create("w_pawn", 5,1),
            Create("w_pawn", 6,1),
            Create("w_pawn", 7,1),

            Create("w_rook", 0,0),
            Create("w_knight", 1,0),
            Create("w_bishop", 2,0),
            Create("w_king", 4,0),
            Create("w_queen", 3,0),
            Create("w_bishop", 5,0),
            Create("w_knight", 6,0),
            Create("w_rook", 7,0),
        };
        playerB = new GameObject[]
        {
            Create("b_pawn", 0, 6),
            Create("b_pawn", 1, 6),
            Create("b_pawn", 2, 6),
            Create("b_pawn", 3, 6),
            Create("b_pawn", 4, 6),
            Create("b_pawn", 5, 6),
            Create("b_pawn", 6, 6),
            Create("b_pawn", 7, 6),

            Create("b_rook", 0,7),
            Create("b_knight", 1,7),
            Create("b_bishop", 2,7),
            Create("b_queen", 3,7),
            Create("b_king", 4,7),
            Create("b_bishop", 5,7),
            Create("b_knight", 6,7),
            Create("b_rook", 7,7),
        };

        for(int i = 0; i < playerW.Length; i++)
        {
            SetPosition(playerB[i]);
            SetPosition(playerW[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        ChessPieceScript cps = obj.GetComponent<ChessPieceScript>();
        cps.name = name;
        cps.SetXBoard(x);
        cps.SetYBoard(y);
        cps.Activate();
        return obj;
    }

    // Pohne danou figurkou
    public void SetPosition(GameObject obj)
    {
        ChessPieceScript cps = obj.GetComponent<ChessPieceScript>();
        positions[cps.GetXBoard(), cps.GetYBoard()] = obj;
    }

    // Nastaví danou pozici v poli pozic jako prázdnou
    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    // Pokud je zvolená pozice out of bounds, tak vrátí false, jinak vrátí true
    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1))
            return false;
        return true;
    }

    // Kdo je na øadì?
    public bool GetCurrentPlayer()
    {
        return player;
    }

    // Skonèila hra?
    public bool IsGameOver()
    {
        return gameOver;
    }

    // Zmìní kdo je na øadì
    public void NextTurn()
    {
        player = !player;
        turnCount++;
    }

    // Pokud hra skonèila, restartuje ji
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gameOver)
            {
                gameOver = false;

                // Nahraje se GameOver menu
                SceneManager.LoadScene("Menu");
            }
        }
        // Umìlá inteligence
        if (difficulty != 0 && player)
        {
            AIMove();
            NextTurn();
        }
    }

    public void CheckStalemate()
    {
        int elements = 0;

        for(int x = 0; x < positions.GetLength(0); x++)
            for(int y = 0; y < positions.GetLength(0); y++)
                if (positions[x, y] != null) elements++;

        if(elements == 2)
        {
            gameOver = true;

            // Zobrazí vítìze
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = "Remíza";
            // Zobrazí restart text
            GameObject.FindGameObjectWithTag("RestartText").GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }

    // Spustí se pøi výhøe jedne strany a zobrazí text vítìze
    public void Winner(string winner)
    {
        // Ukonèí hru
        gameOver = true;

        // Zobrazí vítìze
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = winner + " je vítìz";
        // Zobrazí restart text
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<TextMeshProUGUI>().enabled = true;
    }

    public void AIMove()
    {

        ChessMove move;

        SimulateMove(new ChessMove(0,1,0,6));

        Debug.Log(turnCount);

        // Londýnský systém
        switch (turnCount)
        {
            case 1:
                move = new ChessMove(3, 1, 3, 3); // bílý pìšák
                break;
            case 3:
                move = new ChessMove(2,0,5,3); // bílý støelec
                break;
            case 5:
                move = new ChessMove(4,1,4,2); // bílý pìšák
                break;
            case 7:
                move = new ChessMove(6,0,5,2); // bílý jezdec
                break;
            case 9:
                move = new ChessMove(1,0,3,1); // bílý jezdec
                break;
            case 11:
                move = new ChessMove(2,1,2,2); // èerný pìšák
                break;
            case 13:
                move = new ChessMove(5,0,3,2); // èerný støelec
                break;
            default:
                move = FindBestMove(difficulty + 1);
                break;
        }

        Debug.Log("AI Move: [" + move.StartX +";" + move.StartY+"] -> [" + move.EndX +";"+ move.EndY+"]");

        chesspiece = GetPosition(move.StartX,move.StartY);

        if (positions[move.EndX, move.EndY] != null)
        {
            if (positions[move.EndX, move.EndY].name == "b_king")
                controller.GetComponent<Game>().Winner("Èerná");
            if (!chesspiece.GetComponent<ChessPieceScript>().player)
                Destroy(positions[move.EndX, move.EndY]);
        }

        chesspiece.GetComponent<ChessPieceScript>().SetXBoard(move.EndX);
        chesspiece.GetComponent<ChessPieceScript>().SetYBoard(move.EndY);
        positions[chesspiece.GetComponent<ChessPieceScript>().GetXBoard(), chesspiece.GetComponent<ChessPieceScript>().GetYBoard()] = chesspiece;
        chesspiece.GetComponent<ChessPieceScript>().SetCoordinates();
    }
    
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    // Vypoèítá hodnotu všech figurek na šachovnici z pohledu èerné strany
    public int MaterialEvaluation(bool maximasingPlayer, GameObject[,] chessboard)
    {
        int totalValue = 0;

        for (int y = 0; y < positions.GetLength(0); y++)
            for (int x = 0; x < positions.GetLength(1); x++)
                if (chessboard[x, y] != null)
                {
                    totalValue += PieceEvaluation(x, y, chessboard);
                }

        if (!maximasingPlayer)
            totalValue = -totalValue;
        Debug.Log("Player: "+maximasingPlayer+" - "+totalValue);
        return totalValue;
    }
    
    // Vypoèítá hodnotu dané figurky
    public int PieceEvaluation(int x, int y, GameObject[,] chessboard)
    {
        int value = 0;

        controller = GameObject.FindGameObjectWithTag("GameController");
        GameObject cp = chessboard[x,y];

        switch (cp.name)
        {
            // Hodnoty figurek
            case "w_pawn":
                return -1;
            case "w_rook":
                return -5;
            case "w_knight":
                return -3;
            case "w_bishop":
                return -3;
            case "w_queen":
                return -9;
            case "w_king":
                return -200;

            case "b_pawn":
                return 1;
            case "b_rook":
                return 5;
            case "b_knight":
                return 3;
            case "b_bishop":
                return 3;
            case "b_queen":
                return 9;
            case "b_king":
                return 200;
        }
        return value;
    }

    public List<ChessMove> GenerateAllMoves(GameObject[,] chessboard)
    {
        List<ChessMove> chessMoveList = new List<ChessMove>();

        for (int x = 0; x < chessboard.GetLength(0); x++)
            for (int y = 0; y < chessboard.GetLength(1); y++)
                chessMoveList.AddRange(GenerateOneMove(x, y, chessboard));

        return chessMoveList;
    }

    public List<ChessMove> GenerateOneMove(int x, int y, GameObject[,] chessboard)
    {
        List<ChessMove> allMovesForPiece = new List<ChessMove>();
        GameObject cp = chessboard[x, y];
        ChessMove cm;

        if (cp != null)
        {
            if (cp.name == "w_pawn")
            {
                if (PositionOnBoard(x, y + 2))
                {
                    if (chessboard[x, y + 1] == null && chessboard[x, y + 2] == null && y == 1)
                    {
                        cm = new ChessMove(x, y, x, y + 2);
                        allMovesForPiece.Add(cm);
                    }
                }
                if (PositionOnBoard(x, y + 1))
                {
                    if(chessboard[x,y+1] == null)
                    {
                        cm = new ChessMove(x, y, x, y + 1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Pawn, shortmove: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if(PositionOnBoard(x+1,y+1))
                {
                    if(chessboard[x+1,y+1] != null && chessboard[x + 1, y + 1].GetComponent<ChessPieceScript>().player != chessboard[x, y].GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+1, y + 1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Pawn, attackmove1: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x - 1, y + 1))
                {
                    if (chessboard[x - 1, y + 1] != null && chessboard[x - 1, y + 1].GetComponent<ChessPieceScript>().player != chessboard[x, y].GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-1, y + 1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Pawn, attackmove2: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
            }
            // Èerný pìšák
            if (cp.name == "b_pawn")
            {
                if (PositionOnBoard(x, y - 2))
                {
                    if (chessboard[x, y - 1] == null && chessboard[x, y - 2] == null && y == 6)
                    {
                        cm = new ChessMove(x, y, x, y - 2);
                        allMovesForPiece.Add(cm);
                    }
                }
                if (PositionOnBoard(x, y - 1))
                {
                    if (chessboard[x, y - 1] == null)
                    {
                        cm = new ChessMove(x, y, x, y - 1);
                        allMovesForPiece.Add(cm);
                    }
                }
                if (PositionOnBoard(x + 1, y - 1))
                {
                    if (chessboard[x + 1, y - 1] != null && chessboard[x + 1, y - 1].GetComponent<ChessPieceScript>().player != chessboard[x, y].GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x + 1, y - 1);
                        allMovesForPiece.Add(cm);
                    }
                }
                if (PositionOnBoard(x - 1, y - 1))
                {
                    if (chessboard[x - 1, y - 1] != null && chessboard[x - 1, y - 1].GetComponent<ChessPieceScript>().player != chessboard[x, y].GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x - 1, y - 1);
                        allMovesForPiece.Add(cm);
                    }
                }
            }
            // Vìže
            if (cp.name == "b_rook" || cp.name == "w_rook")
            {
                // Nahoru
                for(int i = y; i < 8; i++)
                {
                    if(PositionOnBoard(x,i))
                    {
                        if (i != y)
                        {
                            if (chessboard[x, i] == null)
                            {
                                cm = new ChessMove(x,y,x,i);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[x, i].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // Dolu
                for (int i = y; i > 0; i--)
                {
                    if (PositionOnBoard(x, i))
                    {
                        if (i != y)
                        {
                            if (chessboard[x, i] == null)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[x, i].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // Doprava
                for (int i = x; i < 8; i++)
                {
                    if (PositionOnBoard(i, y))
                    {
                        if (i != y)
                        {
                            if (chessboard[i, y] == null)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[i, y].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // Doleva
                for (int i = x; i > 0; i--)
                {
                    if (PositionOnBoard(i, y))
                    {
                        if (i != y)
                        {
                            if (chessboard[i, y] == null)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[i, y].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            // Jezdci
            if(cp.name == "w_knight" || cp.name == "b_knight")
            {
                if(PositionOnBoard(x+2,y+1))
                {
                    if(chessboard[x+2,y+1] == null)
                    {
                        cm = new ChessMove(x, y, x+2, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    } else if(chessboard[x+2, y+1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+2, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x+2, y-1))
                {
                    if (chessboard[x+2, y-1] == null)
                    {
                        cm = new ChessMove(x, y, x+2, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x+2, y-1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+2, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-2, y+1))
                {
                    if (chessboard[x-2, y+1] == null)
                    {
                        cm = new ChessMove(x, y, x-2, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-2, y+1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-2, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-2, y-1))
                {
                    if (chessboard[x-2, y-1] == null)
                    {
                        cm = new ChessMove(x, y, x-2, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-2, y-1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-2, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x+1, y+2))
                {
                    if (chessboard[x+1, y+2] == null)
                    {
                        cm = new ChessMove(x, y, x+1, y+2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x+1, y+2].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+1, y+2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x+1, y-2))
                {
                    if (chessboard[x+1, y-2] == null)
                    {
                        cm = new ChessMove(x, y, x+1, y-2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x+1, y-2].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+1, y-2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-1, y+2))
                {
                    if (chessboard[x-1, y+2] == null)
                    {
                        cm = new ChessMove(x, y, x-1, y+2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-1, y+2].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-1, y+2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-1, y-2))
                {
                    if (chessboard[x-1, y-2] == null)
                    {
                        cm = new ChessMove(x, y, x-1, y-2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-1, y-2].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-1, y-2);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("Knight: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
            }
            // Støelci
            if(cp.name == "w_bishop" || cp.name == "b_bishop")
            {
                int i = x;
                int j = y;

                // Nahoru doprava
                while(PositionOnBoard(i,j))
                {
                    if (i != x && j != y) {
                        if (chessboard[i, j] == null)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TR, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[i, j].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TR, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    i++;
                    j++;
                }
                i = x;
                j = y;
                // Dolu doleva
                while (PositionOnBoard(i, j))
                {
                    if (i != x && j != y)
                    {
                        if (chessboard[i, j] == null)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BL, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[i, j].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BL, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    i--;
                    j--;
                }
                i = x;
                j = y;
                // Nahoru doleva
                while(PositionOnBoard(i,j))
                {
                    if (i != x && j != y)
                    {
                        if (chessboard[i, j] == null)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TL, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[i, j].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TL, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    i--;
                    j++;
                }
                i = x;
                j = y;
                // Dolu doprava
                while (PositionOnBoard(i, j))
                {
                    if (i != x && j != y)
                    {
                        if (chessboard[i, j] == null)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BR, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[i, j].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, i, j);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BR, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    i++;
                    j--;
                }
            }
            // Králové
            if(cp.name == "w_king" || cp.name == "b_king")
            {
                if(PositionOnBoard(x+1,y))
                {
                    if(chessboard[x+1,y] == null)
                    {
                        cm = new ChessMove(x, y, x+1, y);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if(chessboard[x, y].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+1, y);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x+1, y+1))
                {
                    if (chessboard[x+1, y+1] == null)
                    {
                        cm = new ChessMove(x, y, x+1, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x+1, y+1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+1, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x+1, y-1))
                {
                    if (chessboard[x+1, y-1] == null)
                    {
                        cm = new ChessMove(x, y, x+1, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x+1, y-1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x+1, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x, y+1))
                {
                    if (chessboard[x, y+1] == null)
                    {
                        cm = new ChessMove(x, y, x, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x, y+1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x, y-1))
                {
                    if (chessboard[x, y-1] == null)
                    {
                        cm = new ChessMove(x, y, x, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x, y-1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-1, y+1))
                {
                    if (chessboard[x-1, y+1] == null)
                    {
                        cm = new ChessMove(x, y, x-1, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-1, y+1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-1, y+1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-1, y))
                {
                    if (chessboard[x-1, y] == null)
                    {
                        cm = new ChessMove(x, y, x-1, y);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-1, y].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-1, y);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
                if (PositionOnBoard(x-1, y-1))
                {
                    if (chessboard[x-1, y-1] == null)
                    {
                        cm = new ChessMove(x, y, x-1, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                    else if (chessboard[x-1, y-1].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                    {
                        cm = new ChessMove(x, y, x-1, y-1);
                        allMovesForPiece.Add(cm);
                        //Debug.Log("King, attack: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                    }
                }
            }
            // Královny
            if(cp.name == "w_queen" || cp.name == "b_queen")
            {
                // Nahoru
                for (int i = y; i < 8; i++)
                {
                    if (PositionOnBoard(x, i))
                    {
                        if (i != y)
                        {
                            if (chessboard[x, i] == null)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[x, i].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // Dolu
                for (int i = y; i > 0; i--)
                {
                    if (PositionOnBoard(x, i))
                    {
                        if (i != y)
                        {
                            if (chessboard[x, i] == null)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[x, i].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, x, i);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // Doprava
                for (int i = x; i < 8; i++)
                {
                    if (PositionOnBoard(i, y))
                    {
                        if (i != y)
                        {
                            if (chessboard[i, y] == null)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[i, y].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // Doleva
                for (int i = x; i > 0; i--)
                {
                    if (PositionOnBoard(i, y))
                    {
                        if (i != y)
                        {
                            if (chessboard[i, y] == null)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                            }
                            else if (chessboard[i, y].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                            {
                                cm = new ChessMove(x, y, i, y);
                                allMovesForPiece.Add(cm);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                int a = x;
                int b = y;

                // Nahoru doprava
                while (PositionOnBoard(a, b))
                {
                    if (a != x && b != y)
                    {
                        if (chessboard[a, b] == null)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TR, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[a, b].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TR, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    a++;
                    b++;
                }
                a = x;
                b = y;
                // Dolu doleva
                while (PositionOnBoard(a, b))
                {
                    if (a != x && b != y)
                    {
                        if (chessboard[a, b] == null)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BL, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[a, b].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BL, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    a--;
                    b--;
                }
                a = x;
                b = y;
                // Nahoru doleva
                while (PositionOnBoard(a, b))
                {
                    if (a != x && b != y)
                    {
                        if (chessboard[a, b] == null)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TL, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[a, b].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-TL, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    a--;
                    b++;
                }
                a = x;
                b = y;
                // Dolu doprava
                while (PositionOnBoard(a, b))
                {
                    if (a != x && b != y)
                    {
                        if (chessboard[a, b] == null)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BR, empty: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                        }
                        else if (chessboard[a, b].GetComponent<ChessPieceScript>().player != cp.GetComponent<ChessPieceScript>().player)
                        {
                            cm = new ChessMove(x, y, a, b);
                            allMovesForPiece.Add(cm);
                            //Debug.Log("Bishop-BR, enemy: [" + cm.StartX + ";" + cm.StartY + "] -> [" + cm.EndX + ";" + cm.EndY + "]");
                            break;
                        }
                        else
                            break;
                    }
                    a++;
                    b--;
                }
            }
        }
        return allMovesForPiece;
    }

    
    public GameObject[,] SimulateMove(ChessMove move)
    {
        GameObject[,] chessboard = new GameObject[8, 8];
        System.Array.Copy(positions, chessboard, positions.Length);

        chesspiece = GetPosition(move.StartX, move.StartY);

        if (chessboard[move.EndX, move.EndY] != null)
        {
            if (chessboard[move.EndX, move.EndY].name == "w_king" 
                || chessboard[move.EndX, move.EndY].name == "b_king")
                simulatedEnd = true;
            chessboard[move.EndX, move.EndY] = null;
        }

        chessboard[move.EndX, move.EndY] = chesspiece;
        chessboard[move.StartX, move.StartY] = null;

        return chessboard;
    }

    // Stará funkce
    public GameObject[,] UndoMove(ChessMove move, GameObject[,] chessboard)
    {
        GameObject[,] newChessboard = new GameObject[8, 8];
        System.Array.Copy(chessboard, newChessboard, chessboard.Length);

        newChessboard[move.StartX, move.StartY] = chessboard[move.EndX, move.EndY];
        if (tempPiece != null)
            newChessboard[move.EndX, move.EndY] = tempPiece;
        else
            newChessboard[move.EndX, move.EndY] = null;

        return newChessboard;
    }



    public ChessMove FindBestMove(int depth)
    {
        ChessMove bestMove = null;
        GameObject[,] tempBoard = new GameObject[8, 8];
        System.Array.Copy(positions, tempBoard, positions.Length);
        int bestValue = int.MinValue;

        List<ChessMove> possibleMoves = GenerateAllMoves(positions);

        foreach (ChessMove move in possibleMoves)
        {
            tempBoard = SimulateMove(move);
            int eval = MinimaxAlgorithm(tempBoard, depth-1, int.MinValue, int.MaxValue, false);
            System.Array.Copy(positions, tempBoard, positions.Length);
            if (eval > bestValue)
            {
                bestValue = eval;
                bestMove = move;
            }
        }
        return bestMove;
    }

    public int MinimaxAlgorithm(GameObject[,] chessboard, int depth, int alpha, int beta, bool maximisingPlayer)
    {
        if (depth == 0 || simulatedEnd)
        {
            simulatedEnd = false;
            return MaterialEvaluation(maximisingPlayer, chessboard);
        }

        List<ChessMove> possibleMoves = GenerateAllMoves(chessboard);
        GameObject[,] tempBoard = new GameObject[8, 8];
        System.Array.Copy(chessboard, tempBoard, chessboard.Length);

        if (maximisingPlayer)
        {
            int maxEval = int.MinValue;
            foreach(ChessMove move in possibleMoves)
            {
                chessboard = SimulateMove(move);
                int eval = MinimaxAlgorithm(chessboard, depth-1, alpha, beta, false);
                System.Array.Copy(tempBoard, chessboard, chessboard.Length);
                maxEval = Mathf.Max(maxEval,eval);
                alpha = Mathf.Max(alpha,eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach(ChessMove move in possibleMoves)
            {
                chessboard = SimulateMove(move);
                int eval = MinimaxAlgorithm(chessboard, depth - 1, alpha, beta, true);
                System.Array.Copy(tempBoard, chessboard, chessboard.Length);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }
}
