using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    // Pozice na �achovnici, ne ve sv�t�
    int matrixX;
    int matrixY;

    // false = pohyb; true = �tok
    public bool attack = false;

    public void Start()
    {
        // Pokud �to��, zm�n� barvu MovePlatu na �ervenou
        if (attack)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        else
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    }

    // Po kliknut� my��
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        

        // Pokud �to��, zni�� figurky na p�ist�vac� pozici
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp.name == "w_king") 
                controller.GetComponent<Game>().Winner("�ern�");
            if (cp.name == "b_king")
                controller.GetComponent<Game>().Winner("B�l�");

            Destroy(cp);
        }

        controller.GetComponent<Game>().CheckStalemate();

        // Nastav� p�vodn� pozici dan� figurky na pr�zdnou
        controller.GetComponent<Game>().SetPositionEmpty(
            reference.GetComponent<ChessPieceScript>().GetXBoard(),
            reference.GetComponent<ChessPieceScript>().GetYBoard());

        // Zm�n� pozici dan� figurky na p�ist�vaj�c� pozici v ChessPieceScriptu
        reference.GetComponent<ChessPieceScript>().SetXBoard(matrixX);
        reference.GetComponent<ChessPieceScript>().SetYBoard(matrixY);
        reference.GetComponent<ChessPieceScript>().SetCoordinates();

        // Zm�n� pozici dan� figurky na p�ist�vaj�c� pozici v Game scriptu
        controller.GetComponent<Game>().SetPosition(reference);

        // Pov��en�
        if ((reference.name == "w_pawn" && reference.GetComponent<ChessPieceScript>().GetYBoard() == 7) 
            || (reference.name == "b_pawn" && reference.GetComponent<ChessPieceScript>().GetYBoard() == 0)
            && reference != null)
        {
            reference.GetComponent<ChessPieceScript>().PawnPromotion();
        }

        // Zm�n� kdo je na �ad�
        controller.GetComponent<Game>().NextTurn();

        // Zni�� v�echny MovePlaty
        reference.GetComponent<ChessPieceScript>().DestroyMovePlates();
    }

    // Nastav� pozici ukazatele
    public void SetCoordinates(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    // Nastav� referenci ukazatele, neboli kter� figurka ho vytvo�ila
    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    // Vr�t� objekt, kter� tento ukazatel vytvo�il
    public GameObject GetReference()
    {
        return reference;
    }
}
