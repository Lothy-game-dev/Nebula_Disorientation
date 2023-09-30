using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShared : FighterShared
{
    #region Shared Variables
    public EnemyHealthBar HealthBar;
    public float ScaleOnStatusBoard;
    public GameObject EnemyStatus;
    private StatusBoard Status;
    #endregion
    #region Shared Functions
    // Set Health to Health Bar
    public void SetHealth()
    {
        HealthBar.SetValue(CurrentHP, MaxHP);
    }

    public void StartEnemy(float MHP)
    {
        Status = EnemyStatus.GetComponent<StatusBoard>();
        MaxHP = MHP;
    }
    public void UpdateEnemy()
    {
        SetHealth();
        UpdateFighter();
        if (CurrentHP <= 0f)
        {
            Status.StopShowing();
        }
    }

    private void OnMouseOver()
    {
        Status.Timer = 5f;
        Status.StartShowing(gameObject);
    }

    private void OnMouseExit()
    {
        Status.CheckOnDestroy();
    }
    #endregion
}
