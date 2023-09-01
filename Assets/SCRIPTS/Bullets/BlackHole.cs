using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public float BasePullingForce;
    public GameObject RangeCheck;
    public GameObject CenterRange;
    public LayerMask HitLayer;
    #endregion
    #region NormalVariables
    public Vector2 PullingVector;
    public float BaseDmg;
    public int RateOfHit;
    public float radius;
    public float RadiusWhenCreate;

    private float centerRadius;
    private float ResetHitTimer;
    private bool alreadyDealDmg;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        radius = Mathf.Abs((transform.position - RangeCheck.transform.position).magnitude);
        centerRadius = Mathf.Abs((transform.position - CenterRange.transform.position).magnitude);
        if (BaseDmg==0)
        {
            BaseDmg = 10;
        }
        if (RateOfHit==0)
        {
            RateOfHit = 1;
        }
        if (RadiusWhenCreate!=0)
        {
            transform.localScale = new Vector3
                (transform.localScale.x * RadiusWhenCreate / radius,
                transform.localScale.y * RadiusWhenCreate / radius,
                transform.localScale.z);
            radius = Mathf.Abs((transform.position - RangeCheck.transform.position).magnitude);
            centerRadius = Mathf.Abs((transform.position - CenterRange.transform.position).magnitude);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        transform.Rotate(new Vector3(0, 0, 1));
        DealDamageToLayerInRange(HitLayer);
        if (ResetHitTimer>0f)
        {
            ResetHitTimer -= Time.deltaTime;
        }
        else
        {
            alreadyDealDmg = false;
            ResetHitTimer = 1f / RateOfHit;
        }
    }
    #endregion
    #region Pulling Vector Cal
    public Vector2 CalculatePullingVector(GameObject go)
    {
        float distance = Mathf.Abs((go.transform.position - transform.position).magnitude);
        if (distance > radius)
        {
            return new Vector2(0, 0);
        } 
        else if (distance <= centerRadius)
        {
            Vector2 vectorDis = transform.position - go.transform.position;
            float ForceX = BasePullingForce * vectorDis.x / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            float ForceY = BasePullingForce * vectorDis.y / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            return new Vector2(ForceX, ForceY);
        }
        else
        {
            Vector2 vectorDis = transform.position - go.transform.position;
            float pullForceCal = (radius - distance / 2) / radius * BasePullingForce;
            float ForceX = pullForceCal * vectorDis.x / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            float ForceY = pullForceCal * vectorDis.y / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            return new Vector2(ForceX, ForceY);
        }
    }

    public void DealDamageToLayerInRange(LayerMask layer)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, layer);
        if (cols.Length > 0)
        {
            if (!alreadyDealDmg)
            {
                alreadyDealDmg = true;
                foreach (var col in cols)
                {
                    if (col.GetComponent<FighterShared>()!=null)
                    {
                        FighterShared fighter = col.GetComponent<FighterShared>();
                        fighter.CurrentHP -= CalculateDamageDealt(fighter.gameObject);
                    }
                }
            }
        }
    }

    public float CalculateDamageDealt(GameObject go)
    {
        float distance = Mathf.Abs((go.transform.position - transform.position).magnitude);
        if (distance > radius)
        {
            return 0;
        }
        else if (distance <= centerRadius)
        {
            return BaseDmg;
        }
        else
        {
            return BaseDmg * (50 + distance / radius * 50) / 100;
        }
    }
    #endregion
}
