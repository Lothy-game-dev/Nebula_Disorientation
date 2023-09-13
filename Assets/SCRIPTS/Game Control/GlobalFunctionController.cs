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
        //Overheat
        string OH = StatList[0];
        if (OH.Contains("OH"))
        {
            OH = OH.Replace("OH-", "");
            string[] listOH = OH.Split(",");
            if (listOH.Length==3)
            {
                finalString += "<b>Overheat:  </b><i>" + listOH[0] + " | " + listOH[1] + " | " + listOH[2] + "</i>\n";
            }
            else if (listOH.Length==1)
            {
                finalString += "<b>Overheat:  </b><i>" + listOH[0] + "</i>\n";
            } else
            {
                finalString += OH;
            }
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
            finalString += "<b>Damage Per Hit:  </b><i>" + DPH + "</i>\n";
        } else
        {
            finalString += DPH;
        }
        //RoF
        string RoF = StatList[2];
        if (RoF.Contains("RoF"))
        {
            RoF = RoF.Replace("RoF-", "");
            finalString += "<b>Rate of fire:  </b><i>" + RoF + "/s</i>\n";
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
            finalString += "<b>Area Of Effect:  </b><i>" + AoE + "</i>\n";
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
            finalString += "<b>Velocity:  </b><i>" + V + "</i>\n";
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
            string[] ranges = R.Split("-");
            if (ranges.Length==1)
            {
                finalString += "<b>Range:  </b><i>" + ranges[0] + "</i>\n";
            } else if (ranges.Length==2)
            {
                finalString += "<b>Range:  </b><i>" + ranges[0] + "-" + ranges[1] + "</i>\n";
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

    // Case 1: BR-xn|DC-n,m
    // Case 2: BR-n|BR-xn|DC-n,m
    // Case 3: DPH-n|AoH-n/s or AoH-n|AoE-n|V-n|R-n|DC-n,m
    // If null will not add to string/dictionary
    public string ConvertPowerStatsToString(string stats)
    {
        string[] StatList = stats.Split("|");
        string finalString = "";
        if (StatList.Length==3)
        {
            if (StatList[0].Contains("BR-"))
            {
                string BR = StatList[0].Replace("BR-", "");
                finalString += "Barrier Strength: " + BR + "%HP\n";
            }
            // BR
            if (StatList[1].Contains("BR-x"))
            {
                string BR = StatList[1].Replace("BR-x", "");
                finalString += "Barrier Strength Multiplier: " + BR + "x\n";
            }
            else
            {
                finalString += StatList[1];
            }
            // DC
            if (StatList[2].Contains("DC-"))
            {
                string DC = StatList[2].Replace("DC-", "");
                string[] DCstats = DC.Split(",");
                if (DCstats[0]=="0")
                {
                    finalString += "Cooldown: " + DCstats[1] + "s\n";
                } else
                finalString += "Duration: " + DCstats[0] + "s\nCooldown: " + DCstats[1] + "s\n";
            }
            else
            {
                finalString += StatList[2];
            }
        } else if (StatList.Length==2)
        {
            // BR
            if (StatList[0].Contains("BR-x"))
            {
                string BR = StatList[0].Replace("BR-x", "");
                finalString += "Barrier Strength Multiplier: " + BR + "x\n";
            } else
            {
                finalString += StatList[0];
            }
            // DC
            if (StatList[1].Contains("DC-"))
            {
                string DC = StatList[1].Replace("DC-", "");
                string[] DCstats = DC.Split(",");
                if (DCstats[0] == "0")
                {
                    finalString += "Cooldown: " + DCstats[1] + "s\n";
                }
                else
                    finalString += "Duration: " + DCstats[0] + "s\nCooldown: " + DCstats[1] + "s\n";
            }
            else
            {
                finalString += StatList[1];
            }
        } else
        {
            //DPH
            if (StatList[0].Contains("DPH-"))
            {
                string DPH = StatList[0].Replace("DPH-", "");
                if (DPH!="null")
                {
                    finalString += "Damage Per Hit: " + DPH + "\n";
                }
            }
            else
            {
                finalString += StatList[0];
            }
            //AoH
            if (StatList[1].Contains("AoH-"))
            {
                string AoH = StatList[1].Replace("AoH-", "");
                if (AoH!="null")
                {
                    finalString += "Amount Of Hit: " + AoH + "\n";
                }
            }
            else
            {
                finalString += StatList[1];
            }
            //AoE
            if (StatList[2].Contains("AoE-"))
            {
                string AoE = StatList[2].Replace("AoE-", "");
                if (AoE!="null")
                {
                    finalString += "Area of Effect: " + AoE + "\n";
                }
            }
            else
            {
                finalString += StatList[2];
            }
            //V
            if (StatList[3].Contains("V-"))
            {
                string V = StatList[3].Replace("V-", "");
                if (V!="null")
                {
                    finalString += "Velocity: " + V + "\n";
                }
            }
            else
            {
                finalString += StatList[3];
            }
            //R
            if (StatList[4].Contains("R-"))
            {
                string R = StatList[4].Replace("R-", "");
                if (R!="null")
                {
                    finalString += "Range: " + R + "\n";
                }
            }
            else
            {
                finalString += StatList[4];
            }
            // DC
            if (StatList[5].Contains("DC-"))
            {
                string DC = StatList[5].Replace("DC-", "");
                string[] DCstats = DC.Split(",");
                if (DCstats[0] == "0")
                {
                    finalString += "Cooldown: " + DCstats[1] + "s\n";
                }
                else
                    finalString += "Duration: " + DCstats[0] + "s\nCooldown: " + DCstats[1] + "s\n";
            }
            else
            {
                finalString += StatList[5];
            }
        }
        return finalString;
    }

    public Dictionary<string, object> ConvertPowerStatsToDictionary(string stats)
    {
        return null;
    }

    public string ConvertModelStatsToString(string stats)
    {
        return "";
    }

    //HP-n|SPD-n|ROT-n|AOF-n,n|DM-n|AM-n|PM-n|SP-n|SC-n
    public Dictionary<string, object> ConvertModelStatsToDictionary(string stats)
    {
        Dictionary<string, object> StatsDictionary = new Dictionary<string, object>();
        string[] StatsList = stats.Split("|");
        //HP
        string HP = StatsList[0];
        if (HP.Contains("HP-"))
        {
            HP = HP.Replace("HP-", "");
            StatsDictionary.Add("HP", HP);
        } else
        {
            StatsDictionary.Add("HP", "0");
        }
        //SPD
        string SPD = StatsList[1];
        if (SPD.Contains("SPD-"))
        {
            SPD = SPD.Replace("SPD-", "");
            StatsDictionary.Add("SPD", SPD);
        } else
        {
            StatsDictionary.Add("SPD", "0");
        }
        //ROT
        string ROT = StatsList[2];
        if (ROT.Contains("ROT-"))
        {
            ROT = ROT.Replace("ROT-", "");
            StatsDictionary.Add("ROT", (float.Parse(ROT)*120).ToString());
        }
        else
        {
            StatsDictionary.Add("ROT", "0");
        }
        //AOF
        string AOF = StatsList[3];
        if (AOF.Contains("AOF-"))
        {
            AOF = AOF.Replace("AOF-", "");
            StatsDictionary.Add("AOF", "-" + AOF.Split(",")[0] + "° ~ " + AOF.Split(",")[1] + "°");
            StatsDictionary.Add("AOFDemo", AOF.Split(",")[0]);
        }
        else
        {
            StatsDictionary.Add("AOF", "0");
        }
        //DM
        string DM = StatsList[4];
        if (DM.Contains("DM-"))
        {
            DM = DM.Replace("DM-", "");
            StatsDictionary.Add("DM", DM + "x");
        }
        else
        {
            StatsDictionary.Add("DM", "0");
        }
        //AM
        string AM = StatsList[5];
        if (AM.Contains("AM-"))
        {
            AM = AM.Replace("AM-", "");
            StatsDictionary.Add("AM", AM + "x");
        }
        else
        {
            StatsDictionary.Add("AM", "0");
        }
        //PM
        string PM = StatsList[6];
        if (PM.Contains("PM-"))
        {
            PM = PM.Replace("PM-", "");
            StatsDictionary.Add("PM", PM + "x");
        }
        else
        {
            StatsDictionary.Add("PM", "0");
        }
        //SP
        string SP = StatsList[7];
        if (SP.Contains("SP-"))
        {
            SP = SP.Replace("SP-", "");
            StatsDictionary.Add("SP", SP);
        }
        else
        {
            StatsDictionary.Add("SP", "0");
        }
        //SC
        string SC = StatsList[8];
        if (SC.Contains("SC-"))
        {
            SC = SC.Replace("SC-", "");
            StatsDictionary.Add("SC", SC);
        }
        else
        {
            StatsDictionary.Add("SC", "0");
        }
        return StatsDictionary;
    }
    #endregion
}
