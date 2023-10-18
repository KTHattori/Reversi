using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEngine.Serialization;

namespace RegisterObject
{
	/// <summary>
	/// オブジェクト媒介用のScriptableObjectへ参照を渡すスクリプト
	/// </summary>
	[DefaultExecutionOrder(-100)]
	public class RegisterToReferencer : MonoBehaviour 
	{
		/// <summary>
		/// アセット生成用の設定を定義するためのクラス
		/// </summary>
		[System.Serializable]
		public class AssetCreationSettings
		{
			/// <summary>
			/// 定数：アセット生成デフォルトパス
			/// </summary>
			public const string DefaultPath = "ScriptableObjects/ObjectReferencer/";

			/// <summary>
			/// 定数：アセットデフォルト接頭辞（ファイル名先頭に付加）
			/// </summary>
			public const string DefaultPrefix = "Ref_";

			/// <summary>
			/// アセットを生成する際のパス
			/// </summary>
			[Header("アセット生成先のパス")]
			public string destPath = DefaultPath;

			/// <summary>
			/// アセットを生成する際のパス
			/// </summary>
			[Header("アセット生成時の接頭辞（ファイル名先頭に付加）")]
			public string prefix = DefaultPrefix;

			/// <summary>
			/// デフォルト設定に戻す
			/// </summary>
			public void RestoreDefault()
			{
				destPath = DefaultPath;
				prefix = DefaultPrefix;
			}
		}
		
		/// <summary>
		/// 登録後に非Activeにする
		/// </summary>
		[SerializeField,Header("受け渡し後に非Activeにする")]
		private bool _deactivateAfterRegister = false;

		/// <summary>
		/// 登録先のObjectReferencer
		/// </summary>
		[SerializeField,Header("対象となる媒介用ScriptableObject")]
        private ObjectReferencer _referencer;

		/// <summary>
		/// アセット生成時の設定を定義
		/// </summary>
		[SerializeField,Header("アセット生成用設定")]
		private AssetCreationSettings _assetCreation;

		/// <summary>
		/// このオブジェクトがアクティブになった際にコールされる関数
		/// セットされているObject Referencerにオブジェクトを登録する
		/// </summary>
		private void Awake()
		{
			if(_referencer)
			{
				_referencer.gameObject = gameObject;
				if(_deactivateAfterRegister) gameObject.SetActive(false);
			}
			else
			{
				Debug.LogError($"There is no referencer set to register for gameObject: {gameObject.name}!");
			}
		}

		/// <summary>
		/// アセットの生成用設定をデフォルトに設定しなおす
		/// </summary>
		[ContextMenu("Restore default asset creation path")]
		public void RestoreDefaultCreationSettings()
		{
			_assetCreation.RestoreDefault();
		}

		/// <summary>
		/// アセット生成パスに基づいてアタッチされたオブジェクトへの参照アセットを作成
		/// </summary>
		[ContextMenu("Create referencer asset to attached gameObject (OVERWRITE)")]
		public void CreateReferencerAsset()
		{
			// ObjectReferencerインスタンス生成
			var referencerInstance = ScriptableObject.CreateInstance<ObjectReferencer>();
			referencerInstance.gameObject = this.gameObject;
			string fileName = $"{_assetCreation.prefix}{this.gameObject.name}.asset";
			string destPath = "Assets/" + _assetCreation.destPath;

			// インスタンスを指定したディレクトリに保存
			if (!Directory.Exists(destPath))
				Directory.CreateDirectory(destPath);
		#if UNITY_EDITOR
			AssetDatabase.CreateAsset(referencerInstance, destPath + fileName);
		#endif

			Debug.Log($"Created referencer asset at {destPath + fileName}.");

			// 生成したインスタンスを登録先としてセット
			_referencer = referencerInstance;
		}

	}
}