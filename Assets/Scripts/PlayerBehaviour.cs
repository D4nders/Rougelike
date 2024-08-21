using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerDamage(1);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerHeal(1);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerAddMaxHealth(1);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerDecreaseMaxHealth(1);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }
    }

    private void PlayerDamage(int damage)
    {
        GameManager.gameManager._playerHealth.DamageUnit(damage);
    }

    private void PlayerHeal(int healing)
    {
        GameManager.gameManager._playerHealth.HealUnit(healing);
    }

    private void PlayerAddMaxHealth(int value)
    {
        GameManager.gameManager._playerHealth.IncreaseMaxHealth(value);
    }

    private void PlayerDecreaseMaxHealth(int value)
    {
        GameManager.gameManager._playerHealth.DecreaseMaxHealth(value);
    }
}
