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
            audioSource.spatialBlend = 1.0f; // D�wi�k 3D, zgodny z systemem Infima Games
            audioSource.volume = 1.0f; // Maksymalna g�o�no��
            audioSource.minDistance = 1.0f; // Minimalna odleg�o�� dla d�wi�ku 3D
            audioSource.maxDistance = 20.0f; // Maksymalna odleg�o�� dla d�wi�ku 3D
        }
        if (audioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        else
        {
            Debug.LogWarning($"Brak Audio Mixer Group w {gameObject.name}. D�wi�k b�dzie odtwarzany bez miksera.", gameObject);
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
