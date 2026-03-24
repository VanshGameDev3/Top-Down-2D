using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArenaProgressUI : MonoBehaviour
{
    [System.Serializable]
    public class ArenaIcon
    {
        public string arenaId;
        public Image icon;
    }

    [SerializeField] private List<ArenaIcon> arenaIcons;

    [Header("Opacity")]
    [SerializeField] private int incompleteAlpha255 = 122;
    [SerializeField] private int completeAlpha255 = 255;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (SaveController.Instance == null)
            return;

        foreach (var entry in arenaIcons)
        {
            bool completed =
                SaveController.Instance.IsArenaCompleted(entry.arenaId);

            SetIconState(entry.icon, completed);
        }
    }

    private void SetIconState(Image img, bool completed)
    {
        int alpha255 = completed ? completeAlpha255 : incompleteAlpha255;

        Color c = img.color;
        c.a = alpha255 / 255f;
        img.color = c;
    }
}