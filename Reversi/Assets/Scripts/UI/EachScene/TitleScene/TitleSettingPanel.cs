using UnityEngine;
using T0R1.UI;
using UnityEngine.UI;
using TMPro;


public class TitleSettingWindow : ModalWindow
{
    [SerializeField]
    private TMP_InputField _nameInputField;
    public TMP_InputField NameInputField => _nameInputField;
    

    void Start()
    {
        NameInputField.onSubmit.AddListener(OnNameInputEndEdit);
    }

	public void OnNameInputEndEdit(string name)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            _baseUI.GetComponent<TitleSceneUI>().SetPlayerNameText(NameInputField.text);
	}
}