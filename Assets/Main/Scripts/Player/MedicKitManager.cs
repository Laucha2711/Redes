using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicKitManager : MonoBehaviour
{
    [SerializeField]
    private ColaMedicKit c1 = new ColaMedicKit();
    [SerializeField]
    private LifeManager lm;
    public void ThrowableUp(GameObject _object)
    {
        if (!c1.ColaCompleta())
        {
            c1.Acolar(_object);
        }
    }

    public void ApplyMedicKit()
    {
        lm.RegenerationLife(1);
    }

    private void FixedUpdate()
    {
        if (lm.currentLife <= 2)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !c1.ColaVacia())
            {
                ApplyMedicKit();
                c1.Desacolar();
            }
        }
    }
}
