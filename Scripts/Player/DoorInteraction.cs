using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class DoorInteraction : Interactable
    {
        public GameObject door;
        
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            InteractWithDoor(playerManager);
        }

        // Placeholder logic, will be used in future to add in door interaction (e.g. open with key)
        private void InteractWithDoor(PlayerManager playerManager)
        {
            PlayerLocomotion playerLocomotion;
            AnimatorHandler animatorHandler;

            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            animatorHandler = playerManager.GetComponent<AnimatorHandler>();

            playerLocomotion.rigidbody.velocity = Vector3.zero;

            // Add door open animation to Unity Animator in future 
            // animatorHandler.PlayerTargetAnimation("Door Open", true);

        }

        // Deaactives door game object
        public void RemoveDoor()
        {
            door.SetActive(false);
        }
    }
}
