using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth
{
    [SerializeField]
    int _currentMaxHealth;
    int _currentHealth;

    public delegate void HealthChangedEventHandler();
    public event HealthChangedEventHandler OnHealthChanged;

    public int Health
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
        }
    }

    public int MaxHealth
    {
        get
        {
            return _currentMaxHealth;
        }
        set
        {
            _currentMaxHealth = value;
        }
    }

    public UnitHealth(int health, int maxHealth)
    {
        _currentHealth = health;
        _currentMaxHealth = maxHealth;
    }

    public void DamageUnit(int damageAmount)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damageAmount;
        }
        OnHealthChanged?.Invoke();
    }

    public void HealUnit(int healAmount)
    {
        if (_currentHealth < _currentMaxHealth)
        {
            _currentHealth += healAmount;
        }
        if (_currentHealth > _currentMaxHealth)
        {
            _currentHealth = _currentMaxHealth;
        }
        OnHealthChanged?.Invoke();
    }

    public void IncreaseMaxHealth(int amount)
    {
        _currentMaxHealth += amount;
        _currentHealth = _currentMaxHealth; 
        OnHealthChanged?.Invoke();
    }

    public void DecreaseMaxHealth(int amount)
    {
        _currentMaxHealth -= amount;
        if (_currentMaxHealth < 1) _currentMaxHealth = 1;

        if (_currentHealth > _currentMaxHealth)
        {
            _currentHealth = _currentMaxHealth;
        }

        OnHealthChanged?.Invoke();
    }
}
