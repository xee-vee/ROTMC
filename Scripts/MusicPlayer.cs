using UnityEngine;

namespace lxxj
{
    public class MusicPlayer : MonoBehaviour
    {
        private static MusicPlayer instance = null;
        
        // Logic for playing music, also singleton pattern
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);

                AudioSource audioSource = GetComponentInChildren<AudioSource>();
                if (audioSource != null)
                {
                    // Set the audio to loop
                    audioSource.loop = true;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("No AudioSource component found on this GameObject.");
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
