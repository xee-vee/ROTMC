using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lxxj
{
    public class GoodbyeMessageObject : Interactable
    {
        public GameObject goodbyeObjectLight;
        public GameObject goodbyeObjectFlame;
        public GameObject goodbyeObject;

        private void Awake()
        {
            goodbyeObjectLight.SetActive(false);
            goodbyeObjectFlame.SetActive(false);
        }

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
        }

        // When player collider triggers with this, load next scene
        private void OnTriggerEnter(Collider other)
        {
            PlayerLocomotion playerLocomotion = other.GetComponent<PlayerLocomotion>();

            if (playerLocomotion != null)
            {
                goodbyeObjectLight.SetActive(true);
                goodbyeObjectFlame.SetActive(true);
                SceneManager.LoadScene("ClosingScreen"); 
            }
        }
    }
}
