using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace polygonGO {
    public class DropBase : PolygonGameObject {
        public float lifetime;
		public override void Tick (float delta)
		{
			base.Tick (delta);
			lifetime -= delta;
            if (lifetime < 0) {
                OnLifeTimeEnd();
				Kill(KillReason.EXPIRED);
            }

            Brake(delta, 2.5f);
		}

		public virtual void OnInteracted(PolygonGameObject picker) {
        }

        public virtual void OnLifeTimeEnd() {
        }

        public virtual void OnGameEnd() {
        }
    }
}