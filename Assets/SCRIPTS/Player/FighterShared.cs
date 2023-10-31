using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterShared : MonoBehaviour
{
    #region Shared Variables
    private SpaceZoneHazardEnvironment HazEnv;
    private GameController controller;
    private SpaceZoneMission misson;
    private StatisticController Statistic;
    // Weapons
    public GameObject LeftWeapon;
    public GameObject RightWeapon;
    // Stats
    public string FighterName;
    public float CurrentHP;
    public float MaxHP;
    public float CurrentBarrier;
    public float MaxBarrier;
    public float BarrierRegenTimer;
    public float BarrierRegenAmount;
    public float BarrierRegenDelay;
    public GameObject Barrier;
    public GameObject BarrierBreak;
    public GameObject Explosion;
    private float BarrierEffectDelay;
    public GameObject HealingEffect;
    public bool isHealing;
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
    // Special Weapon/ Environment Effects
    public bool isBHPulled;
    public List<Vector2> PulledVector;
    public LayerMask BlackHoleLayer;
    public bool isLavaBurned;
    public int LavaBurnedCount;
    public int LavaBurnedDmgNumber;
    public float LavaBurnedDamageTimer;
    public float LavaBurnedDamage;
    public float LavaBurnedDamageExteriorScale;
    public GameObject OnFireGO;
    public bool isGravitationalSlow;
    public float GravitationalSlow;
    public float GravitationalSlowMultiplier;
    public float GravitationalSlowTimer;
    public bool isSFBFreeze;
    public float SFBFreezeTimer;
    public GameObject OnFreezeGO;
    public float NanoEffectFrozenDurationIncrease;
    public bool IsNanoTemp;
    public float NanoTempScale;
    public int NanoTempStacks;
    public float NanoTempTimer;
    //Consumable effect
    public bool isWingShield;
    public float ShieldReducedScale;
    // Bounty
    private GameObject Killer;
    private bool alreadyDestroy;
    public Vector2 PushForce;
    public float ReceiveForceTime;
    public float ReceiveForceDelay;
    public bool alreadyHitByComet;
    public float HitByCometDelay;
    public float DeadPushScale;
    //Statistic
    private Dictionary<string, object> EnemyData;
    private GameObject DamageDealer;
    //LOTW
    public LOTWEffect LOTWEffect;
    //LaserBeam
    public GameObject LeftLaserBeamPos;
    public GameObject RightLaserBeamPos;
    #endregion
    #region Shared Functions
    // Initialize
    public void InitializeFighter()
    {
        currentTemperature = 50;
        if (CurrentHP==0)
        CurrentHP = MaxHP;
        // Check Power Barrier
        if (name.Contains("ZatSBB"))
        {
            MaxBarrier = 0;
        } else
        MaxBarrier = 2000;
        CurrentBarrier = MaxBarrier;
        isRegenThermal = true;
        isImmuneFrozenSlow = false;
        RegenScale = 1f;
        SlowedMoveSpdScale = 1;
        OverheatIncreaseScale = 0;
        GravitationalSlowMultiplier = 1;
        NanoTempScale = 1;
        NanoTempStacks = 0;
        ShieldReducedScale = 0;
        DeadPushScale = 1;
        LavaBurnedDamageExteriorScale = 1;
        HazEnv = FindObjectOfType<SpaceZoneHazardEnvironment>();
        controller = FindObjectOfType<GameController>();
        misson = FindObjectOfType<SpaceZoneMission>();
        Statistic = FindAnyObjectByType<StatisticController>();
    }
    // Update For Fighter
    public void UpdateFighter()
    {
        CheckBarrierAndHealth();
        if (currentTemperature!=50)
        {
            CheckThermal();
        }
        CheckInsideBlackhole();
        CheckSpecialEffectStatus();
        if (HitByCometDelay <= 0f)
        {
            alreadyHitByComet = false;
        }
        else 
        {
            HitByCometDelay -= Time.deltaTime;
        }
    }
    // Check Barrier
    public void CheckBarrierAndHealth()
    {
        BarrierRegenTimer -= Time.deltaTime;
        BarrierRegenDelay -= Time.deltaTime;
        BarrierEffectDelay -= Time.deltaTime;
        if (BarrierRegenTimer<=0f)
        {
            if (BarrierRegenDelay<=0f && CurrentBarrier<MaxBarrier)
            {
                if (CurrentBarrier <= MaxBarrier - BarrierRegenAmount)
                {
                    CurrentBarrier += BarrierRegenAmount;
                    BarrierRegenDelay = 1f;
                } else
                {
                    CurrentBarrier = MaxBarrier;
                    BarrierRegenDelay = 1f;
                }
            }

        }
        if (CurrentHP<=0f)
        {
            CurrentHP = 0f;
            if (!alreadyDestroy)
            {
                alreadyDestroy = true;
                StartCoroutine(DestroySelf());
            }
        }
    }

    private IEnumerator DestroySelf()
    {
        Destroy(LeftWeapon);
        Destroy(RightWeapon);
        GetComponent<SpriteRenderer>().color = Color.black;
        GetComponent<Collider2D>().enabled = false;
        GameObject expl = Instantiate(Explosion, transform.position, Quaternion.identity);
        expl.SetActive(true);
        Destroy(expl, 0.3f);
        yield return new WaitForSeconds(0.1f);
        GameObject expl2 = Instantiate(Explosion, new Vector3(transform.position.x + Random.Range(10,30), transform.position.y + Random.Range(10, 30), transform.position.z), Quaternion.identity);
        expl2.SetActive(true);
        Destroy(expl2, 0.3f);
        yield return new WaitForSeconds(0.1f);
        GameObject expl3 = Instantiate(Explosion, new Vector3(transform.position.x - Random.Range(10, 30), transform.position.y + Random.Range(10, 30), transform.position.z), Quaternion.identity);
        expl3.SetActive(true);
        Destroy(expl3, 0.3f);
        yield return new WaitForSeconds(0.1f);
        GameObject expl4 = Instantiate(Explosion, new Vector3(transform.position.x - Random.Range(10, 30), transform.position.y - Random.Range(10, 30), transform.position.z), Quaternion.identity);
        expl4.SetActive(true);
        Destroy(expl4, 0.3f);
        yield return new WaitForSeconds(0.1f);
        GameObject expl5 = Instantiate(Explosion, new Vector3(transform.position.x + Random.Range(10, 30), transform.position.y - Random.Range(10, 30), transform.position.z), Quaternion.identity);
        expl5.SetActive(true);
        Destroy(expl5, 0.3f);
        if (name == "Player")
        {
            Camera.main.GetComponent<AudioListener>().enabled = true;
            misson.PlayerDestroyed();
        } else if (GetComponent<AlliesShared>()!=null)
        {
            misson.AllyFighterDestroy(name);
        } else if (GetComponent<EnemyShared>()!=null)
        {
            misson.EnemyFighterDestroy(name, GetComponent<EnemyShared>().Tier);
        }
        Destroy(gameObject);
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
                FrozenDuration = 5f * NanoTempScale;
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
        if (LeftWeapon!=null)
        {
            LeftWeapon.GetComponent<SpriteRenderer>().color = c;
        }
        if (RightWeapon!=null)
        {
            RightWeapon.GetComponent<SpriteRenderer>().color = c;
        }
    }
    // Thermal Status
    public void ThermalStatus()
    {
        if (isBurned)
        {
            // If burn, deal dmg per second
            if (BurnedDelay <= 0f)
            {
                ReceiveBurnedDamage(1);
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
            // If overloadded, increasing overheat rate
            OverheatIncreaseScale = (50 + (currentTemperature-50) / 40 * 50) / 100 * NanoTempScale;
        } else
        {
            OverheatIncreaseScale = 0;
        }
        if (isSlowed)
        {
            // if Slowed, reduce move and rotate spd
            if (!isImmuneFrozenSlow)
            {
                SlowedMoveSpdScale = (1 - (50 - currentTemperature) * NanoTempScale / 50)
                    < 0.1f ? 0.1f : (1 - (50 - currentTemperature) * NanoTempScale / 50);
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
                NanoEffectFrozenDurationIncrease = 0f;
                RegenScale = 2;
                RegenDelayTimer = 0f;
                Color c = GetComponent<SpriteRenderer>().color;
                c.r = 1;
                c.g = 1;
                SlowedMoveSpdScale = 1;
            }
        }
    }
    // Receive Burn Damage
    public void ReceiveBurnedDamage(float scale)
    {
        ReceiveDamage(MaxHP * scale * NanoTempScale * 
            (LOTWEffect != null && !LOTWEffect.LOTWAffectEnvironment ? 1 : HazEnv.HazardThermalBurnDmgScale)
            * (1 + (currentTemperature - 90) / 10) / 100, gameObject);
    }
    // Receive Thermal Damage
    public void ReceiveThermalDamage(bool isHeat)
    {
        if (!isImmuneFrozenSlow) currentTemperature += (isHeat ? 1 : -1) * 2f * NanoTempScale;
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
                else if (currentTemperature >= (50 + 5 * RegenScale))
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
        if (PulledVector==null)
        {
            PulledVector = new List<Vector2>();
        } else
        {
            PulledVector.Clear();
        }
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
        GetComponent<Rigidbody2D>().velocity = veloc * GravitationalSlowMultiplier * SlowedMoveSpdScale;
    }
    #endregion
    #region Check Weapon Special Effects
    // Check weapon's special effect
    public void CheckSpecialEffectStatus()
    {
        // Lava orb's Lavaburned
        if (isLavaBurned)
        {
            if (LavaBurnedCount < LavaBurnedDmgNumber)
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
                    ReceiveDamage(LavaBurnedDamage, gameObject);
                    ReceiveThermalDamage(true);
                    LavaBurnedCount++;
                    LavaBurnedDamageTimer = 0.1f;
                }
            } 
            else
            {
                isLavaBurned = false;
                LavaBurnedDamageExteriorScale = 1;
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
        // Nano Temp Effect
        if (IsNanoTemp)
        {
            if (NanoTempTimer > 0f)
            {
                NanoTempTimer -= Time.deltaTime;
                NanoTempScale = 1 + NanoTempStacks * 15f / 100;
            } 
            else
            {
                NanoTempStacks = 0;
                NanoTempScale = 1;
                IsNanoTemp = false;
            }
        }
    }

    // Inflict self with lava burned (called by outer factors)
    public void InflictLavaBurned(float dmg, int numberOfTimes)
    {
        LavaBurnedDamage = dmg * LavaBurnedDamageExteriorScale;
        LavaBurnedDamageTimer = 0f;
        LavaBurnedDmgNumber = numberOfTimes;
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
            SFBFreezeTimer = FreezingDuration * NanoTempScale;
        }
    }
    public void InflictNanoTemp()
    {
        if (!IsNanoTemp)
        {
            IsNanoTemp = true;
            NanoTempStacks = 1;
            NanoTempTimer = 2f;
        } else
        {
            if (NanoTempStacks<4)
            {
                NanoTempStacks++;
            }
            NanoTempTimer = 2f;
        }
    }
    #endregion
    #region Calculate Damage Received
    public void ReceiveDamage(float damage, GameObject DamageSource)
    {
        damage = damage *
            (LOTWEffect != null && !LOTWEffect.LOTWAffectEnvironment ? 1 : HazEnv.HazardNDAllDamageScale) * 
            (LOTWEffect!=null && DamageSource.GetComponent<BulletShared>()!=null ? 1 / LOTWEffect.LOTWWeaponDMGReceivedScale : 1) *
            (LOTWEffect!=null ? LOTWEffect.LOTWAllDamageReceiveScale : 1) *
            (CurrentBarrier > 0 ?
            (LOTWEffect != null && !LOTWEffect.LOTWAffectEnvironment ? 1 : HazEnv.HazardGammaRayBurstScale) 
            : 1) * 
            (isWingShield && CurrentBarrier > 0 ? (100 - ShieldReducedScale) / 100 : 1);
        DeadPushScale = 1;
       
        if (DamageSource != null)
        {
            if (DamageSource.GetComponent<BulletShared>() != null)
            {
                DamageDealer = DamageSource.GetComponent<BulletShared>().WeaponShoot.GetComponent<Weapons>().Fighter;
            }
            if (GetComponent<EnemyShared>() != null && DamageDealer == controller.Player)
            {
                Statistic.DamageDealt += damage;
            }
        }
        if (CurrentBarrier > 0)
        {
            if (CurrentBarrier > damage)
            {
                CurrentBarrier -= damage;
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, transform.position, Quaternion.identity);
                    if (name.Contains("SSTP"))
                    {
                        br.transform.localScale *= 3;
                    }
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 10f;
                BarrierRegenAmount = 200f;
            }
            else
            {
                float AfterDamage = damage - CurrentBarrier;
                CurrentBarrier = 0;
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, transform.position, Quaternion.identity);
                    if (name.Contains("SSTP"))
                    {
                        br.transform.localScale *= 3;
                    }
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 20f;
                BarrierRegenAmount = 100f;
                CurrentHP -= AfterDamage;
            }
        }
        else
        {
            if (BarrierEffectDelay <= 0f)
            {
                BarrierEffectDelay = 0.25f;
                GameObject BRBreak = Instantiate(BarrierBreak, transform.position, Quaternion.identity);
                if (name.Contains("SSTP"))
                {
                    BRBreak.transform.localScale *= 3;
                }
                BRBreak.SetActive(true);
                BRBreak.transform.SetParent(transform);
                Destroy(BRBreak, 0.5f);
            }
            if (CurrentHP > damage)
                CurrentHP -= damage;
            else
            {
                if (DamageSource != null)
                {
                    if (DamageSource.GetComponent<BulletShared>() != null)
                    {
                        Killer = DamageSource.GetComponent<BulletShared>().WeaponShoot.GetComponent<Weapons>().Fighter;
                    }
                    else
                    {
                        Killer = DamageSource;
                    }
                    if (GetComponent<EnemyShared>() != null && Killer == controller.Player)
                    {
                        GetComponent<EnemyShared>().AddBounty();
                        EnemyData = FindAnyObjectByType<AccessDatabase>().GetDataEnemyById(GetComponent<EnemyShared>().EnemyID);
                        switch(EnemyData["TierColor"])
                        {
                            case "#36b37e": Statistic.EnemyTierI += 1; break;
                            case "#4c9aff": Statistic.EnemyTierII += 1; break;
                            case "#bf2600": Statistic.EnemyTierIII += 1; break;
                        }
                        Statistic.TotalEnemyDefeated += (Statistic.EnemyTierI + Statistic.EnemyTierII + Statistic.EnemyTierIII);
                        Statistic.KillEnemy = true;
                    }
                }
                CurrentHP = 0;
                DeadPushScale = 3;
            }
        }
    }
    #endregion
    #region Calculate Healing
    public void ReceiveHealing(float HealAmount)
    {
       CurrentHP += HealAmount * LOTWEffect.LOTWRepairEffectScale;
    }
    #endregion
    #region Push

    public void ReceiveForce(Vector2 force, float strength, float time, string Source)
    {
        if (Source=="Bullet")
        {
            if (CurrentBarrier <= 0)
            {
                StartCoroutine(ApplyForce(force, strength, time));
            }
        } else
        StartCoroutine(ApplyForce(force, strength, time));
    }

    private IEnumerator ApplyForce(Vector2 force, float strength, float time)
    {
        if (time<=0.5f)
        {
            int n = 10;
            for (int i = 0; i < n; i++)
            {
                if (gameObject!=null)
                {
                    GetComponent<Rigidbody2D>().AddForce(force / force.magnitude * (strength / ((n - 1 + n / 2) * n / 2) * (n - n / 2 + n - 1 - i)) * DeadPushScale, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(time / n);
                }
            }
        } else
        {
            int n = 20;
            for (int i = 0; i < n; i++)
            {
                if (gameObject != null)
                {
                    GetComponent<Rigidbody2D>().AddForce(force / force.magnitude * (strength / ((n - 1 + n / 2) * n / 2) * (n - n / 2 + n - 1 - i)) * DeadPushScale, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(time / n);
                }
            }
        }

    }
    #endregion
}