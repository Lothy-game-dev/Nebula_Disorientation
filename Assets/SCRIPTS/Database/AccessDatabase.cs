using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class AccessDatabase : MonoBehaviour
{
    private IDbConnection dbConnection;
    #region Common
    /// <summary>
    /// Get Real name of an item from it no space no capitalize name
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="Type">Weapon/Power/Model/Consumable</param>
    /// <returns></returns>
    public string GetItemRealName(string ItemName, string Type)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        string realName = "";
        if (Type=="Weapon")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT WeaponName FROM ArsenalWeapon WHERE replace(replace(lower(WeaponName),' ',''),'-','')='" + ItemName.Replace("-","").Replace(" ","").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        } else if (Type=="Power")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT PowerName FROM ArsenalPower WHERE replace(replace(lower(PowerName),' ',''),'-','')='" + ItemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        } else if (Type=="Model")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT ModelName FROM FactoryModel WHERE replace(replace(lower(ModelName),' ',''),'-','')='" + ItemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        } else if (Type=="Consumable")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT ItemName FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')='" + ItemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
            IDataReader reader = dbCheckCommand.ExecuteReader();
            while (reader.Read())
            {
                realName = reader.GetString(0);
            }
        }
        return realName;
    }
    #endregion
    #region Access Player Profile
    public string CreateNewPlayerProfile(string name)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "Exist";
        }
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "INSERT INTO PlayerProfile (Name,Rank,CurrentSession,FuelCell,FuelEnergy,Cash,TimelessShard,DailyIncome,DailyIncomeReceived,LastFuelCellUsedTime,CollectedSalaryTime) " +
            "VALUES ('" + name + "',null,null,10,0,500,5,500,'N',null,null)";
        int n = dbCommand.ExecuteNonQuery();
        if (n == 0)
        {
            return "Fail";
        }
        dbConnection.Close();
        return "Success";
    }

    public bool UpdatePlayerProfileRank(int profileId, int rank)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return false;
        }
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "UPDATE PlayerProfile SET Rank=" + rank + " WHERE PlayerId=" + profileId;
        dbCommand.ExecuteNonQuery();
        dbConnection.Close();
        return true;
    }

    public List<List<string>> GetAllNameAndRankFromPlayerProfile()
    {
        List<List<string>> result = new List<List<string>>();
        List<string> Names = new List<string>();
        List<string> Ranks = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT Name,Rank FROM PlayerProfile WHERE 1=1";
        IDataReader dataReader = dbCommand.ExecuteReader();
        while (dataReader.Read())
        {
            Names.Add(dataReader.GetString(0));
            object rankTemp = dataReader.GetValue(1);
            if (rankTemp != null && rankTemp.ToString().Length > 0)
            {
                IDbCommand dbCommand2 = dbConnection.CreateCommand();
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
        }
        dbConnection.Close();
        result.Add(Names);
        result.Add(Ranks);
        return result;
    }

    public string DeletePlayerProfileByName(string name)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "DELETE FROM PlayerProfile WHERE Name='" + name + "'";
        int n = dbCommand.ExecuteNonQuery();
        if (n == 0)
        {
            return "Fail";
        }
        dbConnection.Close();
        return "Success";
    }
    public Dictionary<string, object> GetPlayerInformationById(int PlayerId)
    {
        if (PlayerId == -1)
        {
            return null;
        } else
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            // Open DB
            dbConnection = new SqliteConnection("URI=file:Database.db");
            dbConnection.Open();
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                } else
                {
                    values.Add("RankId", dataReader.GetInt32(2));
                    IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
                    dbCheckCommand2.CommandText = "SELECT RankName,TierColor FROM RankSystem WHERE RankId=" + dataReader.GetInt32(2);
                    IDataReader dataReader2 = dbCheckCommand2.ExecuteReader();
                    string rank = "Unranked";
                    string color = "#ffffff";
                    while (dataReader2.Read())
                    {
                        rank = dataReader2.GetString(0);
                        color = dataReader2.GetString(1);
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
                if (dataReader.IsDBNull(11))
                {
                    values.Add("CollectedSalaryTime", 0);
                }
                else
                {
                    values.Add("CollectedSalaryTime", dataReader.GetString(11));
                }
                
            }
            dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        if (cell==-1)
        {
            dbConnection.Close();
            return "No Data";
        } else
        {
            if (cell>=10)
            {
                dbConnection.Close();
                return "Full";
            } else 
            {
                IDbCommand dbCheck = dbConnection.CreateCommand();
                dbCheck.CommandText = "UPDATE PlayerProfile SET FuelCell = FuelCell + 1 " + 
                    (cell == 9 ? ", LastFuelCellUsedTime = '' " : "") + "WHERE PlayerId =" + PlayerID;
                int n = dbCheck.ExecuteNonQuery();
                dbConnection.Close();
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

    public string CheckIfConvertable(int PlayerId, string ConvertFrom, string ConvertTo, string FromAmount, string ToAmount)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
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
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                dbConnection.Close();
                return "No Exist";
            }
            if (to.Equals("FuelCell"))
            {
                IDbCommand dbCommand2 = dbConnection.CreateCommand();
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
                    dbConnection.Close();
                    return "Fail";
                }
                else
                {
                    if (fuelCheck > 10)
                    {
                        dbConnection.Close();
                        return "Over Limit";
                    }
                }
            }
            IDbCommand dbCommand3 = dbConnection.CreateCommand();
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
                dbConnection.Close();
                return "Fail";
            }
            else
            {
                if (k < 0)
                {
                    dbConnection.Close();
                    return "Not Enough";
                }
            }
            dbConnection.Close();
            return "Success";
        }
        else
        {
            dbConnection.Close();
            return "Fail";
        }
    }
    public string ConvertCurrencyById(int PlayerId, string ConvertFrom, string ConvertTo, string FromAmount, string ToAmount)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
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
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                dbConnection.Close();
                return "No Exist";
            } else
            {
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = "UPDATE PlayerProfile SET " + from + " = " + from + " - " + FromAmount +
                    "," + to + " = " + to + " + " + ToAmount + " WHERE PlayerID=" + PlayerId;
                int n = dbCommand.ExecuteNonQuery();
                dbConnection.Close();
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
            dbConnection.Close();
            return "Fail";
        }
    }

    public string RechargeTimelessShard(int PlayerId, int amount)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "No Exist";
        } else
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerProfile SET TimelessShard = TimelessShard + " + amount.ToString() + " WHERE PlayerID=" + PlayerId;
            int n = dbCommand.ExecuteNonQuery();
            if (n != 1)
            {
                dbConnection.Close();
                return "Fail";
            } else
            {
                dbConnection.Close();
                return "Success";
            }
        }
    }

    public string DecreaseCurrencyAfterBuy(int PlayerId, int Cash, int TimelessShard)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "Not Exist";
        }
        if (cashOwn < Cash)
        {
            dbConnection.Close();
            return "Not Enough Cash";
        }
        if (timelessShardOwn < TimelessShard)
        {
            dbConnection.Close();
            return "Not Enough Shard";
        }
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Cash = Cash - " + Cash
            + ", TimelessShard = TimelessShard - " + TimelessShard + " WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        if (n!=1)
        {
            dbConnection.Close();
            return "Fail";
        } else
        {
            dbConnection.Close();
            return "Success";
        }
    }

    public string IncreaseCurrencyAfterSell(int PlayerId, int Cash, int TimelessShard)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "Not Exist";
        }
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Cash = Cash + " + Cash
            + ", TimelessShard = TimelessShard + " + TimelessShard + " WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        if (n != 1)
        {
            dbConnection.Close();
            return "Fail";
        }
        else
        {
            dbConnection.Close();
            return "Success";
        }
    }


    public string UpdatePlayerProfileName(int PlayerId, string name)
    {
        int checkID = 0;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE " +
           "PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            checkID = dataReader.GetInt32(0);
        }
        if (checkID == 0)
        {
            dbConnection.Close();
            return "Not Exist";
        }
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Name = '"+ name +"'" +
            "" +
            " WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET DailyIncomeReceived = 'N' WHERE 1 = 1";
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnection.Close();
        if (n != 1)
        {
            return "Fail";
        }
        else
        {
            return "Success";
        }
    }
    public string CollectSalary(int PlayerId, int Cash)
    {
        int checkID = 0;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM PlayerProfile WHERE " +
           "PlayerID=" + PlayerId;
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            checkID = dataReader.GetInt32(0);
        }
        if (checkID == 0)
        {
            dbConnection.Close();
            return "Not Exist";
        }
        System.DateTime date = System.DateTime.Now;
        string collectedDate = date.ToString("dd/MM/yyyy");
        Debug.Log(collectedDate);
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET Cash = Cash + " + Cash
            + ", CollectedSalaryTime = '" + collectedDate + "', DailyIncomeReceived = 'Y' WHERE PlayerID=" + PlayerId;
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnection.Close();
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
    #region Access Current Play Session
    public string AddPlaySession(string PlayerName)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "INSERT INTO CurrentPlaySession (PlayerId,SessionStartTime,SessionEndTime) VALUES" +
            "(" + id.ToString() + ",datetime('now', '+7 hours'),null)";
        int n = dbCommand.ExecuteNonQuery();
        dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PlayerId FROM CurrentPlaySession ORDER BY SessionStartTime DESC LIMIT 1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int id = -1;
        while (dataReader.Read())
        {
            id = dataReader.GetInt32(0);
        }
        dbConnection.Close();
        return id;
    }
    #endregion
    #region Access Daily Mission
    public int NumberOfDailyMissionById(int PlayerId)
    {
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT COUNT(*) FROM PlayerDailyMission WHERE PlayerID =" + PlayerId + " AND MissionDate='" + currentDate + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int count = 0;
        while (dataReader.Read())
        {
            count = dataReader.GetInt32(0);
        }
        dbConnection.Close();
        return count;
    }
    public string GenerateDailyMission(int PlayerId, int number)
    {
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT COUNT(*) FROM DailyMissions";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int numberOfDailyMissions = 0;
        while (dataReader.Read())
        {
            numberOfDailyMissions = dataReader.GetInt32(0);
        }
        if (numberOfDailyMissions == 0)
        {
            dbConnection.Close();
            return "Fail";
        }
        IDbCommand dbCommand2 = dbConnection.CreateCommand();
        dbCommand2.CommandText = "SELECT MissionID FROM PlayerDailyMission WHERE PlayerId=" + PlayerId;
        List<int> alreadyMission = new List<int>();
        while (dataReader.Read())
        {
            alreadyMission.Add(dataReader.GetInt32(0));
        }
        IDbCommand dbCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
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
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        List<List<string>> dailyMissions = new List<List<string>>();
        dailyMissions.Add(new List<string>());
        dailyMissions.Add(new List<string>());
        dailyMissions.Add(new List<string>());
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT MissionId, MissionProgess FROM PlayerDailyMission WHERE PlayerId=" + PlayerId + " AND MissionDate='" + currentDate + "' AND IsComplete='N'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check1 = false;
        while (dataReader.Read())
        {
            check1 = true;
            dailyMissions[2].Add(dataReader.GetInt32(1).ToString());
            IDbCommand dbCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return dailyMissions;
    }
    public void UpdateDailyMissionProgess(int PlayerId, string mission, int amount)   
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT MissionId FROM DailyMissions WHERE MissionVerb='" + mission + "'";
        IDataReader dataReader2 = dbCommand.ExecuteReader();
        int id = 0;
        while (dataReader2.Read())
        {
            id = dataReader2.GetInt32(0);
        }      

        string query = "UPDATE PlayerDailyMission SET MissionProgess = MissionProgess + "+ amount + " WHERE PlayerId=" + PlayerId + " AND MissionId = " + id + "";
        IDbCommand dbCommand2 = dbConnection.CreateCommand();
        dbCommand2.CommandText = query;
        dbCommand2.ExecuteNonQuery();
        dbConnection.Close();
    }
    public void DailyMissionDone(int PlayerID, string mission)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT MissionId FROM DailyMissions WHERE MissionVerb='" + mission + "'";
        IDataReader dataReader2 = dbCommand.ExecuteReader();
        int id = 0;
        while (dataReader2.Read())
        {
            id = dataReader2.GetInt32(0);
        }

        string query = "UPDATE PlayerDailyMission SET IsComplete = 'Y' WHERE PlayerId=" + PlayerID + " AND MissionId = " + id + "";
        IDbCommand dbCommand2 = dbConnection.CreateCommand();
        dbCommand2.CommandText = query;
        dbCommand2.ExecuteNonQuery();

        //Update statistic
        IDbCommand dbCheckCommand1 = dbConnection.CreateCommand();
        dbCheckCommand1.CommandText = "UPDATE Statistic SET TotalDailyMissionDone = TotalDailyMissionDone + 1 WHERE PlayerID = " + PlayerID + "";
        dbCheckCommand1.ExecuteNonQuery();

        //Reward
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "UPDATE PlayerProfile SET FuelEnergy = FuelEnergy + 100, TimelessShard = TimelessShard + 5, Cash = Cash + 2500 WHERE PlayerID = " + PlayerID + "";
        dbCheckCommand2.ExecuteNonQuery();
        dbConnection.Close();
        
    }
    #endregion
    #region Option
    public Dictionary<string, object> GetOption()
    {
        Dictionary<string, object> option = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return option;
    }
    public void UpdateOptionSetting(int MVolume, int Music, int Sound, int Fps, string Resol)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "UPDATE Option SET " +
            "MasterVolume = " + MVolume + ", " +
            "MusicVolume = " + Music + ", " +
            "SoundFx = " + Sound + ", " +
            "Fps= " + Fps + ", " +
            "Resolution = '" + Resol + "' Where 1=1";

        dbCheckCommand.ExecuteNonQuery();
        dbConnection.Close();
    }
    #endregion
    #region Access Arsenal Weapon
    public List<List<string>> GetAllArsenalWeapon()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> weaplist;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }

    public string CheckWeaponPowerPrereq(int PlayerID, string ItemName, string Type)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        if (Type == "Weapon")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                dbConnection.Close();
                return "No Prereq";
            }
            else
            {
                IDbCommand dbCommand = dbConnection.CreateCommand();
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
                    dbConnection.Close();
                    return "Pass";
                }
                else
                {
                    IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
                    dbCheckCommand2.CommandText = "SELECT WeaponName, TierColor FROM ArsenalWeapon WHERE WeaponID=" + n;
                    IDataReader dataReader3 = dbCheckCommand2.ExecuteReader();
                    string name = "";
                    while (dataReader3.Read())
                    {
                        name = "<color=" + dataReader3.GetString(1) + ">" + dataReader3.GetString(0) + "</color>";
                    }
                    dbConnection.Close();
                    return name;
                }
            }
        }
        else if (Type == "Power")
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                dbConnection.Close();
                return "No Prereq";
            }
            else
            {
                IDbCommand dbCommand = dbConnection.CreateCommand();
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
                    dbConnection.Close();
                    return "Pass";
                }
                else
                {
                    IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
                    dbCheckCommand2.CommandText = "SELECT PowerName, TierColor FROM ArsenalPower WHERE PowerID=" + n;
                    IDataReader dataReader3 = dbCheckCommand2.ExecuteReader();
                    string name = "";
                    while (dataReader3.Read())
                    {
                        name = "<color=" + dataReader3.GetString(1) + ">" + dataReader3.GetString(0) + "</color>";
                    }
                    dbConnection.Close();
                    return name;
                }
            }
        }
        else return "Unidentified";

    }

    public List<string> GetAllWeaponName()
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName FROM ArsenalWeapon WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }

    public List<string> GetAllOwnedWeapon(int PlayerID)
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ArsenalWeapon.WeaponName FROM ArsenalWeapon " +
            "inner join PlayerOwnership WHERE PlayerID=" + PlayerID + " AND ItemType='Weapon' AND ArsenalWeapon.WeaponID = PlayerOwnership.ItemID ORDER BY ArsenalWeapon.WeaponID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }

    public string GetWeaponRealName(string WeaponName)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName, TierColor FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')='" + WeaponName.Replace(" ","").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        string str = "";
        while (dataReader.Read())
        {
            str = "<color=" + dataReader.GetString(1) + ">" + dataReader.GetString(0) + "</color>";
        }
        return str;
    }
    
    public List<string> GetOwnedWeaponExceptForName(int PlayerID, string WeaponName)
    {
        Dictionary<string, int> DictOwned = new Dictionary<string, int>();
        Dictionary<string, string> NameConversion = new Dictionary<string, string>();
        string checkName = WeaponName.Replace(" ", "").ToLower();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ArsenalWeapon.WeaponName, Quantity FROM ArsenalWeapon " +
            "inner join PlayerOwnership WHERE PlayerID=" + PlayerID + " AND ItemType='Weapon' AND ArsenalWeapon.WeaponID = PlayerOwnership.ItemID ORDER BY ArsenalWeapon.WeaponID ASC";
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
        return new List<string>(DictOwned.Keys);
    }
    public string AddStarterGiftWeapons(int PlayerID)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "INSERT INTO PlayerOwnership (PlayerID,ItemType,ItemID,Quantity) VALUES " +
                "(" + PlayerID + ",'Weapon',1,2)";
            int check = dbCommand.ExecuteNonQuery();
            dbConnection.Close();
            if (check==1)
            {
                return "2";
            } else
            {
                return "Fail";
            }
        } else if (n==1)
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "UPDATE PlayerOwnership SET Quantity = 2 WHERE " +
                "PlayerId=" + PlayerID + " AND ItemID=1 AND ItemType='Weapon'";
            int check = dbCommand.ExecuteNonQuery();
            dbConnection.Close();
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
        Dictionary<string, object> list = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT WeaponName,WeaponType,WeaponDescription,WeaponStats,TierColor FROM ArsenalWeapon WHERE replace(lower(WeaponName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
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
            break;
        }
        dbConnection.Close();
        if (!check)
        {
            return null;
        } else
        {
            return list;
        }
    }
    #endregion
    #region Access Factory Fighter/Model
    public List<List<string>> GetAllFighter()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> fighterlist;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }
    public List<string> GetAllModelName()
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ModelName FROM FactoryModel WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }

    public List<string> GetAllOwnedModel(int PlayerID)
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT FactoryModel.ModelName FROM FactoryModel " +
            "inner join PlayerOwnership WHERE PlayerID=" + PlayerID + " AND ItemType='Model' AND FactoryModel.ModelID = PlayerOwnership.ItemID  ORDER BY FactoryModel.ModelID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }
    public string GetFighterStatsByName(string name)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ModelStats FROM FactoryModel WHERE replace(lower(ModelName),' ','')=='" + name.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        string stats = "";
        while (dataReader.Read())
        {
            check = true;
            stats = dataReader.GetString(0);
        }
        dbConnection.Close();
        if (!check)
        {
            return "Fail";
        } else
        {
            return stats;
        }
    }
    #endregion
    #region Access Power
    public List<List<string>> GetAllPower()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> weaplist;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                weaplist.Add("0");
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
        dbConnection.Close();
        return list;
    }

    public List<string> GetAllPowerName()
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT PowerName FROM ArsenalPower WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }

    public List<string> GetAllOwnedPower(int PlayerID)
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ArsenalPower.PowerName FROM ArsenalPower " +
            "inner join PlayerOwnership WHERE PlayerID=" + PlayerID + " AND ItemType='Power' AND ArsenalPower.PowerID = PlayerOwnership.ItemID ORDER BY ArsenalPower.PowerID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }

    public List<string> GetAllOwnedPowerExceptForName(int PlayerID, string Name)
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
            // Open DB
            dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ArsenalPower.PowerName FROM ArsenalPower " +
            "inner join PlayerOwnership WHERE PlayerID=" + PlayerID + " AND ItemType='Power' AND ArsenalPower.PowerID = PlayerOwnership.ItemID ORDER BY ArsenalPower.PowerID ASC";
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
        dbConnection.Close();
        return list;
    }
    public Dictionary<string, object> GetPowerDataByName(string name)
    {
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        } else
        {
            return datas;
        }
    }
    #endregion
    #region Access Consumables
    public Dictionary<string, object> GetConsumableDataByName(string itemName)
    {
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ItemName, ItemDescription, StockPerDays, ItemEffect, EffectDuration, MaxStack, ItemPrice, Cooldown, TierColor FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')=='" + itemName.Replace("-", "").Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        } else
            return datas;
    }

    public int GetStackLimitOfConsumableByName(string itemName)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT MaxStack FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')=='" + itemName.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        int n = 0;
        while (dataReader.Read())
        {
            check = true;
            n = dataReader.GetInt32(0);
        }
        dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
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
        Dictionary<string, int> data = new Dictionary<string, int>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT SpaceShop.ItemName,PlayerOwnership.Quantity" +
            " FROM SpaceShop inner join PlayerOwnership WHERE PlayerID=" + PlayerID +
            " AND ItemType='Consumable' AND SpaceShop.ItemID=PlayerOwnership.ItemID ORDER BY SpaceShop.ItemID ASC";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            data.Add(dataReader.GetString(0).Replace("-", "").Replace(" ", ""), dataReader.GetInt32(1));
        }
        dbConnection.Close();
        return data;
    }

    public List<string> GetSpaceShopItemNameSearchByName(string name)
    {
        List<string> list = new List<string>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ItemName FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','') LIKE '%" + name.Replace(" ", "").Replace("-", "").ToLower() + "%'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetString(0));
        }
        dbConnection.Close();
        return list;
    }
    public List<List<string>> GetAllConsumable()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> ConsuList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }
    #endregion
    #region Access Rank
    public Dictionary<string, object> GetRankById(int id)
    {
        Dictionary<string, object> rank = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                rank.Add("RankId", dataReader.GetInt32(0).ToString());
                rank.Add("RankName", dataReader.GetString(1));
                rank.Add("RankTier", dataReader.GetString(7));
            }   
        }
        if (!check1) return null;
        dbConnection.Close();
 
        return rank;
     }

    public List<List<string>> GetAllRank()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> RankList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "Select * from RankSystem";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            RankList = new List<string>();
            check = true;
            RankList.Add(dataReader.GetInt32(0).ToString());
            RankList.Add(dataReader.GetString(1));
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
            if (dataReader.IsDBNull(6))
            {
                RankList.Add("N/A");
            }
            else
            {
                RankList.Add(dataReader.GetString(6));
            }           
            RankList.Add(dataReader.GetString(7));
            list.Add(RankList);
        }
        if (!check) return null;
        dbConnection.Close();
        return list;
    }

    #endregion
    #region Access Enemy
    public List<List<string>> GetAllEnemy()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> EnemyList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }

    public Dictionary<string,object> GetDataEnemyById(int enemyID)
    {
        Dictionary<string,object> data = new Dictionary<string,object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        } else
        return data;
    }
    #endregion
    #region Access Warship
    public List<List<string>> GetAllWarship()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> WarshipList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }

    public Dictionary<string, object> GetWSById(int ID)
    {
        Dictionary<string, object> WSDict = new Dictionary<string, object>();
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        return WSDict;
    }
    #endregion
    #region Access Ownership
    public int GetCurrentOwnedNumberOfConsumableByName(int PlayerID, string itemName)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnection.CreateCommand();
        dbCheckCommand1.CommandText = "SELECT ItemID FROM SpaceShop WHERE replace(replace(lower(ItemName),'-',''),' ','')='" + itemName.Replace("-","").Replace(" ","").ToLower() + "'";
        IDataReader dataReader1 = dbCheckCommand1.ExecuteReader();
        int n = 0;
        while (dataReader1.Read())
        {
            n = dataReader1.GetInt32(0);
        }
        if (n!=0)
        {
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE " +
                "PlayerID=" + PlayerID + " AND ItemID=" + n + " AND ItemType='Consumable'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quan = 0;
            while (dataReader.Read())
            {
                quan = dataReader.GetInt32(0);
            }
            dbConnection.Close();
            return quan;
        } else
        {
            dbConnection.Close();
            return 0;
        }
    }

    public int GetCurrentOwnershipWeaponPowerModelByName(int PlayerID, string itemName, string Type)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        int id = -1;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnection.CreateCommand();
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
            IDbCommand dbCheckPower = dbConnection.CreateCommand();
            dbCheckPower.CommandText = "SELECT PowerID FROM ArsenalPower WHERE replace(lower(PowerName),' ','')='" + itemName.Replace(" ", "").ToLower() + "'";
            IDataReader dataReaderPower = dbCheckPower.ExecuteReader();
            while (dataReaderPower.Read())
            {
                id = dataReaderPower.GetInt32(0);
                break;
            }
        } else if ("Model".Equals(Type))
        {
            IDbCommand dbCheckModel = dbConnection.CreateCommand();
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
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT Quantity FROM PlayerOwnership WHERE " +
                "PlayerID=" + PlayerID + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
            IDataReader dataReader = dbCheckCommand.ExecuteReader();
            int quan = 0;
            while (dataReader.Read())
            {
                quan = dataReader.GetInt32(0);
            }
            dbConnection.Close();
            return quan;
        }
        else
        {
            dbConnection.Close();
            return -1;
        }
    }

    public int GetCurrentOwnershipWeaponPowerModel(int PlayerID, string Type) 
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        int id = 0;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnection.CreateCommand();
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
            IDbCommand dbCheckPower = dbConnection.CreateCommand();
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
            IDbCommand dbCheckCons = dbConnection.CreateCommand();
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
            IDbCommand dbCheckModel = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "Not Found";
        } else
        {
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = "UPDATE PlayerOwnership SET Quantity = Quantity + " + Quantity
                        + " WHERE PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
                int n = dbCommand.ExecuteNonQuery();
                dbConnection.Close();
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
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = "INSERT INTO PlayerOwnership (PlayerID,ItemType,ItemID,Quantity) VALUES" +
                    "(" + PlayerId + ",'" + Type + "'," + id + "," + Quantity + ")";
                int n = dbCommand.ExecuteNonQuery();
                dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        int id = 0;
        // Queries
        if ("Weapon".Equals(Type))
        {
            IDbCommand dbCheckWeapon = dbConnection.CreateCommand();
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
            IDbCommand dbCheckPower = dbConnection.CreateCommand();
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
            IDbCommand dbCheckCons = dbConnection.CreateCommand();
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
            IDbCommand dbCheckModel = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "Not Enough Item";
        }
        else
        {
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                    IDbCommand dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = "UPDATE PlayerOwnership SET Quantity = Quantity - " + Quantity
                        + " WHERE PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'";
                    int n = dbCommand.ExecuteNonQuery();
                    dbConnection.Close();
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
                    IDbCommand dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = "DELETE FROM PlayerOwnership WHERE PlayerID=" + PlayerId + " AND ItemID=" + id + " AND ItemType='" + Type + "'"; ;
                    int n = dbCommand.ExecuteNonQuery();
                    dbConnection.Close();
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
                    dbConnection.Close();
                    return "Not Enough Item";
                }
            } else
            {
                dbConnection.Close();
                return "Not Enough Item";
            }
        }
    }
    #endregion
    #region Access Purchase History
    public string AddPurchaseHistory(int PlayerID, int itemID, string Type, int Quantity, bool isBuy)
    {
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "INSERT INTO PurchaseHistory (PlayerID, ItemType, ItemID, Quantity, BuyOrSell, PurchaseDate) VALUES " +
            "(" + PlayerID + ",'" + Type + "'," + itemID + "," + Quantity + ",'" + (isBuy ? "Buy" : "Sell") + "','" + currentDate + "')";
        int n = dbCommand.ExecuteNonQuery();
        dbConnection.Close();
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
        System.DateTime date = System.DateTime.Now;
        string currentDate = date.ToString("yyyy-MM-dd");
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return sum;
    }
    #endregion
    #region Access Tutorial
    public List<List<string>> GetAllTutorial() 
    {
        List<List<string>> list = new List<List<string>>();
        List<string> TList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
        
    }
    #endregion
    #region Access SpaceStation
    public List<List<string>> GetAllSpaceStation()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> SpaceStationList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }

    public Dictionary<string, object> GetSpaceStationById(int id)
    {
        Dictionary<string, object> WSDict = new Dictionary<string, object>();
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        return WSDict;
    }
    #endregion
    #region Access Damage Element
    public List<List<string>> GetAllDMGElement()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> DMGElementList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }
    #endregion
    #region Access Attribute
    public List<List<string>> GetAllAttribute()
    {
        List<List<string>> list = new List<List<string>>();
        List<string> DMGElementList;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        return list;
    }
    #endregion
    #region Access Collect salary history
    public string SalaryCollected(int PlayerId)
    {
        string date = System.DateTime.Now.ToString("dd/MM/yyyy");
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "INSERT INTO CollectSalaryHistory (PlayerId, CollectedTime) VALUES ( "+PlayerId+" , '"+ date +"')";
        int n = dbCheckCommand2.ExecuteNonQuery();
        dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT Count(*) FROM CollectSalaryHistory where PlayerId = "+ PlayerId +" and CollectedTime = '"+ date +"'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        int quan = 0;
        while (dataReader.Read())
        {
            quan = dataReader.GetInt32(0);
        }
        dbConnection.Close();
        return quan;
    }


    #endregion
    #region Access LOTW
    public List<int> GetListIDAllLOTW(int tier)
    {
        List<int> list = new List<int>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT CardID From LuckOfTheWandererCards WHERE " + (tier > 0 && tier <= 3 ? "CardTier=" + tier : "1==1");
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        while (dataReader.Read())
        {
            list.Add(dataReader.GetInt32(0));
        }
        dbConnection.Close();
        return list;
    }

    public Dictionary<string, object> GetLOTWInfoByID(int id)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        } else
        return dict;
    }
    public bool CheckLOTWRepetable(int PlayerID)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return false;
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT * FROM SessionLOTWCards WHERE SessionID=" + sessionId + " AND CardID=34"; 
            IDataReader dataReader2 = dbCheckCommand.ExecuteReader();
            bool check2 = false;
            while (dataReader2.Read())
            {
                check2 = true;
            }
            dbConnection.Close();
            return check2;
        }
    }

    public List<Dictionary<string, object>> GetLOTWInfoOwnedByID(int PlayerID)
    {
        List<Dictionary<string, object>> listFinal = new List<Dictionary<string, object>>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand1 = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return null;
        }
        else
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT LuckOfTheWandererCards.CardID, LuckOfTheWandererCards.CardName, " +
                "LuckOfTheWandererCards.CardEffect, SessionLOTWCards.Duration, SessionLOTWCards.Stack, LuckOfTheWandererCards.CardStackable, " +
                "LuckOfTheWandererCards.TierColor From SessionLOTWCards INNER JOIN LuckOfTheWandererCards " +
                "WHERE SessionID=" + sessionId + " AND LuckOfTheWandererCards.CardID = SessionLOTWCards.CardID " +
                "ORDER BY LuckOfTheWandererCards.CardTier ASC, SessionLOTWCards.Duration DESC, LuckOfTheWandererCards.CardID ASC";
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
            dbConnection.Close();
            return listFinal;
        }
    }
    public List<int> GetAllCardIdInCurrentSession(int PlayerID)
    {
        List<int> list = new List<int>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return null;
        } else
        {
            // Queries
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "SELECT CardID From SessionLOTWCards WHERE SessionID=" + sessionId;
            IDataReader dataReader2 = dbCheckCommand.ExecuteReader();
            while (dataReader2.Read())
            {
                list.Add(dataReader2.GetInt32(0));
            }
            dbConnection.Close();
            return list;
        }
    }

    public string AddCardToCurrentSession(int PlayerID, int CardID)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return "Fail";
        }
        else
        {
            if (CardID==34)
            {
                // Queries
                IDbCommand dbCheckComman = dbConnection.CreateCommand();
                dbCheckComman.CommandText = "UPDATE Session SET SessionCash = SessionCash * 2 WHERE SessionID=" + sessionId;
                int n = dbCheckComman.ExecuteNonQuery();
                if (n!=1)
                {
                    return "Fail";
                }
            }
            // Queries
            IDbCommand dbCheckCommand1 = dbConnection.CreateCommand();
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
                IDbCommand dbCheckCommand = dbConnection.CreateCommand();
                dbCheckCommand.CommandText = "INSERT INTO SessionLOTWCards (SessionID, CardID, Duration, Stack) VALUES (" + sessionId + "," + CardID + "," + duration + ",1)";
                int n = dbCheckCommand.ExecuteNonQuery();
                dbConnection.Close();
                if (n != 1)
                {
                    return "Fail";
                }
                else
                    return "Success";
            } else 
            {
                // Queries
                IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
                        IDbCommand dbCheckCommand3 = dbConnection.CreateCommand();
                        dbCheckCommand3.CommandText = "UPDATE SessionLOTWCards SET Stack = Stack + 1 WHERE ID=" + n;
                        int m = dbCheckCommand3.ExecuteNonQuery();
                        dbConnection.Close();
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
                        IDbCommand dbCheckCommand3 = dbConnection.CreateCommand();
                        dbCheckCommand3.CommandText = "UPDATE SessionLOTWCards SET Stack = Stack + 1, Duration = Duration + " + duration + " WHERE ID=" + n;
                        int m = dbCheckCommand3.ExecuteNonQuery();
                        dbConnection.Close();
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
                    IDbCommand dbCheckCommand4 = dbConnection.CreateCommand();
                    dbCheckCommand4.CommandText = "INSERT INTO SessionLOTWCards (SessionID, CardID, Duration, Stack) VALUES (" + sessionId + "," + CardID + "," + duration + ",1)";
                    int n2 = dbCheckCommand4.ExecuteNonQuery();
                    dbConnection.Close();
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
        string FirstPower, string SecondPower, string Consumables)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
        dbCheckCommand2.CommandText = "SELECT PlayerId From PlayerProfile WHERE PlayerId=" + PlayerID;
        IDataReader dataReader = dbCheckCommand2.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
        }
        if (!check)
        {
            dbConnection.Close();
            return "No Exist";
        } else
        {
            System.DateTime date = System.DateTime.Now;
            string currentDate = date.ToString("yyyy-MM-dd");
            // Queries
            IDbCommand dbGetLastID = dbConnection.CreateCommand();
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
            IDbCommand dbCheckCommand = dbConnection.CreateCommand();
            dbCheckCommand.CommandText = "INSERT INTO SESSION (SessionID, TotalPlayedTime, CurrentStage, CurrentStageHazard, CurrentStageVariant, CreatedDate, LastUpdate, IsCompleted, SessionCash, SessionTimelessShard, SessionFuelEnergy, Model, LeftWeapon, RightWeapon, FirstPower, SecondPower, Consumables) VALUES " +
                "(" + id + ",0,1,1,1,'" + currentDate + "','" + currentDate + "','N',0,0,0,'" + 
                Model + "','" + LeftWeapon + "','" + RightWeapon + "','" +
                FirstPower + "','" + SecondPower + "','" + Consumables + "');";
            int n = dbCheckCommand.ExecuteNonQuery();
            if (n!=1)
            {
                dbConnection.Close();
                return "Fail";
            } else
            {
                // Queries
                IDbCommand dbUpdCommand = dbConnection.CreateCommand();
                dbUpdCommand.CommandText = "UPDATE PlayerProfile SET CurrentSession=" + id + " WHERE PlayerID=" + PlayerID;
                int k = dbUpdCommand.ExecuteNonQuery();
                dbConnection.Close();
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

    public Dictionary<string, object> GetSessionInfoByPlayerId(int PlayerId)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand2 = dbConnection.CreateCommand();
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
            dbConnection.Close();
            return null;
        }
        else
        {
            Dictionary<string, object> datas = new Dictionary<string, object>();
            // Queries
            IDbCommand dbCheckCommand3 = dbConnection.CreateCommand();
            dbCheckCommand3.CommandText = "SELECT * From Session WHERE SessionId=" + n;
            IDataReader dataReader2 = dbCheckCommand3.ExecuteReader();
            while (dataReader2.Read())
            {
                datas.Add("SessionID", dataReader2.GetInt32(0));
                datas.Add("TotalPlayedTime", dataReader2.GetInt32(1));
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
            }
            dbConnection.Close();
            return datas;
        }
    }
    public string UpdateSessionStageData(int SessionId, int CurrentStage, int CurrentStageHazard, int CurrentStageVariant)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnection.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET CurrentStage = " + CurrentStage + ", CurrentStageHazard = " +
            CurrentStageHazard + ", CurrentStageVariant = " + CurrentStageVariant + " WHERE SessionId=" + SessionId;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnection.Close();
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnection.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionTimelessShard = SessionTimelessShard " + (IsIncrease? "+ " : "- ") + amount + " WHERE SessionId=" + SessionId;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnection.Close();
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
        Debug.Log(SessionID + " - " + Cash + " - " + Shard);
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        Dictionary<string, object> datas = new Dictionary<string, object>();
        // Queries
        IDbCommand dbCheckCommand3 = dbConnection.CreateCommand();
        dbCheckCommand3.CommandText = "UPDATE Session SET SessionTimelessShard = SessionTimelessShard " + (IsIncrease ? "+ " : "- ") + Shard +
            ", SessionCash = SessionCash " + (IsIncrease ? "+ " : "- ") + Cash + " WHERE SessionId=" + SessionID;
        int n = dbCheckCommand3.ExecuteNonQuery();
        dbConnection.Close();
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
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
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
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        } else
        return data;
    }

    public Dictionary<string, object> GetStageZoneTemplateByStageValueAndVariant(int StageValue, int Variants)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetFighterGroupsDataByName(string GroupName)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetSpawnPositionDataByType(string PositionType)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "Select PositionLimitTopLeft, PositionLimitBottomRight from SpaceZonePosition WHERE PositionType='" + PositionType + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (dataReader.Read())
        {
            check = true;
            data.Add("PositionLimitTopLeft", dataReader.GetString(0));
            data.Add("PositionLimitBottomRight", dataReader.GetString(1));
        }
        dbConnection.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetHazardAllDatas(int HazardId)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public List<Dictionary<string,object>> GetAvailableHazards(int SpaceZoneNo)
    {
        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
        if (!check)
        {
            return null;
        }
        else
            return data;
    }

    public Dictionary<string, object> GetWarshipMilestoneBySpaceZoneNo(int SpaceZoneNo)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
        dbConnection.Close();
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
        int PlayerId = 0;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
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
            dbConnection.Close();
        } else
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "INSERT INTO Statistic (PlayerID,EnemyDefeated,MaxSZReach,TotalShard,TotalCash,TotalEnemyDefeated,TotalDamageDealt,TotalSalaryReceived,Rank,TotalShardSpent,TotalCashSpent,TotalDailyMissionDone) " +
                "VALUES (" + PlayerId + ",'EI-0|EII-0|EIII-0|WS-0',0,5,500,0,0,0,0,0,0,0)";
            dbCommand.ExecuteNonQuery();          
            dbConnection.Close();
        }        
    }

    public Dictionary<string, object> GetPlayerAchievement(int id)
    {
        Dictionary<string, object> PA = new Dictionary<string, object>();
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT * FROM Statistic WHERE PlayerID='" + id + "'";
        IDataReader reader = dbCheckCommand.ExecuteReader();
        bool check = false;
        while (reader.Read())
        {
            PA.Add("EnemyDefeated", reader.GetString(2));
            PA.Add("MaxSZReach", reader.GetInt32(3));
            PA.Add("TotalShard", reader.GetInt32(4));
            PA.Add("TotalCash", reader.GetInt32(5));
            check = true;
        }
        if (!check)
        {
            dbConnection.Close();
        }
        return PA;
    }

    public void UpdateGameplayStatistic(Dictionary<string, object> data)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "UPDATE Statistic SET " +
            "EnemyDefeated = '" + data["EnemyDefeated"].ToString() + "', MaxSZReach = " + int.Parse(data["SZMaxReach"].ToString()) + " WHERE PlayerID = "+ int.Parse(data["PlayerID"].ToString()) + "";
        dbCommand.ExecuteNonQuery();
        dbConnection.Close();
    }

    public void UpdateEconomyStatistic(int id, int shard, int cash, string type) 
    {
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
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = query;
        dbCommand.ExecuteNonQuery();
        dbConnection.Close();
    }
    public void UpdateDailyMissionStatistic(int playerId)
    {
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries    
        IDbCommand dbCheckCommand1 = dbConnection.CreateCommand();
        dbCheckCommand1.CommandText = "UPDATE Statistic SET TotalDailyMissionDone = TotalDailyMissionDone + 1 WHERE PlayerID = " + playerId + "";
        dbCheckCommand1.ExecuteNonQuery();
        dbConnection.Close();
        
    }
    #endregion
}
