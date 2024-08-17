using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class AnimatorManager : MonoBehaviour
    {
        public bool canRotate;
        public Animator anim;

        // Function that is used elsewhere every time an animation play needs to be triggered
        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", false);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }
    }
}
