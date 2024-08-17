using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

namespace lxxj
{
    public class MetricsUIText : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI rollingText; 
        public TextMeshProUGUI enemyAttackText; 
        public TextMeshProUGUI playerDeathText;

        private int amountOfRolls = 0;
        private int amountOfEnemyAttacks = 0;
        private int amountOfPlayerDeaths = 0;
        #endregion

        // Singleton so this object persists through scenes
        private void Awake()
        {
            if (FindObjectsOfType<MetricsUIText>().Length > 1)
            {
                Destroy(gameObject); 
                return;
            }

            DontDestroyOnLoad(gameObject); 
        }

        private void Start()
        {
            ClearTextFields();
        }

        // Clear text
        private void ClearTextFields()
        {
            rollingText.text = "";
            enemyAttackText.text = "";
            playerDeathText.text = "";
        }

        // Updates roll text
        public void UpdateRollText(string cameraView)
        {
            amountOfRolls += 1;
            rollingText.text = cameraView + "roll dodge input recorded at: " + System.DateTime.Now.ToString("HH:mm:ss.fff") + " X" + amountOfRolls;
        }
        
        // Updates enemy attack text
        public void UpdateEnemyAttackText()
        {
            amountOfEnemyAttacks += 1;
            enemyAttackText.text = "Enemy attack recorded at: " + System.DateTime.Now.ToString("HH:mm:ss.fff") + " X" + amountOfEnemyAttacks;
        }

        // Updates player death text
        public void UpdatePlayerDeathText()
        {
            amountOfPlayerDeaths += 1;
            playerDeathText.text = "Player death recorded at: " + System.DateTime.Now.ToString("HH:mm:ss.fff") + " X" + amountOfPlayerDeaths;
        }
    }
}
