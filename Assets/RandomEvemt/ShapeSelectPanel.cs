using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSelectPanel : MonoBehaviour
{
    public void DisableEventPanel()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        SystemMessage.ShowMessage("次元が崩壊している", true, "中心次元へ急げ", false);
    }
}
