using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// リバーシの結果表示用のオブジェクト
/// </summary>
public class ReversiResultObject : MonoBehaviour
{
    /// <summary>
    /// 勝者カラーを適用するImageオブジェクトのリスト
    /// </summary>
    [SerializeField]
    private List<Image> _imagesToApplyWinnerColor;

    /// <summary>
    /// リザルト表示オブジェクトへの参照
    /// </summary>
    [SerializeField]
    private GameObject _resultPanel;

    /// <summary>
    /// 黒石の数を表示するText
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _blackAmount;

    /// <summary>
    /// 白石の数を表示するText
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _whiteAmount;

    /// <summary>
    /// 勝者を表示するText
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _winnerText;

    /// <summary>
    /// 勝者を表示するImage
    /// </summary>
    [SerializeField]
    private Image _winnerPanel;


    /// <summary>
    /// 初回Update直前にコール
    /// このスクリプトがアタッチされているオブジェクトをリザルトオブジェクトとして設定
    /// </summary>
    private void Start()
    {
        _resultPanel = gameObject;
    }

    /// <summary>
    /// リザルト情報を設置
    /// </summary>
    /// <param name="black">黒石の数</param>
    /// <param name="white">白石の数</param>
    public void SetResult(int black,int white)
    {
        _blackAmount.SetText(black.ToString());
        _whiteAmount.SetText(white.ToString());


        if(black > white)           // 黒の勝ち
        {
            _winnerPanel.color = Color.black;
            _winnerText.color = Color.white;
            _winnerText.SetText("Black Won!");
        }
        else if (black == white)    // 引き分け
        {
            _winnerPanel.color = Color.gray;
            _winnerText.color = Color.white;
            _winnerText.SetText("Draw!");
        }
        else                        // 白の勝ち
        {
            _winnerPanel.color = Color.white;
            _winnerText.color = Color.black;
            _winnerText.SetText("White Won!");
        }
    }

    /// <summary>
    /// リザルト表示
    /// </summary>
    public void Show()
    {
        _resultPanel.SetActive(true);
    }

    /// <summary>
    /// リザルト非表示
    /// </summary>
    public void Hide()
    {
        _resultPanel.SetActive(false);
    }
}
