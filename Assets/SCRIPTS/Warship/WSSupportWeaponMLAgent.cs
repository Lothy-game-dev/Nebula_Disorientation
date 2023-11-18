using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class WSSupportWeaponMLAgent : Agent
{
    #region Component
    private WSShared WS;
    private SpaceStationShared SS;
    private Weapons SPWeapon;
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
        WS = GetComponent<Weapons>().Fighter.GetComponent<WSShared>();
        SS = GetComponent<Weapons>().Fighter.GetComponent<SpaceStationShared>();
        SPWeapon = GetComponent<Weapons>();
    }
    #endregion
    #region Training
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(SPWeapon.Aim == null);
        if (SPWeapon.Aim != null)
        {
            sensor.AddObservation(Vector2.Angle(SPWeapon.ShootingPosition.transform.position - SPWeapon.RotatePoint.transform.position,
                SPWeapon.Aim.GetComponent<FighterMovement>() != null ? SPWeapon.Aim.GetComponent<FighterMovement>().HeadObject.transform.position :
                SPWeapon.Aim.GetComponent<WSMovement>() != null ? SPWeapon.Aim.GetComponent<WSMovement>().HeadObject.transform.position :
                SPWeapon.Aim.GetComponent<PlayerMovement>() != null ? SPWeapon.Aim.GetComponent<PlayerMovement>().HeadObject.transform.position :
                new Vector3(SPWeapon.Aim.transform.position.x + 100, SPWeapon.Aim.transform.position.y, SPWeapon.Aim.transform.position.z) - SPWeapon.Aim.transform.position));
            sensor.AddObservation(SPWeapon.Aim.GetComponent<FighterMovement>() != null ? SPWeapon.Aim.GetComponent<FighterMovement>().CurrentSpeed :
                SPWeapon.Aim.GetComponent<WSMovement>() != null ? SPWeapon.Aim.GetComponent<WSMovement>().CurrentSpeed :
                SPWeapon.Aim.GetComponent<PlayerMovement>() != null ? SPWeapon.Aim.GetComponent<PlayerMovement>().CurrentSpeed : 0);
            sensor.AddObservation((SPWeapon.Aim.transform.position - transform.position).magnitude);
        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (SPWeapon != null)
        {
            SPWeapon.AIShootBullet((float)(SPWeapon.Aim != null && SPWeapon.Aim.GetComponent<SpaceStationShared>() == null ?
                    (SPWeapon.Aim.GetComponent<PlayerMovement>() != null ? SPWeapon.Aim.GetComponent<PlayerMovement>().CurrentSpeed > 0 ? SPWeapon.Aim.GetComponent<PlayerMovement>().RotateDirection : 0
                    : SPWeapon.Aim.GetComponent<FighterMovement>() != null ? SPWeapon.Aim.GetComponent<FighterMovement>().CurrentSpeed > 0 ? SPWeapon.Aim.GetComponent<FighterMovement>().RotateDirection : 0
                    : SPWeapon.Aim.GetComponent<WSMovement>() != null ? SPWeapon.Aim.GetComponent<WSMovement>().CurrentSpeed > 0 ? SPWeapon.Aim.GetComponent<WSMovement>().RotateDirection : 0 : GetComponent<WSMovement>().RotateDirection) : GetComponent<WSMovement>().RotateDirection) * actions.ContinuousActions[0] * 15);
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
