using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurn : MonoBehaviour
{
    public HexGrid grid;
		public BarracksPopup barracksPopup;
		public HexMapEditor mapEditor;

		int fixedSpeed = 2; // public? remove?
		float turnLength = 2f;

	//public bool turnStarted {get; set;}
		bool turnStarted;
		//summon position relative to barracks
		int summonPos;

    // Update is called once per frame
    void Update()
    {
			if(!turnStarted) {
				StartCoroutine(DoTurn());
			}
    }

		void Start() {
			summonPos = 2;
		}

    public IEnumerator DoTurn () {

			WaitForSeconds delay = new WaitForSeconds(turnLength);
			turnStarted = true;

			TargetPhase();
			StartCoroutine(SummonPhase());
			MovePhase();
			StartCoroutine(AttackPhase());
			//StartCoroutine(EffectPhase());
			yield return delay;
			turnStarted = false;

			/*
			foreach (HexUnit unit in grid.GetUnits()) {
				if (unit.target) {
					DoPathfinding(unit);
					if (unit.DistanceToUnit(unit.target) > unit.attackRange){
						StopAllCoroutines();
						DoStep(unit);
					}
					else {
						StartCoroutine(Attack(unit));
						//Attack(unit);
						yield return delay;
					}
				}
			}
			*/
		}




		void TargetPhase() {
			int closestUnitDistance;
			foreach (HexUnit unit in grid.GetUnits()) {
				closestUnitDistance = 10000;
				if (grid.GetUnitsExcept(unit.faction).Count > 0) {
					foreach (HexUnit target in grid.GetUnitsExcept(unit.faction)) {
						if (unit.DistanceToUnit(target) < closestUnitDistance) {
							closestUnitDistance = unit.DistanceToUnit(target);
							unit.target = target;
						}
					}
				}
			}

		}

		IEnumerator SummonPhase() {
			WaitForSeconds delay = new WaitForSeconds(1/12f);
			HexCell cell = grid.GetProps()[0].Location;
			Debug.Log("test");
			if (barracksPopup.summonQueue.Count > 0) {
				foreach (int unit in barracksPopup.summonQueue) {
					Debug.Log("test2");
					summonPos = summonPos > 4 ? 0 : summonPos + 1;
			    mapEditor.CreateUnit(unit, cell.GetNeighbor(summonPos));
					barracksPopup.summonQueue.RemoveAt(0);
					barracksPopup.UpdateSummonQueue();
	        Debug.Log(summonPos);
					yield return delay;
				}
			}
		}

		void MovePhase() {
			foreach (HexUnit unit in grid.GetUnits()) {
				if (unit.target) {
					if (unit.DistanceToUnit(unit.target) > unit.attackRange){
						DoPathfinding(unit);
						TurnDirection(unit);
						DoStep(unit);
					}
				}
			}
		}

		IEnumerator AttackPhase() {
			WaitForSeconds delay = new WaitForSeconds(1/3f);
			foreach (HexUnit unit in SortUnitsBySpeed(grid.GetUnits())) {
				if (unit.target) {
					if ((unit.DistanceToUnit(unit.target) <= unit.attackRange) && unit.target) {
						StartCoroutine(Attack(unit));
						//Attack(unit);
						yield return delay;
					}
				}
			}
		}

		IEnumerator EffectPhase() {
			WaitForSeconds delay = new WaitForSeconds(1/6f);
			foreach (HexUnit unit in grid.GetUnits()) {
				if (unit.poisonDamage > 0) {
					unit.TakePoisonDamage();
					yield return delay;
				}

				if (unit.target.IsDead) {
					unit.SetPath(null);
					unit.target = null;
					grid.RemoveUnit(unit.target);
				}

			}
		}


		public IEnumerator Attack (HexUnit unit) {
			WaitForSeconds delay = new WaitForSeconds(1 / 6f);
			if (unit.animator){
				unit.animator.SetTrigger("Attack");
				yield return delay;
				unit.target.animator.SetTrigger("Hurt");
			}
			unit.TryAttackRotate();

			int h0 = unit.target.health;
			int h1 = unit.target.TakeDamage(unit.attackDamage);
			int d = unit.attackDamage;

			if (unit.poison > 0) {
				unit.target.GainPoison(unit.poison);
			}

			int offset = 0;
			//Vector3 pos = new Vector3(unit.target.Location.Position.x, 0,unit.target.Location.Position.y);
			//Vector3 pos = unit.target.Location.Position;
			//Vector3 pos1 = HexCoordinates.ToVector3(HexCoordinates.FromPosition(
				//unit.target.Location.Position));

			//DamagePopup!!!!
			//DamagePopup.Create(unit.target, d*-1);

			//Debug.Log("target " + unit.target.Location.Position.ToString());
			//Debug.Log("pos " + pos.ToString());
			//Debug.Log("pos1 " + pos1.ToString());


			//Debug.Log(h0 + " - " + d + " = " + h1 + " health remaining");
			if (unit.target.IsDead) {
				unit.SetPath(null);
				unit.target = null;
				grid.RemoveUnit(unit.target);
			}
			yield return delay;

		}

		public void AnimateAttack (HexUnit unit) {
			unit.animator.SetTrigger("Attack");
		}

		List<HexUnit> SortUnitsBySpeed (List<HexUnit> list) {
			list.Sort((x,y) => y.speed.CompareTo(x.speed));
			return list;
		}


		void DoPathfinding (HexUnit unit) {
			grid.FindPath(unit.Location, unit.target.Location, fixedSpeed);
			unit.SetPath(grid.GetPath());
		}


		void DoStep (HexUnit unit) {
			if (unit.target && unit.HasPath) {
				unit.TravelStep(unit.GetPath());
			}
		}

		void TurnDirection(HexUnit unit) {
			if ( unit.GetPath()[0].Position.x <
				unit.GetPath()[1].Position.x ) {
					if(!unit.FacingRight) {
						unit.Flip();
					}
			}
			else if (unit.FacingRight) {
				unit.Flip();
			}
		}
}
