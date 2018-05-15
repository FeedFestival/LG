using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region singleton
    private static Game _game;
    public static Game Instance()
    {
        return _game;
    }
    void Awake()
    {
        _game = this;
    }
    #endregion

    private float _timeSpeed = 1.0f;

    public CameraController CameraController;
    public UiController UiController;

    public Color PlayerHpBars;
    public Color EnemyHpBars;

    private DataService _dataService;
    public DataService DataService
    {
        get
        {
            if (_dataService == null)
                _dataService = new DataService("Database.db");
            return _dataService;
        }
    }

    #region DataBase Editor Functions

    public void CleanUpUsers()
    {
        DataService.CleanUpUsers();
    }

    public void WriteDefaultData()
    {
        DataService.WriteDefaultData();
    }
    
    public void RecreateDataBase()
    {
        DataService.CreateDB();
    }

    #endregion

    #region Global WaitForSeconds, TimeSpeed

    public delegate void EndOfWait();
    public static IEnumerator WaitForSeconds(float seconds, EndOfWait endOfWait)
    {
        yield return new WaitForSeconds(seconds);

        endOfWait();
    }

    /// <summary>
    /// Usefull for debuging
    /// </summary>
    public void ChangeTimeSpeed(float timeSpeed)
    {
        Time.timeScale = timeSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            var newSpeed = _timeSpeed + 0.1f;
            if (newSpeed > 1f)
                return;
            _timeSpeed = _timeSpeed + 0.1f;

            UiController.TimeSpeedText.text = _timeSpeed.ToString();
            Time.timeScale = _timeSpeed;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            var newSpeed = _timeSpeed - 0.1f;
            if (newSpeed < 0f)
                return;
            _timeSpeed = _timeSpeed - 0.1f;

            UiController.TimeSpeedText.text = _timeSpeed.ToString();
            Time.timeScale = _timeSpeed;
        }
    }

    #endregion
}
