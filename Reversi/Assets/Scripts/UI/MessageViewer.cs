using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using T0R1;
using TMPro;
public class MessageViewer : MonoSingleton<MessageViewer>
{
    public override void OnFinalize()
    {
        
    }

    public override void OnInitialize()
    {
        
    }

    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string tex)
    {
        text.SetText(tex);
    }
}
