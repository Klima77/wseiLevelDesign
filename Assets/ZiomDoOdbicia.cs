using UnityEngine;
using System.Collections;

public class ZiomDoOdbicia : MonoBehaviour
{
    [SerializeField] private float followDistance = 5f; // Odleg³oœæ od gracza (5 metrów)
    [SerializeField] private float followSpeed = 5f; // Prêdkoœæ pod¹¿ania
    [SerializeField] private AnimationClip idleAnimation; // Animacja przed wejœciem w trigger
    [SerializeField] private AnimationClip followAnimation; // Animacja po wejœciu w trigger

    private Transform player; // Referencja do gracza
    private Animator animator; // Komponent Animator
    private bool isFollowing = false; // Czy obiekt pod¹¿a za graczem
    private Vector3 targetPosition; // Pozycja docelowa podczas pod¹¿ania

    void Start()
    {
        // Pobierz komponent Animator
        animator = GetComponent<Animator>();

        // Upewnij siê, ¿e Animator istnieje
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
            return;
        }

        // Ustaw pocz¹tkow¹ animacjê (idle)
        if (idleAnimation != null)
        {
            animator.Play(idleAnimation.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // SprawdŸ, czy to gracz wszed³ w trigger
        if (other.CompareTag("Player") && !isFollowing)
        {
            // Pobierz transform gracza
            player = other.transform;
            // Rozpocznij Coroutine z opóŸnieniem
            StartCoroutine(StartFollowing());
        }
    }

    private IEnumerator StartFollowing()
    {
        // Poczekaj 2 sekundy
        yield return new WaitForSeconds(2f);

        // Zmieñ animacjê na followAnimation
        if (followAnimation != null)
        {
            animator.Play(followAnimation.name);
        }

        // Rozpocznij pod¹¿anie
        isFollowing = true;
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            // Oblicz pozycjê docelow¹ (5 metrów od gracza)
            Vector3 directionToPlayer = (transform.position - player.position).normalized;
            targetPosition = player.position + directionToPlayer * followDistance;

            // P³ynnie porusz obiekt w stronê pozycji docelowej
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Obróæ obiekt w stronê gracza (opcjonalne)
            transform.LookAt(player);
        }
    }
}
