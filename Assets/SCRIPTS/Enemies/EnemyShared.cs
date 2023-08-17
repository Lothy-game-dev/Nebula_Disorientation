using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShared : MonoBehaviour
{
    #region Shared Variables
    public float CurrentHP;
    public float MaxHP;
    public EnemyHealthBar HealthBar;
    #endregion
    #region Shared Functions
    // Set Health to Health Bar
    public void SetHealth()
    {
        HealthBar.SetValue(CurrentHP, MaxHP);
    }
    #endregion
}
