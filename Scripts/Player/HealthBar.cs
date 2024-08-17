using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace lxxj
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;
        
        private void Start()
        {
            slider = GetComponent<Slider>();
        }

        // Setting slider values appropriate to max health value
        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        // Setting slider value appropriate to current health value
        public void SetCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }

    }
}
