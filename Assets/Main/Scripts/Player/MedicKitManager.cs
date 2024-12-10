using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MedicKitManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ColaMedicKit c1 = new ColaMedicKit();

    private LifeManager lm;

    private void Start()
    {
        lm = gameObject.GetComponent<LifeManager>();
    }

    public void ThrowableUp(GameObject _object)
    {
        if (!c1.ColaCompleta())
        {
            c1.Acolar(_object);
        }
    }

    public void ApplyMedicKit()
    {
        Debug.LogError("entre");
        lm.RPC_RegenerationLife(1);
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !c1.ColaVacia())
            {
                ApplyMedicKit();
                c1.Desacolar();
            }
        }
    }
}
