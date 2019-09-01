using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

public class HealthController : MonoBehaviour
{
    public float Health { get; private set; }

    [Header("General")]
    public float maxHealth = 100.0f;
    public bool destroyOnDeath = false;

    [Header("FX (optional)")]
    public GameObject damageFX;
    public GameObject addhealthFX;
    public GameObject dieFX;

    [Header("Events (optional)")]
    public bool enableEvents = false;
    
    public FloatEvent healthChangeEvent;
    public UnityEvent dieEvent;
    private bool isDead = false;

    [Header("UI (optional)")]
    public bool useInGameUI = false;
    public string hpUISuffix = "HP";

    void Start()
    {
        Health = maxHealth;

        if (useInGameUI)
            UpdateUI();
    }
    public bool DealDamage(float amount)
    {
        Health -= amount;
        Health = Mathf.Clamp(Health, 0.0f, maxHealth);


        PlayFX(damageFX);

        if (enableEvents)
            healthChangeEvent.Invoke(Health);

        if (useInGameUI)
            UpdateUI();


        if (Health <= 0.0f && !isDead)
        {
            Die();
            return true;
        }
        return false;
    }

    public bool DealDamage(Player source, float amount)
    {
        Health -= amount;
        Health = Mathf.Clamp(Health, 0.0f, maxHealth);

        PlayFX(damageFX);

        if (enableEvents)
            healthChangeEvent.Invoke(Health);

        if (useInGameUI)
            UpdateUI();

        if (Health <= 0.0f && !isDead)
        {
            source.kills++;
            Die();
            return true;
        }
        return false;
    }

    public void AddHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0.0f, maxHealth);

        PlayFX(addhealthFX);

        if (enableEvents)
            healthChangeEvent.Invoke(Health);
        if (useInGameUI)
            UpdateUI();
    }
    private void Die()
    {
        isDead = true;
        PlayFX(dieFX);

        if (enableEvents) 
            dieEvent.Invoke();            
        if (useInGameUI)
            UpdateUI();
        if (destroyOnDeath)
            Destroy(gameObject);
    }
    private void PlayFX(GameObject prefab)
    {
        if (prefab != null)
            Instantiate(prefab, transform.position, transform.rotation);
    }
    private void UpdateUI()
    {
        InGameUI.instance.SetText("health", ((int)Health).ToString() + hpUISuffix);
    }
}
