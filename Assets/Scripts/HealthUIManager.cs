using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager healthUIManager { get; private set; }

    public GameObject _heartPrefab;
    public Transform _heartContainer;

    private List<HeartController> hearts = new();

    private void Awake()
    {
        if (healthUIManager != null && healthUIManager != this)
        {
            Destroy(this);
        }
        else
        {
            healthUIManager = this;
        }
    }

    void Start()
    {
        GameManager.gameManager._playerHealth.OnHealthChanged += UpdateHearts;
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        int totalHearts = GameManager.gameManager._playerHealth.MaxHealth / 4;

        for (int i = 0; i < totalHearts; i++)
        {
            AddHeart(i);
        }

        UpdateHearts();
    }

    private void UpdateHearts()
    {
        int currentHealth = GameManager.gameManager._playerHealth.Health;
        int maxHealth = GameManager.gameManager._playerHealth.MaxHealth;
        int totalHearts = Mathf.CeilToInt((float)maxHealth / 4);

        // Ensure we have the correct number of hearts
        while (hearts.Count < totalHearts)
        {
            AddHeart(totalHearts - 1);
        }
        while (hearts.Count > totalHearts)
        {
            RemoveHeart();
        }

        // Update the fill state of each heart
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartHealth = 4; // Each heart represents 4 health, except potentially the last one

            if (i == totalHearts - 1 && maxHealth % 4 != 0)
            {
                // Last heart, and maxHealth is not a multiple of 4
                heartHealth = maxHealth % 4;
            }

            int fillAmount = Mathf.Min(currentHealth, heartHealth);
            currentHealth -= fillAmount;

            hearts[i].SetFillState(fillAmount);
            Debug.Log("Heart " + i + " filled: " + fillAmount);
        }
    }

    public void AddHeart(int heartIndex)
    {
        // Instantiate a new heart and add it to the list
        GameObject newHeart = Instantiate(_heartPrefab, _heartContainer);
        HeartController heartController = newHeart.GetComponent<HeartController>();
        hearts.Add(heartController);

        // Position the new heart
        float xPos = 16 + (heartIndex * 32) - 80;
        newHeart.transform.localPosition = new Vector3(xPos, 0f, 0f);
        Debug.Log("Heart " + heartIndex + " placed at xPos: " + xPos);
    }

    private void RemoveHeart()
    {
        if (hearts.Count > 1)
        {
            int lastIndex = hearts.Count - 1;
            HeartController lastHeart = hearts[lastIndex];

            // Destroy the GameObject associated with the last heart
            Destroy(lastHeart.gameObject);

            // Remove the reference from the list
            hearts.RemoveAt(lastIndex);
        }
    }
}
