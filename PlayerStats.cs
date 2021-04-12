using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{


    public class PlayerStats : MonoBehaviour
    {
        public int sippyHealAmount = 3;
        public int maxSippyAmount = 20;
        public int sippyCount = 3;
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;
        public Texture2D sippyIcon;

        public HealthBar healthBar;
        
        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            
            healthBar.SetCurrentHealth(currentHealth);
        }

        public void DrinkSippy()
        {
            currentHealth += sippyHealAmount;
            sippyCount--;
            
            healthBar.SetCurrentHealth(currentHealth);
        }

        public void AddSippies(int sippiesIn)
        {
            sippyCount += sippiesIn;
            if (sippyCount > maxSippyAmount)
            {
                sippyCount = maxSippyAmount;
            }
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(10, 60, 100, 50), new GUIContent(" " + sippyCount, sippyIcon));
        }
    }
}
