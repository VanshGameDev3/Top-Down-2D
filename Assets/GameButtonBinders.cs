using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameButtonBinders : MonoBehaviour
{
    public enum ButtonType { Save, Load, Exit }

    public ButtonType buttonType;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveController.Instance != null);

        Button btn = GetComponent<Button>();

        switch (buttonType)
        {
            case ButtonType.Save:
                btn.onClick.AddListener(SaveController.Instance.SaveButton);
                break;

            case ButtonType.Load:
                btn.onClick.AddListener(SaveController.Instance.LoadButton);
                break;

            case ButtonType.Exit:
                btn.onClick.AddListener(SaveController.Instance.ExitButton);
                break;
        }
    }
}