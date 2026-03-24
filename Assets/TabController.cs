using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    
    void Start()
    {
        ActivateTabs(0);
    }

    public void ActivateTabs(int TabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.gray;
        }
        pages[TabNo].SetActive(true);
        tabImages[TabNo].color = Color.white;
    }
}