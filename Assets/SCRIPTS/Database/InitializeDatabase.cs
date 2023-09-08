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
    #region Initialize Database
    // Call to this function for init DB
    public void Initialization()
    {
        string check = CheckInitialize();
        Debug.Log(check);
        if ("No Data".Equals(check))
        {
            CreateDatabase();
        } else if ("Data Error".Equals(check))
        {
            DropDatabase();
            CreateDatabase();
        } else if ("Not Initialize Yet".Equals(check))
        {
            DropDatabase();
            CreateDatabase();
        }
    }
    // Create database
    public void CreateDatabase()
    {
        // Initialize Tables
        string TableInitialize =
            // PlayerArsenalWeapons
            "CREATE TABLE IF NOT EXISTS PlayerArsenalWeapons" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "WeaponID INTEGER, " +
                "IsEquipped TEXT NOT NULL, " +
                "FOREIGN KEY(WeaponID) REFERENCES ArsenalWeapon(WeaponID), " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // PlayerArsenalPower
            "CREATE TABLE IF NOT EXISTS PlayerArsenalPower" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "PowerID INTEGER, " +
                "IsEquipped TEXT NOT NULL, " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "FOREIGN KEY(PowerID) REFERENCES ArsenalPower(PowerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // EnemyMoveset
            "CREATE TABLE IF NOT EXISTS EnemyMoveset" +
                "(ID INTEGER, " +
                "EnemyID INTEGER, " +
                "MoveID INTEGER, " +
                "FOREIGN KEY(EnemyID) REFERENCES Enemies(EnemyID), " +
                "FOREIGN KEY(MoveID) REFERENCES EnemiesMoves(MoveID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // StageEnemy
            "CREATE TABLE IF NOT EXISTS StageEnemy" +
                "(ID INTEGER, StageID INTEGER, " +
                "EnemyID INTEGER, " +
                "Quantity INTEGER NOT NULL, " +
                "SpawnPosition TEXT NOT NULL, " +
                "FOREIGN KEY(StageID) REFERENCES Stages(StageID), " +
                "FOREIGN KEY(EnemyID) REFERENCES Enemies(EnemyID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // PlayerSpaceShopItem
            "CREATE TABLE IF NOT EXISTS PlayerSpaceShopItem" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "ItemID INTEGER, " +
                "Quantity INTEGER NOT NULL, " +
                "FOREIGN KEY(ItemID) REFERENCES SpaceShop(ItemID), " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // PlayerDailyMission
            "CREATE TABLE IF NOT EXISTS PlayerDailyMission" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "MissionID INTEGER, " +
                "IsComplete TEXT NOT NULL, " +
                "MissionDate TEXT NOT NULL, " +
                "FOREIGN KEY(MissionID) REFERENCES DailyMissions(MissionID), " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // SessionArsenalPower
            "CREATE TABLE IF NOT EXISTS SessionArsenalPower" +
                "(ID INTEGER, " +
                "SessionID INTEGER, " +
                "PowerID INTEGER, " +
                "IsEquipped TEXT NOT NULL, " +
                "FOREIGN KEY(SessionID) REFERENCES Session(SessionID), " +
                "FOREIGN KEY(PowerID) REFERENCES ArsenalPower(PowerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // SessionArsenalWeapons
            "CREATE TABLE IF NOT EXISTS SessionArsenalWeapons" +
                "(ID INTEGER, " +
                "SessionID INTEGER, " +
                "WeaponID INTEGER, " +
                "IsEquipped TEXT NOT NULL, " +
                "FOREIGN KEY(SessionID) REFERENCES Session(SessionID), " +
                "FOREIGN KEY(WeaponID) REFERENCES ArsenalWeapon(WeaponID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // SessionLOTWCards
            "CREATE TABLE IF NOT EXISTS SessionLOTWCards" +
                "(ID INTEGER, " +
                "SessionID INTEGER, " +
                "CardID INTEGER, " +
                "AlreadyApplied TEXT NOT NULL, " +
                "FOREIGN KEY(CardID) REFERENCES LuckOfTheWandererCards(CardID), " +
                "FOREIGN KEY(SessionID) REFERENCES Session(SessionID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // PlayerModelOwnership
            "CREATE TABLE IF NOT EXISTS PlayerModelOwnership" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "ModelID INTEGER, " +
                "BuyDate REAL NOT NULL, " +
                "IsUsing TEXT NOT NULL, " +
                "FOREIGN KEY(ModelID) REFERENCES FactoryModel(ModelID), " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // ArsenalPower
            "CREATE TABLE IF NOT EXISTS ArsenalPower" +
                "(PowerID INTEGER, " +
                "PowerType TEXT NOT NULL, " +
                "PowerName TEXT NOT NULL, " +
                "PowerDescription TEXT NOT NULL, " +
                "PowerStats TEXT NOT NULL, " +
                "PowerPrice TEXT NOT NULL, " +
                "PowerPermPrice TEXT, " +
                "PrereqItem INTEGER, RankReq INTEGER, " +
                "TierColor TEXT NOT NULL, " +
                "FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), " +
                "PRIMARY KEY(PowerID AUTOINCREMENT) ); " +
            // ArsenalWeapon
            "CREATE TABLE IF NOT EXISTS ArsenalWeapon" +
                "(WeaponID INTEGER, " +
                "WeaponType TEXT NOT NULL, " +
                "WeaponName TEXT NOT NULL, " +
                "WeaponDescription TEXT NOT NULL, " +
                "WeaponStats TEXT NOT NULL, " +
                "WeaponPrice TEXT NOT NULL, " +
                "WeaponPermPrice TEXT, " +
                "PrereqWeapon INTEGER, " +
                "RankReq INTEGER, " +
                "TierColor TEXT NOT NULL, " +
                "ProjectileColor TEXT NOT NULL, " +
                "FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), " +
                "PRIMARY KEY(WeaponID AUTOINCREMENT) ); " +
            // DailyMissions
            "CREATE TABLE IF NOT EXISTS DailyMissions" +
                "(MissionID INTEGER, " +
                "MissionVerb TEXT NOT NULL, " +
                "MissionNumber INTEGER NOT NULL, " +
                "PRIMARY KEY(MissionID AUTOINCREMENT) ); " +
            // Enemies
            "CREATE TABLE IF NOT EXISTS Enemies" +
                "(EnemyID INTEGER, " +
                "EnemyName TEXT NOT NULL, " +
                "EnemyStats TEXT NOT NULL, " +
                "DefeatReward TEXT NOT NULL, " +
                "IsUnlocked TEXT NOT NULL, " +
                "UnlockedDate REAL, " +
                "PRIMARY KEY(EnemyID AUTOINCREMENT) ); " +
            // EnemiesMoves
            "CREATE TABLE IF NOT EXISTS EnemiesMoves" +
                "(MoveID INTEGER, " +
                "MoveName TEXT NOT NULL, " +
                "MoveStats TEXT NOT NULL, " +
                "MovePriority INTEGER NOT NULL, " +
                "MoveTriggerCondition TEXT NOT NULL, " +
                "PRIMARY KEY(MoveID AUTOINCREMENT) ); " +
            // FactoryModel
            "CREATE TABLE IF NOT EXISTS FactoryModel" +
                "(ModelID INTEGER, " +
                "ModelName TEXT NOT NULL, " +
                "ModelDescription TEXT NOT NULL, " +
                "ModelStats TEXT NOT NULL, " +
                "ModelPrice TEXT NOT NULL, " +
                "RankReq INTEGER, " +
                "TierColor TEXT NOT NULL, " +
                "FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), " +
                "PRIMARY KEY(ModelID AUTOINCREMENT) ); " +
            // LuckOfTheWandererCards
            "CREATE TABLE IF NOT EXISTS LuckOfTheWandererCards" +
                "(CardID INTEGER, " +
                "CardName TEXT NOT NULL, " +
                "CardEffect TEXT NOT NULL, " +
                "CardChance INTEGER NOT NULL, " +
                "CardDuration INTEGER, " +
                "CardRepeatable TEXT NOT NULL, " +
                "TierColor TEXT NOT NULL, " +
                "PRIMARY KEY(CardID AUTOINCREMENT) ); " +
            // PlayerProfile
            "CREATE TABLE IF NOT EXISTS PlayerProfile" +
                "(PlayerID INTEGER, " +
                "Name TEXT NOT NULL, " +
                "Rank INTEGER, " +
                "CurrentSession INTEGER, " +
                "FuelCell INTEGER NOT NULL, " +
                "FuelEnergy INTEGER NOT NULL, " +
                "Cash INTEGER NOT NULL, " +
                "TimelessShard INTEGER NOT NULL, " +
                "DailyIncome INTEGER NOT NULL, " +
                "DailyIncomeReceived TEXT NOT NULL, " +
                "FOREIGN KEY(Rank) REFERENCES RankSystem(RankID), " +
                "FOREIGN KEY(CurrentSession) REFERENCES Session(SessionID), " +
                "PRIMARY KEY(PlayerID AUTOINCREMENT) ); " +
            // RankSystem
            "CREATE TABLE IF NOT EXISTS RankSystem" +
                "(RankID INTEGER, " +
                "RankName TEXT NOT NULL, " +
                "RankConditionSZ INTEGER NOT NULL, " +
                "RankCondition2Verb TEXT, " +
                "RankCondition2Number INTEGER, " +
                "DailyIncome INTEGER NOT NULL, " +
                "Privilege TEXT, " +
                "TierColor TEXT NOT NULL, " +
                "PRIMARY KEY(RankID AUTOINCREMENT) ); " +
            // Session
            "CREATE TABLE IF NOT EXISTS Session" +
                "(SessionID INTEGER, " +
                "PlayedTime REAL NOT NULL, " +
                "CurrentStage INTEGER, " +
                "CreatedDate REAL NOT NULL, " +
                "LastUpdate REAL NOT NULL, " +
                "IsDone TEXT NOT NULL, " +
                "SessionCash INTEGER NOT NULL, " +
                "SessionTimelessShard INTEGER NOT NULL, " +
                "SessionFuelEnergy INTEGER NOT NULL, " +
                "StatsIncreasePercent TEXT NOT NULL, " +
                "StatsIncreaseFlat TEXT NOT NULL, " +
                "FOREIGN KEY(CurrentStage) REFERENCES Stages(StageID), " +
                "PRIMARY KEY(SessionID AUTOINCREMENT) ); " +
            // SpaceShop
            "CREATE TABLE IF NOT EXISTS SpaceShop" +
                "(ItemID INTEGER, " +
                "ItemName TEXT NOT NULL, " +
                "ItemDescription TEXT NOT NULL, " +
                "ItemEffect TEXT NOT NULL, " +
                "EffectDuration INTEGER, " +
                "Stackable TEXT NOT NULL, " +
                "MaxStack INTEGER, " +
                "ItemPrice INTEGER NOT NULL, " +
                "Cooldown INTEGER NOT NULL, " +
                "TierColor TEXT NOT NULL, " +
                "PRIMARY KEY(ItemID AUTOINCREMENT) ); " +
            // Stages
            "CREATE TABLE IF NOT EXISTS Stages" +
                "(StageID INTEGER, " +
                "StageObjectivesVerb TEXT NOT NULL, " +
                "StageObjectivesNumber INTEGER NOT NULL, " +
                "StageRewardMultiplier INTEGER NOT NULL, " +
                "StageTimeLimit INTEGER, " +
                "StageEnemyStatsMultiplier INTEGER NOT NULL, " +
                "StageSceneNo INTEGER NOT NULL, " +
                "PRIMARY KEY(StageID AUTOINCREMENT) );" +
            // Table to check if database already Init
            "CREATE TABLE IF NOT EXISTS DatabaseInitialize" +
                "(AlreadyInitialize TEXT NOT NULL);" +
            // Table for current play session
            "CREATE TABLE IF NOT EXISTS CurrentPlaySession" +
                "(PlaySessionId INTEGER," +
                "PlayerId INTEGER," +
                "SessionStartTime Text," +
                "SessionEndTime Text," +
                "FOREIGN KEY(PlayerId) REFERENCES PlayerProfile(PlayerId)," +
                "PRIMARY KEY(PlaySessionId AUTOINCREMENT) );" +
             // Table for option
             "CREATE TABLE IF NOT EXISTS Option" +
                "(MasterVolume INTEGER," +
                "MusicVolume INTEGER," +
                "SoundFx INTEGER," +
                "Fps INTEGER," +
                "Resolution TEXT);";
        // Initialize Data
        // ArsenalWeapon
        string ArsenalWeapon = "INSERT INTO ArsenalWeapon VALUES " +
            "(1, 'Kinetic', 'Pulse Cannon', '', 'DPH-100|RoF-1|AoE-1|V-100|R-100,200', '0', '0', null, 1, '#36b37e', 'Gray')," +
            "(2, 'Kinetic', 'Advanced Pulse Cannon', '', 'DPH-150|RoF-1|AoE-1|V-100|R-100,200', '2250', '50', 1, 1, '#36b37e', 'White')," +
            "(3, 'Kinetic', 'Superior Pulse Cannon', '', 'DPH-150|RoF-2|AoE-1|V-100|R-100,200', '4500', '50', 2, 1, '#36b37e', 'White')," +
            "(4, 'Thermal', 'Nano Flame Thrower', '', 'DPH-15|RoF-10|AoE-2|V-300|R-50,50', '1500', '50', null, 1, '#36b37e', 'Orange')," +
            "(5, 'Thermal', 'Advanced Nano Flame Thrower', '', 'DPH-25|RoF-10|AoE-2|V-300|R-50,50', '2500', '50', 4, 1, '#36b37e', 'Yellow')," +
            "(6, 'Thermal', 'Superior Nano Flame Thrower ', '', 'DPH-25|RoF-20|AoE-2|V-300|R-50,50', '5000', '50', 5, 1, '#36b37e', 'Yellow')," +
            "(7, 'Kinetic', 'Blast Cannon', '', 'DPH-100|RoF-0.5|AoE-3|V-100|R-100,200', '2250', '50', 1, 1, '#36b37e', 'White')," +
            "(8, 'Kinetic', 'Grand Blast Cannon', '', 'DPH-400|RoF-0.25|AoE-4|V-100|R-100,200', '6000', '50', 7, 1, '#36b37e', 'Light Blue')," +
            "(9, 'Kinetic', 'Gravitational Artillery', '', 'DPH-200|RoF-0.5|AoE-3|V-200|R-150,300', '6750', '150', 7, 5, '#4c9aff', 'Blue')," +
            "(10, 'Thermal', 'Freezing Blaster', '', 'DPH-50|RoF-10|AoE-2|V-300|R-75,75', '7500', '150', null, 5, '#4c9aff', 'Blue')," +
            "(11, 'Thermal', 'Advanced Freezing Blaster', '', 'DPH-50|RoF-15|AoE-2|V-300|R-75,75', '11250', '150', 10, 5, '#4c9aff', 'Light Blue')," +
            "(12, 'Laser', 'Laser Cannon', '', 'DPH-250|RoF-1|AoE-1|V-200|R-300,300', '7500', '150', 7, 5, '#4c9aff', 'Light Red')," +
            "(13, 'Laser', 'Advanced Laser Cannon', '', 'DPH-250|RoF-2|AoE-1|V-200|R-300,300', '12000', '150', 12, 5, '#4c9aff', 'Red')," +
            "(14, 'Thermal', 'Orb of Lava Generator ', '', 'DPH-300|RoF-1|AoE-4|V-50|R-200,200', '12000', '250', null, 5, '#4c9aff', 'Orange')," +
            "(15, 'Grouping', 'Orb of Vacuum Generator ', '', 'DPH-200|RoF-0.25|AoE-10|V-50|R-50,50', '10000', '250', null, 5, '#4c9aff', 'Black')," +
            "(16, 'Kinetic', 'Nano Cannon', '', 'DPH-250|RoF-4|AoE-11|V-200|R-200,400', '40000', null, 12, 9, '#bf2600', 'White')," +
            "(17, 'Kinetic', 'Grand Gravitational Artillery', '', 'DPH-400|RoF-0.5|AoE-5|V-200|R-250,500', '50000', null, 9, 9, '#bf2600', 'Blue')," +
            "(18, 'Thermal', 'Superior Freezing Blaster ', '', 'DPH-90|RoF-20|AoE-2|V-300|R-100,100', '48000', null, 11, 9, '#bf2600', 'White Blue')," +
            "(19, 'Laser', 'Superior Laser Cannon', '', 'DPH-450|RoF-2|AoE-1|V-200|R-400,400', '48000', null, 13, 9, '#bf2600', 'Dark Red')," +
            "(20, 'Kinetic', 'Grand Nano Cannon', '', 'DPH-275|RoF-4|AoE-1|V-200|R-250,500', '55000', null, 16, 9, '#bf2600', 'White')," +
            "(21, 'Thermal', 'Orb of Magma Generator', '', 'DPH-600|RoF-1|AoE-4|V-50|R-200,200', '100000', null, 14, 9, '#800080', 'Red')," +
            "(22, 'Grouping', 'Orb of Gray Hole Generator', '', 'DPH-500|RoF-0.25|AoE-20|V-100|R-100,100', '100000', null, 15, 9, '#800080', 'Gray')," +
            "(23, 'Laser', 'Star Blaster', '', 'DPH-150|RoF-5|AoE-2|V-300|R-400,400', '120000', null, 19, 9, '#800080', 'Purple');";
        // ArsenalPower
        string ArsenalPower = "INSERT INTO ArsenalPower VALUES " +
            "(1, 'DEF', 'Situational Barrier', '', 'BR-x2|DC-10,45', '0', '0', null, 1, '#36b37e')," +
            "(2, 'OFF', 'Short Laser Beam', '', 'DPH-250|AoH-3/s|AoE-2|V-null|R-25|DC-3,45', '0', '0', null, 1, '#36b37e')," +
            "(3, 'OFF', 'Rocket Burst Device', '', 'DPH-150|AoH-10|AoE-2|V-75|R-150|DC-0,45', '3000', '50', null, 1, '#36b37e')," +
            "(4, 'MOV', 'Instant Wormhole', '', 'DPH-null|AoH-null|AoE-null|V-null|R-50|DC-0,60', '3000', '50', null, 1, '#36b37e')," +
            "(5, 'DEF(P)', 'Fortified Barrier', '', 'BR-30', '10000', '150', 1, 5, '#4c9aff')," +
            "(6, 'OFF', 'Enhanced Short Laser Beam', '', 'DPH-500|AoH-3/s|AoE-2|V-null|R-25|DC-3,45', '15000', '150', 2, 5, '#4c9aff')," +
            "(7, 'OFF', 'Enhanced Rocket Burst Device', '', 'DPH-150|AoH-15|AoE-2|V-75|R-150|DC-0,45', '15000', '150', 3, 5, '#4c9aff')," +
            "(8, 'MOV', 'Boosted Instant Wormhole', '', 'DPH-null|AoH-null|AoE-null|V-null|R-50|DC-0,45', '20000', '150', 4, 5, '#4c9aff')," +
            "(9, 'DEF(P)', 'Heavy Barrier', '', 'BR-60', '30000', null, 5, 9, '#bf2600')," +
            "(10, 'OFF', 'Superior Short Laser Beam', '', 'DPH-500|AoH-5/s|AoE-2|V-null|R-50|DC-3,30', '45000', null, 6, 9, '#bf2600')," +
            "(11, 'OFF', 'Superior Rocket Burst Device', '', 'DPH-300|AoH-20|AoE-2|V-100|R-150|DC-0,45', '45000', null, 7, 9, '#bf2600')," +
            "(12, 'MOV', 'Advanced Instant Wormhole', '', 'DPH-null|AoH-null|AoE-null|V-null|R-100|DC-0,45', '60000', null, 8, 9, '#bf2600'); ";
        // FactoryModel
        string FactoryModel = "INSERT INTO FactoryModel VALUES " +
            "(1, 'SS29-MK1', '', 'HP-10000|SPD-11|ROT-180|DM-1.0|RM-1.0|PM-1.0|SP-2|SC-3', '0', null, '#36b37e')," +
            "(2, 'SS29-MK2', '', 'HP-8000|SPD-13|ROT-120|DM-1.0|RM-1.0|PM-1.0|SP-2|SC-3', '20000', null, '#36b37e')," +
            "(3, 'SS29-MK3', '', 'HP-12000|SPD-9|ROT-90|DM-1.0|RM-1.0|PM-1.0|SP-2|SC-3', '20000', null, '#36b37e')," +
            "(4, 'SSS-MK1', '', 'HP-12000|SPD-10|ROT-180|DM-1.1|RM-1.0|PM-1.0|SP-2|SC-4', '50000 | 200', 5, '#4c9aff')," +
            "(5, 'SSS-MK2', '', 'HP-9600|SPD-12|ROT-120|DM-1.1|RM-1.0|PM-1.0|SP-2|SC-4', '50000 | 200', 5, '#4c9aff')," +
            "(6, 'SSS-MK3', '', 'HP-14400|SPD-8|ROT-90|DM-1.1|RM-1.0|PM-1.2|SP-2|SC-4', '50000 | 200', 5, '#4c9aff')," +
            "(7, 'SSS-MKL', '', 'HP-12000|SPD-8|ROT-90|DM-1.1|RM-1.0|PM-1.0|SP-3|SC-5', '75000 | 300', 5, '#4c9aff')," +
            "(8, 'UEC29-MK1', '', 'HP-12000|SPD-10|ROT-180|DM-1.0|RM-1.2|PM-1.0|SP-2|SC-4', '50000 | 200', 5, '#4c9aff')," +
            "(9, 'UEC29-MK2', '', 'HP-9600|SPD-12|ROT-120|DM-1.0|RM-1.2|PM-1.0|SP-2|SC-4', '50000 | 200', 5, '#4c9aff')," +
            "(10, 'UEC29-MK3', '', 'HP-14400|SPD-8|ROT-90|DM-1.0|RM-1.2|PM-1.2|SP-2|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(11, 'UEC29-MKL', '', 'HP-12000|SPD-8|ROT-90|DM-1.0|RM-1.2|PM-1.0|SP-3|SC-5', '75000 | 300', 5, '#4c9aff')," +
            "(12, 'ND-Prot No. 0', '', 'HP-10000|SPD-10|ROT-220|DM-1.0|RM-1.0|PM-2.0|SP-3|SC-4', '250000 | 1500', 13, '#bf2600')," +
            "(13, 'ND-Zartillery', '', 'HP-5000|SPD-15|ROT-120|DM-1.0|RM-2.0|PM-1.0|SP-3|SC-4', '250000 | 1500', 13, '#bf2600')," +
            "(14, 'ND-MKZ', '', 'HP-20000|SPD-6|ROT-90|DM-1.5|RM-1.0|PM-1.0|SP-3|SC-5', '250000 | 1500', 13, '#bf2600'); ";
        // RankSystem
        string RankSystem = "INSERT INTO RankSystem VALUES" +
            "(1, 'Soldier', 0, null, 0, 500, 'UA-3', '#97a0af')," +
            "(2, 'Gunman I', 5, null, 0, 1000, null, '#4c9aff')," +
            "(3, 'Gunman II', 15, null, 0, 1500, null, '#4c9aff')," +
            "(4, 'Gunman III', 25, null, 0, 2000, null, '#4c9aff')," +
            "(5, 'Warrior of the UEC', 30, 'C', 20, 3000, 'UA-2', '#0747a6')," +
            "(6, 'Duelist I', 40, 'D-', 3, 4000, 'UF-3', '#00b8db')," +
            "(7, 'Duelist II', 50, 'D-', 3, 5000, null, '#00b8db')," +
            "(8, 'Duelist III', 60, 'D-', 3, 6000, null, '#00b8db')," +
            "(9, 'Remakable Warrior of the UEC', 75, 'PA', 5, 8000, 'UA-1', '#00aad4')," +
            "(10, 'Master Duelist I', 80, 'D-', 5, 10000, 'UF-2', '#ffc400')," +
            "(11, 'Master Duelist II', 90, 'D-', 5, 12000, null, '#ffc401')," +
            "(12, 'Master Duelist III', 100, 'D-', 5, 14000, null, '#ffc402')," +
            "(13, 'Honored Warrior of the UEC', 115, 'O', 5, 18000, 'UF-1', '#ff991f')," +
            "(14, 'Legendary Falcon I', 130, 'D-', 10, 22000, null, '#ff5631')," +
            "(15, 'Legendary Falcon II', 150, 'D-', 10, 26000, null, '#ff5631')," +
            "(16, 'Legendary Falcon III', 175, 'D-', 10, 30000, null, '#ff5632')," +
            "(17, 'Supreme Warrior of the UEC', 200, 'PA', 15, 40000, 'UF-0', '#bf2600')," +
            "(18, 'Supreme Warrior of the UEC n', 225, null, 0, 45000, null, '#6554c0');";
        // SpaceShop
        string SpaceShop = "INSERT INTO SpaceShop VALUES " +
            "(1, 'Wing Shield', '', 'RED-25', 10, 'T', 5, 200, 30, '#36b37e')," +
            "(2, 'Engine Booster', '', 'AER-2', 10, 'T', 5, 200, 30, '#36b37e')," +
            "(3, 'Auto-Repair Module', '', 'RMH-5', 15, 'T', 5, 250, 30, '#36b37e')," +
            "(4, 'Fortified Wing Shield', '', 'RED-25', 20, 'T', 5, 500, 30, '#36b37e')," +
            "(5, 'Advanced Engine Booster', '', 'AER-2', 20, 'T', 5, 500, 30, '#36b37e')," +
            "(6, 'Advanced Auto-Repair Module', '', 'RMH-10', 15, 'T', 5, 600, 30, '#36b37e')," +
            "(7, 'Reflective Wing Shield', '', 'RED-50', 15, 'T', 3, 1500, 60, '#4c9aff')," +
            "(8, 'Superior Engine Booster', '', 'AER-3', 15, 'T', 3, 1500, 60, '#4c9aff')," +
            "(9, 'Superior Auto-repair Module', '', 'RMH-15', 3, 'T', 3, 1800, 60, '#4c9aff')," +
            "(10, 'Nano-Reflective Coat', '', 'INV', 5, 'T', 2, 5000, 120, '#bf2600')," +
            "(11, 'Emergency Auto-Repair Module', '', 'RMH-30', 3, 'T', 2, 5000, 120, '#bf2600');";
        // LOTWCards
        string LOTWCards = "INSERT INTO LuckOfTheWandererCards VALUES " +
            "(1, 'Gun Extension I', 'ENH-2', 10, 3, 'Yes', '#36b37e')," +
            "(2, 'Gun Extension II', 'ENH-5', 10, 3, 'Yes', '#36b37e')," +
            "(3, 'Engine Booster I', 'BOO-5', 10, 3, 'Yes', '#36b37e')," +
            "(4, 'Engine Booster II', 'BOO-10', 10, 3, 'Yes', '#36b37e')," +
            "(5, 'Power Cooler I', 'RCD-5', 10, 3, 'Yes', '#36b37e')," +
            "(6, 'Power Cooler II', 'RCD-10', 10, 3, 'Yes', '#36b37e')," +
            "(7, 'Gun Extension III', 'ENH-10', 8.125, 3, 'Yes', '#4c9aff')," +
            "(8, 'Engine Booster III', 'BOO-20', 8.125, 3, 'Yes', '#4c9aff')," +
            "(9, 'Power Cooler III', 'RCD-20', 8.125, 3, 'Yes', '#4c9aff')," +
            "(10, 'Foreign Fund', 'DC-2', 8.125, 3, 'Yes', '#4c9aff')," +
            "(11, 'Reformation', 'DAP-2', 2.5, 1, 'Yes', '#bf2600')," +
            "(12, 'Supercharging', 'EAE-25', 2.5, 1, 'Yes', '#bf2600')," +
            "(13, 'Franklin Effect', 'DS-2', 2.5, null, 'No', '#bf2600');";
        // Daily Missions
        string DailyMissions = "INSERT INTO DailyMissions VALUES " +
            "(1, 'KE', 20)," +
            "(2, 'KB', 1)," +
            "(3, 'C', 10)," +
            "(4, 'S', 1000)," +
            "(5, 'P', 30)," +
            "(6, 'CD', 4)," +
            "(7, 'CA', 4)," +
            "(8, 'CAA', 1)," +
            "(9, 'B', 1);";
        string Option = "INSERT INTO Option VALUES (100, 100, 100, 60, '1920x1080');";
        // Initialize Data Success
        string Success = "INSERT INTO DatabaseInitialize VALUES ('T');";
        // Initialize Data Fail
        string Failure = "INSERT INTO DatabaseInitialize VALUES ('F');";
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        // Create Table Query
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = TableInitialize;
        // Insert Data Query
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        // Order: Rank > Other
        dbCommandInsertValue.CommandText = RankSystem + SpaceShop + ArsenalPower + ArsenalWeapon + FactoryModel + LOTWCards + DailyMissions + Option;
        // Insert Check Data Query
        IDbCommand dbCommandInsertCheck = dbConnection.CreateCommand();
        // Check Variable
        string check=null;
        try
        {
            // Create Table
            int CreateCheck = dbCommandCreateTable.ExecuteNonQuery();
            // Insert Data
            int InsertCheck = dbCommandInsertValue.ExecuteNonQuery();
            if (InsertCheck == 0)
            {
                throw new SqliteException();
            }
            check = "T";
        } catch (SqliteException e)
        {
            Debug.Log(e.Message);
            check = "F";
        }
        if (check!=null)
        {
            if (check.Equals("T"))
            {
                dbCommandInsertCheck.CommandText = Success;
            } else if (check.Equals("F"))
            {
                dbCommandInsertCheck.CommandText = Failure;
            }
            dbCommandInsertCheck.ExecuteNonQuery();
        }
        dbConnection.Close();
    }

    // Drop Database
    public void DropDatabase()
    {
        // Drop Tables
        string DropTable =
            // PlayerArsenalWeapons
            "DROP TABLE IF EXISTS PlayerArsenalWeapons;" +
            // PlayerArsenalPower
            "DROP TABLE IF EXISTS PlayerArsenalPower;" +
            // EnemyMoveset
            "DROP TABLE IF EXISTS EnemyMoveset;" +
            // StageEnemy
            "DROP TABLE IF EXISTS StageEnemy;" +
            // PlayerSpaceShopItem
            "DROP TABLE IF EXISTS PlayerSpaceShopItem;" +
            // PlayerDailyMission
            "DROP TABLE IF EXISTS PlayerDailyMission;" +
            // SessionArsenalPower
            "DROP TABLE IF EXISTS SessionArsenalPower;" +
            // SessionArsenalWeapons
            "DROP TABLE IF EXISTS SessionArsenalWeapons;" +
            // SessionLOTWCards
            "DROP TABLE IF EXISTS SessionLOTWCards;" +
            // PlayerModelOwnership
            "DROP TABLE IF EXISTS PlayerModelOwnership;" +
            // ArsenalPower
            "DROP TABLE IF EXISTS ArsenalPower;" +
            // ArsenalWeapon
            "DROP TABLE IF EXISTS ArsenalWeapon;" +
            // DailyMissions
            "DROP TABLE IF EXISTS DailyMissions;" +
            // Enemies
            "DROP TABLE IF EXISTS Enemies;" +
            // EnemiesMoves
            "DROP TABLE IF EXISTS EnemiesMoves;" +
            // FactoryModel
            "DROP TABLE IF EXISTS FactoryModel;" +
            // LuckOfTheWandererCards
            "DROP TABLE IF EXISTS LuckOfTheWandererCards;" +
            // PlayerProfile
            "DROP TABLE IF EXISTS PlayerProfile;" +
            // RankSystem
            "DROP TABLE IF EXISTS RankSystem;" +
            // Session
            "DROP TABLE IF EXISTS Session;" +
            // SpaceShop
            "DROP TABLE IF EXISTS SpaceShop;" +
            // Stages
            "DROP TABLE IF EXISTS Stages;" +
            // Table to check if database already Init
            "DROP TABLE IF EXISTS DatabaseInitialize;";
        // Open DB
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        // Queries
        // Create Table Query
        IDbCommand dbCommandDropTable = dbConnection.CreateCommand();
        dbCommandDropTable.CommandText = DropTable;
        dbCommandDropTable.ExecuteNonQuery();
        dbConnection.Close();
    }
    // Check if already init
    public string CheckInitialize()
    {
        // Open Connection
        dbConnection = new SqliteConnection("URI=file:Database.db");
        dbConnection.Open();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT * FROM DatabaseInitialize";
        IDataReader dataReader = null;
        try
        {
            dataReader = dbCommandReadValues.ExecuteReader();
        } catch (SqliteException)
        {
            return "No Data";
        }
        if (dataReader != null)
        {
            // Count check
            int count = 0;
            // Get Final Result
            string check = "";
            // Count and Read the Final Result
            while (dataReader.Read())
            {
                count++;
                check = dataReader.GetString(0);
            }
            // Case Count = 0 -> No Data
            if (count == 0)
            {
                return "No Data";
            }
            // Case Count > 1 -> Error in Input
            else if (count > 1)
            {
                return "Data Error";
            }
            // Case Count = 1 -> Right case
            else if (count == 1)
            {
                // Case return value = empty -> Error in Input
                if (check == "")
                {
                    return "Data Error";
                }
                // Case return value = T -> Already Init
                else if (check == "T")
                {
                    return "Already Initialize";
                }
                // Case return value = F -> Not Init Yet
                else if (check == "F")
                {
                    return "Not Initialize Yet";
                }
                // Otherwise Error in Input
                else return "Data Error";
            }
        }
        dbConnection.Close();
        return "Data Error";
    }
    // Template Select Data
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

    // Template Insert Data
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

    // Template Update Data
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

    // Template Delete Data
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
