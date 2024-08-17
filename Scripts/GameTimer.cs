using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace lxxj
{
    public class GameTimer : MonoBehaviour
    {
        #region Variables
        private static GameTimer instance;
        private float timerDuration = 600f; 
        private float timer = 0; 
        private bool timerRunning = true; 
        private float elapsedTime = 0f; 
        CameraSwitch cameraSwitch;
        public TextMeshProUGUI timerText;
        #endregion

        // Singleton pattern logic & setting timer duration
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); 
                timerDuration = 600f;
            }
            else if (instance != this)
            {
                Destroy(gameObject); 
            }
            cameraSwitch = FindObjectOfType<CameraSwitch>();
        }

        // Counts down timer from set minutes
        void Update()
        {
            if (timerRunning)
            {
                timer += Time.deltaTime;
                elapsedTime += Time.deltaTime;

                int minutes = Mathf.FloorToInt(timer / 60);
                int seconds = Mathf.FloorToInt(timer % 60);

                // Update the timer text in UI
                timerText.text = string.Format("Time played: {0:00}:{1:00}", minutes, seconds);

                if (timer >= timerDuration)
                {
                    // When timer is full load closing scene
                    LoadNextScene();
                    timerRunning = false;
                }
            }
        }

        // Unused logic — was used to switch camera view every minute but was scrapped, may still have use for other functionality later
        void OnMinutePassed()
        {
            if (!cameraSwitch.isInTopDown)
            {
                cameraSwitch.SwitchToTopDown();
            }
            else
            {
                cameraSwitch.SwitchToThirdPerson();
            }
        }

        // Loads next scene
        void LoadNextScene()
        {
            Debug.Log("Event triggered after 10 minutes!");
            SceneManager.LoadScene("ClosingScreen"); 
        }
    }
}
