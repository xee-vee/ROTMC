using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace lxxj
{
    public class StaminaBar : MonoBehaviour
    {
        public Slider slider;
        
        private void Start()
        {
            slider = GetComponent<Slider>();

            if (slider == null)
            {
                Debug.LogError("Slider component not found!");
            }
        }
        
        // Logic for linking stamina bar to player stats
        public void SetMaxStamina(float maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;
        }

        public void SetCurrentStamina(float currentStamina)
        {
            slider.value = currentStamina;
        }

    }
}