using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRepair : Consumable
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private float HealingAmount;
    private float PlayerMaxHP;
    private bool isStart;
    private float Timer;
    private float TotalHeal;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        
    }
    private void FixedUpdate()
    {
        if (isStart)
        {
            if (Timer >= Duration)
            {
                if (TotalHeal < HealingAmount)
                {
                    if (Fighter.GetComponent<PlayerFighter>().CurrentHP < Fighter.GetComponent<PlayerFighter>().MaxHP - (HealingAmount - TotalHeal))
                    {
                        Fighter.GetComponent<PlayerFighter>().CurrentHP += (HealingAmount - TotalHeal);
                    } else
                    {
                        Fighter.GetComponent<PlayerFighter>().CurrentHP += PlayerMaxHP - Fighter.GetComponent<PlayerFighter>().CurrentHP;
                    }
                }
                isStart = false;
                Timer = 0f;
            }
            else
            {
                
                if (Fighter.GetComponent<PlayerFighter>().CurrentHP < PlayerMaxHP - (PlayerMaxHP * float.Parse(Effect.Split("-")[1]) / (100 * Duration) * Time.fixedDeltaTime))
                {
                    if (TotalHeal < HealingAmount - (PlayerMaxHP * float.Parse(Effect.Split("-")[1]) / (100 * Duration) * Time.fixedDeltaTime))
                    {                        
                        Fighter.GetComponent<PlayerFighter>().CurrentHP += (PlayerMaxHP * float.Parse(Effect.Split("-")[1]) / (100 * Duration) * Time.fixedDeltaTime);
                        TotalHeal += (PlayerMaxHP * float.Parse(Effect.Split("-")[1]) / (100 * Duration) * Time.fixedDeltaTime);                       
                    } else
                    {
                        Fighter.GetComponent<PlayerFighter>().CurrentHP += HealingAmount - TotalHeal;
                        TotalHeal = HealingAmount;
                        Timer = Duration;
                    }      
                    
                }
                else
                {
                    TotalHeal += PlayerMaxHP - Fighter.GetComponent<PlayerFighter>().CurrentHP;
                    Fighter.GetComponent<PlayerFighter>().CurrentHP += PlayerMaxHP - Fighter.GetComponent<PlayerFighter>().CurrentHP ;
                }         
            }
            
            Timer += Time.fixedDeltaTime;           
        }
    }
    #endregion
    #region Activate cons
    // Group all function that serve the same algorithm
    public void ActivateAutoRepair()
    {
        isStart = true;
        PlayerMaxHP = Fighter.GetComponent<PlayerFighter>().MaxHP;
        /*TotalHeal = Fighter.GetComponent<PlayerFighter>().CurrentHP + PlayerMaxHP * float.Parse(Effect.Split("-")[1]) / 100;*/
        HealingAmount = PlayerMaxHP * float.Parse(Effect.Split("-")[1]) / 100;
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
