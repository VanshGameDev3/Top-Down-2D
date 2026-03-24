using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }
    public GameObject mapParent;
    private List<Image> mapImages;

    public Color highlightcolour = Color.red;
    public Color dimmedcolor = new Color(1f, 1f, 1f, 0.5f);

    public RectTransform playerIconTransform;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        mapImages = mapParent.GetComponentsInChildren<Image>().ToList();
    }

    public void HighLightArea(string areaName)
    {
        foreach (Image image in mapImages)
        {
            image.color = dimmedcolor;
        }

        Image currentArea = mapImages.Find(x => x.name == areaName);

        if (currentArea != null)
        {
            currentArea.color = highlightcolour;

            playerIconTransform.position =
                currentArea.GetComponent<RectTransform>().position;
        }
        else
        {
            Debug.Log("Area not found: " + areaName);
        }
    }
}