using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DestroyOnColission2D : MonoBehaviour
{
    public int targetLayer = 8;
    public float destroyTime = 0.5f;
    public bool destroyParent;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            if (destroyParent)
                StartCoroutine(DisableOverTime(transform.parent.gameObject));
            else
                StartCoroutine(DisableOverTime(gameObject));
        }
    }

    IEnumerator DisableOverTime(GameObject obj)
    {
        yield return new WaitForSeconds(destroyTime);
        obj.SetActive(false);
    }
}
