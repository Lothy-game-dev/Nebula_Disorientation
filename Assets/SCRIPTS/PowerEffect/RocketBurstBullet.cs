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
    public GameObject ExploEffect;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float RkAngle;
    public float AoE;
    public LayerMask Layer;
    public float Damage;
    public GameObject Fighter;
    private List<float> DistanceList;
    public float Distance;
    public float DistanceTravel;
    public int DirMov;
    public float Velocity;
    private Vector3 MovingVector;
    private GameObject target;
    private Vector3 ToEnemy;
    private Dictionary<GameObject, float> EnemyDictionary;
    private GameObject AimGen;
    private List<GameObject> EnemyList;
    private float DelayTarget;
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
            if (target == null || target.GetComponent<Collider2D>() == null || !target.GetComponent<Collider2D>().enabled)
            {
                DelayTarget -= Time.deltaTime;
                if (DelayTarget<=0f)
                {
                    DelayTarget = 0.1f;
                    CheckRange();
                }
            }
            else
            {
                if (target.layer == 9)
                {
                    target = null;
                } else
                {
                    MoveToTarget();
                }
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
        EnemyList = new List<GameObject>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1000f, Layer);
        foreach (var col in cols)
        {
            GameObject enemy = col.gameObject;
            if (enemy != null)
            {
                DistanceList.Add(Vector3.Distance(enemy.transform.position, transform.position));
                EnemyList.Add(enemy);
                float minDistance = DistanceList.Min();
                int index = DistanceList.IndexOf(minDistance);
                GameObject nearestEnemy = EnemyList[index];
                target = nearestEnemy;
            }          
           
        }
        if (target != null)
        {
            AimGen = Instantiate(AimEffect, target.transform.position, Quaternion.identity);
            AimGen.transform.SetParent(target.transform);
            AimGen.SetActive(true);
            Destroy(AimGen, 10f);
        }
    }

    #endregion
    #region Check Distance
    // Group all function that serve the same algorithm
    private void CheckDistanceTravel()
    {
        if (DistanceTravel > Distance)
        {
            GameObject explo = Instantiate(ExploEffect, transform.position, Quaternion.identity);
            explo.SetActive(true);
            Destroy(AimGen);
            Destroy(explo, 0.2f);
            Destroy(gameObject);
        }
    }
    #endregion
    #region Homing to Target
    public void MoveToTarget()
    {
        
        MovingVector = gameObject.transform.GetChild(0).position - gameObject.transform.GetChild(1).position;
        rb.velocity = MovingVector / MovingVector.magnitude * Velocity;
        ToEnemy = target.transform.position - transform.position;
        float angle = Vector3.Angle(ToEnemy, MovingVector);
        float curScale = 1;
        if (angle > 75 && angle < 105)
        {
            curScale = Velocity/500f * (1.1f + 0.2f * Mathf.Abs(angle-90));
        }
        CheckIsUpOrDownMovement();
        transform.Rotate(new Vector3(0, 0, -DirMov * curScale * angle / 10));
    }

    private void CheckIsUpOrDownMovement()
    {
        Vector2 HeadToTarget = target.transform.position - gameObject.transform.GetChild(0).position;
        Vector2 MovingVector = gameObject.transform.GetChild(0).position - gameObject.transform.GetChild(1).position;
        float angle = Vector2.Angle(HeadToTarget, MovingVector);
        float DistanceNew = Mathf.Cos(angle * Mathf.Deg2Rad) * HeadToTarget.magnitude;
        Vector2 TempPos = new Vector2(gameObject.transform.GetChild(1).position.x, gameObject.transform.GetChild(1).position.y) + MovingVector / MovingVector.magnitude * (MovingVector.magnitude + DistanceNew);
        Vector2 CheckPos = new Vector2(target.transform.position.x, target.transform.position.y) + (TempPos - new Vector2(target.transform.position.x, target.transform.position.y)) * 2;
        if (gameObject.transform.GetChild(0).position.x == gameObject.transform.GetChild(1).position.x)
        {
            if (gameObject.transform.GetChild(0).position.y > gameObject.transform.GetChild(1).position.y)
            {
                if (target.transform.position.x < gameObject.transform.GetChild(0).position.x)
                {
                    DirMov = -1;
                } else if (target.transform.position.x == gameObject.transform.GetChild(0).position.x)
                {
                    DirMov = 0;
                } else
                {
                    DirMov = 1;
                }
            } else
            {
                if (target.transform.position.x < gameObject.transform.GetChild(0).position.x)
                {
                    DirMov = 1;
                }
                else if (target.transform.position.x == gameObject.transform.GetChild(0).position.x)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = -1;
                }
            }
        }
        else if (gameObject.transform.GetChild(0).position.y == gameObject.transform.GetChild(1).position.y)
        {
            if (gameObject.transform.GetChild(0).position.x > gameObject.transform.GetChild(1).position.x)
            {
                if (target.transform.position.y > gameObject.transform.GetChild(0).position.y)
                {
                    DirMov -= 1;
                } else if (target.transform.position.y == gameObject.transform.GetChild(0).position.y)
                {
                    DirMov = 0;
                } else
                {
                    DirMov = 1;
                }
            } else
            {
                if (target.transform.position.y > gameObject.transform.GetChild(0).position.y)
                {
                    DirMov = 1;
                }
                else if (target.transform.position.y == gameObject.transform.GetChild(0).position.y)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov -= 1;
                }
            }
        } 
        else if (gameObject.transform.GetChild(0).position.x > gameObject.transform.GetChild(1).position.x)
        {
            if (CheckPos.y < target.transform.position.y)
            {
                DirMov = -1;
            }
            else
            {
                DirMov = 1;
            }
        } else
        {
            if (CheckPos.y < target.transform.position.y)
            {
                DirMov = 1;
            }
            else
            {
                DirMov = -1;
            }
        }

    }
    #endregion
    #region Calculate damage
    public void CalculateDamage()
    {
        // Detect enemy
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.GetChild(0).position, 10f, Layer);
        foreach (var col in cols)
        {
            FindObjectOfType<AreaOfEffect>().CreateAreaOfEffect(transform.position, AoE);
            Collider2D[] cols2 = Physics2D.OverlapCircleAll(transform.position, AoE, Layer);
            foreach (var col2 in cols2)
            {
                /*FighterShared enemy = col2.gameObject.GetComponent<FighterShared>();*/
                if (col2.GetComponent<FighterShared>() != null)
                {
                    col2.GetComponent<FighterShared>().ReceiveDamage(Damage, Fighter);
                } else
                {
                    if (col2.GetComponent<WSShared>() != null)
                    {
                        col2.GetComponent<WSShared>().ReceivePowerDamage(Damage, gameObject, Fighter);
                    } else
                    {
                        if (col2.GetComponent<SpaceStationShared>() != null)
                        {
                            col2.GetComponent<SpaceStationShared>().ReceivePowerDamage(Damage, Fighter);
                        }
                    }
                }
            }
            Destroy(AimGen);
            Destroy(gameObject);
            break;
        }
    }
    #endregion
}
