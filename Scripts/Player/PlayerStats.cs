using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class PlayerStats : CharacterStats
    {
        #region Variables
        HealthBar healthBar;
        StaminaBar staminaBar;
        PlayerManager playerManager;
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        GameSession gameSession;
        MetricsUIText metricsUIText;
        CameraSwitch cameraSwitch;
        public bool damageFlag;
        public float staminaRegenAmount = 30f;
        private float staminaRegenTimer = 0f;
        public Vector3 startingPosition;
        public bool successfulRollReading = false;

        [Header("GOD MODE SWITCH")]
        [SerializeField] bool isGodMode = false; 
        #endregion

        private void Awake()
        {
            startingPosition = transform.position;

            gameSession = FindObjectOfType<GameSession>();
            playerManager = GetComponent<PlayerManager>();  
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            animatorHandler = GetComponent<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
            metricsUIText = FindObjectOfType<MetricsUIText>();
            cameraSwitch = FindObjectOfType<CameraSwitch>();
        }

        // Setting max health & stam from level in character stats base class
        void Start()
        {
            maxHealth = SetMaxHealth();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            maxStamina = SetMaxStamina();
            currentStamina = maxStamina;

            staminaBar.SetMaxStamina(maxStamina);
        }
        private int SetMaxHealth()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }
        private float SetMaxStamina()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }
        
        // Logic for taking player damage including animation & godmode implementation
        public void TakeDamage(int damage)
        {
            if (isGodMode) return;

            if (playerManager.isInvulnerable)
            { 
                successfulRollReading = true; 
                return; 
            }

            if (isDead) return;

            damageFlag = true;
            Debug.Log("dmgflag = " + damageFlag);
            StartCoroutine(PauseForFrameOnDamage());

            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);

            animatorHandler.PlayTargetAnimation("Damage_01", true);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;

                metricsUIText.UpdatePlayerDeathText();
                animatorHandler.PlayTargetAnimation("Death", true);

                StartCoroutine(PauseForFrameOnDeath());
            }

        }

        // Coroutine that allows isDead frame to be true for a single frame (analytics logic)
        IEnumerator PauseForFrameOnDeath()
        {
            yield return null;      

            isDead = false;
    
            gameSession.ResetGame();
        }
        // Coroutine that allows damageFlag frame to be true for a single frame (analytics logic)
        IEnumerator PauseForFrameOnDamage()
        {
            yield return null;      

            damageFlag = false;
            Debug.Log("dmgflag = " + damageFlag);

        }

        // Logic for increasing the players health by set amount 
        public void Heal(int healAmount)
        {
            if (playerManager.isInvulnerable) { return; }
            if (isDead) {  return; }

            if (currentHealth < maxHealth)
            {
                currentHealth += healAmount;

                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                
                healthBar.SetCurrentHealth(currentHealth);
            }

        }

        // Logic for taking stamina damage when using ability
        public void TakeStaminaDamage(int damage)
        {
            currentStamina = currentStamina - damage;

            staminaBar.SetCurrentStamina(currentStamina);

        }

        // Logic for draining stam based on time.deltatime when sprinting
        public void DrainStamina(float damagePerSecond)
        {
            float damage = damagePerSecond * Time.deltaTime;
            currentStamina -= damage;

            if (currentStamina < 0)
            {
                currentStamina = 0;
            }

            staminaBar.SetCurrentStamina(currentStamina);
        }

        // Logic for regenerating stamina
        public void RegenStamina()
        {
            if (playerManager.isInteracting || inputHandler.sprintFlag)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
                
                if (currentStamina < maxStamina && staminaRegenTimer > 1f)
                {
                    currentStamina += staminaRegenAmount * Time.deltaTime;
                    staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }
        }
    }
}
