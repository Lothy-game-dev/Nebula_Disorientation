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
        try
        {
            sensor.AddObservation(als.LeftTarget == null);
            if (als.LeftTarget != null)
            {
                sensor.AddObservation(Vector2.Angle(LeftWeapon.ShootingPosition.transform.position - LeftWeapon.RotatePoint.transform.position, als.LeftTarget.GetComponent<FighterMovement>() != null ? als.LeftTarget.GetComponent<FighterMovement>().HeadObject.transform.position : als.LeftTarget.GetComponent<WSMovement>().HeadObject.transform.position - als.LeftTarget.transform.position));
                sensor.AddObservation(als.LeftTarget.GetComponent<FighterMovement>() != null ? als.LeftTarget.GetComponent<FighterMovement>().CurrentSpeed : als.LeftTarget.GetComponent<WSMovement>().CurrentSpeed);
                sensor.AddObservation((als.LeftTarget.transform.position - transform.position).magnitude);
            }
            else
            {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            }
        } catch (System.Exception ex)
        {

        }
        
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (LeftWeapon != null)
        {
            if (als.DelayTimer < 0f && !als.LeftFire)
            {
                float speedScale = 350;
                if (als.Class == "C")
                {
                    speedScale = (als.LeftTarget != null && als.LeftTarget.GetComponent<SpaceStationShared>() == null ?
                    (als.LeftTarget.GetComponent<PlayerMovement>() != null ? als.LeftTarget.GetComponent<PlayerMovement>().CurrentSpeed 
                    : als.LeftTarget.GetComponent<FighterMovement>() != null ? als.LeftTarget.GetComponent<FighterMovement>().CurrentSpeed
                    : als.LeftTarget.GetComponent<WSMovement>() != null ? als.LeftTarget.GetComponent<WSMovement>().CurrentSpeed : 400) : 400);
                }
                als.LeftFire = true;
                LeftWeapon.AIShootBullet((float)(als.LeftTarget != null && als.LeftTarget.GetComponent<SpaceStationShared>() == null ?
                    (als.LeftTarget.GetComponent<PlayerMovement>() != null ? als.LeftTarget.GetComponent<PlayerMovement>().CurrentSpeed > 200 ? als.LeftTarget.GetComponent<PlayerMovement>().RotateDirection : 0
                    : als.LeftTarget.GetComponent<FighterMovement>() != null ? als.LeftTarget.GetComponent<FighterMovement>().CurrentSpeed > 200 ? als.LeftTarget.GetComponent<FighterMovement>().RotateDirection : 0
                    : als.LeftTarget.GetComponent<WSMovement>() != null ? als.LeftTarget.GetComponent<WSMovement>().CurrentSpeed > 200 ? als.LeftTarget.GetComponent<WSMovement>().RotateDirection : 0 : fm.RotateDirection) : fm.RotateDirection)
                    * actions.ContinuousActions[0]
                    * speedScale / 350f
                    * 15);
                als.DelayTimer = als.DelayBetween2Weap;
            }
        }
        if (RightWeapon != null)
        {
            if (als.DelayTimer < 0f && als.LeftFire)
            {
                float speedScale = 350;
                if (als.Class == "C")
                {
                    speedScale = (als.RightTarget != null && als.RightTarget.GetComponent<SpaceStationShared>() == null ?
                    (als.RightTarget.GetComponent<PlayerMovement>() != null ? als.RightTarget.GetComponent<PlayerMovement>().CurrentSpeed
                    : als.RightTarget.GetComponent<FighterMovement>() != null ? als.RightTarget.GetComponent<FighterMovement>().CurrentSpeed
                    : als.RightTarget.GetComponent<WSMovement>() != null ? als.RightTarget.GetComponent<WSMovement>().CurrentSpeed : 400) : 400);
                }
                als.LeftFire = false;
                RightWeapon.AIShootBullet((float)(als.RightTarget != null && als.RightTarget.GetComponent<SpaceStationShared>() == null ?
                    (als.RightTarget.GetComponent<PlayerMovement>() != null ? als.RightTarget.GetComponent<PlayerMovement>().CurrentSpeed > 200 ? als.RightTarget.GetComponent<PlayerMovement>().RotateDirection : 0
                    : als.RightTarget.GetComponent<FighterMovement>() != null ? als.RightTarget.GetComponent<FighterMovement>().CurrentSpeed > 200 ? als.RightTarget.GetComponent<FighterMovement>().RotateDirection : 0
                    : als.RightTarget.GetComponent<WSMovement>() != null ? als.RightTarget.GetComponent<WSMovement>().CurrentSpeed > 200 ? als.RightTarget.GetComponent<WSMovement>().RotateDirection : 0 : fm.RotateDirection) : fm.RotateDirection)
                    * actions.ContinuousActions[0]
                    * speedScale / 350f
                    * 15);
                als.DelayTimer = als.DelayBetween2Weap;
            }
        }
    }
    #endregion
    #region Reward
    public void ReceiveReward(float amount, string extra)
    {
        AddReward(amount);
        EndEpisode();
    }

    public void ReceivePunishment(float amount, string extra)
    {
        AddReward(-amount);
        EndEpisode();
    }
    #endregion
}
