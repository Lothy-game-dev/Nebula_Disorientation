using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShared : MonoBehaviour
{
    #region Shared Variables
    public Weapons WeaponShoot;
    public float MaxEffectiveDistance;
    public float MaximumDistance;
    public float Speed;
    public float AoE;
    public float BaseDamagePerHit;
    public LayerMask EnemyLayer;
    public LayerMask BlackholeLayer;
    public float Range;
    public Vector2 Destination;
    public bool InflictGravitationalSlow;
    public float GravitationalSlowScale;
    public float GravitationalSlowTime;
    public bool ApplyNanoEffect;
    public int LavaBurnCount;
    public GameObject HitBoxRange;

    protected Rigidbody2D rb;
    protected float Distance;
    protected Vector2 Velocity;
    protected Vector2 RealVelocity;
    protected float DistanceTravel;

    private float RealDamage;
    private bool StartCounting;
    private bool isBHPulled;
    private List<Vector2> PulledVector;
    private bool isPenetrating;
    private bool AlreadyHit;
    private List<GameObject> PenetrateAlreadyDealDamge;
    private float HitBox;
    #endregion
    #region Shared Functions
    public void InitializeBullet()
    {
        if (MaximumDistance==0f)
        {
            MaximumDistance = MaxEffectiveDistance;
        }
        PenetrateAlreadyDealDamge = new List<GameObject>();
        if (HitBoxRange!=null)
        HitBox = Mathf.Abs((HitBoxRange.transform.position - transform.position).magnitude);
    }
    public void UpdateBullet()
    {
        CheckInsideBlackhole();
        CalculateVelocity(RealVelocity);
    }
    // Calculate Velocity Required To Reach Destination
    public void CalculateVelocity()
    {
        if (Destination!=null)
        {
            Distance = Mathf.Sqrt((Destination.x - transform.position.x) * (Destination.x - transform.position.x)
            + (Destination.y - transform.position.y) * (Destination.y - transform.position.y));
            Vector2 distanceVec = Destination - new Vector2(transform.position.x, transform.position.y);
            Velocity = distanceVec / distanceVec.magnitude * Speed;
            RealVelocity = new Vector2(0, 0);
        }
    }
    // Acceleration for the first time second
    public IEnumerator Accelerate(float time)
    {
        for (int i=0; i<10; i++)
        {
            if (Mathf.Abs(RealVelocity.x) < Mathf.Abs(Velocity.x) && Mathf.Abs(RealVelocity.y) < Mathf.Abs(Velocity.y))
            {
                RealVelocity = new Vector2(RealVelocity.x + Velocity.x / 10, RealVelocity.y + Velocity.y / 10);
            }
            CalculateVelocity(RealVelocity);
            yield return new WaitForSeconds(time/10f);
        }
    }

    // Laser Special Accel: Increasing in both speed and size 
    public IEnumerator AccelerateLaser(float time, float initScale)
    {
        for (int i = 0; i < 5; i++)
        {
            if (Mathf.Abs(RealVelocity.x) < Mathf.Abs(Velocity.x) && Mathf.Abs(RealVelocity.y) < Mathf.Abs(Velocity.y))
            {
                RealVelocity = new Vector2(RealVelocity.x + Velocity.x / 5, RealVelocity.y + Velocity.y / 5);
            }
            CalculateVelocity(RealVelocity);
            transform.localScale = new Vector3(initScale*i/5,initScale*i/5,transform.localScale.z);
            yield return new WaitForSeconds(time / 5f);
        }
    }

    // Calculate Damage for kinetic/laser/lava orb weapons
    public void CalculateDamage()
    {
        // Detect any enemy with in range
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, HitBox, EnemyLayer);
        foreach (var col in cols)
        {
            // If there is enemy, make sure this function will not be called twice
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                // In case bullet has AoE
                if (AoE>0)
                {
                    // Create AoE effect
                    FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(col.transform.position, AoE);
                    // Check all enemies in AoE range
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(col.transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        // Deal damage to all enemies in AoE range
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy != null)
                        {
                            enemy.CurrentHP -= RealDamage;
                            if (InflictGravitationalSlow)
                            {
                                enemy.InflictGravitationalSlow(GravitationalSlowScale, GravitationalSlowTime);
                            }
                        }
                    }
                } 
                // In case bullet does not have AoE: Deal dmg to the enemy detected only
                else
                {
                    EnemyShared enemy = col.GetComponent<EnemyShared>();
                    if (enemy!=null)
                    {
                        enemy.CurrentHP -= RealDamage;
                        if (InflictGravitationalSlow)
                        {
                            enemy.InflictGravitationalSlow(GravitationalSlowScale, GravitationalSlowTime);
                        }
                    }
                }
                // Destroy after hit
                Destroy(gameObject);
            }
            else
            {
                // If already hit then destroy game object, just in case the above destroy does not work
                Destroy(gameObject);
            }
            // Break just in case, for performance purpose
            break;
        }
    }

    // Flamethrower type damage
    public void CalculateThermalDamage(bool isHeat)
    {
        // Detect enemy
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.01f, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                if (AoE>0)
                {
                    FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(col.transform.position, AoE);
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy != null)
                        {
                            enemy.CurrentHP -= RealDamage;
                            // Receive thermal Damage
                            if (WeaponShoot.CurrentHitCount < 1)
                            {
                                if (WeaponShoot.CurrentHitCount == 0)
                                {
                                    // Set reset timer
                                    WeaponShoot.HitCountResetTimer = 1f / WeaponShoot.RateOfHit;
                                }
                                enemy.ReceiveThermalDamage(isHeat);
                                WeaponShoot.CurrentHitCount = 1;
                            }
                        }
                    }
                } else
                {
                    EnemyShared enemy = col.gameObject.GetComponent<EnemyShared>();
                    if (enemy != null)
                    {
                        enemy.CurrentHP -= RealDamage;
                        if (WeaponShoot.CurrentHitCount < 1)
                        {
                            if (WeaponShoot.CurrentHitCount == 0)
                            {
                                WeaponShoot.HitCountResetTimer = 1f / WeaponShoot.RateOfHit;
                            }
                            enemy.ReceiveThermalDamage(isHeat);
                            WeaponShoot.CurrentHitCount = 1;
                        }
                    }
                }
                Destroy(gameObject);
            }
            break;
        }
    }

    // Superior Freezing Blaster Special Power
    public void CalculateSFreezeBlasterDamage(float freezingChance, float freezingDuration, float addingFreezeDuration)
    {
        // Detect enemy
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.01f, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                EnemyShared enemy = col.gameObject.GetComponent<EnemyShared>();
                if (enemy != null)
                {
                    enemy.CurrentHP -= RealDamage;
                    float a = Random.Range(0, 100f);
                    if (a <= freezingChance)
                    {
                        enemy.InflictSuperiorFreezingBlasterFreeze(freezingDuration, addingFreezeDuration);
                    }
                    if (WeaponShoot.CurrentHitCount < 1)
                    {
                        if (WeaponShoot.CurrentHitCount == 0)
                        {
                            WeaponShoot.HitCountResetTimer = 1f / WeaponShoot.RateOfHit;
                        }
                        enemy.ReceiveThermalDamage(false);
                        WeaponShoot.CurrentHitCount = 1;
                    }
                }
                Destroy(gameObject);
            }
            break;
        }
    } 

    // Lava orb damage
    public void CalculateLavaOrbDamage()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, HitBox, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                if (AoE > 0)
                {
                    FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(col.transform.position, AoE, 0.8f);
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(col.transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy != null)
                        {
                            Debug.Log(RealDamage);
                            enemy.CurrentHP -= RealDamage;
                            // Inflict lava burned
                            enemy.InflictLavaBurned(enemy.MaxHP * 0.1f / 100, LavaBurnCount);
                        }
                    }
                }
                else
                {
                    EnemyShared enemy = col.GetComponent<EnemyShared>();
                    if (enemy != null)
                    {
                        enemy.CurrentHP -= RealDamage;
                        enemy.InflictLavaBurned(enemy.MaxHP * 0.1f / 100, LavaBurnCount);
                    }
                }
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
            break;
        }
    }

    public void CalculatePenetrateDamage()
    {

        // Detect any enemy with in range
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, HitBox*2, EnemyLayer);
        foreach (var col in cols)
        {
            if (AoE>0f)
            {
                // Create AoE effect
                FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(col.transform.position, AoE, 0, 0);
                // Check all enemies in AoE range
                Collider2D[] cols2 = Physics2D.OverlapCircleAll(col.transform.position, AoE, EnemyLayer);
                foreach (var col2 in cols2)
                {
                    // Deal damage to all enemies in AoE range
                    EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                    if (enemy != null)
                    {
                        enemy.CurrentHP -= RealDamage;
                    }
                }
            }
            if (!PenetrateAlreadyDealDamge.Contains(col.gameObject))
            {
                PenetrateAlreadyDealDamge.Add(col.gameObject);
                EnemyShared enemy = col.GetComponent<EnemyShared>();
                if (enemy!=null)
                {
                    enemy.CurrentHP -= RealDamage;
                }
                if (ApplyNanoEffect)
                {
                    enemy.InflictNanoTemp();
                    if (enemy.currentTemperature == 50f)
                    {
                        int a = Random.Range(40, 61);
                        if (a == 50)
                        {
                            a += (Random.Range(0, 1f) >= 0.5f ? 1 : -1) * 10;
                        }
                        else if (a > 50)
                        {
                            a += 10;
                        }
                        else
                        {
                            a -= 10;
                        }
                        enemy.currentTemperature = a;
                    }
                    else if (enemy.currentTemperature > 50f && enemy.currentTemperature < 90f)
                    {
                        enemy.ReceiveThermalDamage(true);
                    }
                    else if (enemy.currentTemperature >= 90f)
                    {
                        enemy.ReceiveThermalDamage(true);
                        enemy.ReceiveBurnedDamage(3);
                    }
                    else if (enemy.currentTemperature < 50f && enemy.currentTemperature > 0f)
                    {
                        enemy.ReceiveThermalDamage(false);
                    }
                    else if (enemy.currentTemperature <= 0f)
                    {
                        enemy.ReceiveThermalDamage(false);
                        if (enemy.FrozenDuration > 0f)
                        {
                            if (enemy.NanoEffectFrozenDurationIncrease < (3f - 1.5f * enemy.NanoTempScale))
                            {
                                enemy.FrozenDuration += 1.5f * enemy.NanoTempScale;
                                enemy.NanoEffectFrozenDurationIncrease += 1.5f * enemy.NanoTempScale;
                            } else
                            {
                                enemy.FrozenDuration += 3f - enemy.NanoEffectFrozenDurationIncrease;
                                enemy.NanoEffectFrozenDurationIncrease = 3f;
                            }
                        }
                    }
                }
                if (!isPenetrating)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    // Black hole orb
    public void CheckCreateBlackhole(GameObject BlackHole, float radius, float timer, float pullingForce)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, HitBox, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                // If hit, create blackhole at hit position
                CreateBlackHole(BlackHole, radius, timer, pullingForce);
                Destroy(gameObject);
                break;
            }
        }
    }

    // Create black hole
    public void CreateBlackHole(GameObject BlackHole, float radius, float timer, float pullingForce)
    {
        GameObject bh = Instantiate(BlackHole, transform.position, Quaternion.identity);
        Collider2D[] cols2 = Physics2D.OverlapCircleAll(transform.position, radius, EnemyLayer);
        foreach (var col2 in cols2)
        {
            // Deal damage to all enemies in AoE range
            EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
            if (enemy != null)
            {
                enemy.CurrentHP -= RealDamage;
            }
        }
        BlackHole bhole = bh.GetComponent<BlackHole>();
        if (bhole != null)
        {
            bhole.RadiusWhenCreate = radius;
            bhole.BasePullingForce = pullingForce;
            bhole.HitLayer = EnemyLayer;
        }
        bh.SetActive(true);
        Destroy(bh, timer);
    }
    // Check distance the bulllet travel, for setting the damage
    public void CheckDistanceTravel()
    {
        // Case Range is set for flamethrower type
        if (Range!=0f)
        {
            MaxEffectiveDistance = Range;
            MaximumDistance = Range;
        }
        // If distace travel <= max effective => DMG = base DMG
        if (DistanceTravel <= MaxEffectiveDistance)
        {
            RealDamage = BaseDamagePerHit;
        }
        // If max effective < distance travel < max distance => DMG and trasparency decreasing 
        if (DistanceTravel > MaxEffectiveDistance && DistanceTravel < MaximumDistance)
        {
            RealDamage = (0.5f + (MaximumDistance - DistanceTravel) / (2*(MaximumDistance - MaxEffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = (MaximumDistance - DistanceTravel) / (MaximumDistance - MaxEffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        // If distance travel > max distance => DMG = 0 (prevent bug for deal dmg out side range) and destroy it
        if (DistanceTravel >= MaximumDistance)
        {
            RealDamage = 0;
            Destroy(gameObject);
        }
    }

    // Check distance the bulllet travel, for setting the damage
    public void CheckDistanceTravelLavaOrb()
    {
        // Case Range is set for flamethrower type
        if (Range != 0f)
        {
            MaxEffectiveDistance = Range;
            MaximumDistance = Range;
        }
        // If distace travel <= max effective => DMG = base DMG
        if (DistanceTravel <= MaxEffectiveDistance)
        {
            RealDamage = BaseDamagePerHit;
        }
        // If max effective < distance travel < max distance => DMG and trasparency decreasing 
        if (DistanceTravel > MaxEffectiveDistance && DistanceTravel < MaximumDistance)
        {
            RealDamage = (0.5f + (MaximumDistance - DistanceTravel) / (2 * (MaximumDistance - MaxEffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = (MaximumDistance - DistanceTravel) / (MaximumDistance - MaxEffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        // If distance travel > max distance => DMG = 0 (prevent bug for deal dmg out side range) and destroy it
        if (DistanceTravel >= MaximumDistance)
        {
            RealDamage = 0;
            Destroy(transform.parent.gameObject);
        }
    }
    // distance travel for blackhole orb only
    public void CheckDistanceTravelBlackhole(GameObject BlackHole, float radius, float timer, float pullingForce)
    {
        if (Range != 0f)
        {
            MaxEffectiveDistance = Range;
            MaximumDistance = Range;
        }
        if (DistanceTravel <= MaxEffectiveDistance)
        {
            RealDamage = BaseDamagePerHit;
        }
        if (DistanceTravel > MaxEffectiveDistance && DistanceTravel < MaximumDistance)
        {
            RealDamage = (0.5f + (MaximumDistance - DistanceTravel) / (2 * (MaximumDistance - MaxEffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = (MaximumDistance - DistanceTravel) / (MaximumDistance - MaxEffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        if (DistanceTravel >= MaximumDistance)
        {
            RealDamage = BaseDamagePerHit;
            // Create blackhole at the end of the distance
            CreateBlackHole(BlackHole, radius, timer, pullingForce);
            Destroy(gameObject);
        }
    }

    public void CheckDistanceTravelPenetrate(float penetrateRange)
    {
        if (Range != 0f)
        {
            MaxEffectiveDistance = Range;
            MaximumDistance = Range;
        }
        if (DistanceTravel <= penetrateRange)
        {
            isPenetrating = true;
        } else
        {
            isPenetrating = false;
        }
        if (DistanceTravel <= MaxEffectiveDistance)
        {
            RealDamage = BaseDamagePerHit;
        }
        if (DistanceTravel > MaxEffectiveDistance && DistanceTravel < MaximumDistance)
        {
            RealDamage = (0.5f + (MaximumDistance - DistanceTravel) / (2 * (MaximumDistance - MaxEffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = (MaximumDistance - DistanceTravel) / (MaximumDistance - MaxEffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        if (DistanceTravel >= MaximumDistance)
        {
            RealDamage = 0;
            // Create blackhole at the end of the distance
            Destroy(gameObject);
        }
    }

    // Check if bullets is pulled by blackhole, Same algorithm as fighters
    public void CheckInsideBlackhole()
    {
        isBHPulled = false;
        PulledVector = new List<Vector2>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f, BlackholeLayer);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                BlackHole bh = col.GetComponent<BlackHole>();
                if (bh != null)
                {
                    isBHPulled = true;
                    PulledVector.Add(bh.CalculatePullingVector(gameObject));
                }
            }
        }
    }
    public void CalculateVelocity(Vector2 veloc)
    {
        if (isBHPulled)
        {
            foreach (Vector2 v in PulledVector)
            {
                veloc += v;
            }
        }
        GetComponent<Rigidbody2D>().velocity = veloc;
    }
    #endregion
}
