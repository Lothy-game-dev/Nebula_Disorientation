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
        dbCommand.CommandText = "INSERT INTO PlayerProfile (Name,Rank,CurrentSession,FuelCore,Cash,TimelessShard,DailyIncome,DailyMissionDone) " +
            "VALUES ('"+ name +"',null,null,10,500,5,500,0)";
        dbCommand.ExecuteNonQuery();
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
    #endregion
}
