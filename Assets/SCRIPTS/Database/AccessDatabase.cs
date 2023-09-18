using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class AccessDatabase : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part

    IDbConnection dbConnection;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm

    public int ArsenalWeaponCount()
    {
        int count = 0;
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        // Create Table Query
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "Select count(*) from ArsenalWeapon";
        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            count = reader.GetInt32(0);
        }
        dbConnection.Close();
        return count;
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
        dbCommand.CommandText = "INSERT INTO PlayerProfile (Name,Rank,CurrentSession,FuelCell,FuelEnergy,Cash,TimelessShard,DailyIncome,DailyIncomeReceived,LastFuelCellUsedTime) " +
            "VALUES ('" + name + "',null,null,10,0,500,5,500,'N',null)";
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
    #endregion
    #region Access To Current Play Session
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
    #region DailyMission
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
        string insertText = "INSERT INTO PlayerDailyMission (PlayerID,MissionID,IsComplete,MissionDate) VALUES ";
        for (int i = 0; i < number; i++)
        {
            if (alreadyMission.Count > 0)
            {
                int n = 0;
                do
                {
                    n = Random.Range(1, numberOfDailyMissions + 1);
                } while (alreadyMission.Contains(n));
                insertText += "(" + PlayerId + "," + n + ",'N','" + currentDate + "')";
                alreadyMission.Add(n);
            } else
            {
                int n = Random.Range(1, numberOfDailyMissions + 1);
                insertText += "(" + PlayerId + "," + n + ",'N','" + currentDate + "')";
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
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT MissionId FROM PlayerDailyMission WHERE PlayerId=" + PlayerId + " AND MissionDate='" + currentDate + "' AND IsComplete='N'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check1 = false;
        while (dataReader.Read())
        {
            check1 = true;
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
    #region Get Weapon List
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
                weaplist.Add("0");
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
            list.Add(weaplist);
        }
        dbConnection.Close();
        return list;
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
    #region Get Fighter List
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
                weaplist.Add("0");
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
        dbCheckCommand.CommandText = "SELECT StockPerDays FROM SpaceShop WHERE replace(replace(lower(ItemName),' ',''),'-','')=='" + itemName.Replace(" ", "").ToLower() + "'";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check = false;
        int n = 0;
        while (dataReader.Read())
        {
            check = true;
            if (dataReader.IsDBNull(0))
            {
                n = 0;
            } else
                n = dataReader.GetInt32(0);
        }
        dbConnection.Close();
        if (!check)
        {
            return -1;
        }
        else
        {
            return n;
        }
    }

    //temp
    public Dictionary<string, int> GetAllDictionarySpaceShopCons()
    {
        Dictionary<string, int> data = new Dictionary<string, int>();
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        IDbCommand dbCheckCommand = dbConnection.CreateCommand();
        dbCheckCommand.CommandText = "SELECT ItemName FROM SpaceShop WHERE 1=1";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        while (dataReader.Read())
        {
            data.Add(dataReader.GetString(0).Replace("-", "").Replace(" ", ""), 10);
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
        dbCheckCommand.CommandText = "SELECT * FROM RankSystem WHERE RankId=" + id + "";
        IDataReader dataReader = dbCheckCommand.ExecuteReader();
        bool check1 = false;
        while (dataReader.Read())
        {
            check1 = true;
            rank.Add("RankId", dataReader.GetInt32(0).ToString());
            rank.Add("RankName", dataReader.GetString(1));
            rank.Add("RankTier", dataReader.GetString(7));
        }   
        if (!check1) return null;
        dbConnection.Close();
 
        return rank;
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
            list.Add(EnemyList);
        }
        if (!check) return null;
        dbConnection.Close();
        return list;
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
            WarshipList.Add(dataReader.GetString(3));
            WarshipList.Add(dataReader.GetString(4));
            list.Add(WarshipList);
        }
        if (!check) return null;
        dbConnection.Close();
        return list;
    }
    #endregion
}
