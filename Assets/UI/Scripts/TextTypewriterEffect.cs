using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TextTypewriterEffect
{

    [RequireComponent(typeof(Text))]
    public class TextTypewriterEffect : MonoBehaviour
    {
        [Header("Speed")]
        [Tooltip("Delay before the text starts appear (in seconds)")]
        public float StartDelay = 2.0f;
        [Tooltip("Speed of printing (symbols per minute)")]
        public int Speed = 600;
        [Tooltip("When checked, allows to immediately interrupt printing and show whole text if user press ESC")]
        public bool ESC = true;
        private float interval = 0.1f;  // calculated interval of printing of one symbol
        private string t1 = null;
        private string t2 = null;
        private Text text;
        private int n = -1;
        private IEnumerator coroutine;

        [Header("Cursor")]
        [Tooltip("Show flashing cursor")]
        public bool Cursor = true;
        private bool _cursor = true;

        [Tooltip("Cursor flashing interval")]
        public float CursorInterval = 0.5f;
        private bool ok = false;

        void Start() {
            text = GetComponent<Text>();
            if (text == null) { Debug.Log("This object does not contain text component!"); return; }
            t2 = text.text;
            if (t2.Length == 0) { Debug.Log("Text doesn't contain any characters!"); return; }
            text.text = null;
            interval = (60.0f/Speed);
            ok = true;
        }

        void OnEnable() {
            //if (!ok) return;
            coroutine = SetText();
            Invoke("Delay", StartDelay);
            if (Cursor) InvokeRepeating("CursorInterruption", 0, CursorInterval);
        }

        // Starts text printing after delay:
        void Delay() { StartCoroutine(coroutine); }

        void Update() {
            // Interrupts printing if ESC key was pressed:
            if (!ESC || !ok) return;
            if (Input.GetKeyDown("escape")) {
                StopCoroutine(coroutine);
                CancelInvoke("Before");
                t1 = t2; text.text = t1;
            }
        }

        // Print text letter by letter:
        IEnumerator SetText() {
            while (true) {
                if (n < t2.Length - 1) {
                    n++; t1 += t2[n]; text.text = t1;
                    if (_cursor && Cursor) text.text = t1 + "_";
                    yield return new WaitForSeconds(interval);
                } else {
                    if (Cursor) text.text = t1 + "_";
                    yield break;
                }
            }
        }

        // Cursor flashing:
        void CursorInterruption() {
            if (_cursor) text.text = t1 + "_";
            else { text.text = t1; }
            _cursor = !_cursor;
        }

        // Reset
        void OnDisable() {
            StopCoroutine(coroutine);
            CancelInvoke();
            t1 = null;
            n = -1;
        }
    }

}//namespace

//end of script
