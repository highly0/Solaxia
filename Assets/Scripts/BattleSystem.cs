using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start(){
		state = BattleState.START;
		StartCoroutine(SetupBattle());	
    }

	void startListen() {
		dialogueText.text = "Your turn!";
		button.onClick.AddListener(PlayerTurn);
	}

	// setting everything up for the battle
	IEnumerator SetupBattle(){
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = playerUnit.unitName + " vs. " + enemyUnit.unitName;

		playerHUD.SetHUD(playerUnit, true);
		enemyHUD.SetHUD(enemyUnit, false);

		yield return new WaitForSeconds(2f);
		
		state = BattleState.PLAYERTURN;
		startListen();
		/*
		// radomize between the player and the enemy's turn
		if(state == BattleState.START){
			int num = Random.Range(1, 2);
			MonoBehaviour.print(num);
			if(num == 1) {
				state = BattleState.PLAYERTURN;
				startListen();
			} else {
				state = BattleState.ENEMYTURN;
				StartCoroutine(EnemyTurn());
				Debug.Log("yo");
			}
		}*/
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
		yield return new WaitForSeconds(3f);
		int randomPower = Random.Range(0, enemyUnit.currentPower);
		Debug.Log(randomPower);
		//enemyHUD.updatePower(enemyUnit.currentPower);

		bool checkPower = enemyUnit.TakePower(randomPower);
		dialogueText.text = "Enemy's power chosen!";
		yield return new WaitForSeconds(2f);
		StartCoroutine(decide(power, randomPower));
		
	}
	

	IEnumerator decide(int playerPower, int enemyPower) {
		Debug.Log("in decide");
		bool playerisDead = false;
		bool enemyisDead = false;
		
		if (playerPower > enemyPower) {
			Debug.Log("player wins");
			yield return new WaitForSeconds(2f);
			
			// healling the player back to full
			if(playerUnit.currentHP == 10) {
				playerUnit.currentHP = playerUnit.maxHP;
				playerHUD.SetHP(playerUnit.currentHP);
			} else {
				enemyisDead = enemyUnit.TakeDamage(playerUnit.damage);
				yield return new WaitForSeconds(2f);
				enemyHUD.SetHP(enemyUnit.currentHP);
			}
			//enemyHUD.updatePower(enemyUnit.currentPower);
			//playerHUD.updatePower(playerUnit.currentPower);
		} else if (playerPower < enemyPower) {
			Debug.Log("enemy wins");
			yield return new WaitForSeconds(2f);

			if(enemyUnit.currentHP == 10) {
				enemyUnit.currentHP = enemyUnit.maxHP;
				enemyHUD.SetHP(enemyUnit.currentHP);
			} else {
				playerisDead = playerUnit.TakeDamage(enemyUnit.damage);
				yield return new WaitForSeconds(2f);
				playerHUD.SetHP(playerUnit.currentHP);
			}
			//enemyHUD.updatePower(enemyUnit.currentPower);
			//playerHUD.updatePower(playerUnit.currentPower);
		} else {
			Debug.Log("Equal scores");
			yield return new WaitForSeconds(2f);
		}
		

		dialogueText.text = "Deciding!";
		yield return new WaitForSeconds(1f);		
		dialogueText.text = "Your turn!";

		if (playerisDead) {
			state = BattleState.LOST;
			EndBattle();
		} else if (enemyisDead) {
			state = BattleState.WON;
			EndBattle();
		} 

	}
	
	/*
	namespace Namespace {
    
		using System;
		
		public static class Module {
			
			public static object print_hp(int first_score, int second_score) {
				var end = false;
				var first_hp = 20;
				var second_hp = 20;
				while (!end) {
					first_score = power;
					second_score = randomPower;
					if (first_score > second_score) {
						second_hp -= 10;
						if (first_hp == 10) {
							first_hp += 10;
						}
					} else if (second_score > first_score) {
						first_hp -= 10;
						if (second_hp == 10) {
							second_hp += 10;
						}
					}
					if (first_hp == 0) {
						Console.WriteLine("Second player wins!");
						end = true;
					} else if (second_hp == 0) {
						Console.WriteLine("First player wins!");
						end = true;
					} else {
						continue;
					}
				}
			}
		}
	}
		/*
	
		/*
		// check if the enemy is dead after the attack 
		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);

		if(isDead){
			state = BattleState.LOST;
			EndBattle();
		} else{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}
		
	}*/

	void EndBattle(){
		if(state == BattleState.WON){
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST){
			dialogueText.text = "You were defeated.";
		}
	}
}
