using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float Damage;
    public Rigidbody2D rb;
    public float Distance;
    public float DistanceTravel;
    public float Dur;
    public Vector2 InitScale;
    public LayerMask Layer;
    public LaserBeam Laser;
    public GameObject Fighter;
    public LOTWEffect LOTWEffect;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(ChangeBulletScale());
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        CheckDistanceTravel();
        DealDamage();
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private IEnumerator ChangeBulletScale()
    {
        for (int i = 0; i < 5; i++)
        {
            transform.localScale = new Vector2(InitScale.x*i/5, InitScale.y * i / 5);
            yield return new WaitForSeconds(0.01f);
        }
    }
    #endregion
    #region Check Distance Travel
    // Group all function that serve the same algorithm
    private void CheckDistanceTravel()
    {
        if (DistanceTravel > Distance)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region
    public void CalculateVelocity(Vector2 veloc)
    {
        GetComponent<Rigidbody2D>().velocity = veloc;
    }
    #endregion
    #region Calculate damage
    public void DealDamage()
    {
        // Detect enemy
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10f, Layer);
        foreach (var col in cols)
        {
            if (!Laser.onHit)
            {
                Laser.onHit = true;
                float LOTWEffectScale = 1;
                if (LOTWEffect!=null)
                {
                    LOTWEffectScale *= LOTWEffect.LOTWPowerDMGIncScale;
                    if (col.GetComponent<FighterShared>() != null)
                    {
                        if (col.GetComponent<FighterShared>().CurrentBarrier > 0)
                        {
                            LOTWEffectScale *= LOTWEffect.LOTWBarrierDMGScale;
                        }
                    }
                    else if (col.GetComponent<WSShared>() != null)
                    {
                        if (col.GetComponent<WSShared>().CurrentBarrier > 0)
                        {
                            LOTWEffectScale *= LOTWEffect.LOTWBarrierDMGScale;
                        }
                    }
                    else if (col.GetComponent<SpaceStationShared>() != null)
                    {
                        if (col.GetComponent<SpaceStationShared>().CurrentBarrier > 0)
                        {
                            LOTWEffectScale *= LOTWEffect.LOTWBarrierDMGScale;
                        }
                    }
                    if ((col.gameObject.transform.position - Fighter.transform.position).magnitude > 2000)
                    {
                        LOTWEffectScale *= LOTWEffect.LOTWFarDMGScale;
                    }
                }
                float FinalDamage = Damage * LOTWEffectScale;
                if (col.GetComponent<FighterShared>() != null)
                {
                    col.GetComponent<FighterShared>().ReceiveDamage(FinalDamage, Fighter, true);
                }
                else
                {
                    if (col.GetComponent<WSShared>() != null)
                    {
                        col.GetComponent<WSShared>().ReceivePowerDamage(FinalDamage, gameObject, Fighter, transform.position);
                    }
                    else
                    {
                        if (col.GetComponent<SpaceStationShared>() != null)
                        {
                            col.GetComponent<SpaceStationShared>().ReceivePowerDamage(FinalDamage, Fighter, transform.position);
                        }
                    }
                }
            }
            Destroy(gameObject);
        }
    }
    #endregion
}
