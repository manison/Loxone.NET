using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loxone.Client
{
    public struct TextState
    {
        private Uuid _uuid;
        public Uuid Uuid => _uuid;

        private Uuid _uuidIcon;
        public Uuid UuidIcon => _uuidIcon;

        private int _textLength;
        public int TextLength => _textLength;

        private string _text;
        public string Text => _text;

        public TextState(Uuid Uuid, Uuid UuidIcon, int TextLength, string Text)
        {
            this._uuid = Uuid;
            this._uuidIcon = UuidIcon;
            this._textLength = TextLength;
            this._text = Text;
        }

        public override string ToString()
        {
            return string.Concat(String.Format("UUID:{0}, UUIDIcon:{1}, Textlength:{2}, Text:{3}", _uuid.ToString(), _uuidIcon.ToString(), _textLength, _text));
        }
    }

}
