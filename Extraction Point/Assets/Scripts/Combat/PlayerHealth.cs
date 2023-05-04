using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject HealthBar, Player, deathPrefab, RespawnPoint, StaticObjects, progressBar, progressContainer, deathMSG;
    [SerializeField] ItemGrid invGrid;
    [SerializeField] GameHandler gameHandler;
    [SerializeField] InventoryController inventoryController;
    public static float t_Width, height, healthPoint, totalHealth = 100.0f, currentHealth = 100.0f, lastUpdatedHealth;
    private RectTransform progressBarRect;
    public static bool isPlayerDead = false;
    private GameObject deathSplat;
    private RectTransform barRect;

    

    private void Awake() {
        progressBarRect = progressBar.GetComponent<RectTransform>();
        barRect = HealthBar.GetComponent<RectTransform>();
        t_Width = barRect.sizeDelta.x;
        healthPoint = t_Width/totalHealth;      
        lastUpdatedHealth = currentHealth;
    }

    private void Update() {
        if(lastUpdatedHealth != currentHealth){
            UpdateHealthBar();
        }
        
        if(currentHealth <= 0.0f && !isPlayerDead){ 
            PlayerHasDied();
        }

        if(Input.GetKeyDown(KeyCode.Space) && isPlayerDead){ //N to respawn for temp
            Respawn();
        }

    }

    private void Respawn()
    {
        isPlayerDead = false;
        deathMSG.SetActive(false);
        currentHealth = totalHealth;
        UpdateHealthBar();
        Player.SetActive(!isPlayerDead);
        Player.transform.position = new Vector3(0, 0, 0);
        SceneManager.LoadScene("HubShip");

        MapGenerator.ResetStatics();
        inventoryController.PlayerDeath();
        
        gameHandler.DeserializeJson();
    }

    private void PlayerHasDied()
    {
        isPlayerDead = true;
        // deathSplat = Instantiate(deathPrefab, Player.transform.position, Quaternion.identity);
        deathMSG.SetActive(true);
        Player.SetActive(!isPlayerDead);
        // Destroy(StaticObjects);
        Debug.Log("Player Died - Press N to respawn");
    }

    public void PlayerHeal(float healAmmount)
    {
        StartCoroutine(healRoutine(healAmmount));
        // if(currentHealth+healAmmount >= 100.0f){
        //     currentHealth = 100.0f;
        // }else{
        //     currentHealth+=healAmmount;
        // }
        // UpdateHealthBar();
    }

    IEnumerator healRoutine(float healAmmount){
        progressContainer.SetActive(true);
        float timeElapsed = 0.0f, progressPoint = progressBarRect.sizeDelta.x / 20, progress = 0, healTick = healAmmount / 20;
        while(true){
            progress+=progressPoint;
            progressBarRect.sizeDelta = new Vector2(progress, progressBarRect.sizeDelta.y);
            timeElapsed+=.1f;
            currentHealth+=healTick;
            if(currentHealth > 100.0f){ currentHealth = 100.0f; }
            UpdateHealthBar();
            yield return new WaitForSeconds(.1f);
            if(timeElapsed >= 2f){
                break;
            }
        }
        progressContainer.SetActive(false);
        yield return null;
    }

    public static void DoDamage(float damageAmmount) //testing purposes
    {
        currentHealth-=damageAmmount;
        //do hit effect here, not needed tho
    }

    private void UpdateHealthBar() //updates the visual for the healthbar
    {
        if(currentHealth <= 0.0f){
            barRect.sizeDelta = new Vector2(0.0f, barRect.sizeDelta.y);
            lastUpdatedHealth = 0.0f;
        } //exit gate if player dead

        barRect.sizeDelta = new Vector2(healthPoint * currentHealth, barRect.sizeDelta.y);
        lastUpdatedHealth = currentHealth;
    }
}
