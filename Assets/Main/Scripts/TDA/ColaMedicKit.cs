using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColaMedicKit : IColaGranada
{
    GameObject[] a;
    int indice;

    public ColaMedicKit()
    {
        InicializarCola();
    }

    public void InicializarCola()
    {
        a = new GameObject[100];
        indice = 0;
    }

    public void Acolar(GameObject x)
    {
        for (int i = indice - 1; i >= 0; i--)
        {
            a[i + 1] = a[i];
        }
        a[0] = x;
        indice++;
    }

    public void Desacolar()
    {
        indice--;
    }

    public bool ColaVacia()
    {
        return (indice == 0);
    }

    public bool ColaCompleta()
    {
        return (indice == 3);
    }

    public GameObject Primero()
    {
        return a[indice - 1];
    }
}