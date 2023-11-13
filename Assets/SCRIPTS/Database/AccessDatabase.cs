using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class AccessDatabase : MonoBehaviour
{
    private IDbConnection dbConnectionData;
    private IDbConnection dbConnectionSave;
    #region Common
    /// <summary>
    /// Get Real name of an item from it no space no capitalize name
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="Type">Weapon/Power/Model/Consumable</param>
    /// <returns></returns>
    public string GetItemRealName(string ItemName, string Type)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        string realName = "";
        if (Type=="Weapon")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
            dbCheckCommand.CommandText = "SELECT WeaponName FROM ArsenalWeapon WHERE replace(replace(lower(WeaponName),' ',''),'-','')='" + ItemName.Replace("-","").Replace(" ","").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        } else if (Type=="Power")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
            dbCheckCommand.CommandText = "SELECT PowerName FROM ArsenalPower WHERE replace(replace(lower(PowerName),' ',''),'-','')='" + ItemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        } else if (Type=="Model")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
            dbCheckCommand.CommandText = "SELECT ModelName FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + ItemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        } else if (Type=="Consumable")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
            dbCheckCommand.CommandText = "SELECT ItemName FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')='" + ItemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        }
        return realName;
    }

    private void OpenConnection()
    {
        dbConnectionData = new SqliteConnection("URI=file:DataTables.db");
        dbConnectionSave = new SqliteConnection("URI=file:SaveTables.db");
    }
    #endregion
    #region Access Player Profile
    public string CreateNewPlayerProfile(string name)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE Name='" + name + "'";
        IDataReader reader = dbCheckCommand.ExecuteReader();
        bool check = true;
        while (reader.Read())
        {
            check = false;
            break;
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "Exist";
        }
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "INSERT INTO PlayerProfile (Name,Rank,CurrentSession,FuelCell,FuelEnergy,Cash,TimelessShard,DailyIncome,DailyIncomeReceived,LastFuelCellUsedTime,CollectedSalaryTime,SupremeWarriorNo,DailyIncomeShard,NewPilotTutorial) " +
            "VALUES ('" + name + "',null,null,10,100,5000,5,500,'N',null,null,0,0, 'Cinematic|MainUEC|Arsenal|Factory|SpaceShop|Loadout|LOTW|Gameplay|SSSum|UECS|ArsenalS|SpaceShopS|Service|SSum|PA')";
        int n = dbCommand.ExecuteNonQuery();
        if (n == 0)
        {
            return "Fail";
        }
        dbConnectionSave.Close();
        return "Success";
    }

    public bool UpdatePlayerProfileRank(int profileId, int rank)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT RankID FROM RankSystem WHERE RankID=" + rank + "";
        bool check = false;
        IDataReader reader = dbCheckCommand.ExecuteReader();
        while (reader.Read())
        {
            check = true;
            break;
        }
        if (!check)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return false;
        }
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "UPDATE PlayerProfile SET Rank=" + rank + " WHERE PlayerId=" + profileId;
        dbCommand.ExecuteNonQuery();
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return true;
    }

    public List<List<string>> GetAllNameRankSessionFromPlayerProfile()
    {
        OpenConnection();
        List<List<string>> result = new List<List<string>>();
        List<string> Names = new List<string>();
        List<string> Ranks = new List<string>();
        List<string> Session = new List<string>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "SELECT Name, Rank, CurrentSession FROM PlayerProfile WHERE 1=1";
        IDataReader dataReader = dbCommand.ExecuteReader();
        while (dataReader.Read())
        {
            Names.Add(dataReader.GetString(0));
            object rankTemp = dataReader.GetValue(1);
            if (rankTemp != null && rankTemp.ToString().Length > 0)
            {
                IDbCommand dbCommand2 = dbConnectionData.CreateCommand();
                dbCommand2.CommandText = "SELECT RankName,TierColor FROM RankSystem WHERE RankID=" + rankTemp;
                IDataReader dataReader2 = dbCommand2.ExecuteReader();
                string rank = "Unranked";
                while (dataReader2.Read())
                {
                    rank = "<color=" + dataReader2.GetString(1) + ">" + dataReader2.GetString(0) + "</color>";
                }
                Ranks.Add(rank);
            } else
            {
                Ranks.Add("Unranked");
            }
            if (!dataReader.IsDBNull(2))
            {
                IDbCommand dbCommand2 = dbConnectionSave.CreateCommand();
                dbCommand2.CommandText = "SELECT CurrentStage FROM Session WHERE SessionID=" + dataReader.GetInt32(2);
                IDataReader dataReader2 = dbCommand2.ExecuteReader();
                while (dataReader2.Read())
                {
                    Session.Add(dataReader2.GetInt32(0).ToString());
                }
            } else
            {
                Session.Add("None");
            }
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        result.Add(Names);
        result.Add(Ranks);
        result.Add(Session);
        return result;
    }

    public string DeletePlayerProfileByName(string name)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PlayerId FROM PlayerProfile WHERE Name='" + name + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            break;
        }
        if (!check)
        {
            return "No Exist";
        }
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "DELETE FROM PlayerProfile WHERE Name='" + name + "'";
        int n = dbCommand.ExecuteNonQuery();
        if (n == 0)
        {
            return "Fail";
        }
        dbConnectionSave.Close();
        return "Success";
    }
    public Dictionary<string, object> GetPlayerInformationById(int PlayerId)
    {
        OpenConnection();
        if (PlayerId == -1)
        {
            return null;
        } else
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            // Open DB
            dbConnectionData.Open();
            dbConnectionSave.Open();
            // Queries
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE PlayerId =" + PlayerId.ToString();
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            bool check = false;
            while (dataReader.Read())
            {
                check = true;
                values.Add("ID", dataReader.GetInt32(0));
                values.Add("Name", dataReader.GetString(1));
                if (dataReader.IsDBNull(2))
                {
                    values.Add("Rank", "Unranked");
                    values.Add("RankId", 0);
                    values.Add("RankColor", "grey");
                } else
                {
                    values.Add("RankId", dataReader.GetInt32(2));
                    IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
                    dbCheckCommand2.CommandText = "SELECT RankName,TierColor FROM RankSystem WHERE RankId=" + dataReader.GetInt32(2);
                    IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
                    string rank = "Unranked";
                    string color = "#ffffff";
                    while (dataReader2.Read())
                    {
                        if (dataReader.GetInt32(2) == 18)
                        {
                            rank = dataReader2.GetString(0).Replace("?", dataReader.GetInt32(12).ToString());
                            color = dataReader2.GetString(1);
                        } else
                        {
                            rank = dataReader2.GetString(0);
                            color = dataReader2.GetString(1);
                        }
                    }
                    values.Add("Rank", rank);
                    values.Add("RankColor", color);
                }
                if (dataReader.IsDBNull(3))
                {
                    values.Add("CurrentSession", -1);
                }
                else
                {
                    values.Add("CurrentSession", dataReader.GetInt32(3));
                }
                values.Add("FuelCell", dataReader.GetInt32(4));
                values.Add("FuelEnergy", dataReader.GetInt32(5));
                values.Add("Cash", dataReader.GetInt32(6));
                values.Add("TimelessShard", dataReader.GetInt32(7));
                values.Add("DailyIncome", dataReader.GetInt32(8));
                values.Add("DailyIncomeReceived", dataReader.GetString(9));
                if (dataReader.IsDBNull(10))
                {
                    values.Add("LastFuelCellUsed", "");
                }
                else
                {
                    values.Add("LastFuelCellUsed", dataReader.GetString(10));
                }
                if (dataReader.IsDBNull(11))
                {
                    values.Add("CollectedSalaryTime", 0);
                }
                else
                {
                    values.Add("CollectedSalaryTime", dataReader.GetString(11));
                }
                values.Add("SupremeWarriorNo", dataReader.GetInt32(12));
                values.Add("DailyIncomeShard", dataReader.GetInt32(13));
                values.Add("NewPilotTutorial", dataReader.GetString(14));

            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            if (!check)
            {
                return null;
            } else
            {
                return values;
            }
        }
    }

    public string AddFuelCell(int PlayerID)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT FuelCell, LastFuelCellUsedTime FROM PlayerProfile WHERE PlayerId =" + PlayerID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cell = -1;
        string lastUsed = "";
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                cell = dataReader.GetInt32(0);
            }
            if (!dataReader.IsDBNull(1))
            {
                lastUsed = dataReader.GetString(1);
            }
        }
        if (cell==-1)
        {
            dbConnectionSave.Close();
            return "No Data";
        } else
        {
            if (cell>=10)
            {
                dbConnectionSave.Close();
                return "Full";
            } else 
            {
                string FinalStr = "";
                if (cell < 10)
                {
                    if (lastUsed!=null && lastUsed!="")
                    {
                        System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
                        System.DateTime LastTime = System.DateTime.ParseExact(lastUsed, "dd/MM/yyyy HH:mm:ss", culture);
                        LastTime = LastTime.AddHours(2);
                        FinalStr = LastTime.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                }
                IDbCommand dbCheck = dbConnectionSave.CreateCommand();
                dbCheck.CommandText = "UPDATE PlayerProfile SET FuelCell = FuelCell + 1, LastFuelCellUsedTime ='" + 
                    (cell == 9 ? "" : FinalStr) + "' WHERE PlayerId =" + PlayerID;
                int n = dbCheck.ExecuteNonQuery();
                dbConnectionSave.Close();
                if (n==1)
                {
                    return "Success";
                } else
                {
                    return "Fail";
                }
            }
        }
    }

    public string ReduceFuelCell(int PlayerID)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT FuelCell FROM PlayerProfile WHERE PlayerId =" + PlayerID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cell = -1;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                cell = dataReader.GetInt32(0);
            }
        }
        if (cell == -1)
        {
            dbConnectionSave.Close();
            return "No Data";
        }
        else
        {
            if (cell <= 0)
            {
                dbConnectionSave.Close();
                return "Full";
            }
            else
            {
                System.DateTime date = System.DateTime.Now;
                string currentDateTime = date.ToString("dd/MM/yyyy HH:mm:ss");
                IDbCommand dbCheck = dbConnectionSave.CreateCommand();
                dbCheck.CommandText = "UPDATE PlayerProfile SET FuelCell = FuelCell - 1" +
                    (cell == 10 ? ", LastFuelCellUsedTime = '" + currentDateTime + "'" : "") + " WHERE PlayerId =" + PlayerID;
                int n = dbCheck.ExecuteNonQuery();
                dbConnectionSave.Close();
                if (n == 1)
                {
                    return "Success";
                }
                else
                {
                    return "Fail";
                }
            }
        }
    }

    public string ClearFuelCell(int PlayerID)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT FuelCell FROM PlayerProfile WHERE PlayerId =" + PlayerID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cell = -1;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                cell = dataReader.GetInt32(0);
            }
        }
        if (cell == -1)
        {
            dbConnectionSave.Close();
            return "No Data";
        }
        else
        {
            if (cell <= 0)
            {
                dbConnectionSave.Close();
                return "Full";
            }
            else
            {
                System.DateTime date = System.DateTime.Now;
                string currentDateTime = date.ToString("dd/MM/yyyy HH:mm:ss");
                IDbCommand dbCheck = dbConnectionSave.CreateCommand();
                dbCheck.CommandText = "UPDATE PlayerProfile SET FuelCell = 0" +
                    (cell == 10 ? ", LastFuelCellUsedTime = '" + currentDateTime + "'" : "") + " WHERE PlayerId =" + PlayerID;
                int n = dbCheck.ExecuteNonQuery();
                dbConnectionSave.Close();
                if (n == 1)
                {
                    return "Success";
                }
                else
                {
                    return "Fail";
                }
            }
        }
    }

    public string ReturnrFuelCell(int PlayerID, int Count)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT FuelCell FROM PlayerProfile WHERE PlayerId =" + PlayerID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cell = -1;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                cell = dataReader.GetInt32(0);
            }
        }
        if (cell == -1)
        {
            dbConnectionSave.Close();
            return "No Data";
        }
        else
        {
            System.DateTime date = System.DateTime.Now;
            string currentDateTime = date.ToString("dd/MM/yyyy HH:mm:ss");
            IDbCommand dbCheck = dbConnectionSave.CreateCommand();
            dbCheck.CommandText = "UPDATE PlayerProfile SET FuelCell =" + Count + " WHERE PlayerId =" + PlayerID;
            int n = dbCheck.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (n == 1)
            {
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }
    }

    public string CheckIfConvertable(int PlayerId, string ConvertFrom, string ConvertTo, string FromAmount, string ToAmount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        string from = "";
        string to = "";
        switch (ConvertFrom.Replace(" ", "").ToLower())
        {
            case "cash": from = "Cash"; break;
            case "timelessshard": from = "TimelessShard"; break;
            case "fuelcell": from = "FuelCell"; break;
            case "fuelenergy": from = "FuelEnergy"; break;
            default: break;
        }
        switch (ConvertTo.Replace(" ", "").ToLower())
        {
            case "cash": to = "Cash"; break;
            case "timelessshard": to = "TimelessShard"; break;
            case "fuelcell": to = "FuelCell"; break;
            case "fuelenergy": to = "FuelEnergy"; break;
            default: break;
        }
        if (from != "" && to != "")
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Name FROM PlayerProfile WHERE PlayerID='" + PlayerId + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            bool check = false;
            while (dataReader.Read())
            {
                check = true;
                break;
            }
            if (!check)
            {
                dbConnectionSave.Close();
                return "No Exist";
            }
            if (to.Equals("FuelCell"))
            {
                IDbCommand dbCommand2 = dbConnectionSave.CreateCommand();
                dbCommand2.CommandText = "SELECT FuelCell FROM PlayerProfile WHERE PlayerID=" + PlayerId;
                IDataReader dataReader3 = dbCommand2.ExecuteReader();
                bool check2 = false;
                int fuelCheck = int.Parse(ToAmount);
                while (dataReader3.Read())
                {
                    check2 = true;
                    fuelCheck += dataReader3.GetInt32(0);
                    break;
                }
                if (!check2)
                {
                    dbConnectionSave.Close();
                    return "Fail";
                }
                else
                {
                    if (fuelCheck > 10)
                    {
                        dbConnectionSave.Close();
                        return "Over Limit";
                    }
                }
            }
            IDbCommand dbCommand3 = dbConnectionSave.CreateCommand();
            dbCommand3.CommandText = "SELECT " + from + " FROM PlayerProfile WHERE PlayerID=" + PlayerId;
            IDataReader dataReader4 = dbCommand3.ExecuteReader();
            int k = -int.Parse(FromAmount);
            bool check3 = false;
            while (dataReader4.Read())
            {
                check3 = true;
                k += dataReader4.GetInt32(0);
                break;
            }
            if (!check3)
            {
                dbConnectionSave.Close();
                return "Fail";
            }
            else
            {
                if (k < 0)
                {
                    dbConnectionSave.Close();
                    return "Not Enough";
                }
            }
            dbConnectionSave.Close();
            return "Success";
        }
        else
        {
            dbConnectionSave.Close();
            return "Fail";
        }
    }
    public string ConvertCurrencyById(int PlayerId, string ConvertFrom, string ConvertTo, string FromAmount, string ToAmount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        string from = "";
        string to = "";
        switch (ConvertFrom.Replace(" ", "").ToLower())
        {
            case "cash": from = "Cash"; break;
            case "timelessshard": from = "TimelessShard"; break;
            case "fuelcell": from = "FuelCell"; break;
            case "fuelenergy": from = "FuelEnergy"; break;
            default: break;
        }
        switch (ConvertTo.Replace(" ", "").ToLower())
        {
            case "cash": to = "Cash"; break;
            case "timelessshard": to = "TimelessShard"; break;
            case "fuelcell": to = "FuelCell"; break;
            case "fuelenergy": to = "FuelEnergy"; break;
            default: break;
        }
        if (from != "" && to != "")
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Name, FuelCell FROM PlayerProfile WHERE PlayerID='" + PlayerId + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            bool check = false;
            int FuelCell = 0;
            while (dataReader.Read())
            {
                check = true;
                FuelCell = dataReader.GetInt32(1);
                break;
            }
            if (!check)
            {
                dbConnectionSave.Close();
                return "No Exist";
            } else
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "UPDATE PlayerProfile SET " + from + " = " + from + " - " + FromAmount +
                    "," + to + " = " + to + " + " + ToAmount + (to == "FuelCell" && FuelCell == 9? ", LastFuelCellUsedTime = ''" : "") +
                    " WHERE PlayerID=" + PlayerId;
                int n = dbCommand.ExecuteNonQuery();
                dbConnectionSave.Close();
                if (n != 1)
                {
                    return "Fail";
                } else
                {
                    // case fuel cell to 10 successful
                    return "Success";
                }
            }
        } else
        {
            dbConnectionSave.Close();
            return "Fail";
        }
    }

    public string RechargeTimelessShard(int PlayerId, int amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Name FROM PlayerProfile WHERE PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            break;
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "No Exist";
        } else
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerProfile SET TimelessShard = TimelessShard + " + amount.ToString() + " WHERE PlayerID=" + PlayerId;
            int n = dbCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (n != 1)
            {
                return "Fail";
            } else
            {
                return "Success";
            }
        }
    }

    public string DecreaseCurrencyAfterBuy(int PlayerId, int Cash, int TimelessShard)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Cash, TimelessShard FROM PlayerProfile WHERE " +
            "PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cashOwn = -1;
        int timelessShardOwn = -1;
        while (dataReader.Read())
        {
            cashOwn = dataReader.GetInt32(0);
            timelessShardOwn = dataReader.GetInt32(1);
        }
        if (cashOwn == -1 || timelessShardOwn == -1)
        {
            dbConnectionSave.Close();
            return "Not Exist";
        }
        if (cashOwn < Cash)
        {
            dbConnectionSave.Close();
            return "Not Enough Cash";
        }
        if (timelessShardOwn < TimelessShard)
        {
            dbConnectionSave.Close();
            return "Not Enough Shard";
        }
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Cash = Cash - " + Cash
            + ", TimelessShard = TimelessShard - " + TimelessShard + " WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n!=1)
        {
            return "Fail";
        } else
        {
            return "Success";
        }
    }

    public string IncreaseCurrencyAfterSell(int PlayerId, int Cash, int TimelessShard)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Cash, TimelessShard FROM PlayerProfile WHERE " +
            "PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cashOwn = -1;
        int timelessShardOwn = -1;
        while (dataReader.Read())
        {
            cashOwn = dataReader.GetInt32(0);
            timelessShardOwn = dataReader.GetInt32(1);
        }
        if (cashOwn == -1 || timelessShardOwn == -1)
        {
            dbConnectionSave.Close();
            return "Not Exist";
        }
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Cash = Cash + " + Cash
            + ", TimelessShard = TimelessShard + " + TimelessShard + " WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }


    public string UpdatePlayerProfileName(int PlayerId, string name)
    {
        OpenConnection();
        int checkID = 0;
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE " +
           "PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            checkID = dataReader.GetInt32(0);
        }
        if (checkID == 0)
        {
            dbConnectionSave.Close();
            return "Not Exist";
        }
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Name = '"+ name +"'" +
            "" +
            " WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public string ResetDailyIncome()
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET DailyIncomeReceived = 'N' WHERE 1 = 1";
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public string CollectSalary(int PlayerId, int Cash, int Shard)
    {
        OpenConnection();
        int checkID = 0;
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE " +
           "PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            checkID = dataReader.GetInt32(0);
        }
        if (checkID == 0)
        {
            dbConnectionSave.Close();
            return "Not Exist";
        }
        System.DateTime date = System.DateTime.Now;
        string collectedDate = date.ToString("dd/MM/yyyy");
        Debug.Log(collectedDate);
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Cash = Cash + " + Cash
            + ", CollectedSalaryTime = '" + collectedDate + "', DailyIncomeReceived = 'Y', TimelessShard = TimelessShard + "+Shard+" WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }

    }

    public string UpdateCash(int PlayerId, int amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Name FROM PlayerProfile WHERE PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            break;
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "No Exist";
        }
        else
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerProfile SET Cash = Cash + " + amount.ToString() + " WHERE PlayerID=" + PlayerId;
            int n = dbCommand.ExecuteNonQuery();
            if (n != 1)
            {
                dbConnectionSave.Close();
                return "Fail";
            }
            else
            {
                dbConnectionSave.Close();
                return "Success";
            }
        }
    }

    public string DecreaseCurrencyAfterBuyForSession(int SessionID, int Cash)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT SessionCash FROM Session WHERE " +
            "SessionID=" + SessionID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int cashOwn = -1;
        while (dataReader.Read())
        {
            cashOwn = dataReader.GetInt32(0);           
        }
        if (cashOwn == -1)
        {
            dbConnectionSave.Close();
            return "Not Exist";
        }
        if (cashOwn < Cash)
        {
            dbConnectionSave.Close();
            return "Not Enough Cash";
        }       
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE Session SET SessionCash = SessionCash - " + Cash
            + " WHERE SessionID=" + SessionID;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public string UpdateFuelEnergy(int PlayerId, int amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Name FROM PlayerProfile WHERE PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            break;
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "No Exist";
        }
        else
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerProfile SET FuelEnergy = FuelEnergy + " + amount + " WHERE PlayerID=" + PlayerId;
            int n = dbCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (n != 1)
            {
                return "Fail";
            }
            else
            {
                return "Success";
            }
        }
    }
    public string UpdateTutorialProgress(int PlayerId, string progress)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Name FROM PlayerProfile WHERE PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            break;
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "No Exist";
        }
        else
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerProfile SET NewPilotTutorial = '"+progress+"' WHERE PlayerID=" + PlayerId;
            int n = dbCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (n != 1)
            {
                return "Fail";
            }
            else
            {
                return "Success";
            }
        }
    }
    #endregion
    #region Access Current Play Session
    public string AddPlaySession(string PlayerName)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PlayerId FROM PlayerProfile WHERE Name='" + PlayerName + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        int id = 0;
        while (dataReader.Read())
        {
            check = true;
            id = dataReader.GetInt32(0);
        }
        if (!check)
        {
            return "No Exist";
        }
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "INSERT INTO CurrentPlaySession (PlayerId,SessionStartTime,SessionEndTime) VALUES" +
            "(" + id.ToString() + ",datetime('now', '+7 hours'),null)";
        int n = dbCommand.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        } else
        {
            return "Success";
        }
    }
    public int GetCurrentSessionPlayerId()
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PlayerId FROM CurrentPlaySession ORDER BY SessionStartTime DESC LIMIT 1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int id = -1;
        while (dataReader.Read())
        {
            id = dataReader.GetInt32(0);
        }
        dbConnectionSave.Close();
        return id;
    }
    public string EndSession(int PlayerID)
    {
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "UPDATE PlayerProfile SET CurrentSession = NULL WHERE PlayerID = " + PlayerID +"";
        int n = dbCheckCommand.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }

    public string InputLoadoutSaveData(int PlayerID, string Model, string LeftWeapon, string RightWeapon, string FirstPower, string SecondPower,
        string Consumables, string ConsumablesCD)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        Dictionary<string, object> Data = new();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT * FROM LoadoutSaveData WHERE PlayerID=" + PlayerID;
        IDataReader reader = dbCheckCommand1.ExecuteReader();
        while (reader.Read())
        {
            Data.Add("ID", reader.GetInt32(0));
            Data.Add("PlayerID", reader.GetInt32(1));
            Data.Add("Model", reader.GetString(2));
            Data.Add("LeftWeapon", reader.GetString(3));
            Data.Add("RightWeapon", reader.GetString(4));
            Data.Add("FirstPower", reader.GetString(5));
            Data.Add("SecondPower", reader.GetString(6));
            Data.Add("Consumables", reader.GetString(7));
        }
        if (Data.ContainsKey("ID"))
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "UPDATE LoadoutSaveData SET " +
                "Model='" + Model + "'" +
                ", LeftWeapon='" + LeftWeapon + "'" +
                ", RightWeapon='" + RightWeapon + "'" +
                ", FirstPower='" + FirstPower + "'" +
                ", SecondPower='" + SecondPower + "'" +
                ", Consumables='" + Consumables + "'" +
                " WHERE LoadoutSaveDataID =" + (int)Data["ID"];
            int n = dbCheckCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (n != 1)
            {
                return "Fail";
            }
            else
            {
                return "Success";
            }
        } else
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "INSERT INTO LoadoutSaveData (PlayerID,Model,LeftWeapon,RightWeapon,FirstPower,SecondPower,Consumables, ConsumablesCD) " +
                "VALUES (" + PlayerID + ",'" + Model + "','" + LeftWeapon + "','" + RightWeapon + "','" + FirstPower + "','" + SecondPower + "','" + Consumables + "', '"+ ConsumablesCD + "')";
            int n = dbCheckCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (n != 1)
            {
                return "Fail";
            }
            else
            {
                return "Success";
            }
        }
    }

    public Dictionary<string, object> GetLoadoutSaveData(int PlayerID)
    {
        OpenConnection();
        Dictionary<string, object> Data = new();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM LoadoutSaveData WHERE PlayerID=" + PlayerID;
        IDataReader reader = dbCheckCommand.ExecuteReader();
        while (reader.Read())
        {
            Data.Add("ID", reader.GetInt32(0));
            Data.Add("PlayerID", reader.GetInt32(1));
            Data.Add("Model", reader.GetString(2));
            Data.Add("LeftWeapon", reader.GetString(3));
            Data.Add("RightWeapon", reader.GetString(4));
            Data.Add("FirstPower", reader.GetString(5));
            Data.Add("SecondPower", reader.GetString(6));
            Data.Add("Consumables", reader.GetString(7));
        }
        dbConnectionSave.Close();
        return Data;
    }
    #endregion
    #region Access Daily Mission
    public int NumberOfDailyMissionById(int PlayerId)
    {
        OpenConnection();
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT COUNT(*) FROM PlayerDailyMission WHERE PlayerID =" + PlayerId + " AND MissionDate='" + currentDate + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int count = 0;
        while (dataReader.Read())
        {
            count = dataReader.GetInt32(0);
        }
        dbConnectionSave.Close();
        return count;
    }
    public string GenerateDailyMission(int PlayerId, int number)
    {
        OpenConnection();
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT COUNT(*) FROM DailyMissions";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int numberOfDailyMissions = 0;
        while (dataReader.Read())
        {
            numberOfDailyMissions = dataReader.GetInt32(0);
        }
        if (numberOfDailyMissions == 0)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Fail";
        }
        IDbCommand dbCommand2 = dbConnectionSave.CreateCommand();
        dbCommand2.CommandText = "SELECT MissionID FROM PlayerDailyMission WHERE PlayerId=" + PlayerId;
        List<int> alreadyMission = new List<int>();
        while (dataReader.Read())
        {
            alreadyMission.Add(dataReader.GetInt32(0));
        }
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        string insertText = "INSERT INTO PlayerDailyMission (PlayerID,MissionID,IsComplete,MissionDate, MissionProgess) VALUES ";
        for (int i = 0; i < number; i++)
        {
            if (alreadyMission.Count > 0)
            {
                int n = 0;
                do
                {
                    n = Random.Range(1, numberOfDailyMissions + 1);
                } while (alreadyMission.Contains(n));
                insertText += "(" + PlayerId + "," + n + ",'N','" + currentDate + "', 0)";
                alreadyMission.Add(n);
            } else
            {
                int n = Random.Range(1, numberOfDailyMissions + 1);
                insertText += "(" + PlayerId + "," + n + ",'N','" + currentDate + "', 0)";
                alreadyMission.Add(n);
            }
            if (i == number - 1)
            {
                insertText += ";";
            } else
            {
                insertText += ",";
            }
        }
        dbCommand.CommandText = insertText;
        int check = dbCommand.ExecuteNonQuery();
        dbConnectionData.Close();
        dbConnectionSave.Close();
        if (check == number)
        {
            return "Success";
        } else
        {
            return "Fail";
        }
    }

    public List<List<string>> GetListDailyMissionUndone(int PlayerId)
    {
        OpenConnection();
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        List<List<string>> dailyMissions = new List<List<string>>();
        List<string> DM;
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT MissionId, MissionProgess FROM PlayerDailyMission WHERE PlayerId=" + PlayerId + " AND MissionDate='" + currentDate + "' AND IsComplete='N'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check1 = false;
        while (dataReader.Read())
        {
            DM = new List<string>();
            check1 = true;
            DM.Add(dataReader.GetInt32(1).ToString());
            IDbCommand dbCommand = dbConnectionData.CreateCommand();
            dbCommand.CommandText = "SELECT MissionVerb, MissionNumber FROM DailyMissions WHERE MissionId=" + dataReader.GetInt32(0);
            IDataReader dataReader2 = dbCommand.ExecuteReader();
            bool check2 = false;
            while (dataReader2.Read())
            {
                check2 = true;
                DM.Add(dataReader2.GetString(0));
                DM.Add(dataReader2.GetInt32(1).ToString());
               
            }
            dailyMissions.Add(DM);
            if (!check2) return null;
        }
        if (!check1) return null;
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return dailyMissions;
    }
    public void UpdateDailyMissionProgess(int PlayerId, string mission, int amount)   
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();

        IDbCommand dbCommand = dbConnectionData.CreateCommand();
        dbCommand.CommandText = "SELECT MissionId FROM DailyMissions WHERE MissionVerb='" + mission + "'";
        IDataReader dataReader2 = dbCommand.ExecuteReader();
        int id = 0;
        while (dataReader2.Read())
        {
            id = dataReader2.GetInt32(0);
        }      

        string query = "UPDATE PlayerDailyMission SET MissionProgess = MissionProgess + "+ amount + " WHERE PlayerId=" + PlayerId + " AND MissionId = " + id + "";
        IDbCommand dbCommand2 = dbConnectionSave.CreateCommand();
        dbCommand2.CommandText = query;
        dbCommand2.ExecuteNonQuery();
        dbConnectionData.Close();
        dbConnectionSave.Close();
    }
    public void DailyMissionDone(int PlayerID, string mission)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();

        IDbCommand dbCommand = dbConnectionData.CreateCommand();
        dbCommand.CommandText = "SELECT MissionId FROM DailyMissions WHERE MissionVerb='" + mission + "'";
        IDataReader dataReader2 = dbCommand.ExecuteReader();
        int id = 0;
        while (dataReader2.Read())
        {
            id = dataReader2.GetInt32(0);
        }

        string query = "UPDATE PlayerDailyMission SET IsComplete = 'Y' WHERE PlayerId=" + PlayerID + " AND MissionId = " + id + "";
        IDbCommand dbCommand2 = dbConnectionSave.CreateCommand();
        dbCommand2.CommandText = query;
        dbCommand2.ExecuteNonQuery();

        //Update statistic
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "UPDATE Statistic SET TotalDailyMissionDone = TotalDailyMissionDone + 1 WHERE PlayerID = " + PlayerID + "";
        dbCheckCommand1.ExecuteNonQuery();

        //Reward
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET FuelEnergy = FuelEnergy + 100, TimelessShard = TimelessShard + 5, Cash = Cash + 2500 WHERE PlayerID = " + PlayerID + "";
        dbCheckCommand2.ExecuteNonQuery();
        dbConnectionData.Close();
        dbConnectionSave.Close();
        
    }

    public List<List<string>> GetListDailyMission(int PlayerId)
    {
        OpenConnection();
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        List<List<string>> dailyMissions = new List<List<string>>();
        dailyMissions.Add(new List<string>());
        dailyMissions.Add(new List<string>());
        dailyMissions.Add(new List<string>());
        dailyMissions.Add(new List<string>());
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT MissionId, MissionProgess, IsComplete FROM PlayerDailyMission WHERE PlayerId=" + PlayerId + " AND MissionDate='" + currentDate + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check1 = false;
        while (dataReader.Read())
        {
            check1 = true;
            dailyMissions[2].Add(dataReader.GetInt32(1).ToString());
            dailyMissions[3].Add(dataReader.GetString(2));
            IDbCommand dbCommand = dbConnectionData.CreateCommand();
            dbCommand.CommandText = "SELECT MissionVerb, MissionNumber FROM DailyMissions WHERE MissionId=" + dataReader.GetInt32(0);
            IDataReader dataReader2 = dbCommand.ExecuteReader();
            bool check2 = false;
            while (dataReader2.Read())
            {
                check2 = true;
                dailyMissions[0].Add(dataReader2.GetString(0));
                dailyMissions[1].Add(dataReader2.GetInt32(1).ToString());

            }
            if (!check2) return null;
        }
        if (!check1) return null;
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return dailyMissions;
    }
    #endregion
    #region Access Option
    public Dictionary<string, object> GetOption()
    {
        OpenConnection();
        Dictionary<string, object> option = new Dictionary<string, object>();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM Option";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        while (dataReader.Read())
        {
            option.Add("MVolume", dataReader.GetInt32(0));
            option.Add("MuVolume", dataReader.GetInt32(1));
            option.Add("Sfx", dataReader.GetInt32(2));
            option.Add("Fps", dataReader.GetInt32(3));
            option.Add("Resol", dataReader.GetString(4));
        }
        dbConnectionSave.Close();
        return option;
    }
    public void UpdateOptionSetting(int MVolume, int Music, int Sound, int Fps, string Resol)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "UPDATE Option SET " +
            "MasterVolume = " + MVolume + ", " +
            "MusicVolume = " + Music + ", " +
            "SoundFx = " + Sound + ", " +
            "Fps= " + Fps + ", " +
            "Resolution = '" + Resol + "' Where 1=1";

        dbCheckCommand.ExecuteNonQuery();
        dbConnectionSave.Close();
    }
    #endregion
    #region Access Arsenal Weapon
    public List<List<string>> GetAllArsenalWeapon()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> weaplist;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM ArsenalWeapon";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        while (dataReader.Read())
        {
            weaplist = new List<string>();
            weaplist.Add(dataReader.GetInt32(0).ToString());
            weaplist.Add(dataReader.GetString(1));
            weaplist.Add(dataReader.GetString(2));
            weaplist.Add(dataReader.GetString(3));
            weaplist.Add(dataReader.GetString(4));
            weaplist.Add(dataReader.GetString(5));
            if (dataReader.IsDBNull(6))
            {
                weaplist.Add("N.A");
            }
            else
            {
                weaplist.Add(dataReader.GetString(6));
            }
            if (dataReader.IsDBNull(7))
            {
                weaplist.Add("N.A");
            } else
            {
                weaplist.Add(dataReader.GetInt32(7).ToString());
            }
            weaplist.Add(dataReader.GetInt32(8).ToString());
            weaplist.Add(dataReader.GetString(9));
            weaplist.Add(dataReader.GetString(10));
            weaplist.Add(dataReader.GetString(11));
            list.Add(weaplist);
        }
        dbConnectionData.Close();
        return list;
    }

    public string CheckWeaponPowerPrereq(int PlayerID, string ItemName, string Type)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        if (Type == "Weapon")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
            dbCheckCommand.CommandText = "SELECT PrereqWeapon FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')=='" + ItemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int n = -1;
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                {
                    n = dataReader.GetInt32(0);
                }
            }
            if (n == -1)
            {
                dbConnectionData.Close();
                return "No Prereq";
            }
            else
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE ItemID=" + n + " AND ItemType='Weapon' AND PlayerID=" + PlayerID;
                IDataReader dataReader2 = dbCommand.ExecuteReader();
                int k = 0;
                while (dataReader2.Read())
                {
                    if (!dataReader2.IsDBNull(0))
                    {
                        k = dataReader2.GetInt32(0);
                    }
                }
                if (k > 0)
                {
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return "Pass";
                }
                else
                {
                    IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
                    dbCheckCommand2.CommandText = "SELECT WeaponName, TierColor FROM ArsenalWeapon WHERE WeaponID=" + n;
                    IDataReader dataReader3 = dbCheckCommand2.ExecuteReader();
                    string name = "";
                    while (dataReader3.Read())
                    {
                        name = "<color=" + dataReader3.GetString(1) + ">" + dataReader3.GetString(0) + "</color>";
                    }
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return name;
                }
            }
        }
        else if (Type == "Power")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
            dbCheckCommand.CommandText = "SELECT PrereqItem FROM ArsenalPower WHERE replace(lower(PowerName),' ','')=='" + ItemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int n = -1;
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                {
                    n = dataReader.GetInt32(0);
                }
            }
            if (n == -1)
            {
                dbConnectionData.Close();
                dbConnectionSave.Close();
                return "No Prereq";
            }
            else
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE ItemID=" + n + " AND ItemType='Power' AND PlayerID=" + PlayerID;
                IDataReader dataReader2 = dbCommand.ExecuteReader();
                int k = 0;
                while (dataReader2.Read())
                {
                    if (!dataReader2.IsDBNull(0))
                    {
                        k = dataReader2.GetInt32(0);
                    }
                }
                if (k > 0)
                {
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return "Pass";
                }
                else
                {
                    IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
                    dbCheckCommand2.CommandText = "SELECT PowerName, TierColor FROM ArsenalPower WHERE PowerID=" + n;
                    IDataReader dataReader3 = dbCheckCommand2.ExecuteReader();
                    string name = "";
                    while (dataReader3.Read())
                    {
                        name = "<color=" + dataReader3.GetString(1) + ">" + dataReader3.GetString(0) + "</color>";
                    }
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return name;
                }
            }
        }
        else
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Unidentified";
        }
    }

    public List<string> GetAllWeaponName()
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName FROM ArsenalWeapon WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        return list;
    }

    public List<string> GetAllOwnedWeapon(int PlayerID)
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand command = dbConnectionSave.CreateCommand();
        command.CommandText = "SELECT file FROM pragma_database_list;";
        IDataReader reader = command.ExecuteReader();
        string dir = "";
        while (reader.Read())
        {
            dir = reader.GetString(0);
        }
        dir = dir.Replace("SaveTables.db", "DataTables.db");
        IDbCommand command2 = dbConnectionSave.CreateCommand();
        command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
        int n = command2.ExecuteNonQuery();
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT DataTables.ArsenalWeapon.WeaponName FROM DataTables.ArsenalWeapon " +
            "inner join PlayerOwnership WHERE PlayerOwnership.PlayerID=" + PlayerID + " AND PlayerOwnership.ItemType='Weapon' " +
            "AND DataTables.ArsenalWeapon.WeaponID = PlayerOwnership.ItemID ORDER BY DataTables.ArsenalWeapon.WeaponID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return list;
    }

    public string GetWeaponRealName(string WeaponName)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName, TierColor FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + WeaponName.Replace(" ","").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        string str = "";
        while (dataReader.Read())
        {
            str = "<color=" + dataReader.GetString(1) + ">" + dataReader.GetString(0) + "</color>";
        }
        dbConnectionData.Close();
        return str;
    }
    
    public List<string> GetOwnedWeaponExceptForName(int PlayerID, string WeaponName)
    {
        OpenConnection();
        Dictionary<string, int> DictOwned = new Dictionary<string, int>();
        Dictionary<string, string> NameConversion = new Dictionary<string, string>();
        string checkName = WeaponName.Replace(" ", "").ToLower();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();

        // Queries
        IDbCommand command = dbConnectionSave.CreateCommand();
        command.CommandText = "SELECT file FROM pragma_database_list;";
        IDataReader reader = command.ExecuteReader();
        string dir = "";
        while (reader.Read())
        {
            dir = reader.GetString(0);
        }
        dir = dir.Replace("SaveTables.db", "DataTables.db");
        IDbCommand command2 = dbConnectionSave.CreateCommand();
        command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
        int n = command2.ExecuteNonQuery();
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT DataTables.ArsenalWeapon.WeaponName, PlayerOwnership.Quantity FROM DataTables.ArsenalWeapon " +
            "inner join PlayerOwnership WHERE  PlayerOwnership.PlayerID=" + PlayerID + " AND  PlayerOwnership.ItemType='Weapon'" +
            " AND DataTables.ArsenalWeapon.WeaponID = PlayerOwnership.ItemID ORDER BY DataTables.ArsenalWeapon.WeaponID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            DictOwned.Add(dataReader.GetString(0), dataReader.GetInt32(1));
            NameConversion.Add(dataReader.GetString(0).Replace(" ", "").ToLower(), dataReader.GetString(0));
        }
        if (NameConversion.ContainsKey(checkName))
        {
            DictOwned[NameConversion[checkName]] = DictOwned[NameConversion[checkName]] - 1;
            if (DictOwned[NameConversion[checkName]] ==0)
            {
                DictOwned.Remove(NameConversion[checkName]);
            }
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return new List<string>(DictOwned.Keys);
    }
    
    public List<string> GetSessionOwnedWeaponExceptForName(int PlayerID, string WeaponName)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader1 = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader1.Read())
        {
            if (!dataReader1.IsDBNull(0))
            {
                check = true;
                n = dataReader1.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return null;
        }
        else
        {
            Dictionary<string, int> DictOwned = new Dictionary<string, int>();
            Dictionary<string, string> NameConversion = new Dictionary<string, string>();
            string checkName = WeaponName.Replace(" ", "").ToLower();
            // Queries
            IDbCommand command = dbConnectionSave.CreateCommand();
            command.CommandText = "SELECT file FROM pragma_database_list;";
            IDataReader reader = command.ExecuteReader();
            string dir = "";
            while (reader.Read())
            {
                dir = reader.GetString(0);
            }
            dir = dir.Replace("SaveTables.db", "DataTables.db");
            IDbCommand command2 = dbConnectionSave.CreateCommand();
            command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
            int sadasdasad = command2.ExecuteNonQuery();
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT DataTables.ArsenalWeapon.WeaponName, SessionOwnership.Quantity FROM DataTables.ArsenalWeapon " +
                "inner join SessionOwnership WHERE SessionOwnership.SessionID=" + n + " AND SessionOwnership.ItemType='Weapon'" +
                " AND DataTables.ArsenalWeapon.WeaponID = SessionOwnership.ItemID ORDER BY DataTables.ArsenalWeapon.WeaponID ASC";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            while (dataReader.Read())
            {
                DictOwned.Add(dataReader.GetString(0), dataReader.GetInt32(1));
                NameConversion.Add(dataReader.GetString(0).Replace(" ", "").ToLower(), dataReader.GetString(0));
            }
            if (NameConversion.ContainsKey(checkName))
            {
                DictOwned[NameConversion[checkName]] = DictOwned[NameConversion[checkName]] - 1;
                if (DictOwned[NameConversion[checkName]] == 0)
                {
                    DictOwned.Remove(NameConversion[checkName]);
                }
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return new List<string>(DictOwned.Keys);
        }
    }
    public string AddStarterGiftWeapons(int PlayerID)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE PlayerID=" + PlayerID + " AND ItemType='Weapon' AND ItemID=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int n = 0;
        while (dataReader.Read())
        {
            if (dataReader.IsDBNull(0))
            {
                n = dataReader.GetInt32(0);
            }
        }
        if (n==0)
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "INSERT INTO PlayerOwnership (PlayerID,ItemType,ItemID,Quantity) VALUES " +
                "(" + PlayerID + ",'Weapon',1,2)";
            int check = dbCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (check==1)
            {
                return "2";
            } else
            {
                return "Fail";
            }
        } else if (n==1)
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerOwnership SET Quantity = 2 WHERE " +
                "PlayerId=" + PlayerID + " AND ItemID=1 AND ItemType='Weapon'";
            int check = dbCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (check==1)
            {
                return "1";
            } else
            {
                return "Fail";
            }
        } else
        return "Fail";
    }
    public Dictionary<string, object> GetWeaponDataByName(string name)
    {
        OpenConnection();
        Dictionary<string, object> list = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName,WeaponType,WeaponDescription,WeaponStats,TierColor,PrereqWeapon FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            list.Add("Name", dataReader.GetString(0));
            list.Add("Type", dataReader.GetString(1));
            list.Add("Description", dataReader.GetString(2));
            list.Add("Stats", dataReader.GetString(3));
            list.Add("Color", dataReader.GetString(4));
            if (dataReader.IsDBNull(5))
            {
                list.Add("Prereq", -1);
            } else
            {
                list.Add("Prereq", dataReader.GetInt32(5));
            }
            break;
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        } else
        {
            return list;
        }
    }

    public Dictionary<string, object> GetWeaponDataByID(int ID)
    {
        OpenConnection();
        Dictionary<string, object> list = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName,WeaponType,WeaponDescription,WeaponStats,TierColor,PrereqWeapon FROM ArsenalWeapon WHERE WeaponID==" + ID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            list.Add("Name", dataReader.GetString(0));
            list.Add("Type", dataReader.GetString(1));
            list.Add("Description", dataReader.GetString(2));
            list.Add("Stats", dataReader.GetString(3));
            list.Add("Color", dataReader.GetString(4));
            if (dataReader.IsDBNull(5))
            {
                list.Add("Prereq", -1);
            }
            else
            {
                list.Add("Prereq", dataReader.GetInt32(5));
            }
            break;
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
        {
            return list;
        }
    }

    public string CheckWeaponPowerPrereqForSession(int PlayerID, string ItemName, string Type)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        IDbCommand dbCheckCommand9 = dbConnectionSave.CreateCommand();
        dbCheckCommand9.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE " +
            "PlayerID=" + PlayerID;
        IDataReader dataReader9 = dbCheckCommand9.ExecuteReader();
        int SessionID = 0;
        while (dataReader9.Read())
        {
            SessionID = dataReader9.GetInt32(0);
        }
        if (SessionID > 0)
        {
            if (Type == "Weapon")
            {
                // Queries
                IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
                dbCheckCommand.CommandText = "SELECT PrereqWeapon FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')=='" + ItemName.Replace(" ", "").ToLower() + "'";
                IDataReader dataReader = dbCheckCommand.ExecuteReader();
                int n = -1;
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0))
                    {
                        n = dataReader.GetInt32(0);
                    }
                }
                if (n == -1)
                {
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return "No Prereq";
                }
                else
                {
                    IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                    dbCommand.CommandText = "SELECT Quantity FROM SessionOwnership WHERE ItemID=" + n + " AND ItemType='Weapon' AND SessionID=" + SessionID;
                    IDataReader dataReader2 = dbCommand.ExecuteReader();
                    int k = 0;
                    while (dataReader2.Read())
                    {
                        if (!dataReader2.IsDBNull(0))
                        {
                            k = dataReader2.GetInt32(0);
                        }
                    }
                    if (k > 0)
                    {
                        dbConnectionData.Close();
                        dbConnectionSave.Close();
                        return "Pass";
                    }
                    else
                    {
                        IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
                        dbCheckCommand2.CommandText = "SELECT WeaponName, TierColor FROM ArsenalWeapon WHERE WeaponID=" + n;
                        IDataReader dataReader3 = dbCheckCommand2.ExecuteReader();
                        string name = "";
                        while (dataReader3.Read())
                        {
                            name = "<color=" + dataReader3.GetString(1) + ">" + dataReader3.GetString(0) + "</color>";
                        }
                        dbConnectionData.Close();
                        dbConnectionSave.Close();
                        return name;
                    }
                }
            }
            else if (Type == "Power")
            {
                // Queries
                IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
                dbCheckCommand.CommandText = "SELECT PrereqItem FROM ArsenalPower WHERE replace(lower(PowerName),' ','')=='" + ItemName.Replace(" ", "").ToLower() + "'";
                IDataReader dataReader = dbCheckCommand.ExecuteReader();
                int n = -1;
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0))
                    {
                        n = dataReader.GetInt32(0);
                    }
                }
                if (n == -1)
                {
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return "No Prereq";
                }
                else
                {
                    IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                    dbCommand.CommandText = "SELECT Quantity FROM SessionOwnership WHERE ItemID=" + n + " AND ItemType='Power' AND SessionID=" + SessionID;
                    IDataReader dataReader2 = dbCommand.ExecuteReader();
                    int k = 0;
                    while (dataReader2.Read())
                    {
                        if (!dataReader2.IsDBNull(0))
                        {
                            k = dataReader2.GetInt32(0);
                        }
                    }
                    if (k > 0)
                    {
                        dbConnectionData.Close();
                        dbConnectionData.Close();
                        return "Pass";
                    }
                    else
                    {
                        IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
                        dbCheckCommand2.CommandText = "SELECT PowerName, TierColor FROM ArsenalPower WHERE PowerID=" + n;
                        IDataReader dataReader3 = dbCheckCommand2.ExecuteReader();
                        string name = "";
                        while (dataReader3.Read())
                        {
                            name = "<color=" + dataReader3.GetString(1) + ">" + dataReader3.GetString(0) + "</color>";
                        }
                        dbConnectionData.Close();
                        dbConnectionSave.Close();
                        return name;
                    }
                }
            }
            else
            {
                dbConnectionData.Close();
                dbConnectionSave.Close();
                return "Unidentified";
            }
        }
        else
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Unidentified";
        }

    }
    #endregion
    #region Access Factory Fighter/Model
    public List<List<string>> GetAllFighter()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> fighterlist;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM FactoryModel";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        while (dataReader.Read())
        {
            fighterlist = new List<string>();
            fighterlist.Add(dataReader.GetInt32(0).ToString());
            fighterlist.Add(dataReader.GetString(1));
            fighterlist.Add(dataReader.GetString(2));
            fighterlist.Add(dataReader.GetString(3));
            fighterlist.Add(dataReader.GetString(4));
            if (dataReader.IsDBNull(5))
            {
                fighterlist.Add("N/A");
            }
            else
            {
                fighterlist.Add(dataReader.GetInt32(5).ToString());
            }
            fighterlist.Add(dataReader.GetString(6));
            fighterlist.Add(dataReader.GetString(7));
            list.Add(fighterlist);
        }
        dbConnectionData.Close();
        return list;
    }
    public List<string> GetAllModelName()
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ModelName FROM FactoryModel WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        return list;
    }

    public List<string> GetAllOwnedModel(int PlayerID)
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand command = dbConnectionSave.CreateCommand();
        command.CommandText = "SELECT file FROM pragma_database_list;";
        IDataReader reader = command.ExecuteReader();
        string dir = "";
        while (reader.Read())
        {
            dir = reader.GetString(0);
        }
        dir = dir.Replace("SaveTables.db", "DataTables.db");
        IDbCommand command2 = dbConnectionSave.CreateCommand();
        command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
        int sadasdasad = command2.ExecuteNonQuery();
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT DataTables.FactoryModel.ModelName FROM DataTables.FactoryModel " +
            "inner join PlayerOwnership WHERE PlayerOwnership.PlayerID=" + PlayerID + " AND PlayerOwnership.ItemType='Model' " +
            "AND DataTables.FactoryModel.ModelID = PlayerOwnership.ItemID ORDER BY DataTables.FactoryModel.ModelID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return list;
    }
    public string GetFighterStatsByName(string name)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ModelStats FROM FactoryModel WHERE replace(lower(ModelName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        string stats = "";
        while (dataReader.Read())
        {
            check = true;
            stats = dataReader.GetString(0);
        }
        dbConnectionData.Close();
        if (!check)
        {
            return "Fail";
        } else
        {
            return stats;
        }
    }

    public string GetFighterTierByName(string name)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT TierColor FROM FactoryModel WHERE replace(lower(ModelName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        string stats = "";
        while (dataReader.Read())
        {
            check = true;
            stats = dataReader.GetString(0);
        }
        dbConnectionData.Close();
        if (!check)
        {
            return "Fail";
        }
        else
        {
            return stats;
        }
    }
    #endregion
    #region Access Power
    public List<List<string>> GetAllPower()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> weaplist;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM ArsenalPower";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        while (dataReader.Read())
        {
            weaplist = new List<string>();
            weaplist.Add(dataReader.GetInt32(0).ToString());
            weaplist.Add(dataReader.GetString(1));
            weaplist.Add(dataReader.GetString(2));
            weaplist.Add(dataReader.GetString(3));
            weaplist.Add(dataReader.GetString(4));
            weaplist.Add(dataReader.GetString(5));
            if (dataReader.IsDBNull(6))
            {
                weaplist.Add("N.A");
            }
            else
            {
                weaplist.Add(dataReader.GetString(6));
            }
            if (dataReader.IsDBNull(7))
            {
                weaplist.Add("N.A");
            }
            else
            {
                weaplist.Add(dataReader.GetInt32(7).ToString());
            }
            weaplist.Add(dataReader.GetInt32(8).ToString());
            weaplist.Add(dataReader.GetString(9));
            weaplist.Add(dataReader.GetString(10));
            list.Add(weaplist);
        }
        dbConnectionData.Close();
        return list;
    }

    public List<string> GetAllPowerName()
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PowerName FROM ArsenalPower WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        return list;
    }

    public List<string> GetAllOwnedPower(int PlayerID)
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand command = dbConnectionSave.CreateCommand();
        command.CommandText = "SELECT file FROM pragma_database_list;";
        IDataReader reader = command.ExecuteReader();
        string dir = "";
        while (reader.Read())
        {
            dir = reader.GetString(0);
        }
        dir = dir.Replace("SaveTables.db", "DataTables.db");
        IDbCommand command2 = dbConnectionSave.CreateCommand();
        command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
        int sadasdasad = command2.ExecuteNonQuery();
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT DataTables.ArsenalPower.PowerName FROM DataTables.ArsenalPower " +
            "inner join PlayerOwnership WHERE PlayerOwnership.PlayerID=" + PlayerID + " AND PlayerOwnership.ItemType='Power' " +
            "AND DataTables.ArsenalPower.PowerID = PlayerOwnership.ItemID ORDER BY DataTables.ArsenalPower.PowerID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return list;
    }

    public List<string> GetAllOwnedPowerExceptForName(int PlayerID, string Name)
    {
        OpenConnection();
        List<string> list = new List<string>();
        string check = "";
        if (Name.Replace(" ", "").ToLower().Contains("barrier"))
        {
            check = "barrier";
        } 
        else if (Name.Replace(" ", "").ToLower().Contains("wormhole"))
        {
            check = "wormhole";
        }
        else if (Name.Replace(" ", "").ToLower().Contains("laser"))
        {
            check = "laser";
        }
        else if (Name.Replace(" ", "").ToLower().Contains("rocket"))
        {
            check = "rocket";
        }
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand command = dbConnectionSave.CreateCommand();
        command.CommandText = "SELECT file FROM pragma_database_list;";
        IDataReader reader = command.ExecuteReader();
        string dir = "";
        while (reader.Read())
        {
            dir = reader.GetString(0);
        }
        dir = dir.Replace("SaveTables.db", "DataTables.db");
        IDbCommand command2 = dbConnectionSave.CreateCommand();
        command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
        int sadasdasad = command2.ExecuteNonQuery();
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT DataTables.ArsenalPower.PowerName FROM DataTables.ArsenalPower " +
            "inner join PlayerOwnership WHERE PlayerOwnership.PlayerID=" + PlayerID + " AND PlayerOwnership.ItemType='Power' " +
            "AND DataTables.ArsenalPower.PowerID = PlayerOwnership.ItemID ORDER BY DataTables.ArsenalPower.PowerID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            if (check!="") 
            {
                if (!dataReader.GetString(0).Replace(" ", "").ToLower().Contains(check))
                {
                    list.Add(dataReader.GetString(0).Replace(" ", ""));
                }
            } else
            {
                list.Add(dataReader.GetString(0).Replace(" ", ""));
            }
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return list;
    }

    public List<string> GetSessionAllOwnedPowerExceptForName(int PlayerID, string Name)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader1 = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check1 = false;
        while (dataReader1.Read())
        {
            if (!dataReader1.IsDBNull(0))
            {
                check1 = true;
                n = dataReader1.GetInt32(0);
            }
        }
        if (!check1)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return null;
        }
        else
        {
            List<string> list = new List<string>();
            string check = "";
            if (Name.Replace(" ", "").ToLower().Contains("barrier"))
            {
                check = "barrier";
            }
            else if (Name.Replace(" ", "").ToLower().Contains("wormhole"))
            {
                check = "wormhole";
            }
            else if (Name.Replace(" ", "").ToLower().Contains("laser"))
            {
                check = "laser";
            }
            else if (Name.Replace(" ", "").ToLower().Contains("rocket"))
            {
                check = "rocket";
            }

            // Queries
            IDbCommand command = dbConnectionSave.CreateCommand();
            command.CommandText = "SELECT file FROM pragma_database_list;";
            IDataReader reader = command.ExecuteReader();
            string dir = "";
            while (reader.Read())
            {
                dir = reader.GetString(0);
            }
            dir = dir.Replace("SaveTables.db", "DataTables.db");
            IDbCommand command2 = dbConnectionSave.CreateCommand();
            command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
            int sadasdasad = command2.ExecuteNonQuery();
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT DataTables.ArsenalPower.PowerName FROM DataTables.ArsenalPower " +
                "inner join SessionOwnership WHERE SessionOwnership.SessionID=" + n + " AND SessionOwnership.ItemType='Power'" +
                " AND DataTables.ArsenalPower.PowerID = SessionOwnership.ItemID ORDER BY DataTables.ArsenalPower.PowerID ASC";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            while (dataReader.Read())
            {
                if (check != "")
                {
                    if (!dataReader.GetString(0).Replace(" ", "").ToLower().Contains(check))
                    {
                        list.Add(dataReader.GetString(0).Replace(" ", ""));
                    }
                }
                else
                {
                    list.Add(dataReader.GetString(0).Replace(" ", ""));
                }
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return list;
        }
    }
    public Dictionary<string, object> GetPowerDataByName(string name)
    {
        OpenConnection();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PowerType, PowerName, PowerDescription, PowerStats, TierColor FROM ArsenalPower WHERE replace(lower(PowerName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            string Type = dataReader.GetString(0);
            if (Type.Equals("DEF"))
            {
                Type = "Defensive";
            } else if (Type.Equals("OFF"))
            {
                Type = "Offensive";
            } else if (Type.Equals("MOV"))
            {
                Type = "Movement";
            }
            datas.Add("Type", Type);
            datas.Add("Name", dataReader.GetString(1));
            datas.Add("Description", dataReader.GetString(2));
            datas.Add("Stats", dataReader.GetString(3));
            datas.Add("Color", dataReader.GetString(4));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        } else
        {
            return datas;
        }
    }

    public string GetPowerRealName(string name)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PowerType, PowerName, PowerDescription, PowerStats, TierColor FROM ArsenalPower WHERE replace(lower(PowerName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        string s = "";
        while (dataReader.Read())
        {
            s = "<color=" + dataReader.GetString(4) + ">" + dataReader.GetString(1) + "</color>";
        }
        dbConnectionData.Close();
        return s;
    }
    #endregion
    #region Access Consumables
    public Dictionary<string, object> GetConsumableDataByName(string itemName)
    {
        OpenConnection();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ItemName, ItemDescription, StockPerDays, ItemEffect, EffectDuration, MaxStack, ItemPrice, Cooldown, TierColor FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')=='" + itemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        Debug.Log(dbCheckCommand.CommandText);
        while (dataReader.Read())
        {
            check = true;
            datas.Add("Name", dataReader.GetString(0));
            datas.Add("Description", dataReader.GetString(1));
            if (dataReader.IsDBNull(2))
            {
                datas.Add("StockPerDay", 0);
            } else
            {
                datas.Add("StockPerDay", dataReader.GetInt32(2));
            }
            datas.Add("Effect", dataReader.GetString(3));
            datas.Add("Duration", dataReader.GetInt32(4));
            datas.Add("Stack", dataReader.GetInt32(5));
            datas.Add("Price", dataReader.GetInt32(6));
            if (dataReader.IsDBNull(7))
            {
                datas.Add("Cooldown", 0);
            }
            else
            {
                datas.Add("Cooldown", dataReader.GetInt32(7));
            }
            datas.Add("Color", dataReader.GetString(8));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        } else
            return datas;
    }

    public int GetStackLimitOfConsumableByName(string itemName)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT MaxStack FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')=='" + itemName.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        int n = 0;
        while (dataReader.Read())
        {
            check = true;
            n = dataReader.GetInt32(0);
        }
        dbConnectionData.Close();
        if (!check)
        {
            return -1;
        } else
        {
            return n;
        }
    }

    public int GetStocksPerDayOfConsumable(string itemName)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ItemID, StockPerDays FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')=='" + itemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        int n = 0;
        int id = -2;
        while (dataReader.Read())
        {
            check = true;
            id = dataReader.GetInt32(0);
            if (dataReader.IsDBNull(1))
            {
                n = -1;
            } else
                n = dataReader.GetInt32(1);
        }
        dbConnectionData.Close();
        if (!check)
        {
            return -2;
        }
        else
        {
            if (n==-1)
            {
                return n;
            } else
            return n - GetTotalBuyOfItemToday(GetCurrentSessionPlayerId(), id, "Consumable"); ;
        }
    }

    public Dictionary<string, int> GetOwnedConsumables(int PlayerID)
    {
        OpenConnection();
        Dictionary<string, int> data = new Dictionary<string, int>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand command = dbConnectionSave.CreateCommand();
        command.CommandText = "SELECT file FROM pragma_database_list;";
        IDataReader reader = command.ExecuteReader();
        string dir = "";
        while (reader.Read())
        {
            dir = reader.GetString(0);
        }
        dir = dir.Replace("SaveTables.db", "DataTables.db");
        IDbCommand command2 = dbConnectionSave.CreateCommand();
        command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
        int sadasdasad = command2.ExecuteNonQuery();
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT DataTables.SpaceShop.ItemName, PlayerOwnership.Quantity" +
            " FROM DataTables.SpaceShop inner join PlayerOwnership WHERE PlayerOwnership.PlayerID=" + PlayerID +
            " AND PlayerOwnership.ItemType='Consumable' AND DataTables.SpaceShop.ItemID = PlayerOwnership.ItemID " +
            "ORDER BY DataTables.SpaceShop.ItemID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            data.Add(dataReader.GetString(0).Replace("-", "").Replace(" ", ""), dataReader.GetInt32(1));
        }
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return data;
    }
    
    public Dictionary<string, int> GetSessionOwnedConsumables(int PlayerID)
    {
        OpenConnection();
        Dictionary<string, int> data = new Dictionary<string, int>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader1 = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader1.Read())
        {
            if (!dataReader1.IsDBNull(0))
            {
                check = true;
                n = dataReader1.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return null;
        }
        else
        {
            // Queries
            IDbCommand command = dbConnectionSave.CreateCommand();
            command.CommandText = "SELECT file FROM pragma_database_list;";
            IDataReader reader = command.ExecuteReader();
            string dir = "";
            while (reader.Read())
            {
                dir = reader.GetString(0);
            }
            dir = dir.Replace("SaveTables.db", "DataTables.db");
            IDbCommand command2 = dbConnectionSave.CreateCommand();
            command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
            int sadasdasad = command2.ExecuteNonQuery();
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT DataTables.SpaceShop.ItemName, SessionOwnership.Quantity" +
                " FROM DataTables.SpaceShop inner join SessionOwnership WHERE SessionOwnership.SessionID=" + n +
                " AND SessionOwnership.ItemType='Consumable' AND DataTables.SpaceShop.ItemID = SessionOwnership.ItemID" +
                " ORDER BY DataTables.SpaceShop.ItemID ASC";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            while (dataReader.Read())
            {
                data.Add(dataReader.GetString(0).Replace("-", "").Replace(" ", ""), dataReader.GetInt32(1));
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return data;
        }
    }

    public List<string> GetSpaceShopItemNameSearchByName(string name)
    {
        OpenConnection();
        List<string> list = new List<string>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ItemName FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','') LIKE '%" + name.Replace(" ", "").Replace("-", "").ToLower() + "%'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnectionData.Close();
        return list;
    }
    public List<List<string>> GetAllConsumable()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> ConsuList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from SpaceShop";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            ConsuList = new List<string>();
            check = true;
            ConsuList.Add(dataReader.GetInt32(0).ToString());
            ConsuList.Add(dataReader.GetString(1));
            ConsuList.Add(dataReader.GetString(2));
            if (dataReader.IsDBNull(3))
            {
                ConsuList.Add("N/A");
            } else
            {
                ConsuList.Add(dataReader.GetInt32(3).ToString());
            }
            ConsuList.Add(dataReader.GetString(4));
            ConsuList.Add(dataReader.GetInt32(5).ToString());
            ConsuList.Add(dataReader.GetString(6));
            ConsuList.Add(dataReader.GetInt32(7).ToString());
            ConsuList.Add(dataReader.GetInt32(8).ToString());      
            if (dataReader.IsDBNull(9))
            {
                ConsuList.Add("N/A");
            }
            else
            {
                ConsuList.Add(dataReader.GetInt32(9).ToString());
            }
            ConsuList.Add(dataReader.GetString(10));
            ConsuList.Add(dataReader.GetString(11));
            list.Add(ConsuList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
    }
    #endregion
    #region Access Rank
    public Dictionary<string, object> GetRankById(int id, int SupremeWarriorNo)
    {
        OpenConnection();
        Dictionary<string, object> rank = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM RankSystem WHERE RankId=" + id;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check1 = false;
        if (id == 0)
        {
            check1 = true;
            rank.Add("RankId", "0");
            rank.Add("RankName", "N/A");
            rank.Add("RankTier", "#FFFFFF");
        } else
        {
            while (dataReader.Read())
            {
                check1 = true;
                
                if (id >= 18)
                {                  
                    rank.Add("RankId", 18);
                    rank.Add("RankName", dataReader.GetString(1).Replace("?" , SupremeWarriorNo.ToString()));
                    rank.Add("RankTier", dataReader.GetString(7));
                    rank.Add("RankConditionSZ", dataReader.GetInt32(2) + SupremeWarriorNo * 25);
                    if (dataReader.IsDBNull(3))
                    {
                        rank.Add("RankCondition2Verb", "Null");
                    }
                    else
                    {
                        rank.Add("RankCondition2Verb", dataReader.GetString(3));
                    }
                    rank.Add("RankCondition2Num", dataReader.GetInt32(4));
                    rank.Add("DailyIncome", dataReader.GetInt32(5) + SupremeWarriorNo * 10000);
                    rank.Add("DailyIncomeShard", dataReader.GetInt32(8) + SupremeWarriorNo * 5);
                    rank.Add("SupremeWarriorNo", SupremeWarriorNo);
                } else
                {
                    rank.Add("RankId", dataReader.GetInt32(0).ToString());
                    rank.Add("RankName", dataReader.GetString(1));
                    rank.Add("RankTier", dataReader.GetString(7));
                    rank.Add("RankConditionSZ", dataReader.GetInt32(2));
                    if (dataReader.IsDBNull(3))
                    {
                        rank.Add("RankCondition2Verb", "Null");
                    } else
                    {
                        rank.Add("RankCondition2Verb", dataReader.GetString(3));
                    }
                    rank.Add("RankCondition2Num", dataReader.GetInt32(4));
                    rank.Add("DailyIncome", dataReader.GetInt32(5));
                    rank.Add("DailyIncomeShard", dataReader.GetInt32(8));
                    rank.Add("SupremeWarriorNo", SupremeWarriorNo);
                }
            }   
        }
        if (!check1) return null;
        dbConnectionData.Close();
 
        return rank;
     }

    public List<List<string>> GetAllRank()
    {
        OpenConnection();
        int PlayerID = GetCurrentSessionPlayerId();
        List<List<string>> list = new List<List<string>>();
        List<string> RankList;
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from RankSystem";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "Select SupremeWarriorNo from PlayerProfile WHERE PlayerID = " + PlayerID + "";
        IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            RankList = new List<string>();
            check = true;
            RankList.Add(dataReader.GetInt32(0).ToString());
            RankList.Add(dataReader.GetString(1));
            if (dataReader.GetInt32(0) == 18)
            {
                while(dataReader1.Read())
                {
                    RankList.Add((dataReader.GetInt32(2) + dataReader1.GetInt32(0) * 25).ToString());
                    if (dataReader.IsDBNull(3))
                    {
                        RankList.Add("N/A");
                    }
                    else
                    {
                        RankList.Add(dataReader.GetString(3));
                    }
                    RankList.Add(dataReader.GetInt32(4).ToString());
                    RankList.Add((dataReader.GetInt32(5) + dataReader1.GetInt32(0) * 5000).ToString());
                }
            } else
            {
                RankList.Add(dataReader.GetInt32(2).ToString());
                if (dataReader.IsDBNull(3))
                {
                    RankList.Add("N/A");
                }
                else
                {
                    RankList.Add(dataReader.GetString(3));
                }           
                RankList.Add(dataReader.GetInt32(4).ToString());        
                RankList.Add(dataReader.GetInt32(5).ToString());
            }
            if (dataReader.IsDBNull(6))
            {
                RankList.Add("N/A");
            }
            else
            {
                RankList.Add(dataReader.GetString(6));
            }           
            RankList.Add(dataReader.GetString(7));
            RankList.Add(dataReader.GetInt32(8).ToString());
            list.Add(RankList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        dbConnectionSave.Close();
        return list;
    }

    public void UpdateRank(int PlayerID, Dictionary<string, object> data)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        string query = "";
        if (int.Parse(data["RankId"].ToString()) == 18)
        {
            query = "UPDATE PlayerProfile SET Rank = " + int.Parse(data["RankId"].ToString()) + ", DailyIncome = " + int.Parse(data["DailyIncome"].ToString()) + ", SupremeWarriorNo = " + int.Parse(data["SupremeWarriorNo"].ToString()) + ", DailyIncomeShard = "+ int.Parse(data["DailyIncomeShard"].ToString()) + " WHERE PlayerID = " + PlayerID + "";
        } else
        {
            query = "UPDATE PlayerProfile SET Rank = " + int.Parse(data["RankId"].ToString()) + ", DailyIncome = " + int.Parse(data["DailyIncome"].ToString()) + ", DailyIncomeShard = " + int.Parse(data["DailyIncomeShard"].ToString()) + "  WHERE PlayerID = " + PlayerID + "";
        }
        // Queries    
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = query;
        dbCheckCommand1.ExecuteNonQuery();
        dbConnectionSave.Close();
    }

    #endregion
    #region Access Enemy
    public List<List<string>> GetAllEnemy()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> EnemyList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from Enemies";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            EnemyList = new List<string>();
            check = true;
            EnemyList.Add(dataReader.GetInt32(0).ToString());
            EnemyList.Add(dataReader.GetString(1));
            EnemyList.Add(dataReader.GetString(2));
            EnemyList.Add(dataReader.GetString(3));
            EnemyList.Add(dataReader.GetString(4));
            EnemyList.Add(dataReader.GetString(5));
            EnemyList.Add(dataReader.GetString(6));
            EnemyList.Add(dataReader.GetString(7));
            EnemyList.Add(dataReader.GetString(8));
            list.Add(EnemyList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
    }

    public Dictionary<string,object> GetDataEnemyById(int enemyID)
    {
        OpenConnection();
        Dictionary<string,object> data = new Dictionary<string,object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select EnemyName, MainTarget, EnemyWeapons, EnemyStats, EnemyPower, DefeatReward, EnemyTier from Enemies WHERE EnemyID=" + enemyID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("Name", dataReader.GetString(0));
            data.Add("MainTarget", dataReader.GetString(1));
            data.Add("Weapons", dataReader.GetString(2));
            data.Add("Stats", dataReader.GetString(3));
            data.Add("Power", dataReader.GetString(4));
            data.Add("DefeatReward", dataReader.GetString(5));
            data.Add("TierColor", dataReader.GetString(6));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        } else
        return data;
    }

    public Dictionary<string, object> GetDataEnemyByName(string enemyName)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select EnemyID, MainTarget, EnemyWeapons, EnemyStats, EnemyPower, DefeatReward, EnemyTier from Enemies WHERE EnemyName='" + enemyName + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("ID", dataReader.GetInt32(0));
            data.Add("MainTarget", dataReader.GetString(1));
            data.Add("Weapons", dataReader.GetString(2));
            data.Add("Stats", dataReader.GetString(3));
            data.Add("Power", dataReader.GetString(4));
            data.Add("DefeatReward", dataReader.GetString(5));
            data.Add("TierColor", dataReader.GetString(6));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }
    #endregion
    #region Access Warship
    public List<List<string>> GetAllWarship()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> WarshipList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from Warship";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            WarshipList = new List<string>();
            check = true;
            WarshipList.Add(dataReader.GetInt32(0).ToString());
            WarshipList.Add(dataReader.GetString(1));
            if (dataReader.IsDBNull(2))
            {
                WarshipList.Add("N/A");
            } else
            {
                WarshipList.Add(dataReader.GetString(2));
            }
            if (dataReader.IsDBNull(3))
            {
                WarshipList.Add("N/A");
            }
            else
            {
                WarshipList.Add(dataReader.GetString(3));
            }
            
            WarshipList.Add(dataReader.GetString(4));
            list.Add(WarshipList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
    }

    public Dictionary<string, object> GetWSById(int ID)
    {
        OpenConnection();
        Dictionary<string, object> WSDict = new Dictionary<string, object>();
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from Warship Where WSid = " + ID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            WSDict.Add("WarshipID", dataReader.GetInt32(0).ToString());
            WSDict.Add("WarshipName", dataReader.GetString(1));
            WSDict.Add("WarshipStat", dataReader.GetString(3));
            WSDict.Add("Tier", dataReader.GetString(4));
            WSDict.Add("MainWeapon", dataReader.GetString(5));
            WSDict.Add("SupWeapon", dataReader.GetString(6));
            WSDict.Add("Bounty", dataReader.GetString(7));
        }
        if (!check) return null;
        dbConnectionData.Close();
        return WSDict;
    }

    public Dictionary<string, object> GetWSByName(string Name)
    {
        OpenConnection();
        Dictionary<string, object> WSDict = new Dictionary<string, object>();
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from Warship Where WSName = '" + Name.Replace("_","-") +"'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            WSDict.Add("WarshipID", dataReader.GetInt32(0).ToString());
            WSDict.Add("WarshipName", dataReader.GetString(1));
            WSDict.Add("WarshipStat", dataReader.GetString(3));
            WSDict.Add("Tier", dataReader.GetString(4));
            WSDict.Add("MainWeapon", dataReader.GetString(5));
            WSDict.Add("SupWeapon", dataReader.GetString(6));
            WSDict.Add("Bounty", dataReader.GetString(7));
        }
        if (!check) return null;
        dbConnectionData.Close();
        return WSDict;
    }
    #endregion
    #region Access Ownership
    public int GetCurrentOwnedNumberOfConsumableByName(int PlayerID, string itemName)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionData.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),'-',''),' ','')='" + itemName.Replace("-","").Replace(" ","").ToLower() + "'";
        IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
        int n = 0;
        while (dataReader1.Read())
        {
            n = dataReader1.GetInt32(0);
        }
        if (n!=0)
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE " +
                "PlayerID=" + PlayerID + " AND ItemID=" + n + " AND ItemType='Consumable'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quan = 0;
            while (dataReader.Read())
            {
                quan = dataReader.GetInt32(0);
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return quan;
        } else
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return 0;
        }
    }

    public int GetCurrentOwnershipWeaponPowerModelByName(int PlayerID, string itemName, string Type)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        int id = -1;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnectionData.CreateCommand();
            dbCheckWeapon.CommandText = "SELECT WeaponID FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderWeapon = dbCheckWeapon.ExecuteReader();
            while (dataReaderWeapon.Read())
            {
                id = dataReaderWeapon.GetInt32(0);
                break;
            }
        }
        else if ("Power".Equals(Type))
        {
            IDbCommand dbCheckPower = dbConnectionData.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        } else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnectionData.CreateCommand();
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-","").ToLower() + "'";
            IDataReader dataReaderModel = dbCheckModel.ExecuteReader();
            while (dataReaderModel.Read())
            {
                id = dataReaderModel.GetInt32(0);
                break;
            }
        }
        if (id!=-1)
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE " +
                "PlayerID=" + PlayerID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quan = 0;
            while (dataReader.Read())
            {
                quan = dataReader.GetInt32(0);
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return quan;
        }
        else
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return -1;
        }
    }

    public int GetCurrentOwnershipWeaponPowerModel(int PlayerID, string Type) 
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Sum(Quantity) FROM PlayerOwnership WHERE " +
            "PlayerID=" + PlayerID + " AND ItemType='" + Type + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int quan = 0;
        while (dataReader.Read())
        {
            if (dataReader.IsDBNull(0))
            {
                quan = 0;
            } else
            {
                quan = dataReader.GetInt32(0);
            }
        }
        dbConnectionSave.Close();
        return quan;
       
    }
    /// <summary>
    /// Use this fuction to add permanent ownership to any item
    /// </summary>
    /// <param name="PlayerId">Id of player</param>
    /// <param name="itemName">Name of item</param>
    /// <param name="Type">Weapon/Power/Consumable/Model</param>
    /// <param name="Quantity">Count</param>
    public string AddOwnershipToItem(int PlayerId, string itemName, string Type, int Quantity)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        int id = 0;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnectionData.CreateCommand();
            dbCheckWeapon.CommandText = "SELECT WeaponID FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + itemName.Replace(" ","").ToLower() + "'";
            IDataReader dataReaderWeapon = dbCheckWeapon.ExecuteReader();
            while (dataReaderWeapon.Read())
            {
                id = dataReaderWeapon.GetInt32(0);
                break;
            }
        } 
        else if ("Power".Equals(Type))
        {
            IDbCommand dbCheckPower = dbConnectionData.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ","").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        }
        else if ("Consumable".Equals(Type))
        {
            IDbCommand dbCheckCons = dbConnectionData.CreateCommand();
            dbCheckCons.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')='" + itemName.Replace(" ","").Replace("-","").ToLower() + "'";
            IDataReader dataReaderCons = dbCheckCons.ExecuteReader();
            while (dataReaderCons.Read())
            {
                id = dataReaderCons.GetInt32(0);
                break;
            }
        }
        else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnectionData.CreateCommand();
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderModel = dbCheckModel.ExecuteReader();
            while (dataReaderModel.Read())
            {
                id = dataReaderModel.GetInt32(0);
                break;
            }
        }
        if (id==0)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Not Found";
        } else
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE" +
                " PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quantity = -1;
            while (dataReader.Read())
            {
                quantity = dataReader.GetInt32(0);
            }
            if (quantity != -1)
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "UPDATE PlayerOwnership SET Quantity = Quantity + " + Quantity
                        + " WHERE PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
                int n = dbCommand.ExecuteNonQuery();
                dbConnectionData.Close();
                dbConnectionSave.Close();
                if (n != 1)
                {
                    return "Fail";
                }
                else
                {
                    string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, true);
                    return check;
                }
            } 
            else
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "INSERT INTO PlayerOwnership (PlayerID,ItemType,ItemID,Quantity) VALUES" +
                    "(" + PlayerId + ",'" + Type + "'," + id + "," + Quantity + ")";
                int n = dbCommand.ExecuteNonQuery();
                dbConnectionData.Close();
                dbConnectionSave.Close();
                if (n != 1)
                {
                    return "Fail";
                }
                else
                {
                    string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, true);
                    return check;
                }
            }

        }
    }
    public string DecreaseOwnershipToItem(int PlayerId, string itemName, string Type, int Quantity)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        int id = 0;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnectionData.CreateCommand();
            dbCheckWeapon.CommandText = "SELECT WeaponID FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderWeapon = dbCheckWeapon.ExecuteReader();
            while (dataReaderWeapon.Read())
            {
                id = dataReaderWeapon.GetInt32(0);
                break;
            }
        }
        else if ("Power".Equals(Type))
        {
            IDbCommand dbCheckPower = dbConnectionData.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        }
        else if ("Consumable".Equals(Type))
        {
            IDbCommand dbCheckCons = dbConnectionData.CreateCommand();
            dbCheckCons.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderCons = dbCheckCons.ExecuteReader();
            while (dataReaderCons.Read())
            {
                id = dataReaderCons.GetInt32(0);
                break;
            }
            
        }
        else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnectionData.CreateCommand();
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderModel = dbCheckModel.ExecuteReader();
            while (dataReaderModel.Read())
            {
                id = dataReaderModel.GetInt32(0);
                break;
            }
        }
        if (id == 0)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Not Enough Item";
        }
        else
        {
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE" +
                " PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type +"'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quantity = -1;
            while (dataReader.Read())
            {
                quantity = dataReader.GetInt32(0);
            }
            if (quantity!=-1)
            {
                if (quantity > Quantity)
                {
                    IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                    dbCommand.CommandText = "UPDATE PlayerOwnership SET Quantity = Quantity - " + Quantity
                        + " WHERE PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
                    int n = dbCommand.ExecuteNonQuery();
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    if (n != 1)
                    {
                        return "Fail";
                    }
                    else
                    {
                        string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, false);
                        return check;
                    }
                } else if (quantity == Quantity)
                {
                    IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                    dbCommand.CommandText = "DELETE FROM PlayerOwnership WHERE PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'"; ;
                    int n = dbCommand.ExecuteNonQuery();
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    if (n != 1)
                    {
                        return "Fail";
                    }
                    else
                    {
                        string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, false);
                        return check;
                    }
                } else
                {
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return "Not Enough Item";
                }
            } else
            {
                dbConnectionData.Close();
                dbConnectionSave.Close();
                return "Not Enough Item";
            }
        }
    }
    #endregion
    #region Access Session Ownership
    public int GetSessionCurrentOwnedNumberOfConsumableByName(int PlayerID, string itemName)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionData.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),'-',''),' ','')='" + itemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
        IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
        int n = 0;
        while (dataReader1.Read())
        {
            n = dataReader1.GetInt32(0);
        }
        if (n != 0)
        {
            IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
            dbCheckCommand2.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE " +
                "PlayerID=" + PlayerID;
            IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
            int SessionID = 0;
            while (dataReader2.Read())
            {
                SessionID = dataReader2.GetInt32(0);
            }
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM SessionOwnership WHERE " +
                "SessionID=" + SessionID + " AND ItemID=" + n + " AND ItemType='Consumable'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quan = 0;
            while (dataReader.Read())
            {
                quan = dataReader.GetInt32(0);
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return quan;
        }
        else
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return 0;
        }
    }

    public int GetSessionCurrentOwnershipWeaponPowerModelByName(int PlayerID, string itemName, string Type)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        int id = -1;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnectionData.CreateCommand();
            dbCheckWeapon.CommandText = "SELECT WeaponID FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderWeapon = dbCheckWeapon.ExecuteReader();
            while (dataReaderWeapon.Read())
            {
                id = dataReaderWeapon.GetInt32(0);
                break;
            }
        }
        else if ("Power".Equals(Type))
        {
            IDbCommand dbCheckPower = dbConnectionData.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        }
        else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnectionData.CreateCommand();
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderModel = dbCheckModel.ExecuteReader();
            while (dataReaderModel.Read())
            {
                id = dataReaderModel.GetInt32(0);
                break;
            }
        }
        if (id != -1)
        {
            IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
            dbCheckCommand2.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE " +
                "PlayerID=" + PlayerID;
            IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
            int SessionID = 0;
            while (dataReader2.Read())
            {
                SessionID = dataReader2.GetInt32(0);
            }
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM SessionOwnership WHERE " +
                "SessionID=" + SessionID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quan = 0;
            while (dataReader.Read())
            {
                quan = dataReader.GetInt32(0);
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return quan;
        }
        else
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return -1;
        }
    }

    public int GetSessionCurrentOwnershipWeaponPowerModel(int PlayerID, string Type)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE " +
            "PlayerID=" + PlayerID;
        IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
        int SessionID = 0;
        while (dataReader2.Read())
        {
            SessionID = dataReader2.GetInt32(0);
        }
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Sum(Quantity) FROM SessionOwnership WHERE " +
            "SessionID=" + SessionID + " AND ItemType='" + Type + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int quan = 0;
        while (dataReader.Read())
        {
            if (dataReader.IsDBNull(0))
            {
                quan = 0;
            }
            else
            {
                quan = dataReader.GetInt32(0);
            }
        }
        dbConnectionSave.Close();
        return quan;

    }
    /// <summary>
    /// Use this fuction to add permanent ownership to any item
    /// </summary>
    /// <param name="PlayerId">Id of player</param>
    /// <param name="itemName">Name of item</param>
    /// <param name="Type">Weapon/Power/Consumable/Model</param>
    /// <param name="Quantity">Count</param>
    public string AddSessionOwnershipToItem(int PlayerId, string itemName, string Type, int Quantity)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        int id = 0;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnectionData.CreateCommand();
            dbCheckWeapon.CommandText = "SELECT WeaponID FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderWeapon = dbCheckWeapon.ExecuteReader();
            while (dataReaderWeapon.Read())
            {
                id = dataReaderWeapon.GetInt32(0);
                break;
            }
        }
        else if ("Power".Equals(Type))
        {
            IDbCommand dbCheckPower = dbConnectionData.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        }
        else if ("Consumable".Equals(Type))
        {
            IDbCommand dbCheckCons = dbConnectionData.CreateCommand();
            dbCheckCons.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderCons = dbCheckCons.ExecuteReader();
            while (dataReaderCons.Read())
            {
                id = dataReaderCons.GetInt32(0);
                break;
            }
        }
        else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnectionData.CreateCommand();
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderModel = dbCheckModel.ExecuteReader();
            while (dataReaderModel.Read())
            {
                id = dataReaderModel.GetInt32(0);
                break;
            }
        }
        if (id == 0)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Not Found";
        }
        else
        {
            IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
            dbCheckCommand2.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE " +
                "PlayerID=" + PlayerId;
            IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
            int SessionID = 0;
            while (dataReader2.Read())
            {
                SessionID = dataReader2.GetInt32(0);
            }
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM SessionOwnership WHERE" +
                " SessionID=" + SessionID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quantity = -1;
            while (dataReader.Read())
            {
                quantity = dataReader.GetInt32(0);
            }
            if (quantity != -1)
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "UPDATE SessionOwnership SET Quantity = Quantity + " + Quantity
                        + " WHERE SessionID=" + SessionID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
                int n = dbCommand.ExecuteNonQuery();
                dbConnectionData.Close();
                dbConnectionSave.Close();
                if (n != 1)
                {
                    return "Fail";
                }
                else
                {
                    string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, true);
                    return check;
                }
            }
            else
            {
                IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                dbCommand.CommandText = "INSERT INTO SessionOwnership (SessionID,ItemType,ItemID,Quantity) VALUES" +
                    "(" + SessionID + ",'" + Type + "'," + id + "," + Quantity + ")";
                int n = dbCommand.ExecuteNonQuery();
                dbConnectionData.Close();
                dbConnectionSave.Close();
                if (n != 1)
                {
                    return "Fail";
                }
                else
                {
                    string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, true);
                    return check;
                }
            }

        }
    }
    public string DecreaseSessionOwnershipToItem(int PlayerId, string itemName, string Type, int Quantity)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        int id = 0;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnectionData.CreateCommand();
            dbCheckWeapon.CommandText = "SELECT WeaponID FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderWeapon = dbCheckWeapon.ExecuteReader();
            while (dataReaderWeapon.Read())
            {
                id = dataReaderWeapon.GetInt32(0);
                break;
            }
        }
        else if ("Power".Equals(Type))
        {
            IDbCommand dbCheckPower = dbConnectionData.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        }
        else if ("Consumable".Equals(Type))
        {
            IDbCommand dbCheckCons = dbConnectionData.CreateCommand();
            dbCheckCons.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderCons = dbCheckCons.ExecuteReader();
            while (dataReaderCons.Read())
            {
                id = dataReaderCons.GetInt32(0);
                break;
            }
        }
        else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnectionData.CreateCommand();
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
            IDataReader dataReaderModel = dbCheckModel.ExecuteReader();
            while (dataReaderModel.Read())
            {
                id = dataReaderModel.GetInt32(0);
                break;
            }
        }
        if (id == 0)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Not Enough Item";
        }
        else
        {
            IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
            dbCheckCommand2.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE " +
                "PlayerID=" + PlayerId;
            IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
            int SessionID = 0;
            while (dataReader2.Read())
            {
                SessionID = dataReader2.GetInt32(0);
            }
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM SessionOwnership WHERE" +
                " SessionID=" + SessionID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quantity = -1;
            while (dataReader.Read())
            {
                quantity = dataReader.GetInt32(0);
            }
            if (quantity != -1)
            {
                if (quantity > Quantity)
                {
                    IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                    dbCommand.CommandText = "UPDATE SessionOwnership SET Quantity = Quantity - " + Quantity
                        + " WHERE SessionID=" + SessionID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
                    int n = dbCommand.ExecuteNonQuery();
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    if (n != 1)
                    {
                        return "Fail";
                    }
                    else
                    {
                        string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, false);
                        return check;
                    }
                }
                else if (quantity == Quantity)
                {
                    IDbCommand dbCommand = dbConnectionSave.CreateCommand();
                    dbCommand.CommandText = "DELETE FROM SessionOwnership WHERE SessionID=" + SessionID + " AND ItemID=" + id + " AND ItemType='" + Type + "'"; ;
                    int n = dbCommand.ExecuteNonQuery();
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    if (n != 1)
                    {
                        return "Fail";
                    }
                    else
                    {
                        string check = AddPurchaseHistory(PlayerId, id, Type, Quantity, false);
                        return check;
                    }
                }
                else
                {
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    return "Not Enough Item";
                }
            }
            else
            {
                dbConnectionData.Close();
                dbConnectionSave.Close();
                return "Not Enough Item";
            }
        }
    }
    #endregion
    #region Access Purchase History
    public string AddPurchaseHistory(int PlayerID, int itemID, string Type, int Quantity, bool isBuy)
    {
        OpenConnection();
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "INSERT INTO PurchaseHistory (PlayerID, ItemType, ItemID, Quantity, BuyOrSell, PurchaseDate) VALUES " +
            "(" + PlayerID + ",'" + Type + "'," + itemID + "," + Quantity + ",'" + (isBuy ? "Buy" : "Sell") + "','" + currentDate + "')";
        int n = dbCommand.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n!=1)
        {
            return "Fail";
        } else
        {
            return "Success";
        }
    }

    public int GetTotalBuyOfItemToday(int PlayerID, int ItemID, string Type)
    {
        OpenConnection();
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "SELECT sum(Quantity) from PurchaseHistory where PlayerId =" + PlayerID + " and ItemID=" + ItemID + 
            " and ItemType='" + Type + "' and BuyOrSell='Buy' and PurchaseDate='" + currentDate +"'";
        IDataReader dataReader = dbCommand.ExecuteReader();
        int sum = -1;
        while (dataReader.Read())
        {
            if (dataReader.IsDBNull(0))
            {
                sum = 0;
            } else
            {
                sum = dataReader.GetInt32(0);
            }
        }
        dbConnectionSave.Close();
        return sum;
    }
    #endregion
    #region Access Tutorial
    public List<List<string>> GetAllTutorial() 
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> TList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM Tutorial";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            TList = new List<string>();
            TList.Add(dataReader.GetInt32(0).ToString());
            TList.Add(dataReader.GetString(1));
            TList.Add(dataReader.GetString(2));
            TList.Add(dataReader.GetString(3));
            
            list.Add(TList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
        
    }
    #endregion
    #region Access SpaceStation
    public List<List<string>> GetAllSpaceStation()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> SpaceStationList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from SpaceStation";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            SpaceStationList = new List<string>();
            check = true;
            SpaceStationList.Add(dataReader.GetInt32(0).ToString());
            SpaceStationList.Add(dataReader.GetString(1));
            if (dataReader.IsDBNull(2))
            {
                SpaceStationList.Add("N/A");
            }
            else
            {
                SpaceStationList.Add(dataReader.GetString(2));
            }
            if (dataReader.IsDBNull(3))
            {
                SpaceStationList.Add("N/A");
            }
            else
            {
                SpaceStationList.Add(dataReader.GetString(3));
            }

            SpaceStationList.Add(dataReader.GetString(4));
            list.Add(SpaceStationList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
    }

    public Dictionary<string, object> GetSpaceStationById(int id)
    {
        OpenConnection();
        Dictionary<string, object> WSDict = new Dictionary<string, object>();
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from SpaceStation Where SSId = " + id;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            WSDict.Add("SpaceStationID", dataReader.GetInt32(0).ToString());
            WSDict.Add("SpaceStationName", dataReader.GetString(1));
            WSDict.Add("SpaceStationEffect", dataReader.GetString(3));
            WSDict.Add("Tier", dataReader.GetString(4));
            WSDict.Add("MainWeapon", dataReader.GetString(5));
            WSDict.Add("SupWeapon", dataReader.GetString(6));
            WSDict.Add("AuraRange", dataReader.GetString(7));
            WSDict.Add("BaseHP", dataReader.GetString(8));
            WSDict.Add("Bounty", dataReader.GetString(9));
        }
        if (!check) return null;
        dbConnectionData.Close();
        return WSDict;
    }

    public Dictionary<string, object> GetSpaceStationByName(string Name)
    {
        OpenConnection();
        Dictionary<string, object> WSDict = new Dictionary<string, object>();
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from SpaceStation Where SSName ='" + Name.Replace("_", "-") +"'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            WSDict.Add("SpaceStationID", dataReader.GetInt32(0).ToString());
            WSDict.Add("SpaceStationName", dataReader.GetString(1));
            WSDict.Add("SpaceStationEffect", dataReader.GetString(3));
            WSDict.Add("Tier", dataReader.GetString(4));
            WSDict.Add("MainWeapon", dataReader.GetString(5));
            WSDict.Add("SupWeapon", dataReader.GetString(6));
            WSDict.Add("AuraRange", dataReader.GetString(7));
            WSDict.Add("BaseHP", dataReader.GetString(8));
            WSDict.Add("Bounty", dataReader.GetString(9));
        }
        if (!check) return null;
        dbConnectionData.Close();
        return WSDict;
    }
    #endregion
    #region Access Damage Element
    public List<List<string>> GetAllDMGElement()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> DMGElementList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from DamageElement";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            DMGElementList = new List<string>();
            check = true;
            DMGElementList.Add(dataReader.GetInt32(0).ToString());
            DMGElementList.Add(dataReader.GetString(1));
            DMGElementList.Add(dataReader.GetString(2));
            list.Add(DMGElementList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
    }
    #endregion
    #region Access Attribute
    public List<List<string>> GetAllAttribute()
    {
        OpenConnection();
        List<List<string>> list = new List<List<string>>();
        List<string> DMGElementList;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from Attribute";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            DMGElementList = new List<string>();
            check = true;
            DMGElementList.Add(dataReader.GetInt32(0).ToString());
            DMGElementList.Add(dataReader.GetString(1));
            DMGElementList.Add(dataReader.GetString(2));
            list.Add(DMGElementList);
        }
        if (!check) return null;
        dbConnectionData.Close();
        return list;
    }
    #endregion
    #region Access Collect salary history
    public string SalaryCollected(int PlayerId)
    {
        OpenConnection();
        string date = System.DateTime.Now.ToString("dd/MM/yyyy");
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "INSERT INTO CollectSalaryHistory (PlayerId, CollectedTime) VALUES ( "+PlayerId+" , '"+ date +"')";
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public int CheckIfCollected(int PlayerId, string date)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Count(*) FROM CollectSalaryHistory where PlayerId = "+ PlayerId +" and CollectedTime = '"+ date +"'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int quan = 0;
        while (dataReader.Read())
        {
            quan = dataReader.GetInt32(0);
        }
        dbConnectionSave.Close();
        return quan;
    }


    #endregion
    #region Access LOTW
    public List<int> GetListIDAllLOTW(int tier)
    {
        OpenConnection();
        List<int> list = new List<int>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CardID From LuckOfTheWandererCards WHERE " + (tier > 0 && tier <= 3 ? "CardTier=" + tier : "1==1");
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetInt32(0));
        }
        dbConnectionData.Close();
        return list;
    }

    public Dictionary<string, object> GetLOTWInfoByID(int id)
    {
        OpenConnection();
        Dictionary<string, object> dict = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionData.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT * From LuckOfTheWandererCards WHERE CardID=" + id;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            dict.Add("Name",dataReader.GetString(1));
            dict.Add("Type", dataReader.GetString(2));
            dict.Add("Effect", dataReader.GetString(3));
            dict.Add("Tier", dataReader.GetInt32(4));
            dict.Add("Duration", dataReader.GetInt32(5));
            dict.Add("Stack", dataReader.GetString(6));
            dict.Add("Repeat", dataReader.GetString(7));
            dict.Add("Color", dataReader.GetString(8));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        } else
        return dict;
    }
    public List<string> CheckLOTWRepetableAlreadyOwn(int PlayerID)
    {
        List<string> Data = new();
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerID=" + PlayerID;
        IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
        bool check = false;
        int sessionId = -1;
        while (dataReader1.Read())
        {
            check = true;
            if (!dataReader1.IsDBNull(0))
            {
                sessionId = dataReader1.GetInt32(0);
            }
        }
        if (!check || sessionId == -1)
        {
            dbConnectionSave.Close();
            return null;
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand34 = dbConnectionSave.CreateCommand();
            dbCheckCommand34.CommandText = "SELECT * FROM SessionLOTWCards WHERE SessionID=" + sessionId + " AND CardID=34"; 
            IDataReader dataReader34 = dbCheckCommand34.ExecuteReader();
            bool check34 = false;
            while (dataReader34.Read())
            {
                check34 = true;
            }
            
            if (check34)
            {
                Data.Add("34");
            }
            // Queries
            IDbCommand dbCheckCommand26 = dbConnectionSave.CreateCommand();
            dbCheckCommand26.CommandText = "SELECT * FROM SessionLOTWCards WHERE SessionID=" + sessionId + " AND CardID=26";
            IDataReader dataReader26 = dbCheckCommand26.ExecuteReader();
            bool check26 = false;
            while (dataReader26.Read())
            {
                check26 = true;
            }
            if (check26)
            {
                Data.Add("25");
            }
            // Queries
            IDbCommand dbCheckCommand9 = dbConnectionSave.CreateCommand();
            dbCheckCommand9.CommandText = "SELECT * FROM SessionLOTWCards WHERE SessionID=" + sessionId + " AND CardID=9 AND Stack >= 10";
            IDataReader dataReader9 = dbCheckCommand9.ExecuteReader();
            bool check9 = false;
            while (dataReader9.Read())
            {
                check9 = true;
            }
            if (check9)
            {
                Data.Add("9");
            }
            // Queries
            IDbCommand dbCheckCommand31 = dbConnectionSave.CreateCommand();
            dbCheckCommand31.CommandText = "SELECT * FROM SessionLOTWCards WHERE SessionID=" + sessionId + " AND CardID=31 AND Stack >= 4";
            IDataReader dataReader31 = dbCheckCommand31.ExecuteReader();
            bool check31 = false;
            while (dataReader31.Read())
            {
                check31 = true;
            }
            if (check31)
            {
                Data.Add("31");
            }
            dbConnectionSave.Close();
            return Data;
        }
    }

    public List<Dictionary<string, object>> GetLOTWInfoOwnedByID(int PlayerID)
    {
        OpenConnection();
        List<Dictionary<string, object>> listFinal = new List<Dictionary<string, object>>();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerID=" + PlayerID;
        IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
        bool check = false;
        int sessionId = -1;
        while (dataReader1.Read())
        {
            check = true;
            if (!dataReader1.IsDBNull(0))
            {
                sessionId = dataReader1.GetInt32(0);
            }
        }
        if (!check || sessionId == -1)
        {
            dbConnectionSave.Close();
            return null;
        }
        else
        {
            // Queries
            IDbCommand command = dbConnectionSave.CreateCommand();
            command.CommandText = "SELECT file FROM pragma_database_list;";
            IDataReader reader = command.ExecuteReader();
            string dir = "";
            while (reader.Read())
            {
                dir = reader.GetString(0);
            }
            dir = dir.Replace("SaveTables.db", "DataTables.db");
            IDbCommand command2 = dbConnectionSave.CreateCommand();
            command2.CommandText = "ATTACH DATABASE \"" + dir + "\" as DataTables;";
            int sadasdasad = command2.ExecuteNonQuery();
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT DataTables.LuckOfTheWandererCards.CardID, DataTables.LuckOfTheWandererCards.CardName, " +
                "DataTables.LuckOfTheWandererCards.CardEffect, SessionLOTWCards.Duration, SessionLOTWCards.Stack, " +
                "DataTables.LuckOfTheWandererCards.CardStackable, DataTables.LuckOfTheWandererCards.TierColor " +
                "From SessionLOTWCards INNER JOIN DataTables.LuckOfTheWandererCards " +
                "WHERE SessionLOTWCards.SessionID=" + sessionId + " AND DataTables.LuckOfTheWandererCards.CardID = SessionLOTWCards.CardID " +
                "ORDER BY DataTables.LuckOfTheWandererCards.CardTier ASC, SessionLOTWCards.Duration DESC, DataTables.LuckOfTheWandererCards.CardID ASC";
            Debug.Log(dbCheckCommand.CommandText);
            IDataReader dataReader2 = dbCheckCommand.ExecuteReader();
            while (dataReader2.Read())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("ID", dataReader2.GetInt32(0));
                dict.Add("Name", dataReader2.GetString(1));
                dict.Add("Effect", dataReader2.GetString(2));
                dict.Add("Duration", dataReader2.GetInt32(3));
                dict.Add("Stack", dataReader2.GetInt32(4));
                dict.Add("Stackable", dataReader2.GetString(5));
                dict.Add("Color", dataReader2.GetString(6));
                listFinal.Add(dict);
            }
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return listFinal;
        }
    }
    public List<int> GetAllCardIdInCurrentSession(int PlayerID)
    {
        OpenConnection();
        List<int> list = new List<int>();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerID=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        bool check = false;
        int sessionId = -1;
        while (dataReader.Read())
        {
            check = true;
            if (!dataReader.IsDBNull(0))
            {
                sessionId = dataReader.GetInt32(0);
            }
        }
        if (!check || sessionId==-1)
        {
            dbConnectionSave.Close();
            return null;
        } else
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "SELECT CardID From SessionLOTWCards WHERE SessionID=" + sessionId;
            IDataReader dataReader2 = dbCheckCommand.ExecuteReader();
            while (dataReader2.Read())
            {
                list.Add(dataReader2.GetInt32(0));
            }
            dbConnectionSave.Close();
            return list;
        }
    }

    public string UpdateReduceDurationAllCardByPlayerID(int PlayerID)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerID=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        bool check = false;
        int sessionId = -1;
        while (dataReader.Read())
        {
            check = true;
            if (!dataReader.IsDBNull(0))
            {
                sessionId = dataReader.GetInt32(0);
            }
        }
        if (!check || sessionId == -1)
        {
            dbConnectionSave.Close();
            return "Fail";
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "UPDATE SessionLOTWCards SET Duration = Duration - IIF(Duration=1000 OR Duration=-1, 0, 1) WHERE SessionID=" + sessionId;
            int n = dbCheckCommand.ExecuteNonQuery();
            // Queries
            IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
            dbCheckCommand1.CommandText = "DELETE FROM SessionLOTWCards WHERE Duration = 0 AND SessionID=" + sessionId;
            int m = dbCheckCommand1.ExecuteNonQuery();
            Debug.Log("Delete");
            dbConnectionSave.Close();
            if (m != 1)
            {
                return "Fail";
            } else
            return "Success";
        }
    }

    public string AddCardToCurrentSession(int PlayerID, int CardID)
    {
        OpenConnection();
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerID=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        bool check = false;
        int sessionId = -1;
        while (dataReader.Read())
        {
            check = true;
            if (!dataReader.IsDBNull(0))
            {
                sessionId = dataReader.GetInt32(0);
            }
        }
        if (!check || sessionId == -1)
        {
            dbConnectionData.Close();
            dbConnectionSave.Close();
            return "Fail";
        }
        else
        {
            if (CardID==34)
            {
                // Queries
                IDbCommand dbCheckComman = dbConnectionSave.CreateCommand();
                dbCheckComman.CommandText = "UPDATE Session SET SessionCash = SessionCash * 2 WHERE SessionID=" + sessionId;
                int n = dbCheckComman.ExecuteNonQuery();
                if (n!=1)
                {
                    return "Fail";
                }
            }
            // Queries
            IDbCommand dbCheckCommand1 = dbConnectionData.CreateCommand();
            dbCheckCommand1.CommandText = "SELECT CardDuration, CardStackable From LuckOfTheWandererCards WHERE CardID=" + CardID;
            IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
            int duration = 0;
            string stack = "";
            while (dataReader1.Read())
            {
                if (!dataReader1.IsDBNull(0))
                {
                    duration = dataReader1.GetInt32(0);
                }
                stack = dataReader1.GetString(1);
            }
            if (stack=="N")
            {
                // Queries
                IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
                dbCheckCommand.CommandText = "INSERT INTO SessionLOTWCards (SessionID, CardID, Duration, Stack) VALUES (" + sessionId + "," + CardID + "," + duration + ",1)";
                int n = dbCheckCommand.ExecuteNonQuery();
                dbConnectionData.Close();
                dbConnectionSave.Close();
                if (n != 1)
                {
                    return "Fail";
                }
                else
                    return "Success";
            } else 
            {
                // Queries
                IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
                dbCheckCommand.CommandText = "SELECT ID FROM SessionLOTWCards WHERE CardID=" + CardID + " AND SessionID=" + sessionId;
                IDataReader reader = dbCheckCommand.ExecuteReader();
                int n = 0;
                while (reader.Read())
                {
                    n = reader.GetInt32(0);
                }
                if (n!=0)
                {
                    if (duration == 1000)
                    {
                        // Queries
                        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
                        dbCheckCommand3.CommandText = "UPDATE SessionLOTWCards SET Stack = Stack + 1 WHERE ID=" + n;
                        int m = dbCheckCommand3.ExecuteNonQuery();
                        dbConnectionData.Close();
                        dbConnectionSave.Close();
                        if (m!=1)
                        {
                            return "Fail";
                        } else
                        {
                            return "Success";
                        }
                    } else
                    {
                        // Queries
                        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
                        dbCheckCommand3.CommandText = "UPDATE SessionLOTWCards SET Stack = Stack + 1, Duration = Duration + " + duration + " WHERE ID=" + n;
                        int m = dbCheckCommand3.ExecuteNonQuery();
                        dbConnectionData.Close();
                        dbConnectionSave.Close();
                        if (m != 1)
                        {
                            return "Fail";
                        }
                        else
                        {
                            return "Success";
                        }
                    }
                } else
                {
                    // Queries
                    IDbCommand dbCheckCommand4 = dbConnectionSave.CreateCommand();
                    dbCheckCommand4.CommandText = "INSERT INTO SessionLOTWCards (SessionID, CardID, Duration, Stack) VALUES (" + sessionId + "," + CardID + "," + duration + ",1)";
                    int n2 = dbCheckCommand4.ExecuteNonQuery();
                    dbConnectionData.Close();
                    dbConnectionSave.Close();
                    if (n2 != 1)
                    {
                        return "Fail";
                    }
                    else
                        return "Success";
                }
            }
        }
    }
    #endregion
    #region Access Session
    public string AddNewSession(int PlayerID, string Model, string LeftWeapon, string RightWeapon,
        string FirstPower, string SecondPower, string Consumables, string PowerCD, int FuelCore)
    {
        OpenConnection();
        Debug.Log(Consumables);
        // Open DB
        dbConnectionData.Open();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT PlayerId From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "No Exist";
        } else
        {
            System.DateTime date = System.DateTime.Now;
            string currentDate = date.ToString("yyyy-MM-dd");
            // Queries
            IDbCommand dbGetLastID = dbConnectionSave.CreateCommand();
            dbGetLastID.CommandText = "SELECT SessionID FROM SESSION WHERE 1==1 ORDER BY SessionID DESC LIMIT 1;";
            IDataReader dataReader1 = dbGetLastID.ExecuteReader();
            int id = 1;
            while (dataReader1.Read())
            {
                if (dataReader1.IsDBNull(0))
                {
                    id = 1;
                } else
                {
                    id = dataReader1.GetInt32(0) + 1;
                }
            }
            // Queries            
            IDbCommand dbCheckCommand1 = dbConnectionData.CreateCommand();
            dbCheckCommand1.CommandText = "SELECT ModelStats FROM FactoryModel WHERE replace(lower(ModelName),' ','')=='" + Model.Replace(" ", "").ToLower() + "'";
            IDataReader dataReader3 = dbCheckCommand1.ExecuteReader();
            string stats = "";
            while (dataReader3.Read())
            {
                stats = dataReader3.GetString(0);
            }
            Dictionary<string, object> data = FindAnyObjectByType<GlobalFunctionController>().ConvertModelStatsToDictionaryForGameplay(stats);
            // Queries
            IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
            dbCheckCommand.CommandText = "INSERT INTO SESSION (SessionID, TotalPlayedTime, CurrentStage, CurrentStageHazard, CurrentStageVariant, CreatedDate, LastUpdate, IsCompleted, SessionCash, SessionTimelessShard, SessionFuelEnergy, Model, LeftWeapon, RightWeapon, FirstPower, SecondPower, Consumables, SessionCurrentHP, EnemyDestroyed, DamageDealt, ConsumablesCD, SessionFuelCore) VALUES " +
                "(" + id + ",'',1,1,1,'" + currentDate + "','" + currentDate + "','N',0,0,0,'" + 
                Model + "','" + LeftWeapon + "','" + RightWeapon + "','" +
                FirstPower + "','" + SecondPower + "','" + Consumables + "', "+ int.Parse(data["HP"].ToString()) + ",0,0, '"+ PowerCD + "'," + FuelCore + ");";
            int n = dbCheckCommand.ExecuteNonQuery();
            if (n!=1)
            {
                dbConnectionData.Close();
                dbConnectionSave.Close();
                return "Fail";
            } else
            {
                // Queries
                IDbCommand dbUpdCommand = dbConnectionSave.CreateCommand();
                dbUpdCommand.CommandText = "UPDATE PlayerProfile SET CurrentSession=" + id + " WHERE PlayerID=" + PlayerID;
                int k = dbUpdCommand.ExecuteNonQuery();
                dbConnectionData.Close();
                dbConnectionSave.Close();
                if (k!=1)
                {
                    return "Fail";
                } else
                {
                    return "Success";
                }
            }
        }
    }

    public string AddSessionCurrentSaveData(int PlayerID, string CurrentPlace)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                n = dataReader.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "Fail";
        }
        else
        {
            IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
            dbCheckCommand3.CommandText = "SELECT * FROM SessionCurrentSaveData WHERE SessionID=" + n;
            IDataReader reader = dbCheckCommand3.ExecuteReader();
            bool check2 = false;
            int id = -1;
            while (reader.Read())
            {
                check2 = true;
                id = reader.GetInt32(0);
            }
            if (!check2)
            {
                IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
                dbCheckCommand.CommandText = "INSERT INTO SessionCurrentSaveData (SessionID, SessionCurrentPlace) VALUES " +
                    "(" + n + ",'" + CurrentPlace + "')";
                int k = dbCheckCommand.ExecuteNonQuery();
                dbConnectionSave.Close();
                if (k != 1)
                {
                    return "Fail";
                }
                else
                {
                    return "Success";
                }
            } else
            {
                IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
                dbCheckCommand.CommandText = "UPDATE SessionCurrentSaveData SET SessionCurrentPlace='" + CurrentPlace + "'" +
                    " WHERE ID=" + id;
                int k = dbCheckCommand.ExecuteNonQuery();
                dbConnectionSave.Close();
                if (k != 1)
                {
                    return "Fail";
                }
                else
                {
                    return "Success";
                }
            }
        }
    }

    public string GetCurrentPlaceOfSession(int PlayerID)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                n = dataReader.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "";
        }
        else
        {
            IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
            dbCheckCommand3.CommandText = "SELECT SessionCurrentPlace FROM SessionCurrentSaveData WHERE SessionID=" + n;
            IDataReader reader = dbCheckCommand3.ExecuteReader();
            string place = "";
            while (reader.Read())
            {
                place = reader.GetString(0);
            }
            dbConnectionSave.Close();
            return place;
        }
    }

    public string UpdateSessionInfo(int PlayerID, string Type, string Value)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                n = dataReader.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "Fail";
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
            string cmd = "UPDATE Session SET ";
            if (Type == "LeftWeapon")
            {
                cmd += "LeftWeapon = '" + Value +"'";
            }
            else if (Type == "RightWeapon")
            {
                cmd += "RightWeapon = '" + Value + "'";
            }
            else if (Type == "FirstPower")
            {
                cmd += "FirstPower = '" + Value + "'";
            }
            else if (Type == "SecondPower")
            {
                cmd += "SecondPower = '" + Value + "'";
            }
            else if (Type=="Consumable")
            {
                cmd += "Consumables = '" + Value + "'";
            }
            dbCheckCommand3.CommandText = cmd + " WHERE SessionId=" + n;
            int k = dbCheckCommand3.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (k!=1)
            {
                return "Fail";
            } else
            return "Success";
        }
    }

    public Dictionary<string, object> GetSessionInfoByPlayerId(int PlayerId)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerId;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                n = dataReader.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return null;
        }
        else
        {
            Dictionary<string, object> datas = new Dictionary<string, object>();
            // Queries
            IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
            dbCheckCommand3.CommandText = "SELECT * From Session WHERE SessionId=" + n;
            IDataReader dataReader2 = dbCheckCommand3.ExecuteReader();
            while (dataReader2.Read())
            {
                datas.Add("SessionID", dataReader2.GetInt32(0));
                datas.Add("TotalPlayedTime", dataReader2.GetString(1));
                datas.Add("CurrentStage", dataReader2.GetInt32(2));
                datas.Add("CurrentStageHazard", dataReader2.GetInt32(3));
                datas.Add("CurrentStageVariant", dataReader2.GetInt32(4));
                datas.Add("CreatedDate", dataReader2.GetString(5));
                datas.Add("LastUpdate", dataReader2.GetString(6));
                datas.Add("IsCompleted", dataReader2.GetString(7));
                datas.Add("SessionCash", dataReader2.GetInt32(8));
                datas.Add("SessionTimelessShard", dataReader2.GetInt32(9));
                datas.Add("SessionFuelEnergy", dataReader2.GetInt32(10));
                datas.Add("Model", dataReader2.GetString(11));
                datas.Add("LeftWeapon", dataReader2.GetString(12));
                datas.Add("RightWeapon", dataReader2.GetString(13));
                datas.Add("FirstPower", dataReader2.GetString(14));
                datas.Add("SecondPower", dataReader2.GetString(15));
                datas.Add("Consumables", dataReader2.GetString(16));
                datas.Add("SessionCurrentHP", dataReader2.GetInt32(17));
                datas.Add("EnemyDestroyed", dataReader2.GetInt32(18));
                datas.Add("DamageDealt", dataReader2.GetInt32(19));
                datas.Add("ConsumablesCD", dataReader2.GetString(20));
                datas.Add("SessionFuelCore", dataReader2.GetInt32(21));
            }
            dbConnectionSave.Close();
            return datas;
        }
    }

    public string UpdateSessionFuelCore(int PlayerID, bool isIncrease)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                n = dataReader.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "Fail";
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
            dbCheckCommand3.CommandText = "UPDATE Session SET SessionFuelCore = SessionFuelCore " + (isIncrease ? "+ " : "- ")
                + "1 WHERE SessionId=" + n;
            int m = dbCheckCommand3.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (m != 1)
            {
                return "Fail";
            }
            else
            {
                return "Success";
            }
        }

    }

    public string UpdateSessionCurrentHP(int PlayerID, bool isIncrease, int Amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int n = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                n = dataReader.GetInt32(0);
            }
        }
        if (!check)
        {
            dbConnectionSave.Close();
            return "Fail";
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
            dbCheckCommand3.CommandText = "UPDATE Session SET SessionCurrentHP = SessionCurrentHP " + (isIncrease ? "+ " : "- ") 
                + Amount + " WHERE SessionId=" + n;
            int m = dbCheckCommand3.ExecuteNonQuery();
            dbConnectionSave.Close();
            if (m != 1)
            {
                return "Fail";
            }
            else
            {
                return "Success";
            }
        }
    }

    public string UpdateSessionStageData(int SessionId, int CurrentStage, int CurrentStageHazard, int CurrentStageVariant)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET CurrentStage = " + CurrentStage + ", CurrentStageHazard = " +
            CurrentStageHazard + ", CurrentStageVariant = " + CurrentStageVariant + " WHERE SessionId=" + SessionId;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public string UpdateSessionShard(int SessionId, bool IsIncrease, int amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionTimelessShard = SessionTimelessShard " + (IsIncrease? "+ " : "- ") + amount + " WHERE SessionId=" + SessionId;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n!=1)
        {
            return "Fail";
        } else
        {
            return "Success";
        }
    }

    public string UpdateSessionCashAndShard(int SessionID, bool IsIncrease, int Cash, int Shard)
    {
        OpenConnection();
        Debug.Log(SessionID + " - " + Cash + " - " + Shard);
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionTimelessShard = SessionTimelessShard " + (IsIncrease ? "+ " : "- ") + Shard +
            ", SessionCash = SessionCash " + (IsIncrease ? "+ " : "- ") + Cash + " WHERE SessionId=" + SessionID;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }

    public string UpdateSessionCashAndShardByPlayerID(int PlayerId, bool IsIncrease, int Cash, int Shard)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnectionSave.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CurrentSession From PlayerProfile WHERE PlayerId=" + PlayerId;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        int m = 0;
        bool check = false;
        while (dataReader.Read())
        {
            if (!dataReader.IsDBNull(0))
            {
                check = true;
                m = dataReader.GetInt32(0);
            }
        }
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionTimelessShard = SessionTimelessShard " + (IsIncrease ? "+ " : "- ") + Shard +
            ", SessionCash = SessionCash " + (IsIncrease ? "+ " : "- ") + Cash + " WHERE SessionId=" + m;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }

    public void UpdateSessionStat(int CurrentHP, int Enemy, int Damage, int SessionID, string Cons)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionCurrentHP = "+ CurrentHP + ", EnemyDestroyed = EnemyDestroyed + "+ Enemy + ", DamageDealt = DamageDealt + " + Damage + ", Consumables = '"+ Cons + "' WHERE SessionId=" + SessionID;
        dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
    }

    public string RepairService(int SessionID, int amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionCurrentHP = " + amount + " WHERE SessionId=" + SessionID;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }

    public string UpdateSessionFuelEnergy(int SessionId, bool IsIncrease, int amount)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionFuelEnergy = SessionFuelEnergy " + (IsIncrease ? "+ " : "- ") + amount + " WHERE SessionId=" + SessionId;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public string UpdateSessionPlayTime(int PlayerId, string playtime)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT CurrentSession FROM PlayerProfile WHERE PlayerID = " + PlayerId;
        IDataReader dataReader = dbCheckCommand1.ExecuteReader();
        int SessionID = 0;
        while(dataReader.Read())
        {
            SessionID = dataReader.GetInt32(0);
        }
        // Queries
        IDbCommand dbCheckCommand3 = dbConnectionSave.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET TotalPlayedTime = '"+ playtime +"' WHERE SessionId=" + SessionID;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnectionSave.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }

    #endregion
    #region Access Allies
    public Dictionary<string, object> GetDataAlliesById(int allyID)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select AllyName, MainTarget, AllyWeapons, AllyStats, AllyPower, AllyTier from Allies WHERE AllyID=" + allyID;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("Name", dataReader.GetString(0));
            data.Add("MainTarget", dataReader.GetString(1));
            data.Add("Weapons", dataReader.GetString(2));
            data.Add("Stats", dataReader.GetString(3));
            data.Add("Power", dataReader.GetString(4));
            data.Add("TierColor", dataReader.GetString(5));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetDataAlliesByName(string Name)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select AllyID, MainTarget, AllyWeapons, AllyStats, AllyPower, AllyTier from Allies WHERE AllyName='" + Name + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("ID", dataReader.GetInt32(0));
            data.Add("MainTarget", dataReader.GetString(1));
            data.Add("Weapons", dataReader.GetString(2));
            data.Add("Stats", dataReader.GetString(3));
            data.Add("Power", dataReader.GetString(4));
            data.Add("TierColor", dataReader.GetString(5));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }
    #endregion
    #region Access Space Zone
    public Dictionary<string,object> GetVariantCountsAndBackgroundByStageValue(int StageValue)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select VariantCount, AvailableBackgrounds, TierColor from SpaceZoneVariants WHERE StageValue=" + StageValue;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("VariantCounts", dataReader.GetInt32(0));
            data.Add("AvailableBackground", dataReader.GetString(1));
            data.Add("TierColor", dataReader.GetString(2));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        } else
        return data;
    }

    public Dictionary<string, object> GetStageZoneTemplateByStageValueAndVariant(int StageValue, int Variants)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from SpaceZoneTemplate WHERE StageValue=" + StageValue + " AND Variant=" +Variants;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("Type", dataReader.GetString(3));
            data.Add("Missions", dataReader.GetString(4));
            data.Add("FighterGroup", dataReader.GetString(5));
            if (dataReader.IsDBNull(6))
            {
                data.Add("Time", 0);
            } else
            {
                data.Add("Time", dataReader.GetInt32(6));
            }
            data.Add("SquadRating", dataReader.GetString(7));
            data.Add("AllySquad", dataReader.GetString(8));
            data.Add("EnemySquad", dataReader.GetString(9));
            if (dataReader.IsDBNull(10))
            {
                data.Add("ArmyRating", null);
                data.Add("AllyWarship", null);
                data.Add("EnemyWarship", null);
            }
            else
            {
                data.Add("ArmyRating", dataReader.GetString(10));
                if (dataReader.IsDBNull(11))
                {
                    data.Add("AllyWarship", "");
                } else
                data.Add("AllyWarship", dataReader.GetString(11));
                data.Add("EnemyWarship", dataReader.GetString(12));
            }
            if (dataReader.IsDBNull(13))
            {
                data.Add("SpawnIRate", 0);
            } else
            {
                data.Add("SpawnIRate", dataReader.GetInt32(13));
            }
            if (dataReader.IsDBNull(14))
            {
                data.Add("SpawnIIRate", 0);
            }
            else
            {
                data.Add("SpawnIIRate", dataReader.GetInt32(14));
            }
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetFighterGroupsDataByName(string GroupName)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from FighterGroup WHERE GroupName='" + GroupName + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("AlliesFighterA", dataReader.GetString(2));
            data.Add("AlliesFighterB", dataReader.GetString(3));
            data.Add("AlliesFighterC", dataReader.GetString(4));
            data.Add("EnemiesFighterA", dataReader.GetString(5));
            data.Add("EnemiesFighterB", dataReader.GetString(6));
            data.Add("EnemiesFighterC", dataReader.GetString(7));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetSpawnPositionDataByType(string PositionType)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select PositionLimitTopLeft, PositionLimitBottomRight from SpaceZonePosition WHERE PositionType='" + PositionType + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("PositionLimitTopLeft", dataReader.GetString(0));
            data.Add("PositionLimitBottomRight", dataReader.GetString(1));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetHazardAllDatas(int HazardId)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from HazardEnvironment WHERE HazardID='" + HazardId + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("HazardCode", dataReader.GetString(1));
            data.Add("HazardName", dataReader.GetString(2));
            data.Add("HazardColor", dataReader.GetString(3));
            data.Add("HazardDescription", dataReader.GetString(4));
            data.Add("HazardChance", dataReader.GetInt32(5));
            data.Add("HazardStartSpawning", dataReader.GetInt32(6));
            data.Add("HazardBackground", dataReader.GetString(7));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public List<Dictionary<string,object>> GetAvailableHazards(int SpaceZoneNo)
    {
        OpenConnection();
        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select HazardID, HazardChance from HazardEnvironment WHERE HazardStartSpawning<=" + SpaceZoneNo;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            Dictionary<string, object> dataTemp = new Dictionary<string, object>();
            dataTemp.Add("HazardID", dataReader.GetInt32(0));
            dataTemp.Add("HazardChance", dataReader.GetInt32(1));
            data.Add(dataTemp);
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetWarshipMilestoneBySpaceZoneNo(int SpaceZoneNo)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from WarshipMilestone WHERE MilestoneNumber<=" + SpaceZoneNo + " ORDER BY MilestoneNumber DESC LIMIT 1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("MilestoneID", dataReader.GetInt32(0));
            data.Add("MilestoneNumber", dataReader.GetInt32(1));
            data.Add("MilestoneAllyClassA", dataReader.GetInt32(2));
            data.Add("MilestoneAllyClassB", dataReader.GetInt32(3));
            data.Add("MilestoneAllyClassC", dataReader.GetInt32(4));
            data.Add("MilestoneEnemyClassA", dataReader.GetInt32(5));
            data.Add("MilestoneEnemyClassB", dataReader.GetInt32(6));
            data.Add("MilestoneEnemyClassC", dataReader.GetInt32(7));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string,object> GetMissionDataByValueAndVariant(int SpaceZoneValue, int SpaceZoneVariant)
    {
        OpenConnection();
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "Select * from SpaceZoneMission WHERE SpaceZoneValue=" + SpaceZoneValue + " AND Variant=" + SpaceZoneVariant;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("ID", dataReader.GetInt32(0));
            data.Add("SpaceZoneValue", dataReader.GetInt32(1));
            data.Add("Variant", dataReader.GetInt32(2));
            data.Add("Mission", dataReader.GetString(3));
            data.Add("VictoryCondition", dataReader.GetString(4));
            data.Add("DefeatCondition", dataReader.GetString(5));
        }
        dbConnectionData.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }
    #endregion
    #region Access Statistic
    public void CreateNewPlayerStatistic(string name)
    {
        OpenConnection();
        int PlayerId = 0;
        // Open DB
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE Name='" + name + "'";
        IDataReader reader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (reader.Read())
        {
            PlayerId = reader.GetInt32(0);
            check = true;
        }
        if (!check)
        {
            dbConnectionSave.Close();
        } else
        {
            IDbCommand dbCommand = dbConnectionSave.CreateCommand();
            dbCommand.CommandText = "INSERT INTO Statistic (PlayerID,EnemyDefeated,MaxSZReach,TotalShard,TotalCash,TotalEnemyDefeated,TotalDamageDealt,TotalSalaryReceived,Rank,TotalShardSpent,TotalCashSpent,TotalDailyMissionDone) " +
                "VALUES (" + PlayerId + ",'EI-0|EII-0|EIII-0|WS-0',0,5,500,0,0,0,0,0,0,0)";
            dbCommand.ExecuteNonQuery();
            dbConnectionSave.Close();
        }        
    }

    public Dictionary<string, object> GetPlayerAchievement(int id)
    {
        OpenConnection();
        Dictionary<string, object> PA = new Dictionary<string, object>();
        dbConnectionSave.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionSave.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM Statistic WHERE PlayerID='" + id + "'";
        IDataReader reader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (reader.Read())
        {
            PA.Add("EnemyDefeated", reader.GetString(2));
            PA.Add("MaxSZReach", reader.GetInt32(3));
            PA.Add("TotalShard", reader.GetInt32(4));
            PA.Add("TotalCash", reader.GetInt32(5));
            PA.Add("TotalMission", reader.GetInt32(12));
            PA.Add("TotalEnemyDefeated", reader.GetInt32(6));
            check = true;
        }
        if (!check)
        {
            dbConnectionSave.Close();
        }
        return PA;
    }

    public void UpdateGameplayStatistic(Dictionary<string, object> data)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = "UPDATE Statistic SET " +
            "EnemyDefeated = '" + data["EnemyDefeated"].ToString() + "', MaxSZReach = " + int.Parse(data["SZMaxReach"].ToString()) + " WHERE PlayerID = "+ int.Parse(data["PlayerID"].ToString()) + "";
        dbCommand.ExecuteNonQuery();
        dbConnectionSave.Close();
    }

    public void UpdateEconomyStatistic(int id, int shard, int cash, string type) 
    {
        OpenConnection();
        string query = "";
        switch(type)
        {
            case "ConvertCash":
                query = "UPDATE Statistic SET " +
                    "TotalCash = TotalCash + " + cash + " WHERE PlayerID = " + id + ""; break;
            case "ConvertShard":
                query = "UPDATE Statistic SET " +
                    "TotalShard = TotalShard + " + shard + " WHERE PlayerID = " + id + ""; break;
            case "ReceiveSalary":
                query = "UPDATE Statistic SET " +
                    "TotalSalaryReceived = TotalSalaryReceived + " + cash + " WHERE PlayerID = " + id + ""; break;
            case "Spent":
                query = "UPDATE Statistic SET " +
                   "TotalCashSpent = TotalCashSpent + " + cash + ",  TotalShardSpent = TotalShardSpent + " + shard + " WHERE PlayerID = " + id + ""; break;
        }
        // Open DB
        dbConnectionSave.Open();
        IDbCommand dbCommand = dbConnectionSave.CreateCommand();
        dbCommand.CommandText = query;
        dbCommand.ExecuteNonQuery();
        dbConnectionSave.Close();
    }
    public void UpdateDailyMissionStatistic(int playerId)
    {
        OpenConnection();
        // Open DB
        dbConnectionSave.Open();
        // Queries    
        IDbCommand dbCheckCommand1 = dbConnectionSave.CreateCommand();
        dbCheckCommand1.CommandText = "UPDATE Statistic SET TotalDailyMissionDone = TotalDailyMissionDone + 1 WHERE PlayerID = " + playerId + "";
        dbCheckCommand1.ExecuteNonQuery();
        dbConnectionSave.Close();
        
    }
    #endregion
    #region Access ArsenalService
    public List<Dictionary<string, object>> GetAllArsenalService()
    {
        OpenConnection();
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        Dictionary<string, object> service;
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM ArsenalService";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        while (dataReader.Read())
        {
            service = new Dictionary<string, object>();
            service.Add("ID", dataReader.GetInt32(0));
            service.Add("Name", dataReader.GetString(1));
            service.Add("Price", dataReader.GetString(2));
            service.Add("Effect", dataReader.GetInt32(3));
            list.Add(service);
        }
        dbConnectionData.Close();
        return list;
    }

    public Dictionary<string, object> GetArsenalServiceById(int id)
    {
        OpenConnection();
        Dictionary<string, object> service = new Dictionary<string, object>();
        // Open DB
        dbConnectionData.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnectionData.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM ArsenalService WHERE ID = "+ id +"";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();

        while (dataReader.Read())
        {
            service.Add("ID", dataReader.GetInt32(0));
            service.Add("Name", dataReader.GetString(1));
            service.Add("Price", dataReader.GetString(2));
            service.Add("Effect", dataReader.GetInt32(3));
        }
        dbConnectionData.Close();
        return service;
    }
    #endregion
}
