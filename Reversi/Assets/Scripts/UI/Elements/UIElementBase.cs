using System.Collections;
using System.Collections.Generic;
using T0R1.UI;
using UnityEngine;
using UnityEngine.Events;

namespace T0R1.UI
{
    public abstract class Element : SealableMonoBehaviour
    {
        protected Element _parent = null;
        protected SceneUIBase _baseUI = null;
        public Element Parent => _parent;
        public SceneUIBase BaseUI => _baseUI;

        protected UnityAction _onHidden = () => {};
        protected UnityAction _onShown = () => {};


        protected sealed override void Reset()
        {
            FetchComponents();
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

        public void Activate()
        {
            OnActivated();
        }

        public void Deactivate()
        {
            OnDeactivated();
        }

        protected T GetBaseUI<T>()
        {
            return _baseUI.GetComponent<T>();
        }

        [ContextMenu("Fetch Components")]
        public void FetchComponentBase()
        {
            FetchComponents();
        }

        protected virtual void FetchComponents()
        {

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
            _onShown.Invoke();
        }

        public virtual void Hide()
        {
            _onHidden.Invoke();
            this.gameObject.SetActive(false);
        }
        public void AddActionOnHidden(UnityAction call)
        {
            _onHidden += call;
        }
        public void AddActionOnShown(UnityAction call)
        {
            _onShown += call;
        }
        public void RemoveActionOnHidden(UnityAction call)
        {
            _onHidden -= call;
        }
        public void RemoveActionOnShown(UnityAction call)
        {
            _onShown -= call;
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

        protected virtual void OnActivated()
        {

        }

        protected virtual void OnDeactivated()
        {

        }

    }

}