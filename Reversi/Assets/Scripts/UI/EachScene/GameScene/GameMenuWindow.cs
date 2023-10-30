using System.Collections;
using System.Collections.Generic;
using T0R1.UI;
using UnityEngine;

public class GameMenuWindow : ModalWindow
{
    [SerializeField]
    ButtonTextEdit _disconnectButton;

    public ButtonTextEdit DisconnectButton { get; }
}
