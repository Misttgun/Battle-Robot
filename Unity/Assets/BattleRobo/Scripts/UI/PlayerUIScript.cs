using System;
using System.Collections;
using System.Globalization;
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
        [SerializeField]
        private Slider healthBar;

        /// <summary>
        /// The shield bar.
        /// </summary>
        [SerializeField]
        private Slider shieldBar;

        /// <summary>
        /// The fuel bar.
        /// </summary>
        [SerializeField]
        private Slider fuelBar;

        /// <summary>
        /// The number of player alive text.
        /// </summary>
        [SerializeField]
        private Text aliveText;

        /// <summary>
        /// The number of kills text.
        /// </summary>
        [SerializeField]
        private Text killsText;

        /// <summary>
        /// The pause logo
        /// </summary>
        [SerializeField]
        private Image pauseImage;

        /// <summary>
        /// The player inventory
        /// </summary>
        [SerializeField]
        private GameObject[] inventorySlotUI = new GameObject[5];

        private int currentActiveSlotIndex;

        /// <summary>
        /// Timer of the storm
        /// </summary>
        [SerializeField]
        private Text stormTimer;

        /// <summary>
        /// Ammo counter
        /// </summary>
        [SerializeField]
        private Text ammoCounter;

        /// <summary>
        /// Damage indicator
        /// </summary>
        [SerializeField]
        private Image damageIndicator;

        /// <summary>
        /// Hit marker
        /// </summary>
        [SerializeField]
        private Image hitMarker;

        /// <summary>
        /// The player owning this UI camera
        /// </summary>
        [SerializeField]
        private Camera playerCam;

        private readonly WaitForSeconds damageIndicatorTimer = new WaitForSeconds(1.5f);
        private readonly WaitForSeconds hitMarkerIndicatorTimer = new WaitForSeconds(0.5f);


        public void UpdateStormTimer(float countdown)
        {
            if (countdown > 1)
            {
                stormTimer.text = "Strom will move in... " + Mathf.Floor(countdown) + "s";
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
            aliveText.text = "Players Alive: " + numberPlayer;
        }

        /// <summary>
        /// Updates the number of kill
        /// </summary>
        /// <param name="kills"></param>
        public void UpdateKillsText(int kills)
        {
            killsText.text = "Kills: " + kills;
        }

        /// <summary>
        /// Enlight the current active slot item
        /// </summary>
        /// <param name="index"></param>
        public void SetActiveUISlot(int index)
        {
            inventorySlotUI[currentActiveSlotIndex].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 96);
            inventorySlotUI[index].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 0, 0, 100);
            currentActiveSlotIndex = index;
        }

        /// <summary>
        /// Set image associated to the object in the UI slot item
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        public void SetItemUISlot(PlayerObjectScript obj, int index)
        {
            inventorySlotUI[index].transform.GetChild(0).GetComponent<Image>().sprite = obj != null ? obj.GetSprite() : null;
        }

        /// <summary>
        /// Update the ammo counter
        /// </summary>
        /// <param name="currentAmmo"></param>
        public void SetAmmoCounter(float currentAmmo)
        {
            ammoCounter.text = Math.Abs(currentAmmo + 1f) > 0.0001f ? currentAmmo.ToString(CultureInfo.CurrentCulture) : "";
        }

        /// <summary>
        /// Update the damage indicator
        /// </summary>
        /// <param name="shooterTransform"></param>
        public void UpdateDamageIndicator(Vector3 shooterTransform)
        {
            damageIndicator.enabled = true;

            //donne la position de la personne qui vous a tiré en fonction du champ du vision de la caméra
            Vector3 direction = playerCam.WorldToScreenPoint(shooterTransform);

            //indique la position de l'ennemi devant nous
            Vector3 pointing = Vector3.zero;
            pointing.z = Mathf.Atan2(-damageIndicator.transform.position.y, damageIndicator.transform.position.x - direction.x) * Mathf.Rad2Deg + 180;

            //si la valeur de z est négatif c'est que l'ennemi n'est pas dans le champ de vision 
            if (direction.z < 0)
            {
                // faire en sorte que le damage indicator indique la position de l'ennemie derrière nous 
                pointing.z = pointing.z + 180;
            }

            damageIndicator.transform.rotation = Quaternion.Euler(pointing);

            //only start the coroutine if the player is active
            if (gameObject.activeInHierarchy)
            {
                //disable damage indicator after 1.5 second
                StartCoroutine(DisableIndicator(damageIndicator, damageIndicatorTimer));
            }
        }

        /// <summary>
        /// Update the damage indicator
        /// </summary>
        public void UpdateHitMarker()
        {
            hitMarker.enabled = true;

            //only start the coroutine if the player is active
            if (gameObject.activeInHierarchy)
            {
                //disable damage indicator after 1.5 second
                StartCoroutine(DisableIndicator(hitMarker, hitMarkerIndicatorTimer));
            }
        }

        /// <summary>
        /// Disable the indicator image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="timer"></param>
        /// <returns></returns>
        private IEnumerator DisableIndicator(Image image, WaitForSeconds timer)
        {
            yield return timer;
            image.enabled = false;
        }

        public void EnablePause(bool enable)
        {
            Debug.Log(enable ? "ENABLE PAUSE !" : "DISABLE PAUSE !");

            pauseImage.gameObject.SetActive(enable);
        }
    }
}