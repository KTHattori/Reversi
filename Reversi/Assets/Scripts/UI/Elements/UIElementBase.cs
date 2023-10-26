using System.Collections;
using System.Collections.Generic;
using T0R1.UI;
using UnityEngine;

namespace T0R1.UI
{
    public class Element : SealableMonoBehaviour
    {
        protected Element _parent = null;
        protected SceneUIBase _baseUI = null;
        public Element Parent => _parent;
        public SceneUIBase BaseUI => _baseUI;

        protected sealed override void Reset()
        {
            OnReset();
        }

        protected sealed override void Awake()
        {
            OnAwake();
        }

        protected sealed override void Start()
        {
            OnStart();
        }

        protected sealed override void Update()
        {
            OnUpdate();
        }

        protected T GetBaseUI<T>()
        {
            return _baseUI.GetComponent<T>();
        }

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

        protected virtual void OnReset()
        {

        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnUpdate()
        {

        }
    }

}