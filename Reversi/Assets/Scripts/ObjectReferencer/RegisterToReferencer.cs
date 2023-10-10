using UnityEngine;
using UnityEngine.Events;

namespace RegisterObject
{
	/// <summary>
	/// オブジェクト媒介用のScriptableObjectへ参照を渡すスクリプト
	/// </summary>
	[DefaultExecutionOrder(-100)]
	public class RegisterToReferencer : MonoBehaviour 
	{
		[SerializeField,Header("受け渡し後に非Activeにする")]
		bool deactivateAfterRegister = false;
		[SerializeField,Header("対象となる媒介用ScriptableObject")]
        ObjectReferencer relay;
		void Awake()
		{
			relay.gameObject = gameObject;
			if(deactivateAfterRegister) gameObject.SetActive(false);
		}
	}
}