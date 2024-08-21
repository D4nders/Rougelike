using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    public Image[] _heartImages;

    public void SetFillState(int fillAmount)
    {
        // Deactivate all heart images initially
        foreach (var image in _heartImages)
        {
            image.gameObject.SetActive(false);
        }

        // Activate the appropriate heart image based on fillAmount
        switch (fillAmount)
        {
            case 0:
                _heartImages[0].gameObject.SetActive(true); // Empty heart
                break;
            case 1:
                _heartImages[1].gameObject.SetActive(true); // 1/4 heart
                break;
            case 2:
                _heartImages[2].gameObject.SetActive(true); // 1/2 heart
                break;
            case 3:
                _heartImages[3].gameObject.SetActive(true); // 3/4 heart
                break;
            case 4:
                _heartImages[4].gameObject.SetActive(true); // Full heart
                break;
            default:
                Debug.LogError("Invalid fillAmount: " + fillAmount);
                break;
        }
    }
}
