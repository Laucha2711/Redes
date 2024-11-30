using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField]
    int maxLife = 3;
    int currentLife;

    [SerializeField]
    private GameManager GM;

    private float time = 3;

    private bool canHit;

    private void Awake()
    {
        currentLife = maxLife;
        canHit = true;
    }

    private void Update()
    {
        if (!canHit)
        {
            time = time - 1 * Time.deltaTime;

            if (time <= 0)
            {
                canHit = true;
                time = 5;
            }
        }
        
    }

    public void GetDamage(int _damage)
    {
        if (canHit)
        {
            currentLife -= _damage;
            Debug.Log(currentLife);
            canHit = false;
            if (currentLife <= 0)
            {
                PlayerDead();
            }
        }
    }

    public void RegenerationLife(int _regeneration)
    {
        currentLife += _regeneration;
        Debug.Log(currentLife);
    }

    private void PlayerDead()
    {
        GM.LoseGame();
    }
}
