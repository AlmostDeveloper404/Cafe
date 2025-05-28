using ReaperGS;
using UnityEngine;

namespace cherrydev
{


    [System.Serializable]
    public struct Sentence
    {
        public string Text;

        public Sentence(string text)
        {
            Text = text;
        }
    }
}