using System.Collections;
using System.Collections.Generic;
using T0R1.UI;
using UnityEngine;

namespace T0R1
{
    public class Element : MonoBehaviour
    {
        protected Element _parent = null;
        protected SceneUIBase _baseUI = null;
        public Element Parent => _parent;
        public SceneUIBase BaseUI => _baseUI;

        public void SetParent(Element element)
        {
            _parent = element;
        }
        public void RemoveParent()
        {
            _parent = null;
        }
        public void SetBaseUI(SceneUIBase sceneUIBase)
        {
            _baseUI = sceneUIBase;
        }
        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public virtual bool IsVisible
        {
            get => this.gameObject.activeSelf;
        }
    }

}