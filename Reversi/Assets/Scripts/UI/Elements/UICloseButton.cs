using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace T0R1.UI
{
    public class CloseButton : ButtonBase
    {
        protected override void OnClickContent()
        {
            _parent.Hide();
        }
    }

}
