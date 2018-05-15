using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public Text TimeSpeedText;

    public RectTransform Canvas;

    public GameObject MainMenuPanel;

    public GameObject HealthBarsPanel;

    public GameObject UnitTestingPanel;

    public GameObject BoardPanel;
    public GameObject BoardListPanel;

    void Awake()
    {
        MainMenuPanel.SetActive(false);
        HealthBarsPanel.gameObject.SetActive(false);
        UnitTestingPanel.SetActive(false);
        BoardPanel.SetActive(false);
    }

    public void Init()
    {

    }
}
