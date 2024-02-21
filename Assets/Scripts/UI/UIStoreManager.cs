using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIStoreManager : MonoBehaviour
{
	public Text playerCapsText;
	public Text gunNameText;
	public Text gunDescriptionText;
	public Text gunPriceText;

	public Image speedImage;
	public Image damageImage;
	public Image shootsImage;

	public Text typeText;
	public Text speedText;
	public Text damageText;
	public Text shootsText;

	public GameObject leftArrow;
	public GameObject rightArrow;

	public GameObject purchaseButton;
	public GameObject purchasedText;
	public GameObject lockedText;

	public GameObject scrollItems;
	public GameObject buttonItem;
	private List<GameObject> instancesButtonItem;

	List<GameObject> elements;

	int currentGun;

	int direction;
	int velocity;

	bool moving;

	// Use this for initialization
	void Start () {
		elements = new List<GameObject> ();
		instancesButtonItem = new List<GameObject> ();

		currentGun = 0;
		velocity = 2;
		moving = false;

		List<GunData> guns = GameData.Instance.guns;
		for (int i = 0; i < guns.Count; i++) {
			GameObject instanceGun = Instantiate (guns [i].Weapon, new Vector3 (i * 2, 0, 0), Quaternion.identity);
			instanceGun.AddComponent (typeof(Rotation));
			elements.Add(instanceGun);

			GameObject instanceButton = Instantiate (this.buttonItem, scrollItems.transform);
			instanceButton.name = guns [i].name;
			instanceButton.GetComponent<Button> ().onClick.AddListener (ScrollToWeapon);
			Image img = instanceButton.GetComponent<Image> ();
			img.sprite = guns [i].Sprite;
			this.instancesButtonItem.Add (instanceButton);
		}

		this.UpdateInfo ();
	}
	
	// Update is called once per frame
	void Update () {
		MoveGuns ();
	}

	void MoveGuns(){
		float distance = Vector3.Distance (elements [currentGun].transform.position, new Vector3 ());
		if (distance > 0.05f * velocity) {
			for (int i = 0; i < this.elements.Count; i++) {
				elements [i].transform.position += new Vector3 (direction * Time.deltaTime * velocity, 0, 0);
			}
			moving = true;
		} else {
			for (int i = 0; i < this.elements.Count; i++) {
				elements [i].transform.position = new Vector3 ((i - currentGun) * 2, 0, 0);
			}
			moving = false;
		}
	}

	public void ScrollRight()
	{
		if (currentGun < (this.elements.Count - 1) && !moving) {
			currentGun++;
			direction = -1;
			this.UpdateInfo ();
		}
	}

	public void ScrollLeft()
	{
		if (currentGun > 0 && !moving) {
			currentGun--;
			direction = 1;
			this.UpdateInfo ();
		}
	}

	public void ScrollToWeapon(){
		if (moving) {
			return;
		}

		GameObject selected = EventSystem.current.currentSelectedGameObject;

		int lastCurrentGun = currentGun;
		List<GunData> guns = GameData.Instance.guns;
		for (int i = 0; i < guns.Count; i++) {
			if (guns [i].name == selected.name) {
				currentGun = i;
				break;
			}
		}
			
		if (lastCurrentGun != currentGun) {
			for (int i = 0; i < this.elements.Count; i++) {
				elements [i].transform.position = new Vector3 ((i - currentGun) * 2, 0, 0);
			}
			this.UpdateInfo ();
		}
	}
	
	void UpdateInfo()
	{
		playerCapsText.text = "x" + UserManager.Instance().UserData.caps;
		gunNameText.text = GameData.Instance.guns [currentGun].GunName;
		gunDescriptionText.text = GameData.Instance.guns [currentGun].Description;
		gunPriceText.text = "Price: x" + GameData.Instance.guns [currentGun].Price.ToString();

		float maxSpeed = 0;
		int maxDamage = 0;
		int maxShoots = 0;

		switch (GameData.Instance.guns [currentGun].GetGunType ()) {
		case GunData.GunType.BOOMERANG:
			typeText.text = "Boomerang";
			maxSpeed = 1;
			maxDamage = 20;
			maxShoots = 50;
			break;
		case GunData.GunType.GRANADE:
			typeText.text = "Granade";
			maxSpeed = 1;
			maxDamage = 30;
			maxShoots = 50;
			break;
		case GunData.GunType.SHOTGUN:
			typeText.text = "Gun";
			maxSpeed = 0.1f;
			maxDamage = 20;
			maxShoots = 1000;
			break;
		}

		float speed = (float)GameData.Instance.guns [currentGun].Frecuency;
		speedText.text = "Reload: " + speed.ToString() + "sec";
		speedImage.fillAmount = maxSpeed / speed;

		float damage = GameData.Instance.guns [currentGun].Damage;
		if (damage == -1) {
			damageText.text = "Damage: ???";
			damageImage.fillAmount = 1;
		} else {
			damageText.text = "Damage: " + damage.ToString ();
			damageImage.fillAmount = damage / maxDamage;
		}

		int shoots = GameData.Instance.guns [currentGun].InitialCount;
		if (shoots <= 1) {
			shootsImage.fillAmount = 1;
			shootsText.text = "Shoots: ∞";
		}else{
			shootsImage.fillAmount = (float)shoots / (float)maxShoots;
			shootsText.text = "Shoots: x" + shoots.ToString();
		}

		PurchaseType type = this.GetPurchaseType (currentGun);
		switch (type) {
		case PurchaseType.PURCHASED:
			this.purchasedText.SetActive (true);
			this.purchaseButton.SetActive (false);
			this.lockedText.SetActive (false);
			break;
		case PurchaseType.CAN:
			this.purchasedText.SetActive (false);
			this.purchaseButton.SetActive (true);
			this.lockedText.SetActive (false);
			break;
		case PurchaseType.CANT:
			this.purchasedText.SetActive (false);
			this.purchaseButton.SetActive (false);
			this.lockedText.SetActive (true);
			break;
		}

		if (currentGun == 0) {
			leftArrow.SetActive (false);
		} else {
			leftArrow.SetActive (true);
		}

		if (currentGun == (this.elements.Count - 1)) {
			rightArrow.SetActive (false);
		} else {
			rightArrow.SetActive (true);
		}
	}

	PurchaseType GetPurchaseType(int gunIndex)
	{
		var gun = GameData.Instance.guns[gunIndex];
		
		if (UserManager.Instance().UserData.guns.Contains(gun.Id))
			return PurchaseType.PURCHASED;

		return UserManager.Instance().UserData.caps >= gun.Price ? PurchaseType.CAN : PurchaseType.CANT;
		
		/*if (DataBase.Instance.EntryHasGun (gunIndex)) {
			return PurchaseType.PURCHASED;
		} else if (GameData.Instance.guns [gunIndex].Price <= playerStats.userData.caps) {
			return PurchaseType.CAN;
		} else {
			return PurchaseType.CANT;
		}*/
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene ("MainMenu");
	}

	public void Purchase()
	{
		var gun = GameData.Instance.guns[currentGun];
        var user = UserManager.Instance();
		if (user.UserData.caps >= gun.Price) {
            user.PurchaseGun(gun);
			this.UpdateInfo ();
		}
	}
}
