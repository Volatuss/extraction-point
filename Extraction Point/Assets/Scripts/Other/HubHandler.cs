using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubHandler : MonoBehaviour
{
    private InventoryController inventoryController;
    private GameHandler gameHandler;
    private InteractToolTips toolTips;
    private void Awake() {
        inventoryController = GameObject.FindObjectOfType<InventoryController>();
        gameHandler = GameObject.FindObjectOfType<GameHandler>();
        toolTips = GameObject.FindObjectOfType<InteractToolTips>();
    }

    public void GoToLevel(){
        inventoryController.SelectedItemGrid = null;
        gameHandler.SerializeJson();

        SceneManager.LoadScene("MapGenerationTesting");
        MapGenerator.playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }
}
