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
            // Purchase History
            "CREATE TABLE IF NOT EXISTS PurchaseHistory" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "ItemType TEXT, " +
                "ItemID INTEGER, " +
                "Quantity INTEGER, " +
                "BuyOrSell TEXT, " +
                "PurchaseDate TEXT, " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // Player Ownership
            "CREATE TABLE IF NOT EXISTS PlayerOwnership" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "ItemType TEXT, " +
                "ItemID INTEGER, " +
                "Quantity INTEGER, " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // PlayerDailyMission
            "CREATE TABLE IF NOT EXISTS PlayerDailyMission" +
                "(ID INTEGER, " +
                "PlayerID INTEGER, " +
                "MissionID INTEGER, " +
                "IsComplete TEXT NOT NULL, " +
                "MissionDate TEXT NOT NULL, " +
                "MissionProgess INTEGER NOT NULL, " +
                "FOREIGN KEY(MissionID) REFERENCES DailyMissions(MissionID), " +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            // Player Ownership
            "CREATE TABLE IF NOT EXISTS SessionOwnership" +
                "(ID INTEGER, " +
                "SessionID INTEGER, " +
                "ItemType TEXT, " +
                "ItemID INTEGER, " +
                "Quantity INTEGER, " +
                "FOREIGN KEY(SessionID) REFERENCES Session(SessionID), " +
                "PRIMARY KEY(ID AUTOINCREMENT) ); " +
            /*            // SessionArsenalPower
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
                            "PRIMARY KEY(ID AUTOINCREMENT) ); " +*/
            // SessionLOTWCards
            "CREATE TABLE IF NOT EXISTS SessionLOTWCards" +
                "(ID INTEGER, " +
                "SessionID INTEGER, " +
                "CardID INTEGER, " +
                "Duration INTEGER, " +
                "Stack INTEGER, " +
                "FOREIGN KEY(CardID) REFERENCES LuckOfTheWandererCards(CardID), " +
                "FOREIGN KEY(SessionID) REFERENCES Session(SessionID), " +
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
                "PowerEncyc TEXT NOT NULL, " +
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
                "WeaponEncyc TEXT NOT NULL, " +
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
                "MainTarget TEXT NOT NULL, " +
                "EnemyWeapons TEXT NOT NULL, " +
                "EnemyStats TEXT NOT NULL, " +
                "EnemyPower TEXT NOT NULL, " +
                "DefeatReward TEXT NOT NULL, " +
                "EnemyEncyc TEXT NOT NULL, " +
                "EnemyTier TEXT NOT NULL, " +
                "PRIMARY KEY(EnemyID AUTOINCREMENT) ); " +
            // Allies
            "CREATE TABLE IF NOT EXISTS Allies" +
                "(AllyID INTEGER, " +
                "AllyName TEXT NOT NULL, " +
                "MainTarget TEXT NOT NULL, " +
                "AllyWeapons TEXT NOT NULL, " +
                "AllyStats TEXT NOT NULL, " +
                "AllyPower TEXT NOT NULL, " +
                "AllyTier TEXT NOT NULL, " +
                "PRIMARY KEY(AllyID AUTOINCREMENT) ); " +
            // FactoryModel
            "CREATE TABLE IF NOT EXISTS FactoryModel" +
                "(ModelID INTEGER, " +
                "ModelName TEXT NOT NULL, " +
                "ModelDescription TEXT NOT NULL, " +
                "ModelStats TEXT NOT NULL, " +
                "ModelPrice TEXT NOT NULL, " +
                "RankReq INTEGER, " +
                "TierColor TEXT NOT NULL, " +
                "ModelEncyc TEXT NOT NULL, " +
                "FOREIGN KEY(RankReq) REFERENCES RankSystem(RankID), " +
                "PRIMARY KEY(ModelID AUTOINCREMENT) ); " +
            // LuckOfTheWandererCards
            "CREATE TABLE IF NOT EXISTS LuckOfTheWandererCards" +
                "(CardID INTEGER, " +
                "CardName TEXT NOT NULL, " +
                "CardType TEXT NOT NULL, " +
                "CardEffect TEXT NOT NULL, " +
                "CardTier INTEGER NOT NULL, " +
                "CardDuration INTEGER, " +
                "CardStackable TEXT NOT NULL, " +
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
                "CollectedSalaryTime TEXT," +
                "SupremeWarriorNo INTEGER," +
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
                "TotalPlayedTime INTEGER, " +
                "CurrentStage INTEGER, " +
                "CurrentStageHazard INTEGER, " +
                "CurrentStageVariant INTEGER, " +
                "CreatedDate TEXT NOT NULL, " +
                "LastUpdate TEXT NOT NULL, " +
                "IsCompleted TEXT NOT NULL, " +
                "SessionCash INTEGER NOT NULL, " +
                "SessionTimelessShard INTEGER NOT NULL, " +
                "SessionFuelEnergy INTEGER NOT NULL, " +
                "Model TEXT, " +
                "LeftWeapon TEXT, " +
                "RightWeapon TEXT, " +
                "FirstPower TEXT, " +
                "SecondPower TEXT, " +
                "Consumables TEXT, " +
                "SessionCurrentHP INTEGER, " +
                "EnemyDestroyed INTEGER, " +
                "DamageDealt INTEGER, " +
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
                "ItemEncyc TEXT NOT NULL, " +
                "PRIMARY KEY(ItemID AUTOINCREMENT) ); " +
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
                "Resolution TEXT);" +
            // Table for Warship
            "CREATE TABLE IF NOT EXISTS Warship" +
                "(WSId INTEGER," +
                "WSName TEXT," +
                "WSDescription TEXT," +
                "WSStat TEXT," +
                "TierColor TEXT," +
                "MainWeapon TEXT NOT NULL," +
                "SupWeapon TEXT NOT NULL," +
                "Bounty TEXT," +
                "PRIMARY KEY(WSId AUTOINCREMENT) );" +
            // Table for SpaceStation
            "CREATE TABLE IF NOT EXISTS SpaceStation" +
                "(SSId INTEGER," +
                "SSName TEXT," +
                "SSDescription TEXT," +
                "Effect  TEXT," +
                "TierColor TEXT," +
                "MainWeapon TEXT NOT NULL," +
                "SupWeapon TEXT NOT NULL," +
                "AuraRange TEXT NOT NULL," +
                "BaseHP TEXT NOT NULL," +
                "Bounty TEXT," +
                "PRIMARY KEY(SSId AUTOINCREMENT) );" +
            // Table for Tutorial
            "CREATE TABLE IF NOT EXISTS Tutorial" +
                "(TutorialID INTEGER," +
                "TutorialType TEXT," +
                "TutorialName TEXT," +
                "TutorialDesc  TEXT," +
                "PRIMARY KEY(TutorialID AUTOINCREMENT) );" +
            // Table for DamageElement
            "CREATE TABLE IF NOT EXISTS DamageElement" +
                "(DEId INTEGER," +
                "DEName TEXT," +
                "DEDesc TEXT," +
                "PRIMARY KEY(DEId AUTOINCREMENT) );" +
            // Table for Attribute
            "CREATE TABLE IF NOT EXISTS Attribute" +
                "(ATId INTEGER," +
                "ATName TEXT," +
                "ATDesc TEXT," +
                "PRIMARY KEY(ATId AUTOINCREMENT) );" +
            // Table for CollectSalaryHistory
            "CREATE TABLE IF NOT EXISTS CollectSalaryHistory" +
                "(Id INTEGER," +
                "PlayerId INTEGER," +
                "CollectedTime TEXT," +
                "PRIMARY KEY(Id AUTOINCREMENT) );" +
            // Table for FighterGroup
            "CREATE TABLE IF NOT EXISTS FighterGroup" +
                "(GroupId INTEGER," +
                "GroupName TEXT NOT NULL," +
                "AlliesFighterA TEXT NOT NULL," +
                "AlliesFighterB TEXT NOT NULL," +
                "AlliesFighterC TEXT NOT NULL," +
                "EnemiesFighterA TEXT NOT NULL," +
                "EnemiesFighterB TEXT NOT NULL," +
                "EnemiesFighterC TEXT NOT NULL," +
                "PRIMARY KEY(GroupId AUTOINCREMENT) );" +
            // Table for SpaceZoneVariants
            "CREATE TABLE IF NOT EXISTS SpaceZoneVariants" +
                "(StageVariantID INTEGER," +
                "StageValue INTEGER NOT NULL," +
                "VariantCount INTEGER NOT NULL," +
                "AvailableBackgrounds TEXT NOT NULL," +
                "TierColor TEXT NOT NULL," +
                "PRIMARY KEY(StageVariantID AUTOINCREMENT) );" +
            // Table for SpaceZoneTemplate
            "CREATE TABLE IF NOT EXISTS SpaceZoneTemplate" +
                "(TemplateID INTEGER," +
                "StageValue INTEGER NOT NULL," +
                "Variant INTEGER NOT NULL," +
                "Type TEXT NOT NULL," +
                "Missions TEXT NOT NULL," +
                "FighterGroup TEXT NOT NULL," +
                "Time INTEGER," +
                "SquadRating TEXT NOT NULL," +
                "AllySquad TEXT NOT NULL," +
                "EnemySquad TEXT NOT NULL," +
                "ArmyRating TEXT," +
                "AllyWarship TEXT," +
                "EnemyWarship TEXT," +
                "SpawnIRate INTEGER," +
                "SpawnIIRate INTEGER," +
                "FOREIGN KEY(FighterGroup) REFERENCES FighterGroup(GroupName), " +
                "PRIMARY KEY(TemplateID AUTOINCREMENT) );" +
            // Table for SpaceZonePosition	
            "CREATE TABLE IF NOT EXISTS SpaceZonePosition" +
                "(PositionID INTEGER," +
                "PositionType TEXT NOT NULL," +
                "PositionLimitTopLeft TEXT NOT NULL," +
                "PositionLimitBottomRight TEXT NOT NULL," +
                "PRIMARY KEY(PositionID AUTOINCREMENT) );" +
            // Table for HazardEnvironment
            "CREATE TABLE IF NOT EXISTS HazardEnvironment" +
                "(HazardID INTEGER," +
                "HazardCode TEXT NOT NULL," +
                "HazardName TEXT NOT NULL," +
                "HazardColor TEXT NOT NULL," +
                "HazardDescription TEXT NOT NULL," +
                "HazardChance INTEGER NOT NULL," +
                "HazardStartSpawning INTEGER NOT NULL," +
                "HazardBackground TEXT," +
                "PRIMARY KEY(HazardID AUTOINCREMENT) );" +
            // Table for WarshipMilestone
            "CREATE TABLE IF NOT EXISTS WarshipMilestone" +
                "(MilestoneID INTEGER," +
                "MilestoneNumber INTEGER NOT NULL," +
                "MilestoneAllyClassA INTEGER NOT NULL," +
                "MilestoneAllyClassB INTEGER NOT NULL," +
                "MilestoneAllyClassC INTEGER NOT NULL," +
                "MilestoneEnemyClassA INTEGER NOT NULL," +
                "MilestoneEnemyClassB INTEGER NOT NULL," +
                "MilestoneEnemyClassC INTEGER NOT NULL," +
                "PRIMARY KEY(MilestoneID AUTOINCREMENT) );" +
            // Table for Statistic
            "CREATE TABLE IF NOT EXISTS Statistic" +
                "(ID INTEGER," +
                "PlayerID INTEGER," +
                "EnemyDefeated TEXT NOT NULL," +
                "MaxSZReach INTEGER NOT NULL," +
                "TotalShard INTEGER NOT NULL," +
                "TotalCash INTEGER NOT NULL," +
                "TotalEnemyDefeated INTEGER NOT NULL," +
                "TotalDamageDealt INTEGER NOT NULL," +
                "TotalSalaryReceived INTEGER NOT NULL," +
                "Rank INTEGER NOT NULL," +
                "TotalShardSpent INTEGER NOT NULL," +
                "TotalCashSpent INTEGER NOT NULL," +
                "TotalDailyMissionDone INTEGER NOT NULL," +
                "FOREIGN KEY(PlayerID) REFERENCES PlayerProfile(PlayerId)," +
                "PRIMARY KEY(ID AUTOINCREMENT));" +
            // Table for SpaceZoneMission
            "CREATE TABLE IF NOT EXISTS SpaceZoneMission" +
                "(ID INTEGER," +
                "SpaceZoneValue INTEGER," +
                "Variant INTEGER NOT NULL," +
                "Mission TEXT NOT NULL," +
                "VictoryCondition TEXT NOT NULL," +
                "DefeatCondition TEX NOT NULL," +
                "PRIMARY KEY(ID AUTOINCREMENT));" +
                "" +
            // Table for ArsenalService
            "CREATE TABLE IF NOT EXISTS ArsenalService" +
                "(ID INTEGER," +
                "Name TEXT NOT NULL," +
                "Price TEXT NOT NULL," +
                "Effect INTEGER NOT NULL," +
                "PRIMARY KEY(ID AUTOINCREMENT));" +
            // Table for SessionCurrentSaveData
            "CREATE TABLE IF NOT EXISTS SessionCurrentSaveData" +
                "(ID INTEGER," +
                "SessionID INTEGER NOT NULL," +
                "SessionCurrentPlace TEXT NOT NULL," +
                "PRIMARY KEY(ID AUTOINCREMENT));" +
                "";
        // Initialize Data
        // ArsenalWeapon
        string ArsenalWeapon = "INSERT INTO ArsenalWeapon VALUES " +
            "(1, 'Kinetic', 'Pulse Cannon', 'A common cannon that fires small, gray bullets.', 'OH-1.25,2,10|DPH-375|RoF-4|AoE-0|V-1500|R-1250,1500', '0', '0', null, 1, '#36b37e', 'Gray', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>CADCC - Common cannon that fires small, gray bullets.')," +
            "(2, 'Kinetic', 'Advanced Pulse Cannon', 'An advanced version of Pulse Cannon.', 'OH-1.25,2,10|DPH-562.5|RoF-4|AoE-0|V-1500|R-1250,1500', '5000', '50', 1, 1, '#36b37e', 'White', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>CADCC - An advanced version of the Pulse Cannon.')," +
            "(3, 'Kinetic', 'Superior Pulse Cannon', 'The most powerful version of Pulse Cannon.', 'OH-1.25,2,10|DPH-1125|RoF-4|AoE-0|V-1500|R-1250,1500', '10000', '50', 2, 1, '#36b37e', 'White', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>CADBC - The most powerful version of the Pulse Cannon.')," +
            "(4, 'Thermal', 'Nano Flame Thrower', 'A Thermal weapon that emits straight beam of yellow flames.', 'OH-0.133,4,20|DPH-7.5|RoF-150|AoE-0|V-500|R-400', '2500', '50', null, 1, '#36b37e', 'Orange', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>DADDD - A thermal weapon that emits a straight beam of yellow flames.')," +
            "(5, 'Thermal', 'Advanced Nano Flame Thrower', 'An upgraded edition of Nano Flame Thrower, which now emits a yellow flame with red aura.', 'OH-0.133,4,20|DPH-12.5|RoF-150|AoE-0|V-500|R-400', '5000', '50', 4, 1, '#36b37e', 'Yellow', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>DADDD - An upgraded edition of the Nano Flame Thrower, which now emits a yellow flame with red aura.')," +
            "(6, 'Thermal', 'Superior Nano Flame Thrower ', 'The most powerful version of the Nano Flame Thrower.', 'OH-0.133,4,20|DPH-25|RoF-150|AoE-0|V-500|R-400', '10000', '50', 5, 1, '#36b37e', 'Yellow', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>DADDD - The most powerful version of the Nano Flame Thrower.')," +
            "(7, 'Kinetic', 'Blast Cannon', 'A cannon that fires large, yellow bullets.', 'OH-2.5,2,10|DPH-800|RoF-2|AoE-100|V-1250|R-1250-1500', '5000', '50', 1, 1, '#36b37e', 'White', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>CCCCC - A cannon that fires large, yellow bullets.')," +
            "(8, 'Kinetic', 'Grand Blast Cannon', 'An advanced version of the Blast Cannon. It now shoots light blue bullets.', 'OH-2.5,2,10|DPH-1600|RoF-2|AoE-150|V-1250|R-1250-1500', '10000', '50', 7, 1, '#36b37e', 'Light Blue', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>CCCCC - An advanced version of the Blast Cannon. It now shoots light blue bullets.')," +
            "(9, 'Kinetic', 'Gravitational Artillery', 'A grand artillery that shoots super artillery shells.', 'OH-0|DPH-6000|RoF-0.25|AoE-250|V-1000|R-2000-2500', '25000', '150', 7, 5, '#4c9aff', 'Blue', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>ADBCA - A grand artillery that shoots super artillery shells.')," +
            "(10, 'Thermal', 'Freezing Blaster', 'A powerful thermal weapon that emits a straight beam of blue freezing ice.', 'OH-0.1,2,30|DPH-25|RoF-150|AoE-0|V-500|R-600', '30000', '150', null, 5, '#4c9aff', 'Blue', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>DADDD - A powerful thermal weapon that emits a straight beam of blue freezing ice.')," +
            "(11, 'Thermal', 'Advanced Freezing Blaster', 'An upgraded edition of the Freezing Blaster, which now emits a beam of light blue ice.', 'OH-0.1,2,30|DPH-37.5|RoF-150|AoE-0|V-500|R-600', '45000', '150', 10, 5, '#4c9aff', 'Light Blue', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>CADDD - An upgraded edition of the Freezing Blaster, which now emits a beam of light blue ice.')," +
            "(12, 'Laser', 'Laser Cannon', 'Laser-powered weapon that shoot light-red laser projectiles.', 'OH-2,3,10|DPH-937.5|RoF-4|AoE-0|V-2000|R-2000', '25000', '150', 7, 5, '#4c9aff', 'Light Red', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BADAB - A laser-powered weapon that shoots light-red laser projectiles. Laser Type Weapons tend to have high shell velocity.')," +
            "(13, 'Laser', 'Advanced Laser Cannon', 'An advanced version of the Laser Cannon. It now shoots pure red laser projectiles.', 'OH-2,3,10|DPH-1500|RoF-4|AoE-0|V-2000|R-2000', '50000', '150', 12, 5, '#4c9aff', 'Red', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BADAB - An advanced version of the Laser Cannon. It now shoots pure red laser projectiles. Laser Type Weapons tend to have high shell velocity.')," +
            "(14, 'Thermal', 'Orb of Lava Generator ', 'A cannon that Generate a large orb of burning lava and direct it into enemies.', 'OH-10,4,20|DPH-2250|RoF-1|AoE-200|V-750|R-1000', '30000', '150', null, 5, '#4c9aff', 'Orange', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BCBCC - Generate a large orb of burning lava and direct it into enemies. Upon hitting enemy, the bullet will deal DMG, and cause a Lava-burn effect to the enemy, dealing 0.1% of the Target''s Max HP as DMG to that enemy every 0.1 seconds, lasting for 1 second (cannot stack and separated from Thermal''s burned status)')," +
            "(15, 'Grouping', 'Orb of Vacuum Generator ', 'A cannon that Generate a space wormhole that group nearby enemies into the center.', 'OH-0|DPH-1500|RoF-0.1|AoE-200|V-750|R-500', '30000', '150', null, 5, '#4c9aff', 'Black', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BDBCD - Generate a space wormhole that groups nearby enemies into the center. Upon hitting enemy/reaching its maximum range, create a blackhole at its position, grouping all enemies within AoE with base pulling force at 600, lasting for 3 seconds.')," +
            "(16, 'Kinetic', 'Nano Cannon', 'A high-tech cannon that shoots nano projectiles.', 'OH-1,3,15|DPH-1875|RoF-8|AoE-0|V-1750|R-1750', '125000', null, 12, 9, '#ff0d11', 'White', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BADBB - A high-tech cannon that shoots nano projectiles. This type of bullet can penetrate all enemies within 250 SU at point blank.')," +
            "(17, 'Kinetic', 'Grand Gravitational Artillery', 'The most powerful version of the Gravitational Artillery.', 'OH-0|DPH-12000|RoF-0.25|AoE-400|V-1000|R-2000-2500', '125000', null, 9, 9, '#ff0d11', 'Blue', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>ADACA - The most powerful version of the Gravitational Artillery. Upon hitting the enemy, all enemy in the AoE will receive the Gravitational Slow effect, which will decrease their moving speed by 70% in 5 seconds (Effect can not stacks)')," +
            "(18, 'Thermal', 'Superior Freezing Blaster ', 'The top tier version of the Freezing Blaster, which now emits a beam of white blue ice.', 'OH-0.1,2,30|DPH-90|RoF-150|AoE-0|V-500|R-600', '105000', null, 11, 9, '#ff0d11', 'White Blue', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>DADDD - The top-tier version of the Freezing Blaster, which now emits a beam of white-blue ice. On hit, there will be a 1% chance of insta-freezing hit enemy, putting the enemy into frozen effect for 2 seconds (enemy''s temperature still acts normally). If the enemy is already frozen and the aforementioned effect is activated, the frozen duration is increased by 1 second, unlimited stacks.')," +
            "(19, 'Laser', 'Superior Laser Cannon', 'The top tier version of the Laser Cannon. It now shoots much refined red laser projectiles.', 'OH-2,3,10|DPH-3375|RoF-4|AoE-0|V-2250|R-2000', '125000', null, 13, 9, '#ff0d11', 'Dark Red', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BBDAB - The top-tier version of the Laser Cannon. It now shoots much refined red laser projectiles. Laser Type Weapons tend to have high shell velocity.')," +
            "(20, 'Laser', 'Plasma Cannon', 'An Plasma version of the Nano Cannon. ', 'OH-1,3,20|DPH-2062.5|RoF-8|AoE-0|V-1750|R-1750', '50000', null, 16, 9, '#ff0d11', 'White', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BADBB - A Plasma version of the Nano Cannon. This type of bullet can penetrate all enemies within 250 SU at point blank.')," +
            "(21, 'Thermal', 'Nano Effect Analyzer', 'A high-level cannon that amplify the power of Thermal Status.', 'OH-2.5,3,20|DPH-100|RoF-3|AoE-0|V-2000|R-2000', '30000', null, 16, 9, '#ff0d11', 'White', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>DCDAB - A high-level cannon that amplifies the power of Thermal Status.')," +
            "(22, 'Thermal', 'Orb of Magma Generator', 'A special edition of the Orb of Lava Generator. It now generate orbs of deadly magma.', 'OH-10,3,30|DPH-4500|RoF-1|AoE-300|V-750|R-1000', '270000', null, 14, 17, '#ff0dff', 'Red', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>ACACC - A special edition of the Orb of Lava Generator. It now generates orbs of deadly magma. Upon hitting the enemy, the bullet will deal DMG, and cause a Magma-burn effect to the enemy, dealing 0.1% of the Target''s Max HP as DMG to that enemy every 0.1 seconds, lasting for 3 seconds (cannot stack and separated from Thermal''s burned status)')," +
            "(23, 'Grouping', 'Orb of Black Hole Generator', 'A special edition of the Orb of Vacuum Generator It now generate a more powerful space wormhole.', 'OH-0|DPH-7500|RoF-0.1|AoE-300|V-750|R-500', '270000', null, 15, 17, '#ff0dff', 'Gray', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>ADACD - A special edition of the Orb of Vacuum Generator It now generates a more powerful space wormhole. Upon hitting the enemy/reaching its maximum range, create a blackhole at its position, grouping all enemies within AoE with base pulling force at 1500, lasting for 7 seconds.')," +
            "(24, 'Laser', 'Star Blaster', 'A superior weapon that fires star-like projectiles with insane capability.', 'OH-1.5,3,10|DPH-2250|RoF-5|AoE-25|V-2500|R-2000', '250000', null, 19, 17, '#ff0dff', 'Purple', 'A-D (Good-Bad) | DPH-ROF-AOE-VELO-RANGE<br><br>BACAB - A superior weapon that fires star-like projectiles with insane capability.');";
        // ArsenalPower
        string ArsenalPower = "INSERT INTO ArsenalPower VALUES " +
            "(1, 'DEF', 'Situational Barrier', 'Double the Barrier HP. (Current HP and Curent Max HP).', 'BR-x2|DC-10,20', '0', '0', null, 1, '#36b37e', 'Double the Barrier HP. (Current HP and Curent Max HP)')," +
            "(2, 'OFF', 'Short Laser Beam', 'Charging for 1.14 secs, then shoot out a powerful beam of laser in front for a quick span of time.<br>Detailed: While shooting, slowdown movement speed & rotation speed to 0.5x.', 'DPH-1000|AoH-15/s|AoE-25|V-1500|R-750|DC-2.15,20', '10000', '25', null, 1, '#36b37e' ,'Quickly Charging then shoot out a powerful beam of laser in front for a quick span of time. While shooting, slowdown movement speed to 0.5x. ')," +
            "(3, 'OFF', 'Rocket Burst Device', 'Spawn dozens of rockets that track down enemies nearby.<br>Priority: Enemy Fighter nearest to the player', 'DPH-1500|AoH-10|AoE-50|V-750|R-1500|DC-0,30', '10000', '25', null, 1, '#36b37e', 'Spawn dozens of rockets that track down enemies nearby. Priority: Enemy Fighter nearest to the player.')," +
            "(4, 'MOV', 'Instant Wormhole', 'Teleport forward for a distance.', 'DPH-null|AoH-null|AoE-null|V-null|R-1000|DC-0,30', '10000', '25', null, 1, '#36b37e', 'Teleport forward for a distance.')," +
            "(5, 'DEF', 'Fortified Barrier', 'Double the Barrier HP. (Current HP and Curent Max HP). <br>Barrier HP now is equaled to 100% of Fighter�s HP. (Passive Buff).', 'BR-100|BR-x2|DC-10,20', '25000', '100', 1, 5, '#4c9aff', 'Double the Barrier HP. (Current HP and Curent Max HP). Barrier HP now is equalled to 100% of Fighter�s HP. (Passive Buff)')," +
            "(6, 'OFF', 'Enhanced Short Laser Beam', 'Charging for 1.14 secs, then shoot out a powerful beam of laser in front for a quick span of time.<br>Detailed: While shooting, slowdown movement speed & rotation speed to 0.5x.', 'DPH-2000|AoH-15/s|AoE-25|V-1500|R-750|DC-2.15,20', '25000', '100', 2, 5, '#4c9aff', 'Quickly Charging then shoot out a powerful beam of laser in front for a quick span of time. While shooting, slowdown movement speed to 0.5x. ')," +
            "(7, 'OFF', 'Enhanced Rocket Burst Device', 'Spawn dozens of rockets that track down enemies nearby.<br>Priority: Enemy Fighter nearest to the player', 'DPH-2000|AoH-20|AoE-75|V-750|R-2000|DC-0,30', '25000', '100', 3, 5, '#4c9aff', 'Spawn dozens of rockets that track down enemies nearby. Priority: Enemy Fighter nearest to the player.')," +
            "(8, 'MOV', 'Boosted Instant Wormhole', 'Teleport forward for a distance.', 'DPH-null|AoH-null|AoE-null|V-null|R-1000|DC-0,30', '25000', '100', 4, 5, '#4c9aff', 'Teleport forward for a distance.')," +
            "(9, 'DEF', 'Heavy Barrier', 'Double the Barrier HP. (Current HP and Curent Max HP). <br>Barrier HP now is equaled to 200% of Fighter�s HP. (Passive Buff).', 'BR-200|BR-x2|DC-10,20', '75000', null, 5, 13, '#ff0d11', 'Double the Barrier HP. (Current HP and Curent Max HP). Barrier HP now is equalled to 100% of Fighter�s HP. (Passive Buff)')," +
            "(10, 'OFF', 'Superior Short Laser Beam', 'Charging for 1.14 secs, then shoot out a powerful beam of laser in front for a quick span of time.<br>Detailed: While shooting, slowdown movement speed & rotation speed to 0.5x.', 'DPH-3000|AoH-15/s|AoE-25|V-1500|R-750|DC-2.15,20', '75000', null, 6, 13, '#ff0d11', 'Quickly Charging then shoot out a powerful beam of laser in front for a quick span of time. While shooting, slowdown movement speed to 0.5x. ')," +
            "(11, 'OFF', 'Superior Rocket Burst Device', 'Spawn dozens of rockets that track down enemies nearby.<br>Priority: Enemy Fighter nearest to the player', 'DPH-3000|AoH-30|AoE-100|V-1000|R-2500|DC-0,30', '75000', null, 7, 13, '#ff0d11', 'Spawn dozens of rockets that track down enemies nearby. Priority: Enemy Fighter nearest to the player.')," +
            "(12, 'MOV', 'Advanced Instant Wormhole', 'Teleport forward for a distance.', 'DPH-null|AoH-null|AoE-null|V-null|R-1000|DC-0,5', '75000', null, 8, 13, '#ff0d11', 'Teleport forward for a distance.'); ";
        // FactoryModel
        string FactoryModel = "INSERT INTO FactoryModel VALUES " +
            "(1, 'SS29-MK1', '', 'HP-10000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0|SP-1|SC-2', '0 | 0', null, '#36b37e', 'SS29s are the first common Fighters ever widely manufactured by the UEC.')," +
            "(2, 'SS29-MK2', '', 'HP-8000|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0|SP-1|SC-2', '20000 | 0', null, '#36b37e', 'MK2 version of the SS29 exchanges Vehicle Structure Health for Faster Movement Speed.')," +
            "(3, 'SS29-MK3', '', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0|SP-1|SC-2', '20000 | 0', null, '#36b37e', 'MK3 version of the SS29 gets sturdier Vehicle Structure Health at the cost of Movement Speed.')," +
            "(4, 'SSS-MK1', '', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff', 'SSS prototypes are the advanced Fighters of the UEC, with better overall characteristics compared to the SS29 prototypes, together with expanded storage for more consumables. Their design tends to focus on better Firepower Damage Implementation, resulting in dealing more DMG from weapons.')," +
            "(5, 'SSS-MK2', '', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff', 'MK2 version of the SSS exchanges Vehicle Structure Health for Faster Movement Speed.')," +
            "(6, 'SSS-MK3', '', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff', 'MK3 version of the SSS gets sturdier Vehicle Structure Health at the cost of Movement Speed.')," +
            "(7, 'SSS-MKL', '', 'HP-12000|SPD-400|ROT-0.5|AOF-90,90|DM-1.1|AM-1.0|PM-0.8|SP-2|SC-4', '75000 | 300', 5, '#4c9aff', 'MKL version - the most refined prototype of the SSS . It has an advanced Power Activating Module that lowers Power Cooldown and grants one extra Power Slot. However, its turning speed is quite sluggish.')," +
            "(8, 'UEC29-MK1', '', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff', 'MKL version - the most refined prototype of the SSS . It has an advanced Power Activating Module that lowers Power Cooldown and grants one extra Power Slot. However, its turning speed is quite sluggish.')," +
            "(9, 'UEC29-MK2', '', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff', 'MK2 version of the UEC29 exchanges Vehicle Structure Health for Faster Movement Speed.')," +
            "(10, 'UEC29-MK3', '', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0|SP-1|SC-3', '50000 | 200', 5, '#4c9aff', 'MK3 version of the UEC29 gets sturdier Vehicle Structure Health at the cost of Movement Speed.')," +
            "(11, 'UEC29-MKL', '', 'HP-12000|SPD-400|ROT-0.5|AOF-90,90|DM-1.0|AM-2.0|PM-0.8|SP-2|SC-4', '75000 | 300', 5, '#4c9aff', 'MKL version - the most refined prototype of the UEC29 . It has an advanced Power Activating Module that lowers Power Cooldown and grants one extra Power Slot. However, its turning speed is quite sluggish.')," +
            "(12, 'ND-Prot No. 0', '', 'HP-15000|SPD-500|ROT-1.25|AOF-120,120|DM-1.15|AM-1.0|PM-0.5|SP-2|SC-3', '250000 | 1500', 13, '#ff0d11', 'ND-Prototype No 0 is the first Elite Fighter Prototype, with better overall characteristics compared to all lower-grade Fighters. It is extremely flexible at turning, having a wider Arc of Fire. On the other hand, it also has great Firepower Damage Implementation & the best Power Activating Module, together with one extra Power Slot.')," +
            "(13, 'ND-Zartillery', '', 'HP-5000|SPD-400|ROT-0.25|AOF-45,45|DM-1.15|AM-2.0|PM-1.0|SP-2|SC-3', '250000 | 1500', 13, '#ff0d11', 'ND-Zartillery is the second version of the Elite Fighter Prototypes. It possesses awesome Firepower Damage Implementation & superior Firepower Area of Effect Implementation, at the cost of both Vehicle Structure Health and Movement Speed. It is also equipped with one extra Power Slot.')," +
            "(14, 'ND-MKZ', '', 'HP-20000|SPD-400|ROT-1|AOF-90,90|DM-1.25|AM-1.0|PM-0.8|SP-2|SC-4', '250000 | 1500', 13, '#ff0d11', 'ND-Zartillery is the second version of the Elite Fighter Prototypes. It possesses awesome Firepower Damage Implementation & superior Firepower Area of Effect Implementation, at the cost of both Vehicle Structure Health and Movement Speed. It is also equipped with one extra Power Slot.'); ";
        // RankSystem
        string RankSystem = "INSERT INTO RankSystem VALUES" +
            "(1, 'Soldier', 0, 'O', 1, 500, 'UA-3', '#97a0af')," +
            "(2, 'Gunman I', 10, null, 0, 1000, null, '#4c9aff')," +
            "(3, 'Gunman II', 20, null, 0, 2000, null, '#4c9aff')," +
            "(4, 'Gunman III', 35, null, 0, 3000, null, '#4c9aff')," +
            "(5, 'Warrior of the UEC', 50, 'C', 5, 5000, 'UA-2', '#0747a6')," +
            "(6, 'Duelist I', 60, 'D-II', 15, 7000, 'UF-3', '#00b8db')," +
            "(7, 'Duelist II', 70, 'D-II', 30, 9000, null, '#00b8db')," +
            "(8, 'Duelist III', 85, 'D-II', 50, 11000, null, '#00b8db')," +
            "(9, 'Remakable Warrior of the UEC', 100, 'PA', 5, 15000, 'UA-1', '#00aad4')," +
            "(10, 'Master Duelist I', 110, 'D-I', 10, 18000, 'UF-2', '#ffc400')," +
            "(11, 'Master Duelist II', 120, 'D-I', 20, 21000, null, '#ffc401')," +
            "(12, 'Master Duelist III', 135, 'D-I', 30, 24000, null, '#ffc402')," +
            "(13, 'Honored Warrior of the UEC', 150, 'O', 5, 30000, 'UF-1', '#ff991f')," +
            "(14, 'Legendary Falcon I', 160, 'D-WS', 5, 35000, null, '#ff5631')," +
            "(15, 'Legendary Falcon II', 170, 'D-WS', 10, 40000, null, '#ff5631')," +
            "(16, 'Legendary Falcon III', 185, 'D-WS', 15, 45000, null, '#ff5632')," +
            "(17, 'Supreme Warrior of the UEC', 200, 'PA', 15, 60000, 'UF-0', '#bf2600')," +
            "(18, 'Supreme Warrior of the UEC ?', 200, null, 0, 60000, null, '#6554c0');";
        // SpaceShop
        string SpaceShop = "INSERT INTO SpaceShop VALUES " +
            "(1, 'Wing Shield', 'Equip your Fighter''s Wings with Protective Shields that last for a duration.', null, 'RED-25', 10, 'T', 5, 200, 15, '#36b37e', 'Equip your Fighter''s Wings with Protective Shields that last for a duration.')," +
            "(2, 'Engine Booster', 'Equip your Fighter''s Engines with extra boosters to improve its performance.', null, 'AER-2', 10, 'T', 5, 200, 15, '#36b37e', 'Equip your Fighter''s Engines with extra boosters to improve its performance.')," +
            "(3, 'Auto-Repair Module', 'A module that can repair your Fighter slightly during battle.', null, 'RMH-3', 5, 'T', 5, 250, 15, '#36b37e', 'A module that can repair your Fighter slightly during battle.')," +
            "(4, 'Fortified Wing Shield', 'Equip your Fighter''s Wings with Fortified Protective Shields that last for a extended duration.', null, 'RED-25', 20, 'T', 5, 500, 15, '#36b37e', 'Equip your Fighter''s Wings with Fortified Protective Shields that last for a extended duration.')," +
            "(5, 'Advanced Engine Booster', 'Equip your Fighter''s Engines with advanced extra boosters to improve its performance.', null, 'AER-2', 20, 'T', 5, 500, 15, '#36b37e', 'Equip your Fighter''s Engines with advanced extra boosters to improve its performance.')," +
            "(6, 'Advanced Auto-Repair Module', 'A module that can repair your Fighter during battle.', null, 'RMH-5', 5, 'T', 5, 600, 15, '#36b37e', 'A module that can repair your Fighter during battle.')," +
            "(7, 'Reflective Wing Shield', 'Equip your Fighter''s Wings with Reflective Shields that last for a duration.', 15, 'RED-50', 15, 'T', 3, 1500, 60, '#4c9aff', 'Equip your Fighter''s Wings with Reflective Shields that last for a duration.')," +
            "(8, 'Superior Engine Booster', 'Equip your Fighter''s Engines with super extra boosters to greatly improve its performance.', 15, 'AER-3', 15, 'T', 3, 1500, 60, '#4c9aff', 'Equip your Fighter''s Engines with super extra boosters to greatly improve its performance.')," +
            "(9, 'Superior Auto-Repair Module', 'A module that can repair your Fighter efficiently during battle.', 15, 'RMH-10', 5, 'T', 3, 1800, 60, '#4c9aff', 'A module that can repair your Fighter efficiently during battle.')," +
            "(10, 'Nano-Reflective Coat', 'A Nano-tech Coat that grants Invisibility & Invulnerability to your Fighter for a few seconds after using.', 5, 'INV', 5, 'T', 2, 5000, 120, '#bf2600', 'A Nano-tech Coat that grants Invisibility & Invulnerability to your Fighter for a few seconds after using.')," +
            "(11, 'Emergency Auto-Repair Module', 'An emergency module that quickly repair your Fighter during battle. ', 5, 'RMH-20', 3, 'T', 2, 5000, 120, '#bf2600', 'An emergency module that quickly repair your Fighter during battle. ')," +
            "(12, 'Fuel Core', 'Fuel Core for sale! Quite expensive though...', 1, 'FC', 3, 'T', 1, 20000, null, '#bf2600', 'Fuel Core for sale'' but only 1 in stock per day.');";
        string LOTWCards = "INSERT INTO LuckOfTheWandererCards VALUES " +
            "(1, 'Structural Upgrader I', 'DEF', 'HP-3', 3, 1000, 'Y', 'Y', '#03c800')," +
            "(2, 'Structural Upgrader II', 'DEF', 'HP-5', 3, 1000, 'Y', 'Y', '#03c800')," +
            "(3, 'Structural Upgrader III', 'DEF', 'HP-7', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(4, 'Engine Booster I', 'DEF', 'MS-5', 3, 3, 'N', 'Y', '#03c800')," +
            "(5, 'Engine Booster II', 'DEF', 'MS-10', 3, 3, 'N', 'Y', '#03c800')," +
            "(6, 'Engine Booster III', 'DEF', 'MS-5', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(7, 'Multi-layer Barrier I', 'DEF', 'RD-5', 3, 3, 'N', 'Y', '#03c800')," +
            "(8, 'Multi-layer Barrier II', 'DEF', 'RD-10', 3, 3, 'N', 'Y', '#03c800')," +
            "(9, 'Multi-layer Barrier III', 'DEF', 'RD-5', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(10, 'Gun Extension I', 'OFF', 'AWD-2', 3, 3, 'N', 'Y', '#03c800')," +
            "(11, 'Gun Extension II', 'OFF', 'AWD-5', 3, 3, 'N', 'Y', '#03c800')," +
            "(12, 'Gun Extension III', 'OFF', 'AWD-2', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(13, 'Gun Heater I', 'OFF', 'TWD-5', 3, 3, 'N', 'Y', '#03c800')," +
            "(14, 'Gun Heater II', 'OFF', 'TWD-10', 3, 3, 'N', 'Y', '#03c800')," +
            "(15, 'Gun Heater III', 'OFF', 'TWD-5', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(16, 'Gun AP Module I', 'OFF', 'BD-10', 3, 3, 'N', 'Y', '#03c800')," +
            "(17, 'Gun AP Module II', 'OFF', 'BD-20', 3, 3, 'N', 'Y', '#03c800')," +
            "(18, 'Gun AP Module III', 'OFF', 'BD-10', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(19, 'Gun Accelerator I', 'OFF', 'FD-5', 3, 3, 'N', 'Y', '#03c800')," +
            "(20, 'Gun Accelerator II', 'OFF', 'FD-10', 3, 3, 'N', 'Y', '#03c800')," +
            "(21, 'Gun Accelerator III', 'OFF', 'FD-5', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(22, 'Power Cooler I', 'SPE', 'PCD-5', 3, 3, 'N', 'Y', '#03c800')," +
            "(23, 'Power Cooler II', 'SPE', 'PCD-10', 3, 3, 'N', 'Y', '#03c800')," +
            "(24, 'Power Cooler III', 'SPE', 'PCD-5', 2, 1000, 'Y', 'Y', '#0800ff')," +
            "(25, 'Reformation Structure', 'DEF', 'R-100', 2, 3, 'Y', 'Y', '#0800ff')," +
            "(26, 'Hazard Protection Coat', 'DEF', 'HAZ', 2, 3, 'Y', 'Y', '#0800ff')," +
            "(27, 'Berserk Enchantment', 'OFF', 'BS-10-25', 2, 3, 'Y', 'Y', '#0800ff')," +
            "(28, 'Consumable Cloner', 'SPE', 'CONS', 2, 3, 'Y', 'Y', '#0800ff')," +
            "(29, 'Foreign Fund', 'SPE', 'C-100', 2, 3, 'Y', 'Y', '#0800ff')," +
            "(30, 'Power Supercharger', 'SPE', 'PCD-30', 1, 3, 'N', 'Y', '#ff0000')," +
            "(31, 'Power Enchanter', 'SPE', 'PCD-10', 1, 1000, 'Y', 'Y', '#ff0000')," +
            "(32, 'Weapon Supercharger', 'OFF', 'WROF-15', 1, 3, 'N', 'Y', '#ff0000')," +
            "(33, 'Weapon Enchanter', 'OFF', 'AWD-5', 1, 1000, 'Y', 'Y', '#ff0000')," +
            "(34, 'Franklin Effect', 'SPE', 'C-x2', 1, -1, 'N', 'N', '#ff0000');";
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
        // Option
        string Option = "INSERT INTO Option VALUES " +
            "(100, 100, 100, 60, '1920x1080');";
        // Tutorial
        string Tutorial = "INSERT INTO Tutorial VALUES "+
            "(1, 'Preparation', 'Basic Playthrough', '<b><u>Preparation</b></u><br><br>   -  Visit the Factory to purchase new Fighters." +
            "<br>   -  Go to the Arsenal to permanently unlock upgrades and powers." +
            "<br>   -  Look for the Space Shop to purchase consumables.')," +
            "(2, 'Loadout', 'Basic Playthrough', '<b><u>Modify the Fighter s loadout before setting off</b></u><br><br>   -  Select 1 Fighter s form among the purchased ones from the Factory for this journey." +
            "<br>   -  Equip Weapons, Special Powers, and Consumables acquired from Arsenal & Space Shop for this journey.')," +
            "(3, 'Session I', 'Basic Playthrough', '<b><u>During a session: LOOPHOLE</b></u><br><br>   -  i.Luck of The Wanderer phase." +
            "<br>   -  ii.Complete Stages (Space Zone) by handling the main mission in that Zone." +
            "<br>   -  iii.Teleport back to UEC?" +
            "<br>      - YES: Players can teleport back to the UEC (using 1 Fuel Core) for maintenance and upgrades. Afterward, they can retreat or continue the journey(d.). " +
            "<br>      - NO: Forward to d. " +
            "<br>   -  iv.Go through Luck of The Wanderer again, and onward to the next Stages (Space Zone).')," +
            "(4, 'Session II', 'Basic Playthrough', '<b><u>End of a session</b></u><br><br>   -  By using a Fuel Core to retreat to the United Earth Capital (UEC) and decide to end the session." +
            "<br>   -  By getting eliminated during the journey.')," +
            "(5, 'Economy I', 'How Economy work?', '<b><u>UEC Economy (Cash, Shard, Fuel)</b></u><br><br>   -  All the currencies here can be spent on everything that doesnt relate to a session." +
            "<br>   -  You cant use UEC Economy Cash and Shard inside a session. Start a session with 0 cash.')," +
            "(6, 'Economy II', 'How Economy work?', '<b><u>Journey Economy (Cash, Fuel)</b></u><br><br>   -  All the currencies here are the only means that can be used for upgrading and buying stuff during a journey. Half of the leftover Cash and all the collected Timeless Shard will be added to the UEC Economy after successfully retreating to the UEC." +
            "<br>   -  (The other half of the collected Cash is given to the UEC Government as tax.)" +
            "<br>   -  Getting destroyed during a journey will only grant you 1/2 of the collected Timeless Shard and no Cash to your UEC Economy.');";
        //Damage Element
        string DElement = "INSERT INTO DamageElement VALUES" +
            "(1, 'Kinetic', 'Normal physical damage.')," +
            "(2, 'Thermal', 'Damage from heat or freezing - related to the drastic changes in temperature.<br><br><b><u>Basic</b></u>: Every fighter will have a temperature system.Their temperature will be at 50'' normally.After not being hit by thermal damage / being changed in temperature for 2 seconds, fighters will receive an auto - adjustment effect, which adjusts self - temperature back to 50'' at the rate of 5''/ second.<br><b><u>Thermal Damage</b></u>: All Damage Instances can only apply Thermal Damage at most 1 time / 0.1s.Every instance of damage dealt by Thermal Weapons will change the target''s temperature by 2'' based on their nature(Heat or Freeze weapon)<br><b><u>Thermal Status</b></u>: Fighter will receive the following thermal status based on their current temperature:(t = temperature)<br>      - t > 90'': Fighter will turn dark red and will be burned, removing overloaded status.While being burned, receiving(1 + (t - 90) / 10) % Max HP as damage per second.<br>      - 50'' > t > 90'': Fighter will be turning from light red to darker and darker as the temperature rises.All weapons will become overloaded, increasing the speed of weapon overheating at a rate of:OverHeatSpeedIncreaseRate = (50 + 50 * (90 - t) / (90 - 50)) / 100<br>      - t = 50'': Normal<br>      - 0'' < t < 50'': Fighter will turn from light blue to darker and darker as the temperature drops.Fighter''s movement speed will also decrease but they still can be able to act.Movement Speed rate: SPDrate = 1 - (50 - t) / 50.<br>      - t = 0'': Fighter will turn dark blue and will be frozen.While being frozen, cannot move, fire weapons, or use skills(except passive skills), lasting for 5 seconds.The frozen effect from Thermal status cannot stack.<br><b><u>Thermal Status Limitation:</b></u><br>      - burned: will wear out upon temperature below 90''<br>      - frozen: When the duration runs out, the fighter will be unaffected by any slow or frozen effect and also immune to thermal DMG''s temperature adjustment(still receive DMG) and auto - adjustment will trigger immediately at 2x speed, all effects lasting for 3 seconds.')," +
            "(3, 'Laser', 'Damage created from the absorption power of lasers.')," +
            "(4, 'Grouping', 'On a hit, create a grouping area that pulls surrounding enemies into the center.<br><br>     - Detailed: While a fighter is hit by this type of weapon, its movement vector will be added with an additional pulling vector that is pointed into the center of the bullet / blackhole, making the movement harder to control. The nearer the fighter is to the center of the black hole, the stronger this pulling vector will become.<br>     - Rule/Limitation:<br>         - Grouping base power(b): equal to? speed in fighter speed at the center(means fighter with higher speed than ? can escape from the center in the fastest way by moving in one absolute opposite direction)<br>         - Grouping power based on distance(d) and range(r) formula: (r - d / 2) / r * b.E.g.: The enemy at the edge of the black hole will only be pulled with the power at 50 % of the power at the center, and this pulling power will increase the nearer the enemy is to the center.')," +
            "(5, 'Nano Effect Analyzer', 'This type of bullet can penetrate all enemies.<br><br>        - Enemies hit will be inflicted with nano - temp effect, which will cause all Thermal''s DMG and Status to have 15 % more effectiveness(including Thermal DMG''s temperature adjustment, frozen duration, overloaded effect, and burned dmg), lasting for 2 seconds, max 4 stacks.Upon hitting enemies, based on enemies temperature: (t = temp)<br>               - t > 90: This bullet will immediately produce bonus dmg equal to 3x enemies burned DMG.<br>               - 50 < t < 90, This bullet will deal Heat Thermal DMG on hit.<br>               - t = 50, this bullet will adjust enemies temperature randomly within the range of 30~40 or 60~70.<br>               - 0 < t < 50: This bullet will deal Freeze Thermal DMG on hit.<br>               - t = 0 / enemies is freezing: This bullet will extend enemies freezing status by 1.5 seconds. This extension can only add at most 3 seconds to an enemy per frozen effect');";
        //Attribute
        string Attribute = "INSERT INTO Attribute VALUES " +
            "(1, 'Base Health', 'The amount of damage the Fighter can sustain before getting destroyed.'), " +
            "(2, 'Base Speed', 'The standard movement speed of the Fighter.'), " +
            "(3, 'Speed (SPD)', 'Current Fighter Speed Rating.')," +
            "(4, 'Barrier (BR)', 'Amount of Shield that prevents damage from directly affecting the HP.<br>Starting as a base of 5000 HP.<br>Regenerate at a rate of 500 HP / sec after not being damaged for 10 seconds.Regenerate at a rate of 250 HP / sec after being destroyed for 20 seconds.')," +
            "(5, 'Accelerator Engine (AE)', 'Having this value above half the max points will allow the Fighter to boost forward at x1.5 normal SPD.Constant Value: 100 points.Regenerate at a rate of 5 points / sec after not boosting for 10 seconds.')," +
            "(6, 'Rotation Speed', 'How fast the Fighter turns.')," +
            "(7, 'Arc Of Fire', 'The angle at which the Fighter''s Weapons can rotate to shoot.')," +
            "(8, 'Modifier - Damage', 'Damage dealt by the weapons on this Fighter will be multiplied by this factor for the final result.')," +
            "(9, 'Modifier - AoE', 'The weapons AoE of this Fighter will be multiplied by this factor for the final result.Weapons with 0 AoE will be modified to 50.Exception: Thermal Weapon')," +
            "(10, 'Modifier - Power Cooldown', 'The Special Power Cooldown of this Fighter will be multiplied by this factor for the final result.')," +
            "(11, 'Slot', 'P: Max Special Powers can be equipped.<br>C: Max Consumables can be equipped.')," +
            "(12, 'Overheat: x | y | z', 'Heat Value starts at 0. When it reaches 100, the Overheat effect is applied - can''t shoot.<br>Overheat = 0: No Overheating.<br>x: Each shot fired increases Heat Value by x.<br>y: Time in sec after not firing that Heat Value will start to decrease(20 points per sec)<br>z: Overheat duration in sec. (After it ends, Heat Value goes back to 0.)')," +
            "(13, 'DPH: Damage Per Hit', 'The amount of damage it can cause standardly if hit.')," +
            "(14, 'AoH: Amount of Hit', 'The number of shots that the power produces peruse.')," +
            "(15, 'Rof: Rate of Fire', 'The number of shots that the weapon can fire per second.')," +
            "(16, 'AoE: Area of Effect', 'The circle area in Space Unit under the effect of the weapon on hit.Weapons with 0 AoE but modified by Fighter AoE modifier will have their AoE increased to 50.Exception: Thermal Weapon')," +
            "(17, 'Velocity', 'Projectile Speed - How fast the shot moves to its target (In Space Unit)')," +
            "(18, 'Range (in Space Unit)', 'Mention the max effective range where the weapon can hit effectively and the furthest range that a shot can hit (with 1/2 power compared to the effective range)DMG when outside of the max effective range: DMGrate = 50 % +(r2 - r) / 2 * (r2 - r1)With r being the current range travel of the bullet, r1 being the max effective range and r2 being the furthest range.')," +
            "(19, 'Duration & Cooldown (in seconds)', 'How long does the effect last? | Cooldown will start counting after the effect has worn out.')," +
            "(20, 'PoE: Points of Effect', 'For example, the amount of DMG the Situational Barrier can sustain before destruction.');";
        // Enemy
        string Enemy = "INSERT INTO Enemies VALUES " +
            "(1, 'ZatSBB', 'WSSS', 'SuicideBombing', 'HP-3000|SPD-700|ROT-0.75|AOF-0|DM-0|AM-0|PM-0', '', '50|0', 'ZatSBBs are the suicide bombing Fighters of the Zaturi. They sacrifice their lives by ramming into enemy''s Warships and Space Stations, dealing notable DMG doing so.', '#36b37e')," +
            "(2, 'ZatFT-MK1', '', 'AdvancedPulseCannon', 'HP-10000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0', 'SituationalBarrier', '150|0', 'ZatFTs are the first common Fighters ever widely manufactured by the UEC. They are equipped with Situational Barrier.', '#36b37e')," +
            "(3, 'ZatFT-MK2', '', 'AdvancedNanoFlameThrower', 'HP-8000|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0', 'SituationalBarrier', '150|0', 'MK2 version of the ZatFT exchanges Vehicle Structure Health for Faster Movement Speed. They are equipped with Situational Barrier.', '#36b37e')," +
            "(4, 'ZatFT-MK3', '', 'BlastCannon', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0', 'SituationalBarrier', '150|0', 'MK3 version of the ZatFT gets sturdier Vehicle Structure Health at the cost of Movement Speed. They are equipped with Situational Barrier.', '#36b37e')," +
            "(5, 'ZatWR-MK1', '', 'LaserCannon', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0', 'FortifiedBarrier', '250|0', 'ZatWR prototypes are the advanced Fighters of the Zaturi, with better overall characteristics compared to the ZatFT prototypes, equipped with Fortified Barrier. Their design tends to focus on better Firepower Damage Implementation, resulting in dealing more DMG from weapons.' , '#4c9aff')," +
            "(6, 'ZatWR-MK2', '', 'FreezingBlaster', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0', 'FortifiedBarrier', '250|0', 'MK2 version of the ZatWR exchanges Vehicle Structure Health for Faster Movement Speed.', '#4c9aff')," +
            "(7, 'ZatWR-MK3', '', 'AdvancedLaserCannon', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0', 'FortifiedBarrier', '250|0', 'MK3 version of the ZatWR gets sturdier Vehicle Structure Health at the cost of Movement Speed.', '#4c9aff')," +
            "(8, 'ZatWR-MKL', '', 'AdvancedLaserCannon|OrbOfVacuumGenerator', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-0.8', 'FortifiedBarrier|EnhancedRocketBurstDevice', '350|1', 'MKL version - the most refined prototype of the ZatWR. It has an advanced Power Activating Module that lowers Power Cooldown and equipped with Enhanced Short Laser Beam as a 2nd Power. However, its turning speed is quite sluggish. ', '#4c9aff')," +
            "(9, 'KazaT-MK1', '', 'LaserCannon', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0', 'FortifiedBarrier', '250|0', 'KazaT prototypes are the advanced Fighters of the ZatWR, with better overall characteristics compared to the ZatFT prototypes, equipped with Fortified Barrier. Their design tends to focus on better Firepower Area of Effect Implementation, resulting in having better AOE using weapons.', '#4c9aff')," +
            "(10, 'KazaT-MK2', '', 'OrbofLavaGenerator', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0', 'FortifiedBarrier', '250|0', 'MK2 version of the KazaT exchanges Vehicle Structure Health for Faster Movement Speed.', '#4c9aff')," +
            "(11, 'KazaT-MK3', 'WSSS', 'GravitationalArtillery', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0', 'FortifiedBarrier', '250|0', 'MK3 version of the KazaT gets sturdier Vehicle Structure Health at the cost of Movement Speed.', '#4c9aff')," +
            "(12, 'KazaT-MKL', 'WSSS', 'AdvancedLaserCannon|GravitationalArtillery', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-0.8', 'FortifiedBarrier|EnhancedRocketBurstDevice', '350|1', 'MKL version - the most refined prototype of the Kazat. It has an advanced Power Activating Module that lowers Power Cooldown and equipped with Enhanced Short Laser Beam as a 2nd Power. However, its turning speed is quite sluggish.', '#4c9aff')," +
            "(13, 'Zaturi-Prot No.1', 'WSSS', 'NanoCannon|SuperiorLaserCannon', 'HP-30000|SPD-500|ROT-1.25|AOF-120,120|DM-1.15|AM-1.0|PM-0.5', 'HeavyBarrier|SuperiorShortLaserBeam', '3500|5', 'Zaturi-Prototype No 1 is the first Elite Fighter Prototype by the Zaturi, with better overall characteristics compared to all lower-grade Fighters. It is extremely flexible at turning, having a wider  Arc of Fire. On the other hand, it also has great Firepower Damage Implementation & the best Power Activating Module, together with Heavy Barrier & Superior Short Laser Beam equipped.', '#bf2600')," +
            "(14, 'Zaturi-Ranger', 'WSSS', 'GrandGravitationalArtillery', 'HP-20000|SPD-400|ROT-0.25|AOF-45,45|DM-1.2|AM-3.0|PM-1.0', 'HeavyBarrier|SuperiorRocketBurstDevice', '3500|5', 'Zaturi-Ranger is the second version of the Elite Fighter Prototypes by the Zaturi. It possesses awesome Firepower Damage Implementation & superior Firepower Area of Effect Implementation, at the cost of both Vehicle Structure Health and Movement Speed. Its Powers are Heavy Barrier & Superior Rocket Burst Device.', '#bf2600')," +
            "(15, 'Zaturi-Warmonger', '', 'StarBlaster', 'HP-40000|SPD-600|ROT-1|AOF-60,60|DM-1.25|AM-1.0|PM-0.8', 'HeavyBarrier|AdvancedInstantWormhole', '5000|5', 'Zaturi-Warmonger is the third version of the Elite Fighter Prototypes by the Zaturi. It is the most fearsome Zaturi Fighter, with fantastic overall attributes. Additionally, it also possesses advanced Power Activating Module, with Heavy Barrier & Advanced Instant Wormhole equipped.', '#bf2600');";
        // Warship
        string Warship = "INSERT INTO Warship VALUES" +
            "(1, 'Zat-Frigate', 'The basic and the most common Warship to appear during any campaign.', 'HP-300000|SPD-200|ROT-0.25', '#4c9aff', 'Zaturi Grand Beam Cannon', 'Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Grand Blast Cannon','30000|5')," +
            "(2, 'Zat-Carrier', 'Carriers bring more Fighters to the battle consistently, at the cost of a Main Weapon.', 'HP-600000|SPD-175|ROT-0.2', '#4c9aff', 'CarrierHatch', 'Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Superior Nano Flame Thrower|Superior Nano Flame Thrower|Grand Blast Cannon','30000|5')," +
            "(3, 'Zat-Cruiser', 'Cruisers are among the advanced Warships out there, they are are the first ones to equip double Main Weapons.', 'HP-600000|SPD-175|ROT-0.2', '#4c9aff', 'Zaturi Grand Beam Cannon|Zaturi Grand Beam Cannon', 'Superior Pulse Cannon|Superior Pulse Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Advanced Freezing Blaster|Advanced Freezing Blaster','50000|10')," +
            "(4, 'Zat-Battleship', 'Battleships are the best non-elite Warships ever joined the war, with super durable armor and powerful weapons.', 'HP-1000000|SPD-175|ROT-0.2', '#4c9aff', 'Zaturi Grand Beam Cannon|Zaturi Grand Beam Cannon', 'Advanced Laser Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Advanced Laser Cannon','75000|30')," +
            "(5, 'Zat-Dreadnaught', 'The Zaturi has only two Dreadnaughts under command. They have extremely powerful firepowers and armor. Dreadnaughts usually appear on the battlefield with a whole squad of Warship.', 'HP-1500000|SPD-150|ROT-0.15', '#bf2600', 'Zaturi Grand Beam Cannon|Zaturi Super Grand Beam Cannon', 'Superior Laser Cannon|Superior Laser Cannon|Superior Laser Cannon|Superior Laser Cannon|Superior Freezing Blaster|Superior Freezing Blaster|Gravitational Artillery|Gravitational Artillery','150000|60')," +
            "(6, 'Zat-FlagShip', 'The one and only Zat-FlagShip is the best Warship out there of the Zaturi. It is commanded by the Leader of the Zaturi. It almost never appears on the battlefield, but when it does, there is a great battle to come for sure.', 'HP-2000000|SPD-150|ROT-0.15', '#bf2600', 'Zaturi Super Grand Beam Cannon|Zaturi Super Grand Beam Cannon', 'Superior Laser Cannon|Superior Laser Cannon|Plasma Cannon|Plasma Cannon|Grand Gravitational Artillery|Grand Gravitational Artillery|Nano Effect Analyzer|Nano Effect Analyzer','250000|100')," +
            "(7, 'UEC-Frigate', 'The basic and the most common Warship to appear during any campaign.', 'HP-300000|SPD-200|ROT-0.25', '#4c9aff', 'UEC Grand Beam Cannon', 'Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Grand Blast Cannon','')," +
            "(8, 'UEC-Carrier', 'Carriers bring more Fighters to the battle consistently, at the cost of a Main Weapon.', 'HP-600000|SPD-175|ROT-0.2', '#4c9aff', 'CarrierHatch', 'Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Superior Pulse Cannon|Superior Nano Flame Thrower|Superior Nano Flame Thrower|Grand Blast Cannon','')," +
            "(9, 'UEC-Cruiser', 'Cruisers are among the advanced Warships out there, they are are the first ones to equip double Main Weapons.', 'HP-600000|SPD-175|ROT-0.2', '#4c9aff', 'UEC Grand Beam Cannon|UEC Grand Beam Cannon', 'Superior Pulse Cannon|Superior Pulse Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Orb of Lava Generator|Orb of Lava Generator','')," +
            "(10, 'UEC-Battleship', 'Battleships are the best non-elite Warships ever joined the war, with super durable armor and powerful weapons.', 'HP-1000000|SPD-175|ROT-0.2', '#4c9aff', 'UEC Grand Beam Cannon|UEC Grand Beam Cannon', 'Advanced Laser Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Advanced Laser Cannon|Gravitational Artillery|Gravitational Artillery','')," +
            "(11, 'UEC-Dreadnaught', 'The United Earth Capital (UEC) has only two Dreadnaughts under command. Commanded by Field Generals, they have extremely powerful firepowers and armor. Dreadnaughts usually appear on the battlefield with a whole squad of Warship.', 'HP-1500000|SPD-150|ROT-0.15', '#bf2600', 'UEC Grand Beam Cannon|UEC Super Grand Beam Cannon', 'Nano Cannon|Nano Cannon|Nano Cannon|Nano Cannon|Superior Freezing Blaster|Superior Freezing Blaster|Gravitational Artillery|Gravitational Artillery','')," +
            "(12, 'UEC-FlagShip', 'The one and only UEC-FlagShip is the best Warship out there of the UEC. It is commanded by the Leader of the UEC. It almost never appears on the battlefield, but when it does, there is a great battle to come for sure.', 'HP-2000000|SPD-150|ROT-0.15', '#bf2600', 'UEC Super Grand Beam Cannon | UEC Super Grand Beam Cannon', 'Nano Cannon|Nano Cannon|Plasma Cannon|Plasma Cannon|Grand Gravitational Artillery|Grand Gravitational Artillery|Nano Effect Analyzer|Nano Effect Analyzer','');";
        // Space Station
        string SpaceStation = "INSERT INTO SpaceStation VALUES" +
            "(1, 'UEC-Station', 'Space Stations provide not only accommodation for the people of the UEC, but also play as a War Strategic Fortress in the Space Zone.', 'Heal|1|5', '#bf2600', 'UEC Super Grand Beam Cannon', 'Advanced Laser Cannon|Advanced Laser Cannon|Advanced Freezing Blaster|Advanced Freezing Blaster|Nano Cannon|Nano Cannon|Grand Gravitational Artillery|Grand Gravitational Artillery', '1000', '1500000', '')," +
            "(2, 'Zat-Station', 'Zaturi Space Stations have always been a pain in the ass for UEC Commanders. Encountering a Zaturi Space Station usually means it is going to be a tough battle for the UEC fellows.', 'Heal|1|5', '#bf2600', 'Zaturi Super Grand Beam Cannon', 'Advanced Laser Cannon|Advanced Laser Cannon|Advanced Freezing Blaster|Advanced Freezing Blaster|Nano Cannon|Nano Cannon|Grand Gravitational Artillery|Grand Gravitational Artillery', '1000','1500000', '100000|50');";
        // Allies
        string Allies = "INSERT INTO Allies VALUES " +
            "(1, 'SSTP', '', 'Transport', 'HP-20000|SPD-100|ROT-0.75|AOF-0|DM-0|AM-0|PM-0', '', '#36b37e')," +
            "(2, 'SS29-MK1', '', 'AdvancedPulseCannon', 'HP-10000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0', 'SituationalBarrier', '#36b37e')," +
            "(3, 'SS29-MK2', '', 'AdvancedNanoFlameThrower', 'HP-8000|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0', 'SituationalBarrier', '#36b37e')," +
            "(4, 'SS29-MK3', '', 'BlastCannon', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-1.0|PM-1.0', 'SituationalBarrier', '#36b37e')," +
            "(5, 'SSS-MK1', '', 'LaserCannon', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0', 'FortifiedBarrier', '#4c9aff')," +
            "(6, 'SSS-MK2', '', 'FreezingBlaster', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0', 'FortifiedBarrier', '#4c9aff')," +
            "(7, 'SSS-MK3', '', 'AdvancedLaserCannon', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-1.0', 'FortifiedBarrier', '#4c9aff')," +
            "(8, 'SSS-MKL', '', 'AdvancedLaserCannon|OrbofVacuumGenerator', 'HP-12000|SPD-400|ROT-0.75|AOF-90,90|DM-1.1|AM-1.0|PM-0.8', 'FortifiedBarrier|EnhancedRocketBurstDevice', '#4c9aff')," +
            "(9, 'UEC29-MK1', '', 'LaserCannon', 'HP-12000|SPD-500|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0', 'FortifiedBarrier', '#4c9aff')," +
            "(10, 'UEC29-MK2', '', 'OrbofLavaGenerator', 'HP-9600|SPD-600|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0', 'FortifiedBarrier', '#4c9aff')," +
            "(11, 'UEC29-MK3', 'WSSS', 'GravitationalArtillery', 'HP-14400|SPD-400|ROT-0.75|AOF-90,90|DM-1.0|AM-2.0|PM-1.0', 'FortifiedBarrier', '#4c9aff')," +
            "(12, 'UEC29-MKL', 'WSSS', 'AdvancedLaserCannon|GravitationalArtillery', 'HP-12000|SPD-400|ROT-0.5|AOF-90,90|DM-1.0|AM-2.0|PM-0.8', 'FortifiedBarrier|EnhancedRocketBurstDevice', '#4c9aff')," +
            "(13, 'ND-Prot No.0', '', 'NanoCannon|SuperiorLaserCannon', 'HP-30000|SPD-500|ROT-1.25|AOF-120,120|DM-1.15|AM-1.0|PM-0.5', 'HeavyBarrier|SuperiorShortLaserBeam', '#bf2600')," +
            "(14, 'ND-Zartillery', 'WSSS', 'GrandGravitationalArtillery', 'HP-20000|SPD-400|ROT-0.25|AOF-45,45|DM-1.2|AM-3.0|PM-1.0', 'HeavyBarrier|SuperiorRocketBurstDevice', '#bf2600')," +
            "(15, 'ND-MKZ', '', 'SuperiorFreezingBlaster|PlasmaCannon', 'HP-40000|SPD-600|ROT-1|AOF-90,90|DM-1.25|AM-1.0|PM-0.8', 'HeavyBarrier|AdvancedInstantWormhole', '#bf2600');";
        // Insert Fighter Group
        string FighterGroup = "INSERT INTO FighterGroup VALUES " +
            "(1, 'A1', '2,3,4', '5,9', '13',  '2,3,4', '5,9', '13')," +
            "(2, 'A2', '2,3,4', '6,10', '13',  '2,3,4', '6,10', '13')," +
            "(3, 'A3', '5,6,7', '8', '13,15',  '5,6,7', '8', '13,15')," +
            "(4, 'B1', '5,6,7', '11,12', '13,14,15',  '5,6,7', '11,12', '13,14,15')," +
            "(5, 'B2', '9,10', '12', '14', '9,10', '12', '14')," +
            "(6, 'A1350', '', '5,9', '13', '', '5,9', '13')," +
            "(7, 'A2350', '', '6,10', '13',  '', '6,10', '13')," +
            "(8, 'A3350', '', '9,8', '13,15',  '', '9,8', '13,15')," +
            "(9, 'B1350', '', '11,12', '13,14,15',  '', '11,12', '13,14,15')," +
            "(10, 'B2350', '', '12', '14', '', '12', '14');";
        // Insert Space Zone Variants
        string SpaceZoneVariants = "INSERT INTO SpaceZoneVariants VALUES " +
            "(1, 1, 2, 'AST', '#faa53d')," +
            "(2, 2, 3, 'DEF', '#1d9aaa')," +
            "(3, 3, 2, 'AST', '#faa53d')," +
            "(4, 4, 3, 'DEF', '#1d9aaa')," +
            "(5, 5, 2, 'AST', '#faa53d')," +
            "(6, 6, 3, 'DEF', '#1d9aaa')," +
            "(7, 7, 2, 'AST', '#faa53d')," +
            "(8, 8, 2, 'ONS', '#ae2a19')," +
            "(9, 9, 2, 'ONS', '#ae2a19')," +
            "(10, 0, 2, 'BOS', '#5e4db2');";
        // Insert SpaceZoneTemplate
        string SpaceZoneTemplate = "INSERT INTO SpaceZoneTemplate VALUES " +
            "(1, 1, 1, 'Assault', 'Eliminate All Enemies', 'A1', null, '50|60', '10-0-0', '10-0-0', null, null, null, null, null)," +
            "(2, 1, 2, 'Assault', 'Eliminate Target Enemies', 'A1', null, '50|75', '10-0-0', '9-1-0', null, null, null, null, null)," +
            "(3, 2, 1, 'Defend', 'Defend a UEC Space Station for an amount of time', 'A1', 60, '100|150', '10-0-0', '9-1-0', null, null, null, 300, 300)," +
            "(4, 2, 2, 'Defend', 'Survive for an amount of time', 'A1', 120, '50|150', '10-0-0', '9-1-0', null, null, null, 100, null)," +
            "(5, 2, 3, 'Defend', 'Escort Allies from A to B on the map', 'A1', null, '50|150', '10-0-0', '10-0-0', null, null, null, 100, null)," +
            "(6, 3, 1, 'Assault', 'Eliminate All Enemies', 'A1', null, '75|90', '10-0-0', '10-0-0', null, null, null, null, null)," +
            "(7, 3, 2, 'Assault', 'Eliminate Target Enemies', 'A1', null, '75|120', '10-0-0', '9-1-0', null, null, null, null, null)," +
            "(8, 4, 1, 'Defend', 'Defend a UEC Space Station for an amount of time', 'A1', 60, '125|175', '10-0-0', '9-1-0', null, null, null, 300, 300)," +
            "(9, 4, 2, 'Defend', 'Survive for an amount of time', 'A1', 120, '75|175', '10-0-0', '9-1-0', null, null, null, 100, null)," +
            "(10, 4, 3, 'Defend', 'Escort Allies from A to B on the map', 'A1', null, '75|175', '10-0-0', '10-0-0', null, null, null, null, null)," +
            "(11, 5, 1, 'Assault', 'Eliminate All Enemies', 'A1', null, '100|120', '10-0-0', '10-0-0', null, null, null, null, null)," +
            "(12, 5, 2, 'Assault', 'Eliminate Target Enemies', 'A1', null, '100|150', '10-0-0', '9-1-0', null, null, null, null, null)," +
            "(13, 6, 1, 'Defend', 'Defend a UEC Space Station for an amount of time', 'A1', 60, '150|200', '10-0-0', '9-1-0', null, null, null, 300, 300)," +
            "(14, 6, 2, 'Defend', 'Survive for an amount of time', 'A1', 120, '100|200', '10-0-0', '9-1-0', null, null, null, 100, null)," +
            "(15, 6, 3, 'Defend', 'Escort Allies from A to B on the map', 'A1', null, '100|200', '10-0-0', '10-0-0', null, null, null, null, null)," +
            "(16, 7, 1, 'Assault', 'Eliminate All Enemies', 'A1', null, '125|150', '10-0-0', '10-0-0', null, null, null, null, null)," +
            "(17, 7, 2, 'Assault', 'Eliminate Target Enemies', 'A1', null, '125|180', '10-0-0', '9-1-0', null, null, null, null, null)," +
            "(18, 8, 1, 'Onslaught', 'Eliminate a whole Zaturi Strike Forces', 'A2', null, '250|300', '9-1-0', '8-2-0', null, null, null, null, null)," +
            "(19, 8, 2, 'Onslaught', 'Join the UEC Warship(s) to defeat Zaturi Warship(s)', 'B1', null, '200|200', '9-1-0', '8-2-0', '15|15', '8-2-0', '8-2-0', 600, 300)," +
            "(20, 9, 1, 'Onslaught', 'Eliminate a whole Zaturi Strike Forces', 'A2', null, '275|325', '9-1-0', '8-2-0', null, null, null, null, null)," +
            "(21, 9, 2, 'Onslaught', 'Join the UEC Warship(s) to defeat Zaturi Warship(s)', 'B1', null, '225|225', '9-1-0', '8-2-0', '15|15', '8-2-0', '8-2-0', 600, 300)," +
            "(22, 0, 1, 'Boss Encounter', 'Defeat Zaturi Warship(s)', 'B2', null, '200|100',  '8-2-0',  '8-2-0', '0|5', null, '1-0-0', null, null)," +
            "(23, 0, 2, 'Boss Encounter', 'Defeat Zaturi Elite Fighter(s)', 'A3', null, '150|175', '8-2-0', '8-1-1', null, null, null, null, null);";
        // Insert SpaceZonePosition
        string SpaceZonePosition = "INSERT INTO SpaceZonePosition VALUES " +
            "(1, 'PN', '(-3500,-700)', '(-2100,700)')," +
            "(2, 'AA', '(-2100,3500)', '(-700,-3500)')," +
            "(3, 'AB', '(-3500,2100)', '(-2100,-2100)')," +
            "(4, 'AC', '(-4900,-700)', '(-4900,700)')," +
            "(5, 'EA', '(2100,3500)', '(700,-3500)')," +
            "(6, 'EB', '(3500,-700)', '(2100,2100)')," +
            "(7, 'EC', '(4900,-700)', '(4900,700)')," +
            "(8, 'AX', '(-2100,3500)|(-2100,-700)', '(-700,700)|(-700,-3500)')," +
            "(9, 'AY', '(-3500,2100)|(-3500,-700)|(-2100,700)', '(-2100,700)|(-2100,-2100)|(-700,-700)')," +
            "(10, 'EX', '(2100,3500)|(2100,-700)', '(700,700)|(700,-3500)')," +
            "(11, 'EY', '(3500,2100)|(3500,-700)|(2100,700)', '(2100,700)|(2100,-2100)|(700,-700)')," +
            "(12, 'AO', '(-3500,700)', '(-2100,-700)')," +
            "(13, 'EO', '(3500,700)', '(2100,-700)')," +
            "(14, 'AS', '(-4900,4900)|(-4900,-3500)', '(-3500,-3500)|(-3500,-4900)')," +
            "(15, 'ES', '(3500,4900)|(3500,-3500)', '(4900,3500)|(4900,-4900)')," +
            "(16, 'PE', '(-3500,3500)', '(-2100,2100)')," +
            "(17, 'AES', '(-4900,4900)', '(-3500,-3500)')," +
            "(18, 'AEE', '(3500,-3500)', '(4900,-4900)')," +
            "(19, 'EES', '(-4900,-4900)|(3500,4900)', '(-3500,-3500)|(4900,3500)')," +
            "(20, 'EAS', '(-4900,3500)|(-3500,4900)', '(-3500,2100)|(-2100,3500)'); ";
        // Insert Hazard Environment
        string HazardEnvironment = "INSERT INTO HazardEnvironment VALUES " +
            "(1, 'N', 'None', '#ffffff', 'Ordinary Environment', 65, 0, '')," +
            "(2, 'SR', 'Unknown Asteroids', '#2898bd', 'Leftovers of destroyed planets around the galaxies', 10, 11, '')," +
            "(3, 'O', 'Overloaded', '#fea362', 'Unexpected Heat from unknown sources', 10, 31, 'OVL')," +
            "(4, 'GR', 'Gamma Ray Burst', '#22a06b', 'Deadly burst that intensively affects Fighter’s survival abilities', 5, 51, 'GAM')," +
            "(5, 'RS', 'Rogue Star', '#ae2e24', 'Random comets wander the nebula', 5, 71, '')," +
            "(6, 'ND', 'Nebula Disorientation', '#5e4db2', 'One of the Origins of the ongoing galaxy war', 5, 101, 'NEB');";
        // Insert WarshipMilestone
        string WarshipMilestone = "INSERT INTO WarshipMilestone VALUES " +
            "(1, 0, 7, 8, 9, 1, 2, 3)," +
            "(2, 250, 9, 8, 10, 3, 2, 4)," +
            "(3, 500, 9, 8, 11, 3, 2, 5)," +
            "(4, 700, 10, 8, 12, 4, 2, 6);";
        // Insert SpaceZoneMission
        string SpaceZoneMission = "INSERT INTO SpaceZoneMission VALUES " +
            "(1, 1, 1, 'Eliminate All Enemies.', 'Elimination of all enemies.|Survival of the player.', 'Destruction of the player.')," +
            "(2, 1, 2, 'Eliminate Target Enemies.', 'Elimination of All Targets.|Survival of the player.', 'Destruction of the player.')," +
            "(3, 2, 1, 'Defend an UEC-Station for 60 seconds.', 'Survival of the UEC-Station.|Timer ran out.|Survival of the player.', 'Destruction of the UEC-Station.|Destruction of the player.')," +
            "(4, 2, 2, 'Survive for 120 seconds.', 'Timer ran out.|Survival of the player.', 'Destruction of the player.')," +
            "(5, 2, 3, 'Escort SSTPs from A to B on the map.', 'At least half of the SSTPs safely escaped.|Survival of the player.', 'At least half of the SSTPs were destroyed.|Destruction of the player.')," +
            "(6, 3, 1, 'Eliminate All Enemies.', 'Elimination of all enemies.|Survival of the player.', 'Destruction of the player.')," +
            "(7, 3, 2, 'Eliminate Target Enemies.', 'Elimination of All Targets.|Survival of the player.', 'Destruction of the player.')," +
            "(8, 4, 1, 'Defend an UEC-Station for 60 seconds.', 'Survival of the UEC-Station.|Timer ran out.|Survival of the player.', 'Destruction of the UEC-Station.|Destruction of the player.')," +
            "(9, 4, 2, 'Survive for 120 seconds.', 'Timer ran out.|Survival of the player.', 'Destruction of the player.')," +
            "(10, 4, 3, 'Escort SSTPs from A to B on the map.', 'At least half of the SSTPs safely escaped.|Survival of the player.', 'At least half of the SSTPs were destroyed.|Destruction of the player.')," +
            "(11, 5, 1, 'Eliminate All Enemies.', 'Elimination of all enemies.|Survival of the player.', 'Destruction of the player.')," +
            "(12, 5, 2, 'Eliminate Target Enemies.', 'Elimination of All Targets.|Survival of the player.', 'Destruction of the player.')," +
            "(13, 6, 1, 'Defend an UEC-Station for 60 seconds.', 'Survival of the UEC-Station.|Timer ran out.|Survival of the player.', 'Destruction of the UEC-Station.|Destruction of the player.')," +
            "(14, 6, 2, 'Survive for 120 seconds.', 'Timer ran out.|Survival of the player.', 'Destruction of the player.')," +
            "(15, 6, 3, 'Escort SSTPs from A to B on the map.', 'At least half of the SSTPs safely escaped.|Survival of the player.', 'At least half of the SSTPs were destroyed.|Destruction of the player.')," +
            "(16, 7, 1, 'Eliminate All Enemies.', 'Elimination of all enemies.|Survival of the player.', 'Destruction of the player.')," +
            "(17, 7, 2, 'Eliminate Target Enemies.', 'Elimination of All Targets.|Survival of the player.', 'Destruction of the player.')," +
            "(18, 8, 1, 'Eliminate a whole Zaturi Strike Forces.', 'Annihilation of All Enemies.|Survival of the player.', 'Destruction of the player.')," +
            "(19, 8, 2, 'Join the UEC Warship(s) to defeat Zaturi Warship(s).', 'Destruction of all Zaturi Warship(s).|Survival of the player.', 'Destruction of all UEC Warship(s).|Destruction of the player.')," +
            "(20, 9, 1, 'Eliminate a whole Zaturi Strike Forces.', 'Annihilation of All Enemies.|Survival of the player.', 'Destruction of the player.')," +
            "(21, 9, 2, 'Join the UEC Warship(s) to defeat Zaturi Warship(s).', 'Destruction of all Zaturi Warship(s).|Survival of the player.', 'Destruction of all UEC Warship(s).|Destruction of the player.')," +
            "(22, 0, 1, 'Defeat Zaturi Warship(s).', 'Destruction of All Zaturi Warship(s).|Survival of the player.', 'Destruction of the player.')," +
            "(23, 0, 2, 'Defeat Zaturi Elite Fighter(s).', 'Elimination of All Zaturi Elite Fighter(s).|Survival of the player.', 'Destruction of the player.');";
        //Insert ArsenalService
        string ArsenalService = "INSERT INTO ArsenalService VALUES " +
            "(1, 'Repairing I', 'Grade3-1250|Grade2-3750|Grade1-7500', '25')," +
            "(2, 'Repairing II', 'Grade3-2500|Grade2-7500|Grade1-15000', '50')," +
            "(3, 'Repairing III', 'Grade3-5000|Grade2-15000|Grade1-15000', '100');";
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
        dbCommandInsertValue.CommandText = 
            RankSystem + SpaceShop + ArsenalPower + ArsenalWeapon + FactoryModel
            + LOTWCards + DailyMissions + Option + Tutorial + DElement 
            + Attribute + Enemy + Warship + SpaceStation + Allies + FighterGroup
            + SpaceZoneVariants + SpaceZoneTemplate + SpaceZonePosition + HazardEnvironment
            + WarshipMilestone + SpaceZoneMission + ArsenalService;
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
            // PlayerSpaceShopItem
            "DROP TABLE IF EXISTS PlayerSpaceShopItem;" +
            // PlayerDailyMission
            "DROP TABLE IF EXISTS PlayerDailyMission;" +
            /*// SessionArsenalPower
            "DROP TABLE IF EXISTS SessionArsenalPower;" +
            // SessionArsenalWeapons
            "DROP TABLE IF EXISTS SessionArsenalWeapons;" +*/
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
