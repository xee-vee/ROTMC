using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class Key : Interactable
    {
        DoorInteraction doorInteraction;
        public GameObject keyLight;
        public GameObject key;

        private void Awake()
        {
            keyLight.SetActive(false);
            doorInteraction = FindObjectOfType<DoorInteraction>();
        }

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
        }

        // When player collider triggers with this, open the door
        private void OnTriggerEnter(Collider other)
        {
            PlayerLocomotion playerLocomotion = other.GetComponent<PlayerLocomotion>();

            if (playerLocomotion != null)
            {
                doorInteraction.RemoveDoor();
                keyLight.SetActive(true);
            }
        }
    }
}
