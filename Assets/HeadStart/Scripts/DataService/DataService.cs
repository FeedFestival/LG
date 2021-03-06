using System;
using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;

public class DataService
{
#pragma warning disable 0414 // private field assigned but not used.
    public static readonly string _version = "1.0.3";
#pragma warning restore 0414 //
    public string DefaultDatabaseName = "Database.db";
    const string _assetsPath = "Assets"; // Assets/HeadStart
    private SQLiteConnection _connection;

    public DataService(string databaseName = null)
    {
        if (string.IsNullOrEmpty(databaseName) == false)
        {
            DefaultDatabaseName = databaseName;
        }
        #region DataServiceInit
#if UNITY_EDITOR
        var dbPath = string.Format(_assetsPath + @"/StreamingAssets/{0}", DefaultDatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DefaultDatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DefaultDatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DefaultDatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DefaultDatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DefaultDatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DefaultDatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif

        #endregion

        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);
    }

    public void CleanUpUsers()
    {
        _connection.DropTable<User>();
        _connection.CreateTable<User>();
    }

    public void CleanDB()
    {
        _connection.DropTable<User>();
        _connection.DropTable<WeekScore>();
        _connection.DropTable<HighScore>();
    }

    public void CreateDB()
    {
        _connection.CreateTable<User>();
        _connection.CreateTable<WeekScore>();
        _connection.CreateTable<HighScore>();
        Debug.Log("Created Tables: User, WeekScore, HighScore");
    }

    public void CreateDBIfNotExists()
    {
        try
        {
            _connection.Table<User>().Where(x => x.Id == 1).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            _connection.CreateTable<User>();
        }
        try
        {
            _connection.Table<WeekScore>().Where(x => x.Id == 1).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            _connection.CreateTable<WeekScore>();
        }
        try
        {
            _connection.Table<HighScore>().Where(x => x.Id == 1).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            _connection.CreateTable<HighScore>();
        }
    }

    /*
     * User
     * * --------------------------------------------------------------------------------------------------------------------------------------
     */

    public void CreateUser(User user)
    {
        _connection.Insert(user);
    }

    public void UpdateUser(User user)
    {
        int rowsAffected = _connection.Update(user);
        Debug.Log("(UPDATE User) rowsAffected : " + rowsAffected);
    }

    public User GetUser()
    {
        try
        {
            return _connection.Table<User>().Where(x => x.Id == 1).FirstOrDefault();
        }
        catch (Exception)
        {
            _connection.CreateTable<User>();
            return _connection.Table<User>().Where(x => x.Id == 1).FirstOrDefault();
        }
    }

    public User GetTheUser()
    {
        if (_connection.Table<User>().Count() > 0)
        {
            return _connection.Table<User>().First();
        }
        return null;
    }

    internal WeekScore GetHighestWeekScore(int weekId)
    {
        WeekScore weekScore = _connection.Table<WeekScore>().FirstOrDefault(ws => ws.Id == weekId);
        return weekScore;
    }

    internal void AddWeekHighScore(WeekScore weekScore)
    {
        _connection.Insert(weekScore);
    }

    internal void UpdateWeekHighScore(WeekScore weekScore)
    {
        int rowsAffected = _connection.Update(weekScore);
        Debug.Log("(UPDATE WeekScore) rowsAffected : " + rowsAffected);
    }

    internal void AddHighScore(HighScore highScore)
    {
        int rowsAffected = _connection.Insert(highScore);
        Debug.Log("(CREATED HighScore) rowsAffected : " + rowsAffected);
    }

    internal List<HighScore> GetHotSeatScores()
    {
        // _connection.Table<HighScore>().Where(hs => hs.);
        List<HighScore> highscores = _connection.Query<HighScore>(@"
SELECT
	hs.TypeId,
	hs.UserId,
	hs.Points,
	usr.Name as UserName
FROM HighScore as hs
	INNER JOIN User as usr ON usr.Id = hs.UserId
WHERE TypeId = " + (int)HighScoreType.HOTSEAT + @"
ORDER BY Points DESC
        ");
        return highscores;
    }

    internal List<User> GetUsers()
    {
        List<User> users = _connection.Query<User>(@"
SELECT
    usr.Id,
    usr.Name
FROM User as usr
ORDER BY usr.Id DESC
LIMIT 4
        ");
        return users;
    }

    internal User GetUserByName(string name)
    {
        string sql = @"
SELECT *
FROM User as usr
WHERE lower(usr.Name) = """ + name.ToLower() + @"""
ORDER BY usr.Id DESC
        ";
        Debug.Log("sql: " + sql);
        List<User> users = _connection.Query<User>(sql);

        if (users == null || users.Count == 0)
        {
            return null;
        }
        return users[0];
    }

    internal void WriteDefaultData()
    {
        // Write Races
        var humanRace = new Race()
        {
            Name = "Human",
            ProgId = "HUMANS",
            Playable = true
        };
        var raceId = CreateRace(humanRace);

        // Write Units
        var unit = new UnitData()
        {
            Name = "Archer",
            PrefabName = "Archer",
            RaceId = raceId
        };
        CreateUnitData(unit);
        unit = new UnitData()
        {
            Name = "ShieldMan",
            PrefabName = "ShieldMan",
            RaceId = raceId
        };
        CreateUnitData(unit);
    }

    #region Races

    public int CreateRace(Race race)
    {
        return _connection.Insert(race);
    }

    public Race GetRace(string raceProgId)
    {
        try
        {
            return _connection.Table<Race>()
                .Where(x => x.ProgId == raceProgId)
                .FirstOrDefault();
        }
        catch (Exception)
        {
            _connection.CreateTable<Race>();
            _connection.CreateTable<UnitData>();
            WriteDefaultData();
            return _connection.Table<Race>()
                .Where(x => x.ProgId == raceProgId)
                .FirstOrDefault();
        }
    }

    #endregion

    #region UnitData 

    public int CreateUnitData(UnitData unitData)
    {
        return _connection.Insert(unitData);
    }

    public IEnumerable<UnitData> GetRaceUnits(int raceId)
    {
        return _connection.Table<UnitData>().Where(x => x.RaceId == raceId);
    }

    #endregion


    //public User GetUserByFacebookId(int facebookId)
    //{
    //    return _connection.Table<User>().Where(x => x.FacebookApp.FacebookId == facebookId).FirstOrDefault();
    //}

    /*
    * User - END
    * * --------------------------------------------------------------------------------------------------------------------------------------
    */

    /*
     * Map
     * * --------------------------------------------------------------------------------------------------------------------------------------
     */

    // X : 0 - 11
    // Y : 0 - 8
    //public int CreateMap(Map map)
    //{
    //    _connection.Insert(map);
    //    return map.Id;
    //}

    //public int UpdateMap(Map map)
    //{
    //    _connection.Update(map);
    //    return map.Id;
    //}

    //public Map GetMap(int mapId)
    //{
    //    return _connection.Table<Map>().Where(x => x.Id == mapId).FirstOrDefault();
    //}

    //public int GetNextMapId(int number)
    //{
    //    return _connection.Table<Map>().Where(x => x.Number == number).FirstOrDefault().Id;
    //}

    //public void CreateTiles(List<MapTile> mapTiles)
    //{
    //    _connection.InsertAll(mapTiles);
    //}

    //public IEnumerable<Map> GetMaps()
    //{
    //    return _connection.Table<Map>();
    //}

    //public IEnumerable<MapTile> GetTiles(int mapId)
    //{
    //    return _connection.Table<MapTile>().Where(x => x.MapId == mapId);
    //}

    //public void DeleteMapTiles(int mapId)
    //{
    //    var sql = string.Format("delete from MapTile where MapId = {0}", mapId);
    //    _connection.ExecuteSql(sql);
    //}
}
