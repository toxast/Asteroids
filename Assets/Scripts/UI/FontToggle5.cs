using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ui5
{
    [ExecuteInEditMode]
    public class FontToggle5 : Toggle5
    {
        [SerializeField] Text elem;
        [SerializeField] Font off;
        [SerializeField] int offSize;
        [SerializeField] Font on;
         [SerializeField] int onSize;

        protected override void Awake()
        {
            base.Awake();
            if (elem == null)
            {
                elem = GetComponent<Text>();
                off = elem.font;
                on = elem.font;
                offSize = onSize = elem.fontSize;
            }
        }

        protected override void RefreshView()
        {
            if (elem == null)
                elem = GetComponent<Text>();

            if (elem == null)
                return;

            elem.font = _state ? on : off;
            elem.fontSize = _state ? onSize : offSize;
        }
    }
}