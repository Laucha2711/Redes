using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private ThrowableManager tm;
    private MedicKitManager mm;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject.layer == 6)
            {
                tm = other.GetComponent<ThrowableManager>();
                tm.ThrowableUp(gameObject);
                Destroy(gameObject);
            }
        }

        if (other.gameObject.tag == "Player")
        {
            if (gameObject.layer == 7)
            {
                mm = other.GetComponent<MedicKitManager>();
                mm.ThrowableUp(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
