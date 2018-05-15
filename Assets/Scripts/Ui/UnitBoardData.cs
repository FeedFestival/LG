using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitBoardData : MonoBehaviour
{
    public Text UnitName;
    public EventTrigger EventTrigger;

    private int _unitId;

    public void Init(
        int unitId,
        string unitName,
        int UnitCost,
        Board.TryPlaceUnitCallback tryPlaceUnit
        // UnitTextureIcon
        // 
        )
    {
        _unitId = unitId;
        UnitName.text = unitName;

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entry.callback.AddListener((eventData) => {
            tryPlaceUnit(_unitId);
        });

        EventTrigger.triggers.Clear();
        EventTrigger.triggers.Add(entry);

        //entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        //entry.callback.AddListener((eventData) => { sphere.StopDirection(Move.Right); });

        //EventTrigger.triggers.Add(entry);
    }
}
