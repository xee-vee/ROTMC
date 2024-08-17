using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lxxj
{
    public class GameSession : MonoBehaviour
    {
        CameraSwitch cameraSwitch;

        private void Awake()
        {
            cameraSwitch = FindObjectOfType<CameraSwitch>();
        }

        // Public function to resets game 
        public void ResetGame()
        {
            Time.timeScale = 0;

            // Add "you died" UI element here if time
            UnityEngine.Debug.Log("YOU DIED");

            StartCoroutine(ResetGameTimer());

        }

        // Coroutine that waits before reloading scene (useful if a UI element appeared after death)
        IEnumerator ResetGameTimer()
        {
            yield return new WaitForSecondsRealtime(2);
            
            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            // Wait for the end of the frame to ensure the scene has reloaded
            yield return new WaitForEndOfFrame();
            
            // Get the camera switch component in the new scene
            CameraSwitch cameraSwitch = FindObjectOfType<CameraSwitch>();

            Time.timeScale = 1;

        }

    }
}
