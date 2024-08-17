using System.Collections;
using UnityEngine;

namespace lxxj
{
    public class CloseGameButton : MonoBehaviour
    {
        private bool canCheckInput = false;

        // Small script for handling logic in final scene, used to close the game with any button after a set time
        void Start()
        {
            StartCoroutine(EnableInputAfterDelay(5f));
        }

        void Update()
        {
            if (canCheckInput && Input.anyKeyDown)
            {
                Application.Quit();
            }
        }

        IEnumerator EnableInputAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            canCheckInput = true;
        }
    }
}
