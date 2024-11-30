using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPilaGranadaTDA
{
    void InicializarPila();

    void Apilar(GameObject x);

    void Desapilar();

    bool PilaVacia();

    GameObject Tope();
}