using UnityEngine;

public class FPHeadBobber : MonoBehaviour
{
    [Header("Bobbing")]
    public float bobbingSpeed = 0.18f; 
    public float bobbingAmount = 0.2f; 

    [Header("Midpoint")]
    public float midpoint = 2.0f; 
 
    private float timer = 0.0f;

    void Update ()
    { 
        float waveslice = 0.0f; 
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 

        if (Mathf.Abs(horizontalInput) == 0 && Mathf.Abs(verticalInput) == 0)
        { 
            timer = 0.0f; 
        } 
        else
        { 
            waveslice = Mathf.Sin(timer); 
            timer = timer + bobbingSpeed; 
            if (timer > Mathf.PI * 2) 
              timer = timer - (Mathf.PI * 2); 
        } 

        if (waveslice != 0)
        { 
            float translateChange = waveslice * bobbingAmount; 
            float totalAxes = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput); 
            totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f); 
            translateChange = totalAxes * translateChange; 
            transform.localPosition = Vector3.up * (midpoint + translateChange); 
        } 
        else
        { 
            transform.localPosition = Vector3.up * (midpoint); 
        } 
    }
}