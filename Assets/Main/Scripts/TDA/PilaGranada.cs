using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilaGranada : IPilaGranadaTDA
{
    GameObject[] a;
    int indice;

    public PilaGranada()
    {
        InicializarPila();
    }

    public void InicializarPila()
    {
        a = new GameObject[3];
        indice = 0;
    }

    public void Apilar(GameObject x)
    {
        a[indice] = x;
        indice++;
    }
    public void Desapilar()
    {
        indice--;
    }

    public bool PilaVacia()
    {
        return (indice == 0);
    }

    public bool PilaCompleta()
    {
        return (indice == 5);
    }

    public GameObject Tope() => a[indice - 1];
}