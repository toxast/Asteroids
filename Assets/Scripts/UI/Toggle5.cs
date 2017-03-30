using System.Collections.Generic;
using UnityEngine;

namespace ui5 {
    public class Toggle5 : MonoBehaviour, IToggle5 {

        [SerializeField]
        protected List<Toggle5> linked;
        [SerializeField]
        protected bool _state = false;

        protected virtual void Awake() { }
        protected virtual void Start() { }

        protected virtual void OnValidate() {
            if ( linked == null )
                linked = new List<Toggle5>();

            foreach ( var item in linked ) {
                item.SetState(IsOn);
                item.SetInteractable(IsInteractable);
            }

            RefreshView();
        }


        public bool IsOn {
            get { return _state; }
        }

        public virtual bool IsInteractable {
            get { return false; }
        }

        public virtual void SetInteractable(bool interactable) { }

        protected virtual void RefreshView() { }

        public void SetState(bool on) {
			if (linked == null)
				linked = new List<Toggle5> ();

            if ( _state == on )
                return;

            _state = on;

            foreach ( var item in linked )
                item.SetState(_state);

            RefreshView();
        }

        public void On() { SetState(true); }
        public void Off() { SetState(false); }
        public void Toggle() { SetState(!_state); }

    }

}
