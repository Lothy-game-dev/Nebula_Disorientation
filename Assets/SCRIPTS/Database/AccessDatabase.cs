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

        return count;
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
