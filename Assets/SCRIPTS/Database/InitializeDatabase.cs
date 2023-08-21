using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class InitializeDatabase : MonoBehaviour
{
    #region InitializeVariable
    IDbConnection dbConnection;
    #endregion
    #region Access Database
    // Create database
    public void CreateDatabase()
    {
        string dbScript = "CREATE TABLE IF NOT EXISTS PlayerArsenalWeapons(ID INTEGER, PlayerID INTEGER, WeaponID INTEGER, IsEquipped TEXT NOT NULL, FOREIGN KEY(WeaponID) REFERENCES ArsenalWeapon(WeaponID), FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS PlayerArsenalPower(ID INTEGER, PlayerID INTEGER, PowerID INTEGER, IsEquipped TEXT NOT NULL, FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), FOREIGN KEY(PowerID) REFERENCES ArsenalPower(PowerID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS EnemyMoveset(ID INTEGER, EnemyID INTEGER, MoveID INTEGER, FOREIGN KEY(EnemyID) REFERENCES Enemies(EnemyID), FOREIGN KEY(MoveID) REFERENCES EnemiesMoves(MoveID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS StageEnemy(ID INTEGER, StageID INTEGER, EnemyID INTEGER, Quantity INTEGER NOT NULL, SpawnPosition TEXT NOT NULL, FOREIGN KEY(StageID) REFERENCES Stages(StageID), FOREIGN KEY(EnemyID) REFERENCES Enemies(EnemyID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS PlayerSpaceShopItem(ID INTEGER, PlayerID INTEGER, ItemID INTEGER, Quantity INTEGER NOT NULL, FOREIGN KEY(ItemID) REFERENCES SpaceShop(ItemID), FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS PlayerDailyMission(ID INTEGER, PlayerID INTEGER, MissionID INTEGER, IsComplete TEXT NOT NULL, MissionDate REAL NOT NULL, FOREIGN KEY(MissionID) REFERENCES DailyMissions(MissionID), FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS SessionArsenalPower(ID INTEGER, SessionID INTEGER, PowerID INTEGER, IsEquipped TEXT NOT NULL, FOREIGN KEY(SessionID) REFERENCES Session(SessionID), FOREIGN KEY(PowerID) REFERENCES ArsenalPower(PowerID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS SessionArsenalWeapons(ID INTEGER, SessionID INTEGER, WeaponID INTEGER, IsEquipped TEXT NOT NULL, FOREIGN KEY(SessionID) REFERENCES Session(SessionID), FOREIGN KEY(WeaponID) REFERENCES ArsenalWeapon(WeaponID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS SessionLOTWCards(ID INTEGER, SessionID INTEGER, CardID INTEGER, Alreadyapplied TEXT NOT NULL, FOREIGN KEY(CardID) REFERENCES LuckOfTheWandererCards(CardID), FOREIGN KEY(SessionID) REFERENCES Session(SessionID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS PlayerModelOwnership(ID INTEGER, PlayerID INTEGER, ModelID INTEGER, BuyDate REAL NOT NULL, IsUsing TEXT NOT NULL, FOREIGN KEY(ModelID) REFERENCES FactoryModel(ModelID), FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), PRIMARY KEY(ID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS ArsenalPower(PowerID INTEGER, PowerType TEXT NOT NULL, PowerName TEXT NOT NULL, PowerDescription TEXT NOT NULL, PowerStats INTEGER NOT NULL, PowerPrice TEXT NOT NULL, PowerPermPrice TEXT NOT NULL, PrereqItem INTEGER, RankReq INTEGER, Color TEXT NOT NULL, FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), PRIMARY KEY(PowerID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS ArsenalWeapon(WeaponID INTEGER, WeaponType TEXT NOT NULL, WeaponName TEXT NOT NULL, WeaponDescription TEXT NOT NULL, WeaponStats TEXT NOT NULL, WeaponPrice TEXT NOT NULL, WeaponPermPrice TEXT NOT NULL, PrereqWeapon INTEGER, RankReq INTEGER, Color TEXT NOT NULL, FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), PRIMARY KEY(WeaponID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS DailyMissions(MissionID INTEGER, MissionVerb TEXT NOT NULL, MissionNumber INTEGER NOT NULL, PRIMARY KEY(MissionID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS Enemies(EnemyID INTEGER, EnemyName TEXT NOT NULL, EnemyStats TEXT NOT NULL, DefeatReward TEXT NOT NULL, IsUnlocked TEXT NOT NULL, UnlockedDate REAL, PRIMARY KEY(EnemyID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS EnemiesMoves(MoveID INTEGER, MoveName TEXT NOT NULL, MoveStats TEXT NOT NULL, MovePriority INTEGER NOT NULL, MoveTriggerCondition TEXT NOT NULL, PRIMARY KEY(MoveID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS FactoryModel(ModelID INTEGER, ModelName TEXT NOT NULL, ModelDescription TEXT NOT NULL, ModelStats TEXT NOT NULL, ModelPrice TEXT NOT NULL, RankReq INTEGER, Color TEXT NOT NULL, FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), PRIMARY KEY(ModelID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS LuckOfTheWandererCards(CardID INTEGER, CardName TEXT NOT NULL, CardEffect TEXT NOT NULL, CardChance INTEGER NOT NULL, CardDuration INTEGER, CardRepeatable TEXT NOT NULL, Color TEXT NOT NULL, PRIMARY KEY(CardID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS PlayerProfile(PlayerID INTEGER, Name TEXT NOT NULL, Rank INTEGER, CurrentSession INTEGER, FuelCore INTEGER NOT NULL, Cash INTEGER NOT NULL, TimelessShard INTEGER NOT NULL, DailyIncome INTEGER NOT NULL, DailyMissionDone INTEGER NOT NULL, FOREIGN KEY(Rank) REFERENCES RankSystem(RankID), FOREIGN KEY(CurrentSession) REFERENCES Session(SessionID), PRIMARY KEY(PlayerID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS RankSystem(RankID INTEGER, RankName TEXT NOT NULL, RankConditionSZ INTEGER NOT NULL, RankCondition2Verb TEXT, RankCondition2Number INTEGER, DailyIncome INTEGER NOT NULL, Privilege TEXT NOT NULL, Color TEXT NOT NULL, PRIMARY KEY(RankID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS Session(SessionID INTEGER, PlayedTime REAL NOT NULL, CurrentStage INTEGER, CreatedDate REAL NOT NULL, LastUpdate REAL NOT NULL, IsDone TEXT NOT NULL, SessionCash INTEGER NOT NULL, SessionTimelessShard INTEGER NOT NULL, SessionFuelEnergy INTEGER NOT NULL, StatsIncreasePercent TEXT NOT NULL, StatsIncreaseFlat TEXT NOT NULL, SessionCurrentStats TEXT NOT NULL, FOREIGN KEY(CurrentStage) REFERENCES Stages(StageID), PRIMARY KEY(SessionID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS SpaceShop(ItemID INTEGER, ItemName TEXT NOT NULL, ItemDescription TEXT NOT NULL, ItemEffect TEXT NOT NULL, EffectDuration INTEGER, Stackable TEXT NOT NULL, MaxStack INTEGER, ItemPrice INTEGER NOT NULL, Cooldown INTEGER NOT NULL, Color TEXT NOT NULL, PRIMARY KEY(ItemID AUTOINCREMENT) ); CREATE TABLE IF NOT EXISTS Stages(StageID INTEGER, StageObjectivesVerb TEXT NOT NULL, StageObjectivesNumber INTEGER NOT NULL, StageRewardMultiplier INTEGER NOT NULL, StageTimeLimit INTEGER, StageEnemyStatsMultiplier INTEGER NOT NULL, StageSceneNo INTEGER NOT NULL, PRIMARY KEY(StageID AUTOINCREMENT) );";
        //open database and create table
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = dbScript;
        dbCommandCreateTable.ExecuteReader();

        dbConnection.Close();
        
    }

    // select data 
    public Weapons SelectData()
    {
        Weapons weapon = new Weapons();
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand(); // 15
        dbCommandReadValues.CommandText = "SELECT * FROM PlayerProfile"; // 16
        IDataReader dataReader = dbCommandReadValues.ExecuteReader(); // 17

        while (dataReader.Read()) // 18
        {
            // The `id` has index 0, our `hits` have the index 1.
            weapon.name = dataReader.GetString(1); // 19
        }

        // Remember to always close the connection at the end.
        dbConnection.Close(); // 20
        return weapon;
    }

    // insert data
    public void InsertData(Weapons weapons)
    {
        // Open connection
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "INSERT INTO PlayerProfile (name) VALUES (" + weapons.name + ")";
        dbCommandInsertValue.ExecuteNonQuery();

        dbConnection.Close();
    }

    // update data
    public void UpdateData(Weapons weapons)
    {
        // Open connection
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "Update PlayerProfile SET ( name = " + weapons.name + ")";
        dbCommandInsertValue.ExecuteNonQuery();

        dbConnection.Close();
    }

    // delete data
    public void DeleteData(Weapons weapons)
    {
        // Open connection
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "Delete from PlayerProfile where name = "+ weapons.name +"";
        dbCommandInsertValue.ExecuteNonQuery();

        dbConnection.Close();
    }
    #endregion
}
