using System;
using UnityEngine;
using UnityEngine.UI;

namespace ui5 {
    public class ButtonToggle5 : ToggleInteractable5 {
        Button _button;
        Button button {
            set { _button = value; }
            get {
                if ( _button == null ) _button = GetComponent<Button>();
                if ( _button == null ) _button = GetComponentInChildren<Button>();

                return _button;
            }
        }

        public event Action clickCallback;

        public RectTransform Rect {
            get {
                return button.transform as RectTransform;
            }
        }

        protected override void OnValidate() {
            base.OnValidate();
        }

        protected override void Awake() {
            base.Awake();
            button.onClick.AddListener(OnClick);
        }

        void OnClick() {
            if ( clickCallback != null )
                clickCallback();
        }

        protected override void RefreshView() {
            if ( _button == null ) return;
            button.interactable = _interactable;
        }

    }
}
