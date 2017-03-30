using UnityEngine;
using UnityEngine.UI;

namespace ui5 {
    public class TextButtonToggle5 : ButtonToggle5, ITextElement5 {
        
        Text _text;
        Text text {
            set { _text = value; }
            get {
                if ( _text == null ) _text = GetComponent<Text>();
                if ( _text == null ) _text = GetComponentInChildren<Text>();

                return _text;
            }
        }

        public string Title {
            get { return text.text; }
            set { text.text = value; }
        }

        public string GetText() { return Title; }

        //protected override void Awake() {
        //    base.Awake();
        //}

    }
}
