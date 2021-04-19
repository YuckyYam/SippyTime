using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{

    // handles the players health and sippy count. would later be used for stamina
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
        
        // instantiates all the values needed at the start of the game, like the amount of health and sippies
        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        // sets a higher max health so that the slider changes more smoothly
        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        // reduces the amount of health the player has and updates the health bar
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            
            healthBar.SetCurrentHealth(currentHealth);
        }

        // heals the player at the cost of a sippy
        public void DrinkSippy()
        {
            // TODO have drinking animation call this when the animation is finished
            currentHealth += sippyHealAmount;
            sippyCount--;
            
            healthBar.SetCurrentHealth(currentHealth);
        }

        // gives more sippies to the player when they pick some up
        public void AddSippies(int sippiesIn)
        {
            sippyCount += sippiesIn;
            if (sippyCount > maxSippyAmount)
            {
                sippyCount = maxSippyAmount;
            }
        }

        // updates the gui every frame with the number of sippies
        private void OnGUI()
        {
            GUI.Box(new Rect(10, 60, 100, 50), new GUIContent(" " + sippyCount, sippyIcon));
        }
    }
}
