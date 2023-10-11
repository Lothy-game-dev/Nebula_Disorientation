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
        sensor.AddObservation(als.LeftTarget == null);
        if (als.LeftTarget!=null)
        {
            sensor.AddObservation(als.LeftTarget.GetComponent<FighterMovement>().CurrentRotateAngle);
            sensor.AddObservation(als.LeftTarget.GetComponent<FighterMovement>().CurrentSpeed);
            sensor.AddObservation((als.LeftTarget.transform.position - transform.position).magnitude);
            sensor.AddObservation(Vector2.Angle(als.LeftTarget.transform.position - transform.position, transform.GetChild(5).position - transform.position));
        } else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
        sensor.AddObservation(LeftWeapon.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance);
        
        sensor.AddObservation(als.RightTarget == null);
        if (als.RightTarget != null)
        {
            sensor.AddObservation(als.RightTarget.GetComponent<FighterMovement>().CurrentRotateAngle);
            sensor.AddObservation(als.RightTarget.GetComponent<FighterMovement>().CurrentSpeed);
            sensor.AddObservation((als.RightTarget.transform.position - transform.position).magnitude);
            sensor.AddObservation(Vector2.Angle(als.RightTarget.transform.position - transform.position, transform.GetChild(5).position - transform.position));

        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
        sensor.AddObservation(RightWeapon.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (LeftWeapon != null)
        {
            if (als.DelayTimer < 0f && !als.LeftFire)
            {
                als.LeftFire = true;
                LeftWeapon.AIShootBullet(actions.ContinuousActions[0] * 200f, actions.ContinuousActions[1] * 200f);
                als.DelayTimer = als.DelayBetween2Weap;
            }
        }
        if (RightWeapon != null)
        {
            if (als.DelayTimer < 0f && als.LeftFire)
            {
                als.LeftFire = false;
                RightWeapon.AIShootBullet(actions.ContinuousActions[2] * 200f, actions.ContinuousActions[3] * 200f);
                als.DelayTimer = als.DelayBetween2Weap;
            }
        }
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
