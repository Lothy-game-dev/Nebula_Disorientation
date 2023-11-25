using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOTWEffect : MonoBehaviour
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
    // Max HP
    public float LOTWMaxHPScale;
    // Move Speed
    public float LOTWMoveSpeedScale;
    // Weapon Red DMG
    public float LOTWWeaponDMGReceivedScale;
    // Weapon DMG increase
    public float LOTWWeaponDMGIncScale;
    // Weapon Rate of Fire
    public float LOTWWeaponROFScale;
    // Thermal Weapon DMG Increase to Freeze/Burn
    public float LOTWThermalWeaponDMGScale;
    // Barrier Damage Increase
    public float LOTWBarrierDMGScale;
    // Far Damage Increase
    public float LOTWFarDMGScale;
    // All Damage Receive
    public float LOTWAllDamageReceiveScale;
    // Power Cooldown 
    public float LOTWPowerCDScale;
    // Power Damage Increase
    public float LOTWPowerDMGIncScale;
    // Repairing Effect
    public float LOTWRepairEffectScale;
    // Increase Cash gain
    public float LOTWCashIncScale;
    // No effect Environment
    public bool LOTWAffectEnvironment;
    // No cost in using consumables
    public bool LOTWConsNoCost;
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
    #endregion
    #region Init LOTW Effect
    // Group all function that serve the same algorithm
    public void InitLOTW()
    {
        LOTWMaxHPScale = 1;
        LOTWMoveSpeedScale = 1;
        LOTWWeaponDMGReceivedScale = 1;
        LOTWWeaponDMGIncScale = 1;
        LOTWWeaponROFScale = 1;
        LOTWThermalWeaponDMGScale = 1;
        LOTWBarrierDMGScale = 1;
        LOTWFarDMGScale = 1;
        LOTWPowerCDScale = 1;
        LOTWPowerDMGIncScale = 1;
        LOTWRepairEffectScale = 1;
        LOTWAllDamageReceiveScale = 1;
        LOTWCashIncScale = 1;
        LOTWAffectEnvironment = true;
        LOTWConsNoCost = false;
        List<Dictionary<string, object>> ListLOTWOwned = FindObjectOfType<AccessDatabase>().GetLOTWInfoOwnedByID(PlayerPrefs.GetInt("PlayerID"));
        foreach (var LOTWData in ListLOTWOwned)
        {
            if ((int)LOTWData["Duration"] > 0)
            {
                CalculateEffectsByStatString((string)LOTWData["Effect"], (int)LOTWData["Stack"]);
            }
        }
    }

    private void CalculateEffectsByStatString(string str, int stack)
    {
        if (str.Contains("HP-"))
        {
            LOTWMaxHPScale += float.Parse(str.Replace("HP-", "")) * stack/100f;
        }
        else if (str.Contains("MS-"))
        {
            LOTWMoveSpeedScale += float.Parse(str.Replace("MS-", "")) * stack / 100f;
        }
        else if (str.Contains("RD-"))
        {
            LOTWWeaponDMGReceivedScale += float.Parse(str.Replace("RD-", "")) * stack / 100f;
        }
        else if (str.Contains("AWD-"))
        {
            LOTWWeaponDMGIncScale += float.Parse(str.Replace("AWD-", "")) * stack / 100f;
        }
        else if (str.Contains("TWD-"))
        {
            LOTWThermalWeaponDMGScale += float.Parse(str.Replace("TWD-", "")) * stack / 100f;
        }
        else if (str.Contains("BD-"))
        {
            LOTWBarrierDMGScale += float.Parse(str.Replace("BD-", "")) * stack / 100f;
        }
        else if (str.Contains("FD-"))
        {
            LOTWFarDMGScale += float.Parse(str.Replace("FD-", "")) * stack / 100f;
        }
        else if (str.Contains("PCD-"))
        {
            LOTWPowerCDScale -= float.Parse(str.Replace("PCD-", "")) * stack / 100f;
            if (LOTWPowerCDScale < 0f)
            {
                LOTWPowerCDScale = 0f;
            }
        }
        else if (str.Contains("R-"))
        {
            LOTWRepairEffectScale += float.Parse(str.Replace("R-", "")) * stack / 100f;
        }
        else if (str.Equals("HAZ"))
        {
            LOTWAffectEnvironment = false;
        }
        else if (str.Contains("BS-"))
        {
            LOTWWeaponDMGIncScale += float.Parse(str.Replace("BS-", "").Split("-")[0]) * stack / 100f;
            LOTWPowerDMGIncScale += float.Parse(str.Replace("BS-", "").Split("-")[0]) * stack / 100f;
            LOTWAllDamageReceiveScale += float.Parse(str.Replace("BS-", "").Split("-")[1]) * stack / 100f;
        }
        else if (str.Equals("CONS"))
        {
            LOTWConsNoCost = true;
        }
        else if (str.Contains("C-") && !str.Contains("C-x"))
        {
            LOTWCashIncScale += float.Parse(str.Replace("C-", "")) * stack / 100f;
        }
        else if (str.Contains("WROF-"))
        {
            LOTWWeaponROFScale += float.Parse(str.Replace("WROF-", "")) * stack / 100f;
        }
    }
    #endregion
}
