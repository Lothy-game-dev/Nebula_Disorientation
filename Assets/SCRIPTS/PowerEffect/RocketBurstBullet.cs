using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RocketBurstBullet : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private Rigidbody2D rb;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject AimEffect;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float RkAngle;
    public LayerMask Layer;
    public float Damage;
    private List<float> DistanceList;
    public float Distance;
    public float DistanceTravel;
    private bool isUp;
    private Vector3 MovingVector;
    private GameObject target;
    private Vector3 ToEnemy;
    private Dictionary<float, GameObject> EnemyDictionary;
    private GameObject AimGen;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        if (DistanceTravel > 100)
        {
            if (target == null || target.GetComponent<Collider2D>()==null || !target.GetComponent<Collider2D>().enabled)
            {
                CheckRange();
            } else
            {
                MoveToTarget();
            }

        }
        CheckDistanceTravel();
        CalculateDamage();
        
    }
    #endregion
    #region Check range
    // Group all function that serve the same algorithm
    public void CheckRange()
    {
        if (AimGen!=null)
        {
            Destroy(AimGen);
        }
        DistanceList = new List<float>();
        EnemyDictionary = new Dictionary<float, GameObject>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1000f, Layer);
        foreach (var col in cols)
        {
            GameObject enemy = col.gameObject;
            if (enemy != null)
            {
                DistanceList.Add(Vector3.Distance(enemy.transform.position, transform.position));
                EnemyDictionary.Add(Vector3.Distance(enemy.transform.position, transform.position), enemy);
                float minDistance = DistanceList.Min();
                GameObject nearestEnemy = EnemyDictionary[minDistance];
                target = nearestEnemy;
            }          
           
        }
        CheckIsUpOrDownMovement();
        AimGen = Instantiate(AimEffect, target.transform.position, Quaternion.identity);
        AimGen.SetActive(true);
    }

    #endregion
    #region Check Distance
    // Group all function that serve the same algorithm
    private void CheckDistanceTravel()
    {
        if (DistanceTravel > Distance)
        {
            Destroy(AimGen);
            Destroy(gameObject);
        }
    }
    #endregion
    #region 
    public void MoveToTarget()
    {
        
        MovingVector = gameObject.transform.GetChild(0).position - gameObject.transform.GetChild(1).position;
        rb.velocity = MovingVector / MovingVector.magnitude * 500;
        ToEnemy = target.transform.position - transform.position;
        float angle = Vector3.Angle(ToEnemy, MovingVector);
        transform.Rotate(new Vector3(0, 0, (isUp ? 1 : -1) * angle / 10));
    }

    private void CheckIsUpOrDownMovement()
    {
        if (gameObject.transform.GetChild(0).position.y > gameObject.transform.GetChild(1).position.y)
        {
            if (target.transform.position.x < transform.position.x)
            {
                isUp = true;
            } else
            {
                isUp = false;
            }
        } else
        {
            if (target.transform.position.x < transform.position.x)
            {
                isUp = false;
            }
            else
            {
                isUp = true;
            }
        }
    }
    #endregion
    #region Calculate damage
    public void CalculateDamage()
    {
        // Detect enemy
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10f, Layer);
        foreach (var col in cols)
        {            
            EnemyShared enemy = col.gameObject.GetComponent<EnemyShared>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(Damage);
            }
            Destroy(AimGen);
            Destroy(gameObject);
        }
    }
    #endregion
}
