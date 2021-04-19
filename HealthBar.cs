using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controls
{
    // controls the slider box on the screen that represents health
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;
        
        // setting the slider's maximum value to the player's maximum value
        // the slider's current value will equal to the player's current health
        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        // setting the slider to equal the current health
        public void SetCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }
    }
}