using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker2D : MonoBehaviour
{
    public bool useLayer = false;
    public int groundLayer = 8;

    [HideInInspector] public bool grounded { get; private set; }

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (useLayer)
        {
            if (other.gameObject.layer == groundLayer)
                grounded = true;
        }
        else
        {
            grounded = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (useLayer)
        {
            if (other.gameObject.layer == groundLayer)
                grounded = false;
        }
        else
        {
            grounded = false;
        }
    }
}
