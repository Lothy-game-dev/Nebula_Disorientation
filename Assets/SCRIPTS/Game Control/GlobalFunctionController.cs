using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Must be added to main camera/Game controller object
public class GlobalFunctionController : MonoBehaviour
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
    #region Convert Stats String Json
    // Format stats string:
    // OH-a,b,c|DPH-n|RoF-n|AoE-n|V-n|R-n(,m)
    public string ConvertWeaponStatsToString(string stats)
    {
        string[] StatList = stats.Split("|");
        string finalString = "";
        //Overheat WIP
        string OH = StatList[0];
        if (OH.Contains("OH"))
        {
            OH = OH.Replace("OH-", "");
            string[] OHL = OH.Split(",");
            finalString += "<b><i>Overheat: </i></b> ";
        }
        else
        {
            finalString += OH;
        }
        //DPH
        string DPH = StatList[1];
        if (DPH.Contains("DPH"))
        {
            DPH = DPH.Replace("DPH-", "");
            finalString += "<b><i>Damage Per Hit:</i></b> " + DPH;
        } else
        {
            finalString += DPH;
        }
        //RoF
        string RoF = StatList[2];
        if (RoF.Contains("RoF"))
        {
            RoF = RoF.Replace("RoF-", "");
            finalString += "<b><i>Rate of fire:</i></b> " + RoF;
        }
        else
        {
            finalString += RoF;
        }
        //AoE
        string AoE = StatList[3];
        if (AoE.Contains("AoE"))
        {
            AoE = AoE.Replace("AoE-", "");
            finalString += "<b><i>Area Of Effect:</i></b> " + AoE;
        }
        else
        {
            finalString += AoE;
        }
        //V
        string V = StatList[4];
        if (V.Contains("V"))
        {
            V = V.Replace("V-", "");
            finalString += "<b><i>Velocity:</i></b> " + V;
        }
        else
        {
            finalString += V;
        }
        //R
        string R = StatList[5];
        if (R.Contains("R"))
        {
            R = R.Replace("R-", "");
            string[] ranges = R.Split(",");
            if (ranges.Length==1)
            {
                finalString += "<b><i>Bullet Range:</i></b> " + ranges[0];
            } else if (ranges.Length==2)
            {
                finalString += "<b><i>Bullet Range:</i></b> " + ranges[0] + "-" + ranges[1];
            } else
            {
                finalString += R;
            }
        }
        else
        {
            finalString += R;
        }
        return finalString;
    }
    public Dictionary<string, object> ConvertWeaponStatsToDictionary(string stats)
    {
        return null;
    }

    public string ConvertPowerStatsToString(string stats)
    {
        return "";
    }

    public Dictionary<string, object> ConvertPowerStatsToDictionary(string stats)
    {
        return null;
    }

    public string ConvertModelStatsToString(string stats)
    {
        return "";
    }

    public Dictionary<string, object> ConvertModelStatsToDictionary(string stats)
    {
        return null;
    }
    #endregion
}
