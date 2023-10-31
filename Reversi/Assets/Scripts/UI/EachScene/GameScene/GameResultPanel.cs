using System.Collections;
using System.Collections.Generic;
using Interpolation;
using T0R1.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultPanel : ModalPanel
{
    [System.Serializable]
    public class ScoreLayout
    {
        public TextMeshProUGUI _scoreText;
        public LayoutElement _upperLE;
        public LayoutElement _lowerLE;

        private float _upperInitHeight;
        private float _lowerInitHeight;

        public void GetInitValue()
        {
            _lowerInitHeight = _lowerLE.flexibleHeight;
            _upperInitHeight = _upperLE.flexibleHeight;
        }

        public void Ascend(float value)
        {
            _lowerLE.flexibleHeight = _lowerInitHeight + value;
        }

        public void Descend(float value)
        {
            _upperLE.flexibleHeight = _upperInitHeight + value;
        }
    }

    [SerializeField]
    ButtonTextEdit _backToTitleButton;
    public ButtonTextEdit BackTitleButton { get { return _backToTitleButton; } }

    [SerializeField]
    TextMeshProUGUI _winnerText;
    public TextMeshProUGUI WinnerText { get { return _winnerText; } }

    [SerializeField]
    ScoreLayout _blackScore;

    [SerializeField]
    ScoreLayout _whiteScore;

    [SerializeField]
    LayoutElement _middleAreaLE;

    private float _animationProg;
    private bool _isAnimating = false;
    private bool _blackIsWin = false;

    private static readonly float TIME_KEY = 0.5f;

    protected override void OnStart()
    {
        _winnerText.gameObject.SetActive(false);
    }

    public void SetResult(int black,int white,int clientSide)
    {
        _blackScore._scoreText.SetText(black.ToString());
        _whiteScore._scoreText.SetText(white.ToString());
        if(black > white) _blackIsWin = true;
        else _blackIsWin = false;
        if(clientSide == 0)
        {
            if(_blackIsWin) _winnerText.SetText("WIN!");
            else _winnerText.SetText("LOSE...");
        }
        else
        {
            if(_blackIsWin) _winnerText.SetText("LOSE...");
            else _winnerText.SetText("WIN!");
        }
        ShowAnimation();
    }

    public void ShowAnimation()
    {
        _isAnimating = true;
        _animationProg = 0.0f;
    }

    private void OnAnimationCompleted()
    {
        _isAnimating = false;
        _winnerText.gameObject.SetActive(true);
        _backToTitleButton.Show();
    }

    protected override void OnUpdate()
    {
        if(_isAnimating)
        {
            _animationProg += Time.deltaTime * 0.5f;
            if(TIME_KEY < _animationProg)
            {
                if(_blackIsWin)
                {
                    _blackScore.Ascend(Easing.EaseInOut(0.0f,5.0f,_animationProg,1.0f,Easing.Style.Exponential));
                    _whiteScore.Descend(Easing.EaseInOut(0.0f,5.0f,_animationProg,1.0f,Easing.Style.Exponential));
                }
                else
                {
                    _blackScore.Descend(Easing.EaseInOut(0.0f,5.0f,_animationProg,1.0f,Easing.Style.Exponential));
                    _whiteScore.Ascend(Easing.EaseInOut(0.0f,5.0f,_animationProg,1.0f,Easing.Style.Exponential));
                }
            }
            else
            {
                _middleAreaLE.flexibleWidth = Easing.EaseInOut(0.0f,5.0f,_animationProg,TIME_KEY,Easing.Style.Exponential);
            }
            if(_animationProg >= 1.0f) OnAnimationCompleted();
        }
    }
    


}
