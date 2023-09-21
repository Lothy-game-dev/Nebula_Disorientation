using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class AccessDatabase : MonoBehaviour
{
    private IDbConnection dbConnection;
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
                    (cell == 9 ? "AND LastFuelCellUsedTime = '' " : "") + "WHERE PlayerId =" + PlayerID;
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
            if (!dataReader.GetString(0).Replace(" ","").ToLower().Equals(Name.Replace(" ","").ToLower()))
            {
                list.Add(dataReader.GetString(0).Replace(" ",""));
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
        dbCheckCommand.CommandText = "SELECT * FROM RankSystem WHERE RankId=" + id + "";
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
            if (dataReader.IsDBNull(5))
            {
                EnemyList.Add("N/A");
            } else
            {
                EnemyList.Add(dataReader.GetString(5));
            }
            EnemyList.Add(dataReader.GetString(6));
            EnemyList.Add(dataReader.GetString(7));
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
            dbCheckModel.CommandText = "SELECT ModelID FROM FactoryModel WHERE replace(replace(lower(WeaponName),' ',''),'-','')='" + itemName.Replace(" ", "").Replace("-", "").ToLower() + "'";
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
}
