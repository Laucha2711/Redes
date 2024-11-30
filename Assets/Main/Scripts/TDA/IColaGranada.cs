using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IColaGranada
{
    void InicializarCola();
    void Acolar(GameObject x);
    void Desacolar();
    bool ColaVacia();
    GameObject Primero();
}
