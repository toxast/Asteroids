using UnityEngine;
using System.Collections;

namespace polygonGO
{
	public class Drop : DropBase 
	{
		public int value;
		bool moneyAdded = false;

        private void AddMoneyIfNotYet() {
            if (!moneyAdded) {
                moneyAdded = true;
				Singleton<Main>.inst.AddMoneyOnDropNotInterated(this);
            }
        }

		public override void OnInteracted(PolygonGameObject picker) {
            if (!moneyAdded) {
                moneyAdded = true;
				Singleton<Main>.inst.AddMoneyOnDropInterated(this);
            }
        }

        public override void OnLifeTimeEnd() {
            AddMoneyIfNotYet();
        }

        public override void OnGameEnd() {
            AddMoneyIfNotYet();
        }
	}
}
