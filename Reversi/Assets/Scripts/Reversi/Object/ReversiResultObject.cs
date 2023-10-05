using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ReversiResultObject : MonoBehaviour
{
    [SerializeField]
    List<Image> imagesToApplyWinnerColor = new List<Image>();

    [SerializeField]
    GameObject resultPanel;

    [SerializeField]
    TextMeshProUGUI blackAmount;

    [SerializeField]
    TextMeshProUGUI whiteAmount;

    [SerializeField]
    TextMeshProUGUI winnerText;
    [SerializeField]
    Image winnerPanel;


    // Start is called before the first frame update
    void Start()
    {
        resultPanel = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResult(int black,int white)
    {
        blackAmount.SetText(black.ToString());
        whiteAmount.SetText(white.ToString());

        if(black > white)
        {
            winnerPanel.color = Color.black;
            winnerText.color = Color.white;
            winnerText.SetText("Black Won!");
        }
        else if (black == white)
        {
            winnerPanel.color = Color.gray;
            winnerText.color = Color.white;
            winnerText.SetText("Draw!");
        }
        else
        {
            winnerPanel.color = Color.white;
            winnerText.color = Color.black;
            winnerText.SetText("White Won!");
        }
    }

    public void Show()
    {
        resultPanel.SetActive(true);
    }

    public void Hide()
    {
        resultPanel.SetActive(false);
    }
}
