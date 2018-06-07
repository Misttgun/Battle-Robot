﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo
{
    public class PlayerUIScript : MonoBehaviour
    {
        /// <summary>
        /// The text for the player name.
        /// </summary>
        public Text playerNameText;
        
        /// <summary>
        /// The health bar.
        /// </summary>
        [SerializeField] private Slider healthBar;
        
        /// <summary>
        /// The shield bar.
        /// </summary>
        [SerializeField] private Slider shieldBar;

        /// <summary>
        /// The fuel bar.
        /// </summary>
        [SerializeField] private Slider fuelBar;

        /// <summary>
        /// The number of player alive text.
        /// </summary>
        [SerializeField] private Text aliveTexT;
        
        /// <summary>
        /// The number of kills text.
        /// </summary>
        [SerializeField] private Text killsTexT;

        /// <summary>
        /// The pause logo
        /// </summary>
        [SerializeField] private Image pauseImage;
        
        /// <summary>
        /// The player inventory
        /// </summary>
        [SerializeField] private GameObject[] inventorySlotUI = new GameObject[5];
        private int currentActiveSlotIndex; 

        /// <summary>
        /// Timer of the storm
        /// </summary>
        [SerializeField] private Text stormTimer;
        
        /// <summary>
        /// Ammo counter
        /// </summary>
        [SerializeField] private Text ammoCounter;
        
        
        public void UpdateStormTimer(float countdown)
        {
            if (countdown > 1)
            {
                stormTimer.text = "Strom will move in... " + Mathf.Floor(countdown)+ "s";
            }
            else
            {
                stormTimer.text = "";
            }
        }
        
        /// <summary>
        /// Updates the player health
        /// </summary>
        /// <param name="currHealth"></param>
        /// <param name="maxHealth"></param>
        public void UpdateHealth(float currHealth, float maxHealth = 100f)
        {
            var health = currHealth / maxHealth;
            healthBar.value = health;
        }
        
        /// <summary>
        /// Updates the player shield
        /// </summary>
        /// <param name="currShield"></param>
        /// <param name="maxShield"></param>
        public void UpdateShield(float currShield, float maxShield = 100f)
        {
            var shield = currShield / maxShield;
            shieldBar.value = shield;
        }

        /// <summary>
        /// Updates the player fuel
        /// </summary>
        /// <param name="fuel"></param>
        public void UpdateFuel(float fuel)
        {
            fuelBar.value = fuel;
        }

        /// <summary>
        /// Updates the number of player alive in the game
        /// </summary>
        /// <param name="numberPlayer"></param>
        public void UpdateAliveText(int numberPlayer)
        {
            aliveTexT.text = "Players Alive: " + numberPlayer;
        }
        
        /// <summary>
        /// Updates the number of kill
        /// </summary>
        /// <param name="kills"></param>
        public void UpdateKillsText(int kills)
        {
            aliveTexT.text = "Kills: " + kills;
        }
        
        /// <summary>
        /// - enlight the current active slot item
        /// </summary>
        /// <param name="index"></param>
        public void SetActiveUISlot(int index)
        {
            inventorySlotUI[currentActiveSlotIndex].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 96);
            inventorySlotUI[index].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 0, 0, 100);
            currentActiveSlotIndex = index;
        }

        /// <summary>
        // - set image associated to the object in the UI slot item
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        public void SetItemUISlot(PlayerObjectScript obj, int index)
        {
            inventorySlotUI[index].transform.GetChild(0).GetComponent<Image>().sprite = obj != null ? obj.GetSprite() : null;
        }
        
        /// <summary>
        // - Update the ammo counter
        /// </summary>
        /// <param name="currentAmmo"></param>
        /// <param name="magazineSize"></param>
        public void SetAmmoCounter(float currentAmmo, float magazineSize)
        {
            if (Math.Abs(currentAmmo + 1f) > 0.0001f && Math.Abs(magazineSize + 1f) > 0.0001f)
                ammoCounter.text = currentAmmo + " / " + magazineSize;

            else
                ammoCounter.text = "";
        }

        public void EnablePause(bool enable)
        {
            Debug.Log(enable ? "ENABLE PAUSE !" : "DISABLE PAUSE !");

            pauseImage.gameObject.SetActive(enable);
        }
    }
}