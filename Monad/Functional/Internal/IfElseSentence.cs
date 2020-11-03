using System;
using System.Collections.Generic;
using System.Text;

namespace CfmArt.Functional.Internal
{
    /// <summary>if-else</summary>
    public class IfElseSentence
    {
        #region if-else
        /// <summary>
        /// If then Else
        /// </summary>
        public interface ElseSentence
        {
            /// <summary></summary>
            void Else(Action onElse);
        }

        /// <summary>else</summary>
        public interface ElseSentence<U>
        {
            /// <summary></summary>
            U Else(Func<U> onElse);
        }

        /// <summary>else</summary>
        internal class DoElse
            : ElseSentence
        {
            /// <summary></summary>
            public void Else(Action onElse) => onElse();
        }

        /// <summary>else</summary>
        internal class DoElse<U>
            : ElseSentence<U>
        {
            /// <summary></summary>
            public U Else(Func<U> onElse) => onElse();
        }

        /// <summary></summary>
        internal class DoNotDoElse
            : ElseSentence
        {
            /// <summary></summary>
            public DoNotDoElse() { }
            /// <summary></summary>
            public void Else(Action _) { }
        }

        /// <summary></summary>
        internal class DoNotDoElse<U>
            : ElseSentence<U>
        {
            /// <summary></summary>
            private U Value { get; }
            /// <summary></summary>
            public DoNotDoElse(U v) => Value = v;
            /// <summary></summary>
            public U Else(Func<U> _) => Value;
        }
        #endregion
    }
}

