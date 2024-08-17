using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

namespace lxxj
{

    public class AnalyticsManager : MonoBehaviour
    {
        #region Variables
        InputHandler inputHandler;
        CameraSwitch cameraSwitch;
        PlayerStats playerStats;
        AttackState attackState;
        List<EnemyManager> enemyManagers; 
        MetricsUIText metricsUIText;
        PlayerManager playerManager;
        StartGameButton startGameButton;
        private float enemyAttackTime;
        private float playerRollTime;
        #endregion
        async void Start()
        {
            // Opens the game to Unity analytics services
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();

            inputHandler = FindObjectOfType<InputHandler>();
            cameraSwitch = FindObjectOfType<CameraSwitch>();
            playerStats = FindObjectOfType<PlayerStats>();
            attackState = FindObjectOfType<AttackState>();
            // Find all EnemyManager instances
            enemyManagers = new List<EnemyManager>(FindObjectsOfType<EnemyManager>()); 

            metricsUIText = FindObjectOfType<MetricsUIText>();
            playerManager = FindObjectOfType<PlayerManager>();
            startGameButton = FindObjectOfType<StartGameButton>();

            foreach (var enemyManager in enemyManagers)
            {
                Debug.Log("EnemyManager reference: " + enemyManager);
            }
        }
        private void FixedUpdate()
        {
            CheckRollAndEnemyAttackFrameFlags();
            CheckForDamageTaken();
            CheckForPlayerDeath();
            CheckForInactivity();
            CheckForSuccessfulRoll();
        }

        // Checks for roll input & enemy attack, sends events to Unity Analytics
        private void CheckRollAndEnemyAttackFrameFlags()
        {
        
            #region Player roll event

            if (cameraSwitch.isInTopDown)
            {
                if (inputHandler.rollFrameFlag && !playerManager.isInteracting && playerStats.currentStamina > 0)
                {
                    // Update UI first
                    metricsUIText.UpdateRollText("Top down ");

                    playerRollTime = Time.time;

                    // Unity analytics event sender syntax
                    CustomEvent myEvent = new CustomEvent("Top_down_roll_event")
                    {
                        { "Top_down_roll_event_parameter", "This is testing parameter"}
                    };
                    AnalyticsService.Instance.RecordEvent(myEvent);

                    Debug.Log("Recording top down event");                   
                    
                    CalculateTopDownTimeDifference();

                    inputHandler.rollFrameFlag = false;
                }
            }
            else
            {
                if (inputHandler.rollFrameFlag && !playerManager.isInteracting && playerStats.currentStamina > 0)
                {
                    // Update UI first
                    metricsUIText.UpdateRollText("Third person ");

                    playerRollTime = Time.time;

                    CustomEvent myEvent = new CustomEvent("Third_person_roll_event")
                    {
                        { "Third_person_roll_event_parameter", "This is testing parameter"}
                    };
                    AnalyticsService.Instance.RecordEvent(myEvent);

                    Debug.Log("Recording third person event");

                    CalculateThirdPersonTimeDifference();
                    inputHandler.rollFrameFlag = false;

                }
            }

            #endregion

            #region Enemy attack event

            foreach (var enemyManager in enemyManagers)
            {
                if (enemyManager.isPerformingAction)
                {         
                    // Update the UI first           
                    metricsUIText.UpdateEnemyAttackText();

                    enemyAttackTime = Time.time;

                    CustomEvent myEvent = new CustomEvent("Enemy_attack_event");
                    // {
                    //     { "Enemy_attack_event_parameter", "This is testing parameter"}
                    // }; 
                    AnalyticsService.Instance.RecordEvent(myEvent);

                    Debug.Log("Recording enemy attack event for " + enemyManager.name);
                    enemyManager.isPerformingAction = false;
                }
            }

            #endregion
        }

        // Calculates the difference between top-down enemy attack time and player dodge roll
        private void CalculateTopDownTimeDifference()
        {
            if (enemyAttackTime != 0 && playerRollTime != 0)
            {
                float difference = Mathf.Abs(enemyAttackTime - playerRollTime);
                Debug.Log("Top down time Difference: " + difference + " seconds");

                // Create the custom event parameters dictionary
                var eventParams = new Dictionary<string, object>
                {
                    { "Top_down_time_difference_event_parameter_1", "This is testing parameter" },
                    { "Top_down_time_difference", difference }
                };

                // Record the custom event with parameters
                AnalyticsService.Instance.CustomData("Top_down_time_difference_event", eventParams);
            }
            else
            {
                Debug.Log("Please record both times first.");
            }
        }

        // Calculates the difference between third person enemy attack time and player dodge roll
        private void CalculateThirdPersonTimeDifference()
        {
            if (enemyAttackTime != 0 && playerRollTime != 0)
            {
                float difference = Mathf.Abs(enemyAttackTime - playerRollTime);
                Debug.Log("Third person time Difference: " + difference + " seconds");

                // Create the custom event parameters dictionary
                var eventParams = new Dictionary<string, object>
                {
                    { "Third_person_time_difference_event_parameter_1", "This is testing parameter" },
                    { "Third_person_time_difference", difference }
                };

                // Record the custom event with parameters
                AnalyticsService.Instance.CustomData("Third_person_time_difference_event", eventParams);
            }
            else
            {
                Debug.Log("Please record both times first.");
            }
        }
    
        // Reads if player is taking damage and sends top-down & third person events to Unity Analytics
        private void CheckForDamageTaken()
        {
            if (cameraSwitch.isInTopDown && playerStats.damageFlag)
            {
                CustomEvent myEvent = new CustomEvent("Top_down_take_damage_event")
                {
                    { "Top_down_take_damage_event_parameter", "This is testing parameter"}
                };
                AnalyticsService.Instance.RecordEvent(myEvent);
            }
            else if (!cameraSwitch.isInTopDown && playerStats.damageFlag)
            {
                CustomEvent myEvent = new CustomEvent("Third_person_take_damage_event")
                {
                    { "Third_person_take_damage_event_parameter", "This is testing parameter"}
                };
                AnalyticsService.Instance.RecordEvent(myEvent);
            }
        }
        
        // Player death sends event to unity analytics for both camera views
        private void CheckForPlayerDeath()
        {         
            if (cameraSwitch.isInTopDown && playerStats.isDead)
            {
                CustomEvent myEvent = new CustomEvent("Top_down_death_event")
                {
                    { "Top_down_death_event_parameter", "This is testing parameter"}
                };
                AnalyticsService.Instance.RecordEvent(myEvent);
                Debug.Log("Recording top down player death event");
            }   
            else if (!cameraSwitch.isInTopDown && playerStats.isDead) 
            {
                CustomEvent myEvent = new CustomEvent("Third_person_death_event")
                {
                    { "Third_person_death_event_parameter", "This is testing parameter"}
                };
                AnalyticsService.Instance.RecordEvent(myEvent);

            }
        }
    
        // Checking for inactivity, if player has not done anything, send event to analytics
        private void CheckForInactivity()
        {
            if (inputHandler != null && inputHandler.isInactive)
            {
                CustomEvent myEvent = new CustomEvent("Player_inactive_event")
                {
                    { "Player_inactive_event_parameter", "This is a testing parameter" }
                };
                AnalyticsService.Instance.RecordEvent(myEvent);
                Debug.Log("Recording Player_inactive_event");

                // Reset isInactive to prevent multiple triggers
                inputHandler.isInactive = false;
            }
        }
    
        // Checks if player has avoided damage with a dodge roll, if so sends an event to unity analytics
        private void CheckForSuccessfulRoll()
        {
            if (playerStats.successfulRollReading)
            {
                CustomEvent myEvent = new CustomEvent("Successful_roll_event")
                    {
                        { "Successful_roll_event_parameter", "This is testing parameter"}
                    };
                    AnalyticsService.Instance.RecordEvent(myEvent);
                Debug.Log("roll success read");

                playerStats.successfulRollReading = false;
            }
        }  
       
    }
}
