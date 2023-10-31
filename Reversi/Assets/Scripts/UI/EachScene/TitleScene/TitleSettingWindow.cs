using UnityEngine;
using T0R1.UI;
using TMPro;


public class TitleSettingWindow : ModalWindow
{
    [SerializeField]
    private TMP_InputField _nameInputField;
    public TMP_InputField NameInputField => _nameInputField;
    
    protected override void OnStart()
    {
        NameInputField.onSubmit.AddListener(OnNameInputEndEdit);
        GetBaseUI<TitleSceneUI>().SetPlayerNameText(NameInputField.text);
    }

	public void OnNameInputEndEdit(string name)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            GetBaseUI<TitleSceneUI>().SetPlayerNameText(NameInputField.text);
	}
}