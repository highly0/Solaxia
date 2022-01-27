using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState { IDLE, RUN }
public class Unit : MonoBehaviour{

	public string unitName;
	public int unitLevel;

	public int maxHP = 20;
	public int currentHP;

	public int damage = 10;
	public int power  = 100;
	public int currentPower;
	public float RunSpeed = 0f;
	
	public Animator anim;
	public AnimState UnitState;

	public Vector3 TargetPosition;
	Vector3 StartPosition;
	public Action onRunComplete;

	void Start() {
		StartPosition = GetPosition();
	}

	void Update() {
		switch (UnitState) { 
			case AnimState.IDLE: 
				break;
			case AnimState.RUN:
				//Debug.Log("Running"); 
				RunSpeed = 5f;
				anim.SetFloat("Run", RunSpeed);	
				transform.position += (TargetPosition - GetPosition()).normalized * RunSpeed * Time.deltaTime;
				float buf = 0.01f;
				if(Vector3.Distance(GetPosition(), TargetPosition) < buf) {
					// at target position 
					transform.position = TargetPosition;
					RunSpeed = 0f;
					anim.SetFloat("Run", RunSpeed);	
					onRunComplete();
				}
				break;
		}
	}

	public void Death() {
		anim.SetTrigger("Death");
	}

	public void Damage() {
		anim.SetTrigger("Hit");
	}

	public bool TakeDamage(int dmg){
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Attack(Unit TargetUnit) {
		// getting the position of the other unit
		Vector3 OffsetPos = TargetUnit.GetPosition() + (GetPosition() - TargetUnit.GetPosition()).normalized * 5f;

		RunToPosition(OffsetPos, () => { 
			anim.SetTrigger("Attack");
			UnitState = AnimState.IDLE;
		});
	}

	public void returnPos() {
		RunToPosition(StartPosition, () => { 
		});
	}

	void RunToPosition(Vector3 TargetPosition, Action onRunComplete) {
		this.TargetPosition = TargetPosition;
		this.onRunComplete = onRunComplete;
		UnitState = AnimState.RUN;
	}

	public Vector3 GetPosition() {
		return transform.position;
	}

	// check for validity of power and take it
	public bool TakePower(int power){
		if (currentPower - power >= 0 && power >= 0) {
			currentPower -= power;
			return true;
		} else {
			return false;
		}
	}

	public void setPower(int power){
		currentPower = power;
	}
}
