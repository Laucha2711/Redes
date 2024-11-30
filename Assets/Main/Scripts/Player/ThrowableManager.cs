using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableManager : MonoBehaviour
{
    [SerializeField]
    private PilaGranada p1 = new PilaGranada();
    [SerializeField]
    private GameObject throwablePrefab;
    [SerializeField]
    private Transform throwableSpawnPoint;
    public float throwableSpeed = 10;
    float range = 10f;
    public void ThrowableUp(GameObject _object)
    {
        if (!p1.PilaCompleta())
        {
            p1.Apilar(_object);
        }
    }

    public void SpawnThrowable()
    {
        //p1.Tope().SetActive(true);
        GameObject Throwable = Instantiate(throwablePrefab, throwableSpawnPoint.position, throwableSpawnPoint.rotation);
        Throwable.GetComponent<Rigidbody>().AddForce(throwableSpawnPoint.forward * range, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G) && !p1.PilaVacia())
        {
            SpawnThrowable();
            p1.Desapilar();
        }
    }
}