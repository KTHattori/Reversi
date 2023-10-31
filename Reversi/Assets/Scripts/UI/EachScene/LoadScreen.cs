using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T0R1.UI
{
    public class LoadScreen : MonoSingleton<LoadScreen>
    {
        [SerializeField]
        CanvasGroup _canvasGroup;

        public override void OnFinalize()
        {
            
        }

        public override void OnInitialize()
        {
            DontDestroyOnLoad(this);
        }

        void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public static void Show()
        {
            Instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            Instance.gameObject.SetActive(false);
        }
    }

}
