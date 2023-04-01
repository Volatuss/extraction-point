using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject HealthBar, Player, deathPrefab, RespawnPoint;
    public static float t_Width, height, healthPoint, totalHealth = 100.0f, currentHealth = 100.0f;
    public static bool isPlayerDead = false;
    private GameObject deathSplat;
    private RectTransform barRect;
    

    private void Awake() {
        barRect = HealthBar.GetComponent<RectTransform>();
        t_Width = barRect.sizeDelta.x;
        healthPoint = t_Width/totalHealth;      
    }

    private void Update() {
        if(currentHealth <= 0.0f && !isPlayerDead){ 
            PlayerHasDied();
        }

        if(Input.GetKeyDown(KeyCode.N)){ //N to respawn for temp
            Respawn();
        }

        if(Input.GetKeyDown(KeyCode.K)){ //K to deal damage to the player
            DoDamage(24.99f);
        }

        if(Input.GetKeyDown(KeyCode.H)){ //H to heal the player
            PlayerHeal(25.0f);
        }

    }

    private void Respawn()
    {
        isPlayerDead = false;
        Destroy(deathSplat); // temporary until i get a death anim/sprite
        currentHealth = totalHealth;
        UpdateHealthBar();
        Player.SetActive(!isPlayerDead);
        Player.transform.position = RespawnPoint.transform.position;
    }

    private void PlayerHasDied()
    {
        isPlayerDead = true;
        deathSplat = Instantiate(deathPrefab, Player.transform.position, Quaternion.identity);
        Player.SetActive(!isPlayerDead);
        Debug.Log("Player Died - Press N to respawn");
    }

    public void PlayerHeal(float healAmmount)
    {
        if(currentHealth+healAmmount >= 100.0f){
            currentHealth = 100.0f;
        }else{
            currentHealth+=healAmmount;
        }
        UpdateHealthBar();
    }

    public void DoDamage(float damageAmmount) //testing purposes
    {
        currentHealth-=damageAmmount;
        UpdateHealthBar();        
    }

    private void UpdateHealthBar() //updates the visual for the healthbar
    {
        if(currentHealth <= 0.0f){
            barRect.sizeDelta = new Vector2(0.0f, barRect.sizeDelta.y);
        } //exit gate if player dead

        barRect.sizeDelta = new Vector2(healthPoint * currentHealth, barRect.sizeDelta.y);
    }
}
