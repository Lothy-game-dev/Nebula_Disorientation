using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AlliesFighterMLAgent : Agent
{
    #region Component
    private AlliesShared als;
    private FighterMovement fm;
    private Weapons LeftWeapon;
    private Weapons RightWeapon;
    public GameObject TopBound;
    public GameObject LeftBound;
    public GameObject RightBound;
    public GameObject BottomBound;
    #endregion
    #region Normal Variable
    private float moveUpDown;
    private float moveLeftRight;
    private int FireLeft;
    private int FireRight;
    public GameObject Target;
    private Vector2 RecordPos;
    #endregion
    #region Initialize
    public override void Initialize()
    {
        als = GetComponent<AlliesShared>();
        fm = GetComponent<FighterMovement>();
        LeftWeapon = als.LeftWeapon.GetComponent<Weapons>();
        RightWeapon = als.RightWeapon.GetComponent<Weapons>();
    }
    #endregion
    #region Training
    public override void CollectObservations(VectorSensor sensor)
    {
        Target = GetNearestEnemy();
        sensor.AddObservation((TopBound.transform.position - transform.position).magnitude);
        sensor.AddObservation((LeftBound.transform.position - transform.position).magnitude);
        sensor.AddObservation((RightBound.transform.position - transform.position).magnitude);
        sensor.AddObservation((BottomBound.transform.position - transform.position).magnitude);
        if (Target!=null)
        {
            sensor.AddObservation((Target.transform.position - transform.position).magnitude);
            sensor.AddObservation(Vector2.Angle(Target.transform.position - transform.position, new Vector2(0, 1)));
            sensor.AddObservation(Target.transform.position);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("Discrete: " + actions.DiscreteActions[0]);
        Debug.Log("Discrete: " + actions.DiscreteActions[1]);
        if (actions.DiscreteActions[0]==0)
        {
            fm.UpMove();
        }
        else if (actions.DiscreteActions[0] == 1)
        {
            fm.DownMove();
        }
        if (actions.DiscreteActions[1] == 0)
        {
            fm.LeftMove();
        }
        else if (actions.DiscreteActions[1] == 1)
        {
            fm.RightMove();
        }
        else if (actions.DiscreteActions[1] == 2)
        {
            fm.NoLeftRightMove();
        }
        CheckPositionToTarget();

    }
    #endregion
    #region Reward
    private void CheckPositionToTarget()
    {
        if (Target!=null)
        {
            if ((transform.position - Target.transform.position).magnitude < 10f)
            {
                ReceiveReward(1f, "Finish");
                FindObjectOfType<SpawnAlliesFighter>().SpawnAlly();
                Destroy(gameObject);
            }
            else
            {
                ReceivePunishment(0.01f, "Far");
            }
        }
    }
    private void CheckLeftWeaponShooting()
    {
        if (als.LeftTarget==null)
        {
            ReceivePunishment(0.1f, "Left Shooting");
        } else
        {
            if (LeftWeapon.FireTimer>0)
            {
                ReceivePunishment(0.1f, "Left Shooting Too Much");
            } else
            {
                ReceiveReward(0.1f, "Left Shooting");
            }
            
        }
    }
    private void CheckRightWeaponShooting()
    {
        if (als.RightTarget == null)
        {
            ReceivePunishment(0.1f, "Right Shooting");
        }
        else
        {
            if (LeftWeapon.FireTimer > 0)
            {
                ReceivePunishment(0.1f, "Right Shooting Too Much");
            }
            else
                ReceiveReward(0.1f, "Right Shooting");
        }
    }

    private void CheckCurrentStatus()
    {
        if (als.CurrentBarrier < als.MaxBarrier)
        {
            ReceivePunishment(0.1f, "Barrier");
        }
        if (als.CurrentHP < als.MaxHP && als.CurrentBarrier==0)
        {
            ReceivePunishment(0.2f, "Healthpoint");
        }
    }

    public void MovingLimitReward()
    {
        ReceivePunishment(10f, "Limit");
    }
    #endregion
    #region Reward
    public void ReceiveReward(float amount, string extra)
    {
        Debug.Log(name + " Receive Reward For " + extra + " :" + amount);
        AddReward(amount);
    }

    public void ReceivePunishment(float amount, string extra)
    {
        Debug.Log(name + " Receive Punishment For " + extra + " :-" + amount);
        AddReward(-amount);
    }

    public GameObject GetNearestEnemy()
    {
        EnemyShared[] gos = FindObjectsOfType<EnemyShared>();
        if (gos.Length>0)
        {
            float nearest = (transform.position - gos[0].transform.position).magnitude;
            GameObject nearestPos = gos[0].gameObject;
            for (int i=1; i<gos.Length;i++)
            {
                float distance = (transform.position - gos[i].transform.position).magnitude;
                if (nearest > distance)
                {
                    nearest = distance;
                    nearestPos = gos[i].gameObject;
                }
            }
            return nearestPos;
        }
        return null;
    }
    #endregion
}
