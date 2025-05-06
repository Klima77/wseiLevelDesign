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
