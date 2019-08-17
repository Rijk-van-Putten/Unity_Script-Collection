using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Debug.LogError("Trying to create more then one singleton!");
    }
}
