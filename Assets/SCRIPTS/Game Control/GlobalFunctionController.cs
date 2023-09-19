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
        string[] StatList = stats.Split("|");
        Dictionary<string,object> finalString = new Dictionary<string, object>();
        //Overheat
        string OH = StatList[0];
        if (OH.Contains("OH"))
        {
            OH = OH.Replace("OH-", "");
            string[] listOH = OH.Split(",");
            if (listOH.Length == 3)
            {
                finalString.Add("OH", listOH[0] + " | " + listOH[1] + " | " + listOH[2]);
            }
            else if (listOH.Length == 1)
            {
                finalString.Add("OH", listOH[0]);
            }
            else
            {
                finalString.Add("OH", OH);
            }
        }
        else
        {
            finalString.Add("OH", OH);
        }
        //DPH
        string DPH = StatList[1];
        if (DPH.Contains("DPH"))
        {
            DPH = DPH.Replace("DPH-", "");
            finalString.Add("DPH", DPH);
        }
        else
        {
            finalString.Add("DPH", DPH);
        }
        //RoF
        string RoF = StatList[2];
        if (RoF.Contains("RoF"))
        {
            RoF = RoF.Replace("RoF-", "");
            finalString.Add("ROF", RoF);
        }
        else
        {
            finalString.Add("ROF", RoF);
        }
        //AoE
        string AoE = StatList[3];
        if (AoE.Contains("AoE"))
        {
            AoE = AoE.Replace("AoE-", "");
            finalString.Add("AOE", AoE);
        }
        else
        {
            finalString.Add("AOE", AoE);
        }
        //V
        string V = StatList[4];
        if (V.Contains("V"))
        {
            V = V.Replace("V-", "");
            finalString.Add("SPD", V);
        }
        else
        {
            finalString.Add("SPD", V);
        }
        //R
        string R = StatList[5];
        if (R.Contains("R"))
        {
            R = R.Replace("R-", "");
            string[] ranges = R.Split(",");
            if (ranges.Length == 1)
            {
                finalString.Add("R", ranges[0]);
            }
            else if (ranges.Length == 2)
            {
                finalString.Add("R", ranges[0] + "---" + ranges[1]);
            }
            else
            {
                finalString.Add("R", R);
            }
        }
        else
        {
            finalString.Add("R", R);
        }
        return finalString;
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
        string[] StatList = stats.Split("|");
        Dictionary<string,object> finalString = new Dictionary<string, object>();
        if (StatList.Length == 3)
        {
            if (StatList[0].Contains("BR-"))
            {
                string BR = StatList[0].Replace("BR-", "");
                finalString.Add("BR", BR + "%HP (P)");
            }
            // BR
            if (StatList[1].Contains("BR-x"))
            {
                string BR = StatList[1].Replace("BR-x", "");
                finalString["BR"] += " x" + BR;
            }
            else
            {
                finalString.Add("BR", StatList[1]);
            }
            // DC
            if (StatList[2].Contains("DC-"))
            {
                string DC = StatList[2].Replace("DC-", "");
                string[] DCstats = DC.Split(",");
                if (DCstats[0] == "0")
                {
                    
                    finalString.Add("CD", DCstats[1] + "s");
                }
                else
                {
                    finalString.Add("Dur", DCstats[0] + "s");
                    finalString.Add("CD", DCstats[1] + "s");

                }
                    
            }
            else
            {
                finalString.Add("CD", StatList[2]);
            }
        }
        else if (StatList.Length == 2)
        {
            // BR
            if (StatList[0].Contains("BR-x"))
            {
                string BR = StatList[0].Replace("BR-x", "");
                finalString.Add("BR", "x" + BR);
            }
            else
            {
                finalString.Add("BR", StatList[0]);
            }
            // DC
            if (StatList[1].Contains("DC-"))
            {
                string DC = StatList[1].Replace("DC-", "");
                string[] DCstats = DC.Split(",");
                if (DCstats[0] == "0")
                {
                    finalString.Add("CD", DCstats[1] + "s");
                }
                else
                {
                    finalString.Add("Dur", DCstats[0] + "s");
                    finalString.Add("CD", DCstats[1] + "s");

                }
            }
            else
            {
                finalString.Add("CD", StatList[1]);
            }
        }
        else
        {
            //DPH
            if (StatList[0].Contains("DPH-"))
            {
                string DPH = StatList[0].Replace("DPH-", "");
                if (DPH != "null")
                {
                    finalString.Add("DPH", DPH);
                }
            }
            else
            {
                finalString.Add("DPH", StatList[0]);
            }
            //AoH
            if (StatList[1].Contains("AoH-"))
            {
                string AoH = StatList[1].Replace("AoH-", "");
                if (AoH != "null")
                {
                    finalString.Add("AOH", AoH);
                }
            }
            else
            {
                finalString.Add("DPH", StatList[1]);
            }
            //AoE
            if (StatList[2].Contains("AoE-"))
            {
                string AoE = StatList[2].Replace("AoE-", "");
                if (AoE != "null")
                {
                    finalString.Add("AOE", AoE);
                }
            }
            else
            {
                finalString.Add("AOE", StatList[2]);
            }
            //V
            if (StatList[3].Contains("V-"))
            {
                string V = StatList[3].Replace("V-", "");
                if (V != "null")
                {
                    finalString.Add("V", V);
                }
            }
            else
            {
                finalString.Add("V", StatList[3]);
            }
            //R
            if (StatList[4].Contains("R-"))
            {
                string R = StatList[4].Replace("R-", "");
                if (R != "null")
                {
                    finalString.Add("R", R);
                }
            }
            else
            {
                finalString.Add("R", StatList[4]);
            }
            // DC
            if (StatList[5].Contains("DC-"))
            {
                string DC = StatList[5].Replace("DC-", "");
                string[] DCstats = DC.Split(",");
                if (DCstats[0] == "0")
                {
                    finalString.Add("CD", DCstats[1] + "s");
                }
                else
                {
                finalString.Add("Dur", DCstats[0] + "s");
                finalString.Add("CD", DCstats[1] + "s");
                }
                    
            }
            else
            {
                finalString.Add("CD", StatList[5]);
            }
        }
        return finalString;
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

    // RED-
    // AER-
    // RMH-
    // INV
    // FC
    public string ConvertEffectAndDurationOfConsumables(string Effect, int Duration)
    {
        string final = "";
        if (Effect.Contains("RED-"))
        {
            Effect = Effect.Replace("RED-","");
            final = "Reduce Damage received of <color=\"blue\"><b>Barrier</b></color> by " + Effect
                + "% for " + Duration.ToString() + " seconds.";
        } 
        else if (Effect.Contains("AER-"))
        {
            Effect = Effect.Replace("AER-", "");
            string count = Effect == "2" ? "Double" : Effect == "3" ? "Triple" : "";
            final = count + " the <color=\"blue\"><b>AE Regen Speed</b></color> for " + Duration.ToString() + " seconds.";
        }
        else if (Effect.Contains("RMH-"))
        {
            Effect = Effect.Replace("RMH-", "");
            final = "Repair " + Effect + "% <color=\"green\"><b>Max Health</b></color> in "
                + Duration.ToString() + " seconds.";
        }
        else if (Effect.Contains("INV"))
        {
            final = "Render the <color=\"black\"><b>Fighter</b></color> invisible for " + Duration.ToString() + " seconds. (Cannot be targeted)";
        }
        else if (Effect.Contains("FC"))
        {
            final = "Instantly gain 1 <color=\"green\"><b>Fuel Core</b></color>. Can be purchased even when Fuel Core is full.";
        }
        return final;
    }

    public Dictionary<string,string> ConvertDictionaryDataToOutputCons(Dictionary<string,object> datas)
    {
        Dictionary<string,string> output = new Dictionary<string,string>();
        output.Add("Name", "<color=" + (string)datas["Color"] + ">" + (string)datas["Name"] + "</color>");
        output.Add("Rarity", "<color=" + (string)datas["Color"] + ">"
            + (((string)datas["Color"]).Equals("#36b37e") ? "Common" : 
            ((string)datas["Color"]).Equals("#4c9aff") ? "Uncommon" : "Rare") + "</color>");
        output.Add("Description", (string)datas["Description"]);
        string final = "";
        string Effect = (string)datas["Effect"];
        if (Effect.Contains("RED-"))
        {
            Effect = Effect.Replace("RED-", "");
            final = "Reduce Damage received of <color=\"blue\"><b>Barrier</b></color> by " + Effect + "%";
        }
        else if (Effect.Contains("AER-"))
        {
            Effect = Effect.Replace("AER-", "");
            string count = Effect == "2" ? "Double" : Effect == "3" ? "Triple" : "";
            final = count + " the <color=\"blue\"><b>AE Regen Speed</b></color>";
        }
        else if (Effect.Contains("RMH-"))
        {
            Effect = Effect.Replace("RMH-", "");
            final = "Repair " + Effect + "% <color=\"green\"><b>Max Health</b></color> in total.";
        }
        else if (Effect.Contains("INV"))
        {
            final = "Render the <color=\"black\"><b>Fighter</b></color> invisible. (Cannot be targeted)";
        }
        else if (Effect.Contains("FC"))
        {
            final = "Instantly gain 1 <color=\"green\"><b>Fuel Core</b></color>. Can be purchased even when Fuel Core is full.";
        }
        output.Add("Effect", "Effect: " + final);
        output.Add("Duration", "Duration: " + (int)datas["Duration"] + " seconds.");
        output.Add("Stack", "Max Stack: " + (int)datas["Stack"] + " Per Session.");
        output.Add("Price", ((int)datas["Price"]).ToString());
        output.Add("Cooldown", "Cooldown: " + ((int)datas["Cooldown"]==0? "No cooldown." : 
            (int)datas["Cooldown"]) + " seconds.");
        output.Add("Stock", ((int)datas["StockPerDay"]).ToString());
        return output;
    }

    public Dictionary<string, object> ConvertModelPriceIntoTwoTypePrice(string Price)
    {
        Dictionary<string, object> ModelPrice = new Dictionary<string, object>();
        if (Price != null)
        {
            Price = Price.Replace(" ", "");
            string[] NewPrice = Price.Split("|");
            ModelPrice.Add("Cash", NewPrice[0]);
            ModelPrice.Add("Timeless", NewPrice[1]);
        }
        return ModelPrice;
    }
    #endregion
}
