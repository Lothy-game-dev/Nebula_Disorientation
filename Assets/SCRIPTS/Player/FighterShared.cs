using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterShared : MonoBehaviour
{
    #region Shared Variables
    // Stats
    public float CurrentHP;
    public float MaxHP;
    // Thermal Stats
    public float currentTemperature;
    public bool isOverloadded;
    public float OverheatIncreaseScale;
    public bool isBurned;
    public float BurnedDelay;
    public bool isSlowed;
    public float SlowedMoveSpdScale;
    public bool isFrozen;
    public float FrozenDuration;
    public bool isRegenThermal;
    public float RegenDelayTimer;
    public float RegenScale;
    public bool isImmuneFrozenSlow;
    public float ImmuneDuration;
    public bool isBHPulled;
    public List<Vector2> PulledVector;
    public LayerMask BlackHoleLayer;
    public bool isLavaBurned;
    public int LavaBurnedCount;
    public float LavaBurnedDamageTimer;
    public float LavaBurnedDamage;
    public GameObject OnFireGO;
    public bool isGravitationalSlow;
    public float GravitationalSlow;
    public float GravitationalSlowMultiplier;
    public float GravitationalSlowTimer;
    public bool isSFBFreeze;
    public float SFBFreezeTimer;
    public GameObject OnFreezeGO;
    #endregion
    #region Shared Functions
    // Initialize
    public void InitializeFighter()
    {
        currentTemperature = 50;
        CurrentHP = MaxHP;
        isRegenThermal = true;
        isImmuneFrozenSlow = false;
        RegenScale = 1f;
        SlowedMoveSpdScale = 1;
        OverheatIncreaseScale = 0;
        GravitationalSlowMultiplier = 1;
    }
    // Update For Fighter
    public void UpdateFighter()
    {
        CheckThermal();
        CheckInsideBlackhole();
        CheckSpecialEffectStatus();
    }
    // Check Thermal Status, must be called in Update()
    public void CheckThermal()
    {
        Color c = GetComponent<SpriteRenderer>().color;
        // If temp > 100 then set back to 100
        if (currentTemperature > 100f)
        {
            currentTemperature = 100f;
        }
        // If temp < 0 then set back to 0
        if (currentTemperature < 0f)
        {
            currentTemperature = 0f;
        }
        // If 50 < temp < 90 -> overloadded
        if (currentTemperature > 50 && currentTemperature < 90)
        {
            isOverloadded = true;
            isBurned = false;
            isSlowed = false;
            c.g = (90 - currentTemperature) / (90 - 50);
            c.b = (90 - currentTemperature) / (90 - 50);
        }
        // If temp >= 90 -> burned
        else if (currentTemperature >= 90)
        {
            isOverloadded = false;
            OverheatIncreaseScale = 0;
            if (!isBurned)
            {
                isBurned = true;
                BurnedDelay = 0f;
            }
            isSlowed = false;
            c.g = 0;
            c.b = 0;
        } 
        else if (currentTemperature==50)
        {
            isOverloadded = false;
            isBurned = false;
            isSlowed = false;
        }
        // If 0 < temp < 50 -> slowed
        else if (currentTemperature > 0 && currentTemperature < 50 && !isImmuneFrozenSlow)
        {
            isOverloadded = false;
            isBurned = false;
            isSlowed = true;
            c.r = (currentTemperature - 0) / (50 - 10);
            c.g = (currentTemperature - 0) / (50 - 10);
        } 
        // if temp = 0 -> frozen
        else if (currentTemperature == 0 && !isFrozen)
        {
            isOverloadded = false;
            isBurned = false;
            isSlowed = false;
            if (isSFBFreeze)
            {
                isSFBFreeze = false;
                SFBFreezeTimer = 0;
            }
            if (!isImmuneFrozenSlow)
            {
                isFrozen = true;
                FrozenDuration = 5f;
                c.r = 0;
                c.g = 0;
            }
        }
        // Check status
        ThermalStatus();
        // Timer removal for immune frozen slow
        if (isImmuneFrozenSlow)
        {
            c.r = 1;
            c.g = 1;
            if (ImmuneDuration <= 0f)
            {
                isImmuneFrozenSlow = false;
                RegenScale = 1;
            } else
            {
                ImmuneDuration -= Time.deltaTime;
            }
        }
        // Delay auto regen timer removal
        if (!isRegenThermal)
        {
            if (RegenDelayTimer <= 0f)
            {
                isRegenThermal = true;
            } else
            {
                RegenDelayTimer -= Time.deltaTime;
            }
        }
        // Auto regen
        if (isRegenThermal)
        {
            RegenThermal();
        }
        // Set Color
        GetComponent<SpriteRenderer>().color = c;
    }
    // Thermal Status
    public void ThermalStatus()
    {
        if (isBurned)
        {
            // If burn, deal dmg per second
            if (BurnedDelay <= 0f)
            {
                CurrentHP -= MaxHP * (1 + (currentTemperature - 90) / 10) / 100;
                BurnedDelay = 1f;
            } else if (BurnedDelay > 0f)
            {
                BurnedDelay -= Time.deltaTime;
            }
            if (!OnFireGO.activeSelf)
            {
                OnFireGO.SetActive(true);
            }
        } else
        {
            if (OnFireGO.activeSelf && !isLavaBurned)
            {
                OnFireGO.SetActive(false);
            }
        }
        if (isOverloadded)
        {
            // If overloadded, reduce RoF
            OverheatIncreaseScale = (50 + (currentTemperature-50) / 40 * 50) / 100;
        } else
        {
            OverheatIncreaseScale = 0;
        }
        if (isSlowed)
        {
            // if Slowed, reduce move and rotate spd
            if (!isImmuneFrozenSlow)
            {
                SlowedMoveSpdScale = 1 - (50 - currentTemperature) / 50;
            } else
            {
                SlowedMoveSpdScale = 1;
            }
        }
        if (isFrozen)
        {
            // if frozen wear off, set immune frozen in 3 seconds
            // and auto regen immediately and 2x regen scale
            // and immune to thermal status
            if (FrozenDuration > 0f)
            {
                FrozenDuration -= Time.deltaTime;
                CalculateVelocity(new Vector2(0, 0));
                if (!OnFreezeGO.activeSelf)
                {
                    OnFreezeGO.SetActive(true);
                }
            }
            else
            {
                if (OnFreezeGO.activeSelf)
                {
                    OnFreezeGO.SetActive(false);
                }
                isFrozen = false;
                isImmuneFrozenSlow = true;
                ImmuneDuration = 3f;
                isRegenThermal = true;
                RegenScale = 2;
                RegenDelayTimer = 0f;
                Color c = GetComponent<SpriteRenderer>().color;
                c.r = 1;
                c.g = 1;
                SlowedMoveSpdScale = 1;
            }
        }
    }
    // Receive Thermal Damage
    public void ReceiveThermalDamage(bool isHeat)
    {
        if (!isImmuneFrozenSlow) currentTemperature += (isHeat ? 1 : -1) * 2f;
        isRegenThermal = false;
        RegenDelayTimer = 2f;
    }

    // Regen Thermal
    public void RegenThermal()
    {
        if (isRegenThermal)
        {
            // Regen Thermal per second
            if (RegenDelayTimer <= 0f)
            {
                // If temp < 50, increase to 50
                if (currentTemperature <= (50 - 5 * RegenScale))
                {
                    currentTemperature += 5 * RegenScale;
                } 
                else if (currentTemperature > (50 - 5 * RegenScale) && currentTemperature < 50f)
                {
                    currentTemperature = 50f;
                } 
                // If temp > 50, decrease to 50
                else if (currentTemperature > 55f)
                {
                    currentTemperature -= 5 * RegenScale;
                } else if (currentTemperature < (50 + 5 * RegenScale) && currentTemperature > 50f)
                {
                    currentTemperature = 50f;
                }
                RegenDelayTimer = 1f;
            } else
            {
                RegenDelayTimer -= Time.deltaTime;
            }
        }
    }
    #endregion
    #region Check Blackhole Pulling
    // Check blackhole pulling force
    public void CheckInsideBlackhole()
    {
        isBHPulled = false;
        PulledVector = new List<Vector2>();
        // Get all blackhole at current position
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f, BlackHoleLayer);
        if (cols.Length>0)
        {
            // If there are blackhole(s), add its pulling vector to list
            foreach (var col in cols)
            {
                BlackHole bh = col.GetComponent<BlackHole>();
                if (bh != null && bh.HitLayer.value == 1<<gameObject.layer)
                {
                    isBHPulled = true;
                    Vector2 vectorPull = bh.CalculatePullingVector(gameObject);
                    PulledVector.Add(vectorPull);
                }
            }
        }
    }

    // Calculate Velocity based on blackhole pulling vector
    public void CalculateVelocity(Vector2 veloc)
    {
        if (isBHPulled)
        {
            foreach (Vector2 v in PulledVector)
            {
                veloc += v;
            }
        }
        GetComponent<Rigidbody2D>().velocity = veloc * GravitationalSlowMultiplier;
    }
    #endregion
    #region Check Weapon Special Effects
    // Check weapon's special effect
    public void CheckSpecialEffectStatus()
    {
        // Lava orb's Lavaburned
        if (isLavaBurned)
        {
            if (LavaBurnedCount < 10)
            {
                if (!OnFireGO.activeSelf)
                {
                    OnFireGO.SetActive(true);
                }
                if (LavaBurnedDamageTimer>0f)
                {
                    LavaBurnedDamageTimer -= Time.deltaTime;
                } else
                {
                    CurrentHP -= LavaBurnedDamage;
                    ReceiveThermalDamage(true);
                    LavaBurnedCount++;
                    LavaBurnedDamageTimer = 0.1f;
                }
            } 
            else
            {
                isLavaBurned = false;
                if (OnFireGO.activeSelf && !isBurned)
                {
                    OnFireGO.SetActive(false);
                }
            }
        }   
        // Gravitational Slow
        if (isGravitationalSlow)
        {
            if (GravitationalSlowTimer > 0f)
            {
                GravitationalSlowTimer -= Time.deltaTime;
                GravitationalSlowMultiplier = 1 - GravitationalSlow;
            }
            else
            {
                isGravitationalSlow = false;
                GravitationalSlowMultiplier = 1;
            }
        }
        // Superior Freezing Blaster Freeze
        if (isSFBFreeze)
        {
            if (SFBFreezeTimer > 0f)
            {
                SFBFreezeTimer -= Time.deltaTime;
                CalculateVelocity(new Vector2(0, 0));
                if (!OnFreezeGO.activeSelf)
                {
                    OnFreezeGO.SetActive(true);
                }
            } 
            else
            {
                isSFBFreeze = false;
                if (OnFreezeGO.activeSelf)
                {
                    OnFreezeGO.SetActive(false);
                }
            }
        }
    }

    // Inflict self with lava burned (called by outer factors)
    public void InflictLavaBurned(float dmg)
    {
        LavaBurnedDamage = dmg;
        LavaBurnedDamageTimer = 0f;
        LavaBurnedCount = 0;
        isLavaBurned = true;
    }

    public void InflictGravitationalSlow(float SlowScale, float Time)
    {
        GravitationalSlowTimer = Time;
        isGravitationalSlow = true;
        GravitationalSlow = SlowScale;
    }

    public void InflictSuperiorFreezingBlasterFreeze(float FreezingDuration, float AddingFreezingDuration)
    {
        if (isFrozen)
        {
            FrozenDuration += AddingFreezingDuration;
        } else
        {
            if (!isSFBFreeze)
            {
                isSFBFreeze = true;
            }
            SFBFreezeTimer = FreezingDuration;
        }
    }
    #endregion
}