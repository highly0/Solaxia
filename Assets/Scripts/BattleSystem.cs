using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public Button button;
    public InputField input;

	public SoundManagerScript SoundFX;
	public int numCoins;
	public int earn;

	public GameObject returnButton;

    // Start is called before the first frame update
    void Start(){
		state = BattleState.START;
		StartCoroutine(SetupBattle());
		numCoins  =  PlayerPrefs.GetInt("coins");
		earn = PlayerPrefs.GetInt("earn");
    }

	void startListen() {
		dialogueText.text = "Your turn!";
		button.onClick.AddListener(PlayerTurn);
	}


	// setting everything up for the battle
	IEnumerator SetupBattle(){
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		GameObject playerChild = playerGO.transform.GetChild(0).gameObject;
		playerUnit = playerChild.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		GameObject enemyChild = enemyGO.transform.GetChild(0).gameObject;
		enemyUnit = enemyChild.GetComponent<Unit>();

		dialogueText.text = playerUnit.unitName + " vs. " + enemyUnit.unitName;

		playerHUD.SetHUD(playerUnit, true);
		enemyHUD.SetHUD(enemyUnit, false);

		yield return new WaitForSeconds(2f);
		
		state = BattleState.PLAYERTURN;
		startListen();
	}

	void PlayerTurn(){
		int power = int.Parse(input.text);
		StartCoroutine(PlayerAttack(power));
	}

	IEnumerator PlayerAttack(int power){
		bool checkPower = playerUnit.TakePower(power);
		if(checkPower == false) {
			dialogueText.text = "Invalid power, please choose again!";
		} else {
 			dialogueText.text = power.ToString() + " power was spent!";
			playerHUD.updatePower(playerUnit.currentPower);
			yield return new WaitForSeconds(2f);

			// switch to the enemy's turn
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn(power));
		}   
	}

	IEnumerator EnemyTurn(int power){
		dialogueText.text = "Enemy turn, waiting for their power!";
		yield return new WaitForSeconds(2f);
		int randomPower = Random.Range(0, enemyUnit.currentPower);
		Debug.Log(randomPower);
		//enemyHUD.updatePower(enemyUnit.currentPower);

		bool checkPower = enemyUnit.TakePower(randomPower);
		dialogueText.text = "Enemy's power chosen!";
		yield return new WaitForSeconds(1f);
		StartCoroutine(decide(power, randomPower));
		
	}
	

	IEnumerator decide(int playerPower, int enemyPower) {
		bool playerisDead = false;
		bool enemyisDead = false;

		dialogueText.text = "Deciding...";
		yield return new WaitForSeconds(3f);
		
		if (playerPower > enemyPower) {
			
			// healling the player back to full
			if(playerUnit.currentHP == 10) {
				SoundFX.PlaySound("heal");
				dialogueText.text = "Back to full health!";
				yield return new WaitForSeconds(1f);
				playerUnit.currentHP = playerUnit.maxHP;
				playerHUD.SetHP(playerUnit.currentHP);
			} else { // attack the opponent
				dialogueText.text = "Your attack was succesfull!";
				// attacking the enemy
				playerUnit.Attack(enemyUnit);
				
				
				yield return new WaitForSeconds(0.5f);
				SoundFX.PlaySound("hit");
				// enemy taking damage
				enemyUnit.Damage();

				// return to the original position
				yield return new WaitForSeconds(1.5f);
				playerUnit.returnPos();

				enemyisDead = enemyUnit.TakeDamage(playerUnit.damage);
				yield return new WaitForSeconds(2f);
				enemyHUD.SetHP(enemyUnit.currentHP);
			}
		} else if (playerPower < enemyPower) {
			if(enemyUnit.currentHP == 10) {
				SoundFX.PlaySound("heal");
				dialogueText.text = "Enemy health is regained!";
				yield return new WaitForSeconds(1f);
				enemyUnit.currentHP = enemyUnit.maxHP;
				enemyHUD.SetHP(enemyUnit.currentHP);
			} else {
				dialogueText.text = "Enemy's attack was succesfull!";
				// attacking the player
				enemyUnit.Attack(playerUnit);
				

				yield return new WaitForSeconds(2f);
				SoundFX.PlaySound("hit");
				// player taking damage 
				playerUnit.Damage();

				// return to the original position
				yield return new WaitForSeconds(1.5f);
				enemyUnit.returnPos();

				playerisDead = playerUnit.TakeDamage(enemyUnit.damage);
				yield return new WaitForSeconds(2f);
				playerHUD.SetHP(playerUnit.currentHP);
			}
		} else {
			dialogueText.text = "Attack unsucessful!";
			if(playerPower == 0 && enemyPower == 0) {
				yield return new WaitForSeconds(1f);
				dialogueText.text = "You both ran out of power!";
				yield return new WaitForSeconds(1f);
				dialogueText.text = "Power replenished!";
				playerUnit.setPower(playerUnit.power);
				enemyUnit.setPower(enemyUnit.power);
				playerHUD.updatePower(playerUnit.power);
			}
			yield return new WaitForSeconds(2f);
		}
				
		dialogueText.text = "Your turn!";


		// checking if either object is dead at every turn
		if (playerisDead) {
			playerUnit.Death();
			SoundFX.PlaySound("death");
			state = BattleState.LOST;

			//yield return new WaitForSeconds(1.5f);
			numCoins -= 5;
			EndBattle();
			if(earn == 1) {
				dialogueText.text = "You lost 5 coins";
				dialogueText.text = "";
			} else {
				dialogueText.text = "You lost 0 coins";
				dialogueText.text = "";
			}
			
			yield return new WaitForSeconds(1.5f);
			returnButton.GetComponent<GameOver>().appear();
		} else if (enemyisDead) {
			enemyUnit.Death();
			state = BattleState.WON;
			
			//yield return new WaitForSeconds(1.5f);
			numCoins += 5;
			EndBattle();
			//yield return new WaitForSeconds(1.5f);

			if(earn == 1) {
				dialogueText.text = "You won 5 coins";
				dialogueText.text = "";
			} else {
				dialogueText.text = "You won 0 coins";
				dialogueText.text = "";
			}

			yield return new WaitForSeconds(1.5f);
			returnButton.GetComponent<GameOver>().appear();
		} 
	}

	void EndBattle(){
		if(earn == 1) {
			// updating the number of coins
			PlayerPrefs.SetInt("coins", numCoins);
		}
		
		if(state == BattleState.WON){
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST){
			dialogueText.text = "You were defeated.";
		}
		
	}
}
