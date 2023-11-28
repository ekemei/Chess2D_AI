using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    // Pozice na šachovnici, ne ve svìtì
    int matrixX;
    int matrixY;

    // false = pohyb; true = útok
    public bool attack = false;

    public void Start()
    {
        // Pokud útoèí, zmìní barvu MovePlatu na èervenou
        if (attack)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        else
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    }

    // Po kliknutí myší
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        

        // Pokud útoèí, znièí figurky na pøistávací pozici
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp.name == "w_king") 
                controller.GetComponent<Game>().Winner("Èerná");
            if (cp.name == "b_king")
                controller.GetComponent<Game>().Winner("Bílá");

            Destroy(cp);
        }

        controller.GetComponent<Game>().CheckStalemate();

        // Nastaví pùvodní pozici dané figurky na prázdnou
        controller.GetComponent<Game>().SetPositionEmpty(
            reference.GetComponent<ChessPieceScript>().GetXBoard(),
            reference.GetComponent<ChessPieceScript>().GetYBoard());

        // Zmìní pozici dané figurky na pøistávající pozici v ChessPieceScriptu
        reference.GetComponent<ChessPieceScript>().SetXBoard(matrixX);
        reference.GetComponent<ChessPieceScript>().SetYBoard(matrixY);
        reference.GetComponent<ChessPieceScript>().SetCoordinates();

        // Zmìní pozici dané figurky na pøistávající pozici v Game scriptu
        controller.GetComponent<Game>().SetPosition(reference);

        // Povýšení
        if ((reference.name == "w_pawn" && reference.GetComponent<ChessPieceScript>().GetYBoard() == 7) 
            || (reference.name == "b_pawn" && reference.GetComponent<ChessPieceScript>().GetYBoard() == 0)
            && reference != null)
        {
            reference.GetComponent<ChessPieceScript>().PawnPromotion();
        }

        // Zmìní kdo je na øadì
        controller.GetComponent<Game>().NextTurn();

        // Znièí všechny MovePlaty
        reference.GetComponent<ChessPieceScript>().DestroyMovePlates();
    }

    // Nastaví pozici ukazatele
    public void SetCoordinates(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    // Nastaví referenci ukazatele, neboli která figurka ho vytvoøila
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    // Vrátí objekt, který tento ukazatel vytvoøil
    public GameObject GetReference()
    {
        return reference;
    }
}
