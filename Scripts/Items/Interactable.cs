using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{

    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;

        // Debugging Gizmos for interactable area
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        // Called when player interacts
        public virtual void Interact(PlayerManager playerManager)
        {
            
        }
    }

}
