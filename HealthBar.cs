using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controls
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        public void SetMaxHealth(int maxHealth)
        {
            //setting the slider's maximum value to the player's maximum value
            //the slider's current value will equal to the player's current health
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetCurrentHealth(int currentHealth)
        {
            //setting the slider to equal the current health
            slider.value = currentHealth;
        }
    }
}