using UnityEngine;

/// <summary>
/// Unityの組み込みイベント関数を継承先で上書きさせないようにするためのMonoBehaviourクラス <br/>
/// 現状対応している関数: <br/>
/// Awake, OnEnable, Reset, Start <br/>
/// FixedUpdate, Update, LateUpdate
/// </summary>
/// 
namespace T0R1.UI
{
    public class SealableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Reset()
        {
            
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void Update()
        {
            
        }

        protected virtual void LateUpdate()
        {

        }
    }

}
