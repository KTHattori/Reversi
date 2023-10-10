using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ReversiResultObject : MonoBehaviour
{
    [SerializeField]
    private List<Image> _imagesToApplyWinnerColor;

    [SerializeField]
    private GameObject _resultPanel;

    [SerializeField]
    private TextMeshProUGUI _blackAmount;

    [SerializeField]
    private TextMeshProUGUI _whiteAmount;

    [SerializeField]
    private TextMeshProUGUI _winnerText;

    [SerializeField]
    private Image _winnerPanel;


    // Start is called before the first frame update
    private void Start()
    {
        _resultPanel = gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void SetResult(int black,int white)
    {
        _blackAmount.SetText(black.ToString());
        _whiteAmount.SetText(white.ToString());

        if(black > white)
        {
            _winnerPanel.color = Color.black;
            _winnerText.color = Color.white;
            _winnerText.SetText("Black Won!");
        }
        else if (black == white)
        {
            _winnerPanel.color = Color.gray;
            _winnerText.color = Color.white;
            _winnerText.SetText("Draw!");
        }
        else
        {
            _winnerPanel.color = Color.white;
            _winnerText.color = Color.black;
            _winnerText.SetText("White Won!");
        }
    }

    public void Show()
    {
        _resultPanel.SetActive(true);
    }

    public void Hide()
    {
        _resultPanel.SetActive(false);
    }
}
