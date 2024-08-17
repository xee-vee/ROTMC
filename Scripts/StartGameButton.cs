using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;


namespace lxxj
{
    public class StartGameButton : MonoBehaviour
    {
        public Button contrtollerButton;
        public Button keyboardButton;
        public GameObject canvasToDestroy;
        public bool isUsingKeyboard;
        public bool isUsingController;

        // If any key is pressed move to next scene (start of game)
        private void Update()
        {
            if (Input.anyKey)
            {
                StartCoroutine(WaitForLevelLoad());
            }
        }

        IEnumerator WaitForLevelLoad()
        {
            yield return new WaitForSecondsRealtime(1);
            SceneManager.LoadScene("Level1");         
        }

    }
}
