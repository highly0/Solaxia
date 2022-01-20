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
		}
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
			StartCoroutine(EnemyTurn());
		}   
	}

	IEnumerator EnemyTurn(){
		dialogueText.text = "Enemy turn, waiting for their power!";
		yield return new WaitForSeconds(3f);
		int randomPower = Random.Range(0, enemyUnit.currentPower);
		Debug.Log(randomPower);

		bool checkPower = enemyUnit.TakePower(randomPower);
		dialogueText.text = "Enemy's power chosen!";
		yield return new WaitForSeconds(2f);
		dialogueText.text = "Your turn!";
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
		*/
	}

	void EndBattle(){
		if(state == BattleState.WON){
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST){
			dialogueText.text = "You were defeated.";
		}
	}
}
