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
        dbCommand.CommandText = "INSERT INTO PlayerProfile (Name,Rank,CurrentSession,FuelCell,FuelEnergy,Cash,TimelessShard,DailyIncome,DailyIncomeReceived,DailyMissionDone) " +
            "VALUES ('"+ name +"',null,null,10,0,500,5,500,N,0)";
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
            if (rankTemp!=null && rankTemp.ToString().Length>0)
            {
                Ranks.Add(rankTemp.ToString());
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
        dbCommand.CommandText = "DELETE FROM PlayerProfile WHERE Name='" + name +"'";
        int n = dbCommand.ExecuteNonQuery();
        if (n==0)
        {
            return "Fail";
        }
        dbConnection.Close();
        return "Success";
    }
    public Dictionary<string, object> GetPlayerInformationById(int PlayerId)
    {
        if (PlayerId==-1)
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
                values.Add("Name", dataReader.GetString(1));
                values.Add("Rank", dataReader.GetInt32(2));
                values.Add("CurrentSession", dataReader.GetInt32(3));
                values.Add("FuelCell", dataReader.GetInt32(4));
                values.Add("FuelEnergy", dataReader.GetInt32(5));
                values.Add("Cash", dataReader.GetInt32(6));
                values.Add("TimelessShard", dataReader.GetInt32(7));
                values.Add("DailyIncome", dataReader.GetInt32(8));
                values.Add("DailyIncomeReceived", dataReader.GetString(9));
                values.Add("DailyMissionDone", dataReader.GetInt32(10));
            }
            if (!check)
            {
                return null;
            } else
            {
                return values;
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
            "(" + id.ToString() + ",datetime('now'),null)";
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
        return id;
    }
    #endregion
}
