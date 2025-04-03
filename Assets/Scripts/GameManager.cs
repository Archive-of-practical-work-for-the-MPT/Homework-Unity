using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int Health;

    public int MaxHealth = 100;

    private int Stamina = 500;

    public bool IsStaminaRestoring = false;

    public GameObject Player;

    public static GameManager ManagerInstance;

    public Slider StaminaBar;

    public Slider HealthBar;

    private void Awake()
    {
        ManagerInstance = this;
    }

    private void Start()
    {
        StaminaBar.maxValue = Stamina;
        HealthBar.maxValue = MaxHealth;
        Health = MaxHealth;
    }

    private IEnumerator StaminaRestore()
    {
        IsStaminaRestoring = true;
        yield return new WaitForSeconds(3);
        Stamina = 500;
        StaminaBar.value = 500;
        IsStaminaRestoring = false;
    }

    private void StaminaCheck()
    {
        if (Stamina <= 0) StartCoroutine(StaminaRestore()); 
    }

    public void FixedUpdate()
    {
        StaminaCheck();
    }

    public void SpendStamina()
    {
        Stamina -= 1;
        StaminaBar.value -= 1;
    }

    public bool Healing(int HealthPointCount)
    {
        if (Health != 100)
        {
            if (Health + HealthPointCount >= MaxHealth)
            {
                Health = MaxHealth;
                HealthBar.value = MaxHealth;
            }
            else {
                Health += HealthPointCount;
                HealthBar.value += HealthPointCount;
            }
            return true;
        }
        return false;
    }

    public void DamagePlayer(int Count)
    {
        if (Health > 0)
        {
            Health -= Count;
            HealthBar.value -= Count;
            Debug.Log("Вам нанесли урон в размере: " + Count);
        }

        if (Health <= 0)
        {
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(2);

        FindObjectOfType<PlayerController>().Respawn();
    }

    public void RespawnPlayer()
    {
        Health = MaxHealth;
        HealthBar.value = MaxHealth;
    }
}
