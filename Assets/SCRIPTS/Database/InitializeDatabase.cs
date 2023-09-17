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
                "LastFuelCellUsedTime TEXT," +
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
                "StockPerDays INTEGER, " +
                "ItemEffect TEXT NOT NULL, " +
                "EffectDuration INTEGER, " +
                "Stackable TEXT NOT NULL, " +
                "MaxStack INTEGER, " +
                "ItemPrice INTEGER NOT NULL, " +
                "Cooldown INTEGER, " +
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
            "(1, 'Kinetic', 'Pulse Cannon', 'A common cannon that fires small, gray bullets.', 'OH-1.25,2,10|DPH-375|RoF-4|AoE-0|V-1250|R-1250,1500', '0', '0', null, 1, '#36b37e', 'Gray')," +
            "(2, 'Kinetic', 'Advanced Pulse Cannon', 'An advanced version of Pulse Cannon.', 'OH-1.25,2,10|DPH-562.5|RoF-4|AoE-0|V-1250|R-1250,1500', '2250', '50', 1, 1, '#36b37e', 'White')," +
            "(3, 'Kinetic', 'Superior Pulse Cannon', 'The most powerful version of Pulse Cannon.', 'OH-1.25,2,10|DPH-1125|RoF-4|AoE-0|V-1500|R-1250,1500', '4500', '50', 2, 1, '#36b37e', 'White')," +
            "(4, 'Thermal', 'Nano Flame Thrower', 'A Thermal weapon that emits straight beam of yellow flames.', 'OH-0.133,4,20|DPH-7.5|RoF-150|AoE-0|V-500|R-400', '1500', '50', null, 1, '#36b37e', 'Orange')," +
            "(5, 'Thermal', 'Advanced Nano Flame Thrower', 'An upgraded edition of Nano Flame Thrower, which now emits a yellow flame with red aura.', 'OH-0.133,4,20|DPH-12.5|RoF-150|AoE-0|V-500|R-400', '2500', '50', 4, 1, '#36b37e', 'Yellow')," +
            "(6, 'Thermal', 'Superior Nano Flame Thrower ', 'The most powerful version of the Nano Flame Thrower.', 'OH-0.133,4,20|DPH-25|RoF-150|AoE-0|V-500|R-400', '5000', '50', 5, 1, '#36b37e', 'Yellow')," +
            "(7, 'Kinetic', 'Blast Cannon', 'A cannon that fires large, yellow bullets.', 'OH-2.5,2,10|DPH-375|RoF-2|AoE-100|V-1250|R-1250-1500', '2250', '50', 1, 1, '#36b37e', 'White')," +
            "(8, 'Kinetic', 'Grand Blast Cannon', 'An advanced version of the Blast Cannon. It now shoots light blue bullets.', 'OH-2.5,2,10|DPH-750|RoF-2|AoE-150|V-1250|R-1250-1500', '6000', '50', 7, 1, '#36b37e', 'Light Blue')," +
            "(9, 'Kinetic', 'Gravitational Artillery', 'A grand artillery that shoots super artillery shells.', 'OH-0|DPH-6000|RoF-0.25|AoE-250|V-1000|R-2000-2500', '6750', '150', 7, 5, '#4c9aff', 'Blue')," +
            "(10, 'Thermal', 'Freezing Blaster', 'A powerful thermal weapon that emits a straight beam of blue freezing ice.', 'OH-0.1,2,30|DPH-25|RoF-150|AoE-0|V-500|R-600', '7500', '150', null, 5, '#4c9aff', 'Blue')," +
            "(11, 'Thermal', 'Advanced Freezing Blaster', 'An upgraded edition of the Freezing Blaster, which now emits a beam of light blue ice.', 'OH-0.1,2,30|DPH-37.5|RoF-150|AoE-0|V-500|R-600', '11250', '150', 10, 5, '#4c9aff', 'Light Blue')," +
            "(12, 'Laser', 'Laser Cannon', 'Laser-powered weapon that shoot light-red laser projectiles.', 'OH-2,3,10|DPH-937.5|RoF-4|AoE-0|V-2000|R-2000', '7500', '150', 7, 5, '#4c9aff', 'Light Red')," +
            "(13, 'Laser', 'Advanced Laser Cannon', 'An advanced version of the Laser Cannon. It now shoots pure red laser projectiles.', 'OH-2,3,10|DPH-1500|RoF-4|AoE-0|V-2000|R-2000', '12000', '150', 12, 5, '#4c9aff', 'Red')," +
            "(14, 'Thermal', 'Orb of Lava Generator ', 'A cannon that Generate a large orb of burning lava and direct it into enemies.', 'OH-10,4,20|DPH-2250|RoF-1|AoE-200|V-750|R-1000', '12000', '250', null, 5, '#4c9aff', 'Orange')," +
            "(15, 'Grouping', 'Orb of Vacuum Generator ', 'A cannon that Generate a space wormhole that group nearby enemies into the center.', 'OH-0|DPH-1500|RoF-0.1|AoE-200|V-750|R-500', '10000', '250', null, 5, '#4c9aff', 'Black')," +
            "(16, 'Kinetic', 'Nano Cannon', 'A high-tech cannon that shoots nano projectiles.', 'OH-1,3,15|DPH-1875|RoF-8|AoE-0|V-1750|R-1750', '40000', null, 12, 9, '#bf2600', 'White')," +
            "(17, 'Kinetic', 'Grand Gravitational Artillery', 'The most powerful version of the Gravitational Artillery.', 'OH-0|DPH-12000|RoF-0.25|AoE-400|V-1000|R-2000-2500', '50000', null, 9, 9, '#bf2600', 'Blue')," +
            "(18, 'Thermal', 'Superior Freezing Blaster ', 'The top tier version of the Freezing Blaster, which now emits a beam of white blue ice.', 'OH-0.1,2,30|DPH-90|RoF-150|AoE-0|V-500|R-600', '48000', null, 11, 9, '#bf2600', 'White Blue')," +
            "(19, 'Laser', 'Superior Laser Cannon', 'The top tier version of the Laser Cannon. It now shoots much refined red laser projectiles.', 'OH-2,3,10|DPH-3375|RoF-4|AoE-0|V-2250|R-2000', '48000', null, 13, 9, '#bf2600', 'Dark Red')," +
            "(20, 'Laser', 'Plasma Cannon', 'An Plasma version of the Nano Cannon. ', 'OH-1,3,20|DPH-2062.5|RoF-8|AoE-0|V-1750|R-1750', '55000', null, 16, 9, '#bf2600', 'White')," +
            "(21, 'Thermal', 'Nano Effect Analyzer', 'A high-level cannon that amplify the power of Thermal Status.', 'OH-2.5,3,20|DPH-100|RoF-3|AoE-0|V-2000|R-2000', '50000', null, 16, 9, '#bf2600', 'White')," +
            "(22, 'Thermal', 'Orb of Magma Generator', 'A special edition of the Orb of Lava Generator. It now generate orbs of deadly magma.', 'OH-10,3,30|DPH-4500|RoF-1|AoE-300|V-750|R-1000', '100000', null, 14, 9, '#800080', 'Red')," +
            "(23, 'Grouping', 'Orb of Black Hole Generator', 'A special edition of the Orb of Vacuum Generator It now generate a more powerful space wormhole.', 'OH-0|DPH-7500|RoF-0.1|AoE-300|V-750|R-500', '100000', null, 15, 9, '#800080', 'Gray')," +
            "(24, 'Laser', 'Star Blaster', 'A superior weapon that fires star-like projectiles with insane capability.', 'OH-1.5,3,10|DPH-2250|RoF-5|AoE-25|V-2500|R-2000', '120000', null, 19, 9, '#800080', 'Purple');";
        // ArsenalPower
        string ArsenalPower = "INSERT INTO ArsenalPower VALUES " +
            "(1, 'DEF', 'Situational Barrier', 'Create a protecting sphere around the Fighter during its effect.', 'BR-x2|DC-10,20', '0', '0', null, 1, '#36b37e')," +
            "(2, 'OFF', 'Short Laser Beam', 'Charging for 3 secs, then shoot out a powerful beam of laser in front for 10 seconds.', 'DPH-1000|AoH-10/s|AoE-25|V-100000|R-750|DC-3,20', '0', '0', null, 1, '#36b37e')," +
            "(3, 'OFF', 'Rocket Burst Device', 'Spawn dozens of rockets that track down enemies nearby.', 'DPH-1500|AoH-10|AoE-50|V-5000|R-1500|DC-0,30', '3000', '50', null, 1, '#36b37e')," +
            "(4, 'MOV', 'Instant Wormhole', 'Teleport forward for a distance.', 'DPH-null|AoH-null|AoE-null|V-null|R-1000|DC-0,30', '3000', '50', null, 1, '#36b37e')," +
            "(5, 'DEF', 'Fortified Barrier', 'Create a protecting sphere around the Fighter during its effect. Passively increase shield strength.', 'BR-30|BR-x2|DC-10,20', '10000', '150', 1, 5, '#4c9aff')," +
            "(6, 'OFF', 'Enhanced Short Laser Beam', 'Charging for 3 secs, then shoot out a powerful beam of laser in front for 10 seconds.', 'DPH-2000|AoH-10/s|AoE-25|V-100000|R-750|DC-3,20', '15000', '150', 2, 5, '#4c9aff')," +
            "(7, 'OFF', 'Enhanced Rocket Burst Device', 'Spawn dozens of rockets that track down enemies nearby.', 'DPH-2000|AoH-20|AoE-75|V-5000|R-2000|DC-0,30', '15000', '150', 3, 5, '#4c9aff')," +
            "(8, 'MOV', 'Boosted Instant Wormhole', 'Teleport forward for a distance.', 'DPH-null|AoH-null|AoE-null|V-null|R-1000|DC-0,30', '20000', '150', 4, 5, '#4c9aff')," +
            "(9, 'DEF', 'Heavy Barrier', 'Create a protecting sphere around the Fighter during its effect. Passively increase shield strength.', 'BR-60|BR-x2|DC-10,20', '30000', null, 5, 9, '#bf2600')," +
            "(10, 'OFF', 'Superior Short Laser Beam', 'Charging for 3 secs, then shoot out a powerful beam of laser in front for 10 seconds.', 'DPH-3000|AoH-10/s|AoE-25|V-100000|R-750|DC-3,20', '45000', null, 6, 9, '#bf2600')," +
            "(11, 'OFF', 'Superior Rocket Burst Device', 'Spawn dozens of rockets that track down enemies nearby.', 'DPH-3000|AoH-30|AoE-100|V-7500|R-2500|DC-0,30', '45000', null, 7, 9, '#bf2600')," +
            "(12, 'MOV', 'Advanced Instant Wormhole', 'Teleport forward for a distance.', 'DPH-null|AoH-null|AoE-null|V-null|R-1000|DC-0,5', '60000', null, 8, 9, '#bf2600'); ";
        // FactoryModel
        string FactoryModel = "INSERT INTO FactoryModel VALUES " +
            "(1, 'SS29-MK1', '', 'HP-10000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0|SP-2|SC-3', '0 | 0', null, '#36b37e')," +
            "(2, 'SS29-MK2', '', 'HP-8000|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0|SP-2|SC-3', '20000 | 0', null, '#36b37e')," +
            "(3, 'SS29-MK3', '', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0|SP-2|SC-3', '20000 | 0', null, '#36b37e')," +
            "(4, 'SSS-MK1', '', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(5, 'SSS-MK2', '', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(6, 'SSS-MK3', '', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(7, 'SSS-MKL', '', 'HP-12000|SPD-400|ROT-0.5|AOF-90,90|DM-1.1|AM-1.0|PM-0.8|SP-2|SC-4', '75000 | 300', 5, '#4c9aff')," +
            "(8, 'UEC29-MK1', '', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(9, 'UEC29-MK2', '', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(10, 'UEC29-MK3', '', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff')," +
            "(11, 'UEC29-MKL', '', 'HP-12000|SPD-400|ROT-0.5|AOF-90,90|DM-1.0|AM-2.0|PM-0.8|SP-2|SC-4', '75000 | 300', 5, '#4c9aff')," +
            "(12, 'ND-Prot No. 0', '', 'HP-12000|SPD-500|ROT-1.25|AOF-120,120|DM-1.15|AM-1.0|PM-0.5|SP-2|SC-3', '250000 | 1500', 13, '#bf2600')," +
            "(13, 'ND-Zartillery', '', 'HP-5000|SPD-400|ROT-0.25|AOF-45,45|DM-1.15|AM-2.0|PM-1.0|SP-2|SC-3', '250000 | 1500', 13, '#bf2600')," +
            "(14, 'ND-MKZ', '', 'HP-20000|SPD-400|ROT-1|AOF-90,90|DM-1.25|RM-1.0|AM-0.8|SP-2|SC-4', '250000 | 1500', 13, '#bf2600'); ";
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
            "(1, 'Wing Shield', 'Equip your Fighter’s Wings with Protective Shields that last for a duration.', null, 'RED-25', 10, 'T', 5, 200, 15, '#36b37e')," +
            "(2, 'Engine Booster', 'Equip your Fighter’s Engines with extra boosters to improve its performance.', null, 'AER-2', 10, 'T', 5, 200, 15, '#36b37e')," +
            "(3, 'Auto-Repair Module', 'A module that can repair your Fighter slightly during battle.', null, 'RMH-3', 5, 'T', 5, 250, 15, '#36b37e')," +
            "(4, 'Fortified Wing Shield', 'Equip your Fighter’s Wings with Fortified Protective Shields that last for a extended duration.', null, 'RED-25', 20, 'T', 5, 500, 15, '#36b37e')," +
            "(5, 'Advanced Engine Booster', 'Equip your Fighter’s Engines with advanced extra boosters to improve its performance.', null, 'AER-2', 20, 'T', 5, 500, 15, '#36b37e')," +
            "(6, 'Advanced Auto-Repair Module', 'A module that can repair your Fighter during battle.', null, 'RMH-5', 5, 'T', 5, 600, 15, '#36b37e')," +
            "(7, 'Reflective Wing Shield', 'Equip your Fighter’s Wings with Reflective Shields that last for a duration.', 15, 'RED-50', 15, 'T', 3, 1500, 60, '#4c9aff')," +
            "(8, 'Superior Engine Booster', 'Equip your Fighter’s Engines with super extra boosters to greatly improve its performance.', 15, 'AER-3', 15, 'T', 3, 1500, 60, '#4c9aff')," +
            "(9, 'Superior Auto-Repair Module', 'A module that can repair your Fighter efficiently during battle.', 15, 'RMH-10', 5, 'T', 3, 1800, 60, '#4c9aff')," +
            "(10, 'Nano-Reflective Coat', 'A Nano-tech Coat that grant Invisibility to your Fighter for a few seconds after using.', 5, 'INV', 5, 'T', 2, 5000, 120, '#bf2600')," +
            "(11, 'Emergency Auto-Repair Module', 'An emergency module that quickly repair your Fighter during battle. ', 5, 'RMH-20', 3, 'T', 2, 5000, 120, '#bf2600')," +
            "(12, 'Fuel Core', 'Fuel Core for sale! Quite expensive though…', 1, 'FC', 3, 'T', 1, 20000, null, '#bf2600');";
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
            "(4, 'S', 10000)," +
            "(5, 'P', 30)," +
            "(6, 'CD', 3)," +
            "(7, 'CA', 3)," +
            "(8, 'CAA', 1)," +
            "(9, 'B', 2);";
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
