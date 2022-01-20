using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour{

	public Text nameText;
	public Text levelText;
	public Slider hpSlider;

	public void SetHUD(Unit unit, bool check){
		nameText.text = unit.unitName;
		if(check == true)
			levelText.text = "Pwr " + unit.currentPower;
		else 
			levelText.text = "Pwr ???";
		hpSlider.maxValue = unit.maxHP;
		hpSlider.value = unit.currentHP;
	}

	public void updatePower(int pwr){
		levelText.text = "Pwr " + pwr;
	}

	public void SetHP(int hp){
		hpSlider.value = hp;
	}

}
