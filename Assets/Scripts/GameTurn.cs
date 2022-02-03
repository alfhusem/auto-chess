using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurn : MonoBehaviour
{
    public HexGrid grid;
	public BarracksPopup barracksPopup;
	public StatsPopup statsPopup;
	public HexMapEditor mapEditor;
	public TurnCountUI turnCount;

	public List<HexUnit> toBeRemoved;

	//List<HexCell> path = ListPool<HexCell>.Get();

	int fixedSpeed = 2; // public? remove?
	float turnLength = 2f;

	//public bool turnStarted {get; set;}
	public bool turnStarted;
	//summon position relative to barracks
	int summonPos;


    void Update()
    {
			if(!turnStarted) {
				StartCoroutine(DoTurn());
			}
    }


		void Start() {
			summonPos = 2;
			toBeRemoved = new List<HexUnit>();
		}

    public IEnumerator DoTurn () {

			WaitForSeconds delay = new WaitForSeconds(turnLength);
			WaitForSeconds intermission = new WaitForSeconds(turnLength/12);
			turnStarted = true;
			turnCount.IncrementTurnCount();

			SummonPhase();
			yield return intermission;
			TargetPhase();
			yield return intermission;
			MovePhase();
			yield return intermission;
			StartCoroutine(AttackPhase());
			//StartCoroutine(EffectPhase());
			yield return delay;


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

		void SummonPhase() { //IEnumerator
			//WaitForSeconds delay = new WaitForSeconds(1/12f);
			HexCell cell = grid.GetProps()[0].Location;
			if (barracksPopup.summonQueue.Count > 0) {
				foreach (int unit in barracksPopup.summonQueue) {
					summonPos = summonPos > 4 ? 0 : summonPos + 1;
			    mapEditor.CreateUnit(unit, cell.GetNeighbor(summonPos));
					barracksPopup.summonQueue.RemoveAt(0);
					barracksPopup.UpdateSummonQueue();
					//yield return delay;
					//yield return null;
				}
			}
			if (grid.unitSummonQueue.Count > 0) {
				foreach (KeyValuePair<HexUnit, HexCell> item in grid.unitSummonQueue) {
					grid.AddUnit(item.Key, item.Value);

				}
				grid.unitSummonQueue.Clear();

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
			//toBeRemoved.Clear();
			List<HexUnit> list = SortUnitsBySpeed(grid.GetUnits());
			foreach (HexUnit unit in list) {
				if (unit.target && unit.health > 0) {
					if ((unit.DistanceToUnit(unit.target) <= unit.attackRange) && unit.target) {
						StartCoroutine(Attack(unit));
						//Attack(unit);
						yield return delay;
					}
				}
			}
			if (toBeRemoved.Count > 0) {
				foreach (HexUnit unit in toBeRemoved) {
					grid.RemoveUnit(unit);
					unit.SetPath(null);
					unit.target = null;
					unit.Die();
				}
			}

			//toBeRemoved.Clear();

			//All units are finished attacking
			statsPopup.UpdateHealth();
			StartCoroutine(EndOfTurn());
		}

		IEnumerator EffectPhase() {
			WaitForSeconds delay = new WaitForSeconds(1/6f);
			foreach (HexUnit unit in grid.GetUnits()) {
				if (unit.poisonDamage > 0) {
					unit.TakePoisonDamage();
					//yield return delay;
					yield return null;
				}
				if (unit.target) {
					if (unit.target.IsDead) {
						unit.SetPath(null);
						unit.target = null;
						grid.RemoveUnit(unit.target);
					}
				}

			}
		}

		IEnumerator EndOfTurn() {
			WaitForSeconds delay = new WaitForSeconds(1/6f);
			//StartCoroutine(EffectPhase());

			//yield return delay;
			yield return null;
			turnStarted = false;
		}


		public IEnumerator Attack (HexUnit unit) {
			WaitForSeconds delay = new WaitForSeconds(1 / 6f);
			if (unit.animator){
				unit.animator.SetTrigger("Attack");
				yield return delay;
				unit.target.animator.SetTrigger("Hurt");
			}
			unit.TryAttackRotate();

			//int h0 = unit.target.health;
			int h1 = unit.target.TakeDamage(unit.attackDamage);
			//int d = unit.attackDamage;

			if (unit.poison > 0) {
				unit.target.GainPoison(unit.poison);
			}

			int offset = 0;

			//if (unit.target.IsDead) {
			if (h1 <= 0) {
				toBeRemoved.Add(unit.target);
			}
			yield return delay;

			//Vector3 pos = new Vector3(unit.target.Location.Position.x, 0,unit.target.Location.Position.y);
			//Vector3 pos = unit.target.Location.Position;
			//Vector3 pos1 = HexCoordinates.ToVector3(HexCoordinates.FromPosition(
				//unit.target.Location.Position));

			//DamagePopup!!!!
			//DamagePopup.Create(unit.target, d*-1);

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
