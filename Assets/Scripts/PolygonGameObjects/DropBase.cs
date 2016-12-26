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
                Kill();
            }
		}

        public virtual void OnUserInteracted() {
        }

        public virtual void OnLifeTimeEnd() {
        }

        public virtual void OnGameEnd() {
        }
    }
}