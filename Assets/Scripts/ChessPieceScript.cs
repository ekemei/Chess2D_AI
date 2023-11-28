using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ChessPieceScript : MonoBehaviour
{
    // Reference na objekt Controller
    public GameObject controller;
    public GameObject movePlate;

    // Pozice na šachovnici
    private int xBoard = -1;
    private int yBoard = -1;

    // Tým
    public bool player;

    // Reference pro každý chesspiece
    public Sprite b_pawn, b_rook, b_knight, b_bishop, b_queen, b_king;
    public Sprite w_pawn, w_rook, w_knight, w_bishop, w_queen, w_king;

    // Pøiøadí figurce sprite a pøemìní pozici v poli na reálnou pozici na ploše
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        // Promìní pozice na šachovnici v pozice v Unity
        SetCoordinates();

        switch(this.name)
        {
            // Èerná strana
            case "b_pawn": this.GetComponent<SpriteRenderer>().sprite = b_pawn; player = false; break;
            case "b_rook": this.GetComponent<SpriteRenderer>().sprite = b_rook; player = false; break;
            case "b_knight": this.GetComponent<SpriteRenderer>().sprite = b_knight; player = false; break;
            case "b_bishop": this.GetComponent<SpriteRenderer>().sprite = b_bishop; player = false; break;
            case "b_queen": this.GetComponent<SpriteRenderer>().sprite = b_queen; player = false; break;
            case "b_king": this.GetComponent<SpriteRenderer>().sprite = b_king; player = false; break;
            // Bílá strana
            case "w_pawn": this.GetComponent<SpriteRenderer>().sprite = w_pawn; player = true;  break;
            case "w_rook": this.GetComponent<SpriteRenderer>().sprite = w_rook; player = true; break;
            case "w_knight": this.GetComponent<SpriteRenderer>().sprite = w_knight; player = true; break;
            case "w_bishop": this.GetComponent<SpriteRenderer>().sprite = w_bishop; player = true; break;
            case "w_queen": this.GetComponent<SpriteRenderer>().sprite = w_queen; player = true; break;
            case "w_king": this.GetComponent<SpriteRenderer>().sprite = w_king; player = true; break;
        }
    }

    // Zmìní typ figurky z pìšáka na královku a spraví sprite   
    public void PawnPromotion()
    {
        if (this.name == "w_pawn")
            this.name = "w_queen";
        if (this.name == "b_pawn")
            this.name = "b_queen";
        Activate();
    }

    // Pøemìní pozice v poli na reálne pozice ve høe
    public void SetCoordinates()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.66f;
        y *= 0.66f;

        // Kompenzace rozdílu, jelikož støed šachovnice je na pozici [0,0]
        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x, y, -1);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    // Vygeneruje ukazatele, pokud je hráè na øadì
    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            // Znièí ukazatele
            DestroyMovePlates();

            // Vytvoøí ukazatele
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        // Vytvoø pole MovePlatù
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");

        // Zniè každý MovePlate
        for(int i = 0; i < movePlates.Length; i++)
            Destroy(movePlates[i]);
    }

    // Vytvoøí moveplate pro danou figurku podle jejího typu
    public void InitiateMovePlates()
    {
        switch(this.name)
        {
            
            case "b_queen":
            case "w_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "b_knight":
            case "w_knight":
                LMovePlate();
                break;
            case "b_bishop":
            case "w_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "b_king":
            case "w_king":
                SurroundMovePlate();
                break;
            case "b_rook":
            case "w_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "b_pawn":
                // Pokud je na startovní pozici
                if (yBoard == 6)
                    PawnMovePlate(xBoard, yBoard - 2, true);
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "w_pawn":
                // Pokud je na startovní pozici
                if(yBoard == 1)
                    PawnMovePlate(xBoard, yBoard + 2, true);
                PawnMovePlate(xBoard, yBoard + 1);
                break;
        }
    }

    // Generuje rovnou èáru MovePlatù v jednom smìru (i diagonálnì)
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        // Neustále generuje ukazatel pohybu v jednom smìru dokud nenarazí na konec herní plochy nebo figurku
        while(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        // Pokud je figurka nepøátelská, tak vytvoøí ukazatel útoku
        if(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<ChessPieceScript>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    // Generuje MovePlate ve tvaru L pro jezdce
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    // Generuje MovePlate o délce 1 v každém smìru
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    // Generuje 1 MovePlate na souøadnicích x a y
    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        // Kontroluje zda je pozice mimo herní plochu
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);
            
            // Pokud je pozice prázdná, generuje ukazatel pohybu, pokud je na pozici nepøitel generuje ukazatel útoku
            if(cp == null)
                MovePlateSpawn(x, y);
            else if (cp.GetComponent<ChessPieceScript>().player != player)
                MovePlateAttackSpawn(x, y);
        }
    }

    // Generuje MovePlate pro pìšce
    public void PawnMovePlate(int x, int y, bool longMove = false)
    {
        Game sc = controller.GetComponent<Game>();

        // Kontroluje zda je pozice na herní ploše
        if (sc.PositionOnBoard(x, y))
        {
            // Pokud je pozice prázdná, generuje ukazatel pohybu
            if(sc.GetPosition(x,y) == null)
            {
                MovePlateSpawn(x, y);
            }
            // Pokud se nehýbe o 2 políèka
            if (!longMove)
            {
                // Pokud je šikmo nepøátelská figurka, generuje ukazatel útoku
                if (sc.PositionOnBoard(x + 1, y)
                    && sc.GetPosition(x + 1, y) != null
                    && sc.GetPosition(x + 1, y).GetComponent<ChessPieceScript>().player != player)
                {
                    MovePlateAttackSpawn(x + 1, y);
                }
                // Generuje ukazatel útoku v druhém smìru
                if (sc.PositionOnBoard(x - 1, y)
                    && sc.GetPosition(x - 1, y) != null
                    && sc.GetPosition(x - 1, y).GetComponent<ChessPieceScript>().player != player)
                {
                    MovePlateAttackSpawn(x - 1, y);
                }
            }
        }
    }

    // Vytvoøí MovePlate pohybu
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        // Vytvoøí MovePlate
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        // Vytvoøí referenci pro danou figurku a nastaví souøadnice ukazatele
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoordinates(matrixX, matrixY);
    }

    // Vytvoøí MovePlate útoku
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        // Vytvoøí MovePlate
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        // Vytvoøí referenci pro danou figurku, nastaví souøadnice ukazatele, a nastaví Attack na true
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoordinates(matrixX, matrixY);
    }
}
