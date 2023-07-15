using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject CatchPanel;
    [SerializeField] private GameObject ToFarPanel;
    bool IsCatchPanel;
    bool IsToFarPanel;
    int tempEvent;
    [SerializeField] private EventManager eventManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayCatchPanel(int eventID)
    {
        if (IsCatchPanel==false){
            tempEvent = eventID;
            CatchPanel.SetActive(true);
            IsCatchPanel = true;
        }
    }

    public void OnJoinButtonClick()
    {
        eventManager.ActivateEvent(tempEvent);
    }

    public void DisplayToFarPanel()
    {
        if (IsToFarPanel==false){
            ToFarPanel.SetActive(true);
            IsToFarPanel = true;
        }
    }
    public void CloseButtonClick()
    {
        CatchPanel.SetActive(false);
        ToFarPanel.SetActive(false);
        IsCatchPanel = false;
        IsToFarPanel = false;
    }
}
