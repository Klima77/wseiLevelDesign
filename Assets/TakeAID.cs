using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class TakeAID : MonoBehaviour
{
    [SerializeField] private AudioClip ammoSFX;
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
            if (ammoSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(ammoSFX);
                StartCoroutine(DeactivateAfterSound(ammoSFX.length));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator DeactivateAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        Debug.Log($"Obiekt {gameObject.name} dezaktywowany po {delay} sekundach.");
    }
}
