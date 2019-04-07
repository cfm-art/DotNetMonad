using System;
using System.Collections.Generic;
using System.Text;

namespace CfmArt.Functional.Internal
{
    public class IfElseSentence
    {
        #region if-else
        /// <summary>
        /// If then Else
        /// </summary>
        public interface ElseSentence
        {
            void Else(Action onElse);
        }

        public interface ElseSentence<U>
        {
            U Else(Func<U> onElse);
        }

        internal class DoElse
            : ElseSentence
        {
            public void Else(Action onElse) => onElse();
        }

        internal class DoElse<U>
            : ElseSentence<U>
        {
            public U Else(Func<U> onElse) => onElse();
        }

        internal class DoNotDoElse
            : ElseSentence
        {
            public DoNotDoElse() { }
            public void Else(Action _) { }
        }

        internal class DoNotDoElse<U>
            : ElseSentence<U>
        {
            private U Value { get; }
            public DoNotDoElse(U v) => Value = v;
            public U Else(Func<U> _) => Value;
        }
        #endregion
    }
}

