using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    /*
     * Singleton GAME!
     */
    private static Board _this;
    public static Board _ { get { return _this; } }
    void Awake()
    {
        _this = this;

        BuildingGrid.SetActive(false);
    }

    public delegate void DrawUnitCallback(
        int x, int y
        );

    public delegate void TryPlaceUnitCallback(
        int unitId
        );

    public MapController MapController;
    public GameObject BuildingGrid;

    public Place[,] boardGridPlaces;

    public List<Unit> PlayerUnitsPool;

    public GameObject UnitGoInHand;
    public int SelectedUnitIndex;

    LTDescr _moveUnitInHand;

    private int _xLength = 13;
    private int _yLength = 10;

    private bool _listenForPlacementConfirmation;

    private Vector3 _unitsToSelectPosition;

    int boardX;
    int boardY;
    int box1X;
    int box1Y;
    int box2X;
    int box2Y;
    int box3X;
    int box3Y;

    void Start()
    {
        _unitsToSelectPosition = new Vector3(0, 100, 100);
    }

    public void Init(string raceProgId)
    {
        var race = Game._.DataService.GetRace(raceProgId);
        var units = Game._.DataService.GetRaceUnits(race.Id);

        BoardController._.gameObject.SetActive(true);

        PlayerUnitsPool = new List<Unit>();

        // 2.Show Units in the ui.
        foreach (UnitData unit in units)
        {
            GameObject go = Instantiate(
                Resources.Load("Prefabs/UI/UnitBoardData", typeof(GameObject)),
                Vector3.zero, Quaternion.identity,
                BoardController._.BoardListPanel.transform
            ) as GameObject;
            var rect = go.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localEulerAngles = Vector3.zero;

            // Instantiate a unit copy too.
            GameObject unitGo = Instantiate(
            Resources.Load("Prefabs/" + unit.PrefabName, typeof(GameObject)),
            _unitsToSelectPosition, Quaternion.identity
            ) as GameObject;

            var u = unitGo.GetComponent<Unit>();
            u.BoardSetup(unit.Id);

            PlayerUnitsPool.Add(u);

            var ubd = go.GetComponent<UnitBoardData>();
            ubd.Init(
                unit.Id,
                unit.Name,
                100,
                TryPlaceUnit
                );
        }

        InitGrids();
    }

    private void TryPlaceUnit(int unitId)
    {
        SelectedUnitIndex = PlayerUnitsPool.FindIndex(u => u.DataId == unitId);
        UnitGoInHand = PlayerUnitsPool[SelectedUnitIndex].gameObject;

        ShowUnitGrid();
    }

    private void ConfirmPlaceUnit()
    {
        HideGrid();
        _listenForPlacementConfirmation = false;

        GameObject go = Instantiate(UnitGoInHand, UnitGoInHand.transform.position, UnitGoInHand.transform.rotation) as GameObject;

        Fight._.AddPlayerUnit(go.GetComponent<Unit>());

        UnitGoInHand = null;
        Debug.Log("- reset position");
        if (_moveUnitInHand != null)
        {
            LeanTween.cancel(_moveUnitInHand.id);
        }
        PlayerUnitsPool[SelectedUnitIndex].gameObject.transform.localPosition = _unitsToSelectPosition;
    }

    private void InitGrids()
    {
        boardGridPlaces = new Place[_xLength + 1, _yLength + 1];
        float y = 0f;
        int fakeX = 0;
        for (var x = 0; x >= -_xLength; x--)
        {
            for (var z = 0; z <= _yLength; z++)
            {
                var go = Instantiate(Resources.Load("Prefabs/map/plane", typeof(GameObject)),
                                        Vector3.zero, Quaternion.identity
                                        ) as GameObject;
                go.transform.SetParent(BuildingGrid.transform);
                go.transform.localPosition = new Vector3(x, y, z);
                go.transform.localEulerAngles = new Vector3(-90, 0, 0);

                var mouseArea = go.GetComponent<MouseArea>();

                var place = new Place(
                    fakeX,
                    z,
                    mouseArea,
                    DrawUnit
                    );
                boardGridPlaces[fakeX, z] = place;
            }
            fakeX++;
        }
    }

    private void DrawUnit(int x, int y)
    {
        boardGridPlaces[boardX, boardY].ChangeState(Place.PlaceState.Idle);
        boardGridPlaces[box1X, box1Y].ChangeState(Place.PlaceState.Idle);
        boardGridPlaces[box3X, box3Y].ChangeState(Place.PlaceState.Idle);
        boardGridPlaces[box2X, box2Y].ChangeState(Place.PlaceState.Idle);

        boardX = x;
        boardY = y;

        // base off of unit.
        var unitGridSize = 4;
        if (unitGridSize == 4)
        {
            // horizontal
            box1Y = y;
            box1X = ReturnBounds(x, _xLength - 1);
            // angle
            box3X = ReturnBounds(x, _xLength - 1);
            box3Y = ReturnBounds(y, _yLength - 1);
            // down
            box2X = x;
            box2Y = ReturnBounds(y, _yLength - 1);

            boardGridPlaces[boardX, boardY].ChangeState(Place.PlaceState.Hover);
            boardGridPlaces[box1X, box1Y].ChangeState(Place.PlaceState.Hover);
            boardGridPlaces[box3X, box3Y].ChangeState(Place.PlaceState.Hover);
            boardGridPlaces[box2X, box2Y].ChangeState(Place.PlaceState.Hover);
        }

        var pos = WorldUtils.GetMidPointOffset(
                                    boardGridPlaces[x, y].mouseArea.transform.position,
                                    boardGridPlaces[box3X, box3Y].mouseArea.transform.position,
                                    returnMidPoint: true
                                    );
        var distance = Vector3.Distance(UnitGoInHand.transform.position, pos);
        if (distance > 15)
            UnitGoInHand.transform.position = pos;
        else
        {
            if (_moveUnitInHand != null)
                LeanTween.cancel(_moveUnitInHand.id);
            _moveUnitInHand = LeanTween.move(UnitGoInHand, pos, 0.5f).setEase(LeanTweenType.easeOutBack);
        }
        _listenForPlacementConfirmation = true;
    }

    private int ReturnBounds(int val, int maxVal)
    {
        if (val + 1 < maxVal)
        {
            return val + 1;
        }
        return val - 1;
    }

    public void ShowUnitGrid()
    {
        if (MapController.CurrentPlatformPosition == MapController.PlatformPosition.Down)
        {
            Timer._.InternalWait(ShowGrid, MapController.GetLastBlockTimeToPosition());
        }
        else
        {
            ShowGrid();
        }
    }

    private void ShowGrid()
    {
        BuildingGrid.SetActive(true);
    }

    private void HideGrid()
    {
        BuildingGrid.SetActive(false);
    }

    void LateUpdate()
    {
        if (_listenForPlacementConfirmation == true)
            if (Input.GetMouseButton(0))
                ConfirmPlaceUnit();
    }
}
