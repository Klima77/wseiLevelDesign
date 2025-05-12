using UnityEngine;
using System.Collections;

public class ObjectDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
         StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(2f);
        DestroyGameObject();
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
