using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("HP Settings")]
    public int currentHp;
    public int maxHp;

    [Header("Death Settings")]
    public bool slowmoOnDeath = false;
    public bool stopVinyl = false;
    //Time before an entity dies after its hp drops below 0
    public float timeBeforeDeath = 0f;

    [Header("Damage Cooldown")]
    [HideInInspector] public bool OnCooldown = false;
    public float damageCooldownTime;
    bool _coroutineActivated = false;

    public bool isDying = false;
    public bool IsInvincible;

    public UnityEvent onDeath;
    public UnityEvent<int> onHit;


    void Start()
    {
        //Fill hp to its max
        currentHp = maxHp;
    }

    void OnEnable()
    {
        OnCooldown = false;
        _coroutineActivated = false;
    }

    public void TakeDamage(int damageAmount) //Take damage, if hp is below or equal to 0 - start death coroutine
    {
        if (OnCooldown) return;
        if (IsInvincible) return;

        currentHp -= (damageAmount);
        onHit.Invoke(damageAmount);

        StartCoroutine(DamageCooldown());

        if (currentHp <= 0 && isDying == false) //Activate death coroutine if hp is below or equal to 0 and if entity isnt already dying
        {
            StartCoroutine(DeathRoutine());
        }
    }

    public void ResetHealth()
    {
        currentHp = maxHp;
    }

    public void SetMaxHealth(int value)
    {
        maxHp = value;
    }


    IEnumerator DeathRoutine() //Wait for X seconds and then destroy entity
    {
        isDying = true;
        yield return new WaitForSeconds(timeBeforeDeath);
        onDeath.Invoke();
    }

    public IEnumerator DamageCooldown() //Use this to give Invulnerability frames if needed.
    {
        if (!_coroutineActivated)
        {
            _coroutineActivated = true;
            OnCooldown = true;

            yield return new WaitForSeconds(damageCooldownTime);

            OnCooldown = false;
            _coroutineActivated = false;
        }
    }

    public IEnumerator DamageCooldown(float duration)
    {
        if (!_coroutineActivated)
        {
            _coroutineActivated = true;
            OnCooldown = true;

            yield return new WaitForSeconds(duration);

            OnCooldown = false;
            _coroutineActivated = false;
        }
    }

}
