using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace T0R1
{
    public interface IAspectAffectable
    {
        protected virtual void OnPortrait(object payload = null)
        {

        }
        protected virtual void OnLandscape(object payload = null)
        {

        }
        protected virtual void OnOrientationChanged(object payload = null)
        {
            
        }
    }

    public abstract class AspectAffectable : MonoBehaviour,IAspectAffectable
    {
        protected virtual void OnPortrait(object payload)
        {

        }

        protected virtual void OnLandscape(object payload)
        {

        }

        protected virtual void OnOrientationChanged(object payload)
        {
            
        }

        void Start()
        {
            EventDispatcher.AddEventListener(Orientation.Changed,OnOrientationChanged);
            EventDispatcher.AddEventListener(Orientation.Portrait,OnPortrait);
            EventDispatcher.AddEventListener(Orientation.Landscape,OnLandscape);
        }

        void OnDestroy()
        {
            EventDispatcher.RemoveEventListener(Orientation.Changed,OnOrientationChanged);
            EventDispatcher.RemoveEventListener(Orientation.Portrait,OnPortrait);
            EventDispatcher.RemoveEventListener(Orientation.Landscape,OnLandscape); 
        }
    }

}
