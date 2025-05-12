using UnityEngine;
using System.Collections;

public class ZiomDoOdbicia : MonoBehaviour
{
    [SerializeField] private float followDistance = 5f; // Odleg�o�� od gracza (5 metr�w)
    [SerializeField] private float followSpeed = 5f; // Pr�dko�� pod��ania
    [SerializeField] private AnimationClip idleAnimation; // Animacja przed wej�ciem w trigger
    [SerializeField] private AnimationClip followAnimation; // Animacja po wej�ciu w trigger

    private Transform player; // Referencja do gracza
    private Animator animator; // Komponent Animator
    private bool isFollowing = false; // Czy obiekt pod��a za graczem
    private Vector3 targetPosition; // Pozycja docelowa podczas pod��ania

    void Start()
    {
        // Pobierz komponent Animator
        animator = GetComponent<Animator>();

        // Upewnij si�, �e Animator istnieje
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
            return;
        }

        // Ustaw pocz�tkow� animacj� (idle)
        if (idleAnimation != null)
        {
            animator.Play(idleAnimation.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sprawd�, czy to gracz wszed� w trigger
        if (other.CompareTag("Player") && !isFollowing)
        {
            // Pobierz transform gracza
            player = other.transform;
            // Rozpocznij Coroutine z op�nieniem
            StartCoroutine(StartFollowing());
        }
    }

    private IEnumerator StartFollowing()
    {
        // Poczekaj 2 sekundy
        yield return new WaitForSeconds(2f);

        // Zmie� animacj� na followAnimation
        if (followAnimation != null)
        {
            animator.Play(followAnimation.name);
        }

        // Rozpocznij pod��anie
        isFollowing = true;
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            // Oblicz pozycj� docelow� (5 metr�w od gracza)
            Vector3 directionToPlayer = (transform.position - player.position).normalized;
            targetPosition = player.position + directionToPlayer * followDistance;

            // P�ynnie porusz obiekt w stron� pozycji docelowej
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Obr�� obiekt w stron� gracza (opcjonalne)
            transform.LookAt(player);
        }
    }
}
