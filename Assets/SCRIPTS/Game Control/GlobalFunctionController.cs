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
            if (listOH.Length == 3)
            {
                finalString += "<b>Overheat:  </b><i>" + listOH[0] + " | " + listOH[1] + " | " + listOH[2] + "</i>\n";
            }
            else if (listOH.Length == 1)
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
            if (ranges.Length == 1)
            {
                finalString += "<b>Range:  </b><i>" + ranges[0] + "</i>\n";
            } else if (ranges.Length == 2)
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
        Dictionary<string, object> finalString = new Dictionary<string, object>();
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
        if (StatList.Length == 3)
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
                if (DCstats[0] == "0")
                {
                    finalString += "Cooldown: " + DCstats[1] + "s\n";
                } else
                    finalString += "Duration: " + DCstats[0] + "s\nCooldown: " + DCstats[1] + "s\n";
            }
            else
            {
                finalString += StatList[2];
            }
        } else if (StatList.Length == 2)
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
                if (DPH != "null")
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
                if (AoH != "null")
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
                if (AoE != "null")
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
                if (V != "null")
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
                if (R != "null")
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
        Dictionary<string, object> finalString = new Dictionary<string, object>();
        if (StatList.Length == 3)
        {
            if (StatList[0].Contains("BR-"))
            {
                string BR = StatList[0].Replace("BR-", "");
                finalString.Add("BR", BR);
            }
            // BR
            if (StatList[1].Contains("BR-x"))
            {
                string BR = StatList[1].Replace("BR-x", "");
                finalString.Add("BRx", BR);
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

                    finalString.Add("Dur", DCstats[1]);
                }
                else
                {
                    finalString.Add("Dur", DCstats[0]);
                    finalString.Add("CD", DCstats[1]);

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
                finalString.Add("BRx", BR);
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
                    finalString.Add("Dur", DCstats[1]);
                }
                else
                {
                    finalString.Add("Dur", DCstats[0]);
                    finalString.Add("CD", DCstats[1]);

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
                    if (AoH.Contains("/s"))
                    {
                        finalString.Add("AOH", AoH.Replace("/s", ""));
                    } else
                    {
                        finalString.Add("AOH", AoH);
                    }
                }
            }
            else
            {
                finalString.Add("AOH", StatList[1]);
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
                finalString.Add("Dur", DCstats[0]);
                finalString.Add("CD", DCstats[1]);
                

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
            StatsDictionary.Add("ROT", (float.Parse(ROT) * 120).ToString());
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

    //HP-n|SPD-n|ROT-n|AOF-n,n|DM-n|AM-n|PM-n|SP-n|SC-n
    public Dictionary<string, object> ConvertModelStatsToDictionaryForGameplay(string stats)
    {
        Dictionary<string, object> StatsDictionary = new Dictionary<string, object>();
        string[] StatsList = stats.Split("|");
        //HP
        string HP = StatsList[0];
        if (HP.Contains("HP-"))
        {
            HP = HP.Replace("HP-", "");
            StatsDictionary.Add("HP", HP);
        }
        else
        {
            StatsDictionary.Add("HP", "0");
        }
        //SPD
        string SPD = StatsList[1];
        if (SPD.Contains("SPD-"))
        {
            SPD = SPD.Replace("SPD-", "");
            StatsDictionary.Add("SPD", SPD);
        }
        else
        {
            StatsDictionary.Add("SPD", "0");
        }
        //ROT
        string ROT = StatsList[2];
        if (ROT.Contains("ROT-"))
        {
            ROT = ROT.Replace("ROT-", "");
            StatsDictionary.Add("ROT", ROT);
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
            StatsDictionary.Add("AOFNegative", "-" + AOF.Split(",")[0]);
            StatsDictionary.Add("AOFPositive", AOF.Split(",")[1]);
        }
        else
        {
            StatsDictionary.Add("AOFNegative", "-90");
            StatsDictionary.Add("AOFPositive", "90");
        }
        //DM
        string DM = StatsList[4];
        if (DM.Contains("DM-"))
        {
            DM = DM.Replace("DM-", "");
            StatsDictionary.Add("DM", DM);
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
            StatsDictionary.Add("AM", AM);
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
            StatsDictionary.Add("PM", PM);
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
            Effect = Effect.Replace("RED-", "");
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
            final = "Instantly gain 1 <sprite index='2'>. Can't be purchased if Fuel Core is full.";
        }
        return final;
    }

    public Dictionary<string, string> ConvertDictionaryDataToOutputCons(Dictionary<string, object> datas)
    {
        Dictionary<string, string> output = new Dictionary<string, string>();
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
            final = "Render the <b>Fighter</b> invincible. (Cannot be targeted and damaged)";
        }
        else if (Effect.Contains("FC"))
        {
            final = "Instantly gain 1 <sprite index='2'>. Can't be purchased if Fuel Core is full.";
        }
        output.Add("Effect", "Effect: " + final);
        output.Add("Duration", "Duration: " + (Effect.Contains("FC") ? "-" : (int)datas["Duration"] + " seconds."));
        output.Add("Stack", "Max Stack: " + (int)datas["Stack"] + " Per Session.");
        output.Add("Price", ((int)datas["Price"]).ToString());
        output.Add("Cooldown", "Cooldown: " + ((int)datas["Cooldown"] == 0 ? "No cooldown." :
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

    // Format: HP-n,MS-n,RD-n,AWD-n,TWD-n,BD-n,FD-n,PCD-n,R-n,HAZ,BS-n-n,CONS,C-n,WROF,C-xn
    public string ConvertEffectStringToText(string str)
    {
        string result = "";
        if (str.Contains("HP-"))
        {
            result = "Permanently increase max HP by " + str.Replace("HP-","") + "%";
        }
        else if (str.Contains("MS-"))
        {
            result = "Boost the moving speed by " + str.Replace("MS-", "") + "%";
        }
        else if (str.Contains("RD-"))
        {
            result = "-" + str.Replace("RD-", "") + "% Damage received from all weapons types";
        }
        else if (str.Contains("AWD-"))
        {
            result = "Enhance all weapons' damage by " + str.Replace("AWD-", "") + "%";
        }
        else if (str.Contains("TWD-"))
        {
            result = "Thermal weapon deal +" + str.Replace("TWD-","") + "% Damage to enemies with burned or freeze effect";
        }
        else if (str.Contains("BD-"))
        {
            result = "Deal +" + str.Replace("BD-", "") + "% Damage against Barriers";
        }
        else if (str.Contains("FD-"))
        {
            result = "Deal +" + str.Replace("FD-", "") + "% Damage against enemies 1500+ Space Units from you"; 
        }
        else if (str.Contains("PCD-"))
        {
            result = "Reduce the CD of all powers by " + str.Replace("PCD-", "") + "%";
        }
        else if (str.Contains("R-"))
        {
            result = "+" + str.Replace("R-", "") + "% all repairing effects that the Fighter receives";
        }
        else if (str.Equals("HAZ"))
        {
            result = "Your Fighter won’t be affected by any Hazardous Environment effects";
        }
        else if (str.Contains("BS-"))
        {
            result = "Deal +" + str.Replace("BS-", "").Split("-")[0] + "% Damage with Weapons and Powers. Take +" + str.Replace("BS-", "").Split("-")[1] + "% Damage from all sources";
        }
        else if (str.Equals("CONS"))
        {
            result = "Using consumables won’t cost any charges";
        }
        else if (str.Contains("C-") && !str.Contains("C-x"))
        {
            result = "+" + str.Replace("C-", "") + "% Cash received in the upcoming Space Zones";
        }
        else if (str.Contains("WROF-"))
        {
            result = "Enhance all weapons' rate of fire by " + str.Replace("WROF-", "") + "%";
        }
        else if (str.Contains("C-x"))
        {
            result = "Double the cash received by far in the session";
        } 
        else
        {
            result = str;
        }
        return result;
    }

    public List<string> ConvertEffectStringToArrayString(string str)
    {
        List<string> result = new List<string>();
        if (str.Contains("HP-"))
        {
            result.Add("Maximum HP");
        }
        else if (str.Contains("MS-"))
        {
            result.Add("Maximum Moving Speed");
        }
        else if (str.Contains("RD-"))
        {
            result.Add("Weapon Damage Received");
        }
        else if (str.Contains("AWD-"))
        {
            result.Add("Weapon Damage");
        }
        else if (str.Contains("TWD-"))
        {
            result.Add("Thermal Weapon Damage To Thermal Status");
        }
        else if (str.Contains("BD-"))
        {
            result.Add("Damage To Barrier");
        }
        else if (str.Contains("FD-"))
        {
            result.Add("Damage To Far Enemies");
        }
        else if (str.Contains("PCD-"))
        {
            result.Add("Power Cooldown");
        }
        else if (str.Contains("R-"))
        {
            result.Add("All Repairing Effect");
        }
        else if (str.Equals("HAZ"))
        {
            result.Add("Affection Of Hazard Environment");
        }
        else if (str.Contains("BS-"))
        {
            result.Add("Weapon Damage");
            result.Add("Power Damage");
            result.Add("All Damage Received");
        }
        else if (str.Equals("CONS"))
        {
            result.Add("Consumable Costs");
        }
        else if (str.Contains("C-") && !str.Contains("C-x"))
        {
            result.Add("All Cash gain");
        }
        else if (str.Contains("WROF-"))
        {
            result.Add("Weapon Rate Of Fire");
        }
        else
        {
            result.Add(str);
        }
        return result;
    }

    //HP-3000|SPD-700|ROT-0.75|AOF-0 or 90,90|DM-0|AM-0|PM-0
    public Dictionary<string, object> ConvertEnemyStatsToDictionary(string stats)
    {
        Dictionary<string, object> DataDict = new Dictionary<string, object>();
        string[] dataStr = stats.Split("|");
        if (dataStr.Length > 0)
        {
            if (dataStr[0].Contains("HP-"))
            {
                DataDict.Add("HP", dataStr[0].Replace("HP-",""));
            }
            if (dataStr[1].Contains("SPD-"))
            {
                DataDict.Add("SPD", dataStr[1].Replace("SPD-",""));
            }
            if (dataStr[2].Contains("ROT-"))
            {
                DataDict.Add("ROT", dataStr[2].Replace("ROT-", ""));
            }
            if (dataStr[3].Contains("AOF-"))
            {
                string[] AOF = dataStr[3].Replace("AOF-", "").Split(",");
                if (AOF.Length==1)
                {
                    DataDict.Add("AOFNegative", "0");
                    DataDict.Add("AOFPositive", "0");
                }
                else
                {
                    DataDict.Add("AOFNegative", "-" + AOF[0]);
                    DataDict.Add("AOFPositive", AOF[1]);
                }
            }
            if (dataStr[4].Contains("DM-"))
            {
                DataDict.Add("DM", dataStr[4].Replace("DM-", ""));
            }
            if (dataStr[5].Contains("AM-"))
            {
                DataDict.Add("AM", dataStr[5].Replace("AM-", ""));
            }
            if (dataStr[6].Contains("PM-"))
            {
                DataDict.Add("PM", dataStr[6].Replace("PM-", ""));
            }
        }
        return DataDict;
    }
    public Dictionary<string, object> ConvertWarshipStatToDictionary(string stat)
    {
        Dictionary<string, object> WSStat = new Dictionary<string, object>();
        string[] splitstat = stat.Split("|");
        //HP
        string HP = splitstat[0];
        if (HP.Contains("HP-"))
        {
            HP = HP.Replace("HP-", "");
            WSStat.Add("HP", HP);
        }
        else
        {
            WSStat.Add("HP", "0");
        }
        //SPD
        string SPD = splitstat[1];
        if (SPD.Contains("SPD-"))
        {
            SPD = SPD.Replace("SPD-", "");
            WSStat.Add("SPD", SPD);
        }
        else
        {
            WSStat.Add("SPD", "0");
        }
        //ROT
        string ROT = splitstat[2];
        if (ROT.Contains("ROT-"))
        {
            ROT = ROT.Replace("ROT-", "");
            WSStat.Add("ROT", ROT);
        }
        else
        {
            WSStat.Add("ROT", "0");
        }
        return WSStat;
    }
    #endregion
    #region Convert Currency To Icons
    public string ConvertToIcon(string Currency)
    {
        if ("timelessshard".Equals(Currency.Replace(" ", "").ToLower())
            || "shard".Equals(Currency.Replace(" ", "").ToLower()))
        {
            return "<sprite index='0'>";
        }
        else if ("fuelenergy".Equals(Currency.Replace(" ", "").ToLower()))
        {
            return "<sprite index='1'>";
        }
        else if ("fuelcore".Equals(Currency.Replace(" ", "").ToLower())
            || "core".Equals(Currency.Replace(" ", "").ToLower())
            || "fuel".Equals(Currency.Replace(" ", "").ToLower()))
        {
            return "<sprite index='2'>";
        }
        else if ("cash".Equals(Currency.Replace(" ", "").ToLower()))
        {
            return "<sprite index='3'>";
        }
        else return Currency;
    }
    #endregion
    #region Convert Rankup Conditions
    public string ConvertRankUpConditions(string ConditionSZ, string ConditionSZ2, string ConditionSZ2Number)
    {
        string FinalString = "";
        // First condition
        if (ConditionSZ == "0")
        {
            FinalString += "- Acquire a Fighter\n";
        } else
        {
           FinalString += "- Reach Space Zone No." + ConditionSZ + "\n";           
        }
        // Second condition
        if (ConditionSZ2 == "N/A")
        {
            FinalString += "";
        } else
        {
            // Compelete Daily Mission
            if (ConditionSZ2.Contains("C"))
            {
                FinalString += "- Compelete Daily Mission " + ConditionSZ2Number + " times";
            } else
            {
                // Defeat boss x times
                if (ConditionSZ2.Contains("D-"))
                {
                    string boss = "";
                    switch (ConditionSZ2.Split("-")[1])
                    {
                        case "I": boss = "<color=#FF0D11>Tier I Zaturi Fighter</color>"; break;
                        case "II": boss = "<color=#4C9AFF>Tier II Zaturi Fighter</color>"; break;
                        case "WS": boss = "<color=#FF0D11>Zaturi Warship</color>"; break;
                    }
                    FinalString += "- Defeat " + boss + " " + ConditionSZ2Number + " times";
                } else
                {
                    //Permanently acquire x Arsenal's Items
                    if (ConditionSZ2.Contains("PA"))
                    {
                        FinalString += "- Permanently acquire " + ConditionSZ2Number + "+ Arsenal's Items";
                    } else
                    {
                        // O - Own ? Fighters
                        if (ConditionSZ2.Contains("O"))
                        {
                            FinalString += "- Own " + ConditionSZ2Number + " Fighters";
                        }
                    }
                }
            }
        }
        return FinalString;
    }

    #endregion
    #region Convert Achievement
    public Dictionary<string, string> ConvertEnemyDefeated(string stat)
    {
        Dictionary<string, string> EnemyDefeated = new Dictionary<string, string>();
        string[] splitStat = stat.Split("|");

        // EnemyTier I
        string EI = splitStat[0];
        if (EI.Contains("EI-"))
        {
            EI = EI.Replace("EI-", "");
            EnemyDefeated.Add("EnemyTierI", EI);
        }

        // EnemyTier II
        string EII = splitStat[1];
        if (EII.Contains("EII-"))
        {
            EII = EII.Replace("EII-", "");
            EnemyDefeated.Add("EnemyTierII", EII);
        }

        // EnemyTier III
        string EIII = splitStat[2];
        if (EIII.Contains("EIII-"))
        {
            EIII = EIII.Replace("EIII-", "");
            EnemyDefeated.Add("EnemyTierIII", EIII);
        }

        // Warship
        string Ws = splitStat[3];
        if (Ws.Contains("WS-"))
        {
            Ws = Ws.Replace("WS-", "");
            EnemyDefeated.Add("Warship", Ws);
        }

        return EnemyDefeated;
    }
    #endregion
    #region Convert Price Arsenal Service
    public Dictionary<string, string> ConvertPrice(string price)
    {
        Dictionary<string, string> pricedict = new Dictionary<string, string>();
        string[] priceSplit = price.Split("|");
        //Grade III
        string gradeIII = priceSplit[0];
        if (gradeIII.Contains("Grade3"))
        {
            pricedict.Add(gradeIII.Split("-")[0], gradeIII.Split("-")[1]);
        }

        //Grade II
        string gradeII = priceSplit[1];
        if (gradeII.Contains("Grade2"))
        {
            pricedict.Add(gradeII.Split("-")[0], gradeII.Split("-")[1]);
        }

        //Grade I
        string gradeI = priceSplit[2];
        if (gradeI.Contains("Grade1"))
        {
            pricedict.Add(gradeI.Split("-")[0], gradeI.Split("-")[1]);
        }
        return pricedict;
    }
    #endregion
}
