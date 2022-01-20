using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour{

	public string unitName;
	public int unitLevel;

	public int maxHP = 20;
	public int currentHP;

	public int damage = 10;
	public int power  = 100;
	public int currentPower;

	public bool TakeDamage(int dmg){
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public bool TakePower(int power){
		if (currentPower - power >= 0) {
			currentPower -= power;
			return true;
		} else {
			return false;
		}
	}
}
