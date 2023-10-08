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
    #endregion
    #region Normal Variable
    private int moveUpDown;
    private int moveLeftRight;
    private int FireLeft;
    private int FireRight;
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
        sensor.AddObservation(fm.SpeedUp);
        sensor.AddObservation(fm.RotateDirection);
        sensor.AddObservation(als.LeftTarget);
        sensor.AddObservation(als.RightTarget);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(LeftWeapon.FireTimer);
        sensor.AddObservation(RightWeapon.FireTimer);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        moveUpDown = actions.DiscreteActions[0];
        if (moveUpDown==0)
        {
            fm.UpMove();
        } else if (moveUpDown==1)
        {
            fm.DownMove();
        } else if (moveUpDown == 2)
        {
            fm.NoUpDownMove();
        }
        moveLeftRight = actions.DiscreteActions[1];
        if (moveLeftRight == 0)
        {
            fm.LeftMove();
        }
        else if (moveLeftRight == 1)
        {
            fm.RightMove();
        }
        else if (moveLeftRight == 2)
        {
            fm.NoLeftRightMove();
        }
        CheckPositionToTarget();
        FireLeft = actions.DiscreteActions[2];
        if (FireLeft==0)
        {
            als.LeftWeapon.GetComponent<Weapons>().AIShootBullet();
            CheckLeftWeaponShooting();
        }
        FireRight = actions.DiscreteActions[3];
        if (FireRight == 0)
        {
            als.RightWeapon.GetComponent<Weapons>().AIShootBullet();
            CheckRightWeaponShooting();
        }
        CheckCurrentStatus();

    }
    #endregion
    #region Reward
    private void CheckPositionToTarget()
    {
        if (als.LeftTarget == null && als.RightTarget==null)
        {
            ReceivePunishment(0.1f,"Positioning");
        } else
        {
            ReceiveReward(0.1f, "Positioning");
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
        ReceivePunishment(0.5f, "Limit");
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
    #endregion
}
