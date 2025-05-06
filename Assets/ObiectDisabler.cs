using UnityEngine;
using UnityEngine.Audio;

public class ObiectDisabler : MonoBehaviour
{
    [SerializeField] private GameObject objectToAnimate;
    [SerializeField] private AudioClip openDoors;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Nie odtwarzaj przy starcie
            audioSource.spatialBlend = 1.0f; // DŸwiêk 3D, zgodny z systemem Infima Games
            audioSource.volume = 1.0f; // Maksymalna g³oœnoœæ
            audioSource.minDistance = 1.0f; // Minimalna odleg³oœæ dla dŸwiêku 3D
            audioSource.maxDistance = 20.0f; // Maksymalna odleg³oœæ dla dŸwiêku 3D
        }
        if (audioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        else
        {
            Debug.LogWarning($"Brak Audio Mixer Group w {gameObject.name}. DŸwiêk bêdzie odtwarzany bez miksera.", gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectToAnimate != null)
            {
                audioSource.PlayOneShot(openDoors);
                Animation anim = objectToAnimate.GetComponent<Animation>();
                if (anim != null)
                {
                    anim.Play("open");
                }
            }
        }
    }
}
