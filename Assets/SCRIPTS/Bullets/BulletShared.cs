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

    protected Rigidbody2D rb;
    protected float Distance;
    protected Vector2 Velocity;
    protected Vector2 RealVelocity;
    protected float DistanceTravel;

    private float RealDamage;
    private bool StartCounting;
    private bool isBHPulled;
    private List<Vector2> PulledVector;
    private bool AlreadyHit;
    #endregion
    #region Shared Functions
    public void InitializeBullet()
    {
        if (MaximumDistance==0f)
        {
            MaximumDistance = MaxEffectiveDistance;
        }
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

    public IEnumerator AccelerateLaser(float time, float initScale)
    {
        for (int i = 0; i < 10; i++)
        {
            if (Mathf.Abs(RealVelocity.x) < Mathf.Abs(Velocity.x) && Mathf.Abs(RealVelocity.y) < Mathf.Abs(Velocity.y))
            {
                RealVelocity = new Vector2(RealVelocity.x + Velocity.x / 10, RealVelocity.y + Velocity.y / 10);
            }
            CalculateVelocity(RealVelocity);
            transform.localScale = new Vector3(initScale*i/10,initScale*i/10,transform.localScale.z);
            yield return new WaitForSeconds(time / 10f);
        }
    }

    public void CalculateDamage()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                if (AoE>0)
                {
                    FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(col.transform.position, AoE);
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(col.transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy != null)
                        {
                            enemy.CurrentHP -= RealDamage;
                        }
                    }
                } else
                {
                    EnemyShared enemy = col.GetComponent<EnemyShared>();
                    if (enemy!=null)
                    {
                        enemy.CurrentHP -= RealDamage;
                    }
                }
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            break;
        }
    }

    public void CalculateThermalDamage(bool isHeat)
    {
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

    public void CalculateLavaOrbDamage()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.1f, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                if (AoE > 0)
                {
                    FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(col.transform.position, AoE);
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(col.transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy != null)
                        {
                            Debug.Log(RealDamage);
                            enemy.CurrentHP -= RealDamage;
                            enemy.InflictLavaBurned(RealDamage/10f);
                        }
                    }
                }
                else
                {
                    EnemyShared enemy = col.GetComponent<EnemyShared>();
                    if (enemy != null)
                    {
                        enemy.CurrentHP -= RealDamage;
                        enemy.InflictLavaBurned(RealDamage / 10f);
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

    public void CheckCreateBlackhole(GameObject BlackHole, float radius, float timer, float pullingForce)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.1f, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                CreateBlackHole(BlackHole, radius, timer, pullingForce);
                Destroy(gameObject);
                break;
            }
        }
    }

    public void CreateBlackHole(GameObject BlackHole, float radius, float timer, float pullingForce)
    {
        GameObject bh = Instantiate(BlackHole, transform.position, Quaternion.identity);
        BlackHole bhole = bh.GetComponent<BlackHole>();
        if (bhole != null)
        {
            bhole.BaseDmg = BaseDamagePerHit;
            bhole.RadiusWhenCreate = radius;
            bhole.BasePullingForce = pullingForce;
        }
        bh.SetActive(true);
        Destroy(bh, timer);
    }
    public void CheckDistanceTravel()
    {
        if (Range!=0f)
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
            RealDamage = (0.5f + (MaximumDistance - DistanceTravel) / (2*(MaximumDistance - MaxEffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = (MaximumDistance - DistanceTravel) / (MaximumDistance - MaxEffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        if (DistanceTravel >= MaximumDistance)
        {
            RealDamage = 0;
            Destroy(gameObject);
        }
    }

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
            RealDamage = 0;
            CreateBlackHole(BlackHole, radius, timer, pullingForce);
            Destroy(gameObject);
        }
    }
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
