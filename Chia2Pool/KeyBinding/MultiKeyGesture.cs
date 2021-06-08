using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Chia2Pool.KeyBinding
{
    /// <summary>
    ///   Class used to define a multi-key gesture.
    /// </summary>
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    public class MultiKeyGesture : InputGesture
    {
        /// <summary>
        ///   The maximum delay between key presses.
        /// </summary>
        static readonly TimeSpan maximumDelay = TimeSpan.FromSeconds(1);

        /// <summary>
        ///   Determines whether the keyis define.
        /// </summary>
        /// <param name="key"> The key to check. </param>
        /// <returns> <c>True</c> if the key is defined as a gesture key; otherwise, <c>false</c> . </returns>
        static bool IsDefinedKey(Key key)
        {
            return ((key >= Key.None) && (key <= Key.OemClear));
        }

        /// <summary>
        ///   Gets the key sequences string.
        /// </summary>
        /// <param name="sequences"> The key sequences. </param>
        /// <returns> The string representing the key sequences. </returns>
        static string GetKeySequencesString(params KeySequence[] sequences)
        {
            if (sequences == null)
                throw new ArgumentNullException("sequences");
            if (sequences.Length == 0)
                throw new ArgumentException("At least one sequence must be specified.", "sequences");

            var builder = new StringBuilder();

            builder.Append(sequences[0].ToString());

            for (var i = 1; i < sequences.Length; i++)
                builder.Append(", " + sequences[i]);

            return builder.ToString();
        }

        /// <summary>
        ///   Determines whether the specified key is a modifier key.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns> <c>True</c> if the specified key is a modifier key; otherwise, <c>false</c> . </returns>
        static bool IsModifierKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftAlt || key == Key.RightAlt || key == Key.LWin || key == Key.RWin;
        }

        /// <summary>
        ///   The display string.
        /// </summary>
        readonly string displayString;

        /// <summary>
        ///   The key sequences composing the gesture.
        /// </summary>
        readonly KeySequence[] keySequences;

        /// <summary>
        ///   The index of the current gesture key.
        /// </summary>
        int currentKeyIndex;

        /// <summary>
        ///   The current sequence index.
        /// </summary>
        int currentSequenceIndex;

        /// <summary>
        ///   The last time a gesture key was pressed.
        /// </summary>
        DateTime lastKeyPress;

        /// <summary>
        ///   Gets the key sequences composing the gesture.
        /// </summary>
        /// <value> The key sequences composing the gesture. </value>
        public KeySequence[] KeySequences
        {
            get { return keySequences; }
        }

        /// <summary>
        ///   Gets the display string.
        /// </summary>
        /// <value> The display string. </value>
        public string DisplayString
        {
            get { return displayString; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MultiKeyGesture" /> class.
        /// </summary>
        /// <param name="sequences"> The key sequences. </param>
        public MultiKeyGesture(params KeySequence[] sequences)
            : this(GetKeySequencesString(sequences), sequences)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MultiKeyGesture" /> class.
        /// </summary>
        /// <param name="displayString"> The display string. </param>
        /// <param name="sequences"> The key sequences. </param>
        public MultiKeyGesture(string displayString, params KeySequence[] sequences)
        {
            if (sequences == null)
                throw new ArgumentNullException("sequences");
            if (sequences.Length == 0)
                throw new ArgumentException("At least one sequence must be specified.", "sequences");

            this.displayString = displayString;
            keySequences = new KeySequence[sequences.Length];
            sequences.CopyTo(keySequences, 0);
        }

        /// <summary>
        ///   Determines whether this <see cref="System.Windows.Input.KeyGesture" /> matches the input associated with the specified <see
        ///    cref="System.Windows.Input.InputEventArgs" /> object.
        /// </summary>
        /// <param name="targetElement"> The target. </param>
        /// <param name="inputEventArgs"> The input event data to compare this gesture to. </param>
        /// <returns> true if the event data matches this <see cref="System.Windows.Input.KeyGesture" /> ; otherwise, false. </returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var args = inputEventArgs as KeyEventArgs;

            if (args == null || args.IsRepeat)
                return false;

            var key = args.Key != Key.System ? args.Key : args.SystemKey;
            //Check if the key identifies a gesture key...
            if (!IsDefinedKey(key))
                return false;

            var currentSequence = keySequences[currentSequenceIndex];
            var currentKey = currentSequence.Keys[currentKeyIndex];

            //Check if the key is a modifier...
            if (IsModifierKey(key))
            {
                //If the pressed key is a modifier, ignore it for now, since it is tested afterwards...
                return false;
            }

            //Check if the current key press happened too late...
            if (currentSequenceIndex != 0 && ((DateTime.Now - lastKeyPress) > maximumDelay))
            {
                //The delay has expired, abort the match...
                ResetState();
#if DEBUG_MESSAGES
                System.Diagnostics.Debug.WriteLine("Maximum delay has elapsed", "[" + MultiKeyGestureConverter.Default.ConvertToString(this) + "]");
#endif
                return false;
            }

            //Check if current modifiers match required ones...
            if (currentSequence.Modifiers != args.KeyboardDevice.Modifiers)
            {
                //The modifiers are not the expected ones, abort the match...
                ResetState();
#if DEBUG_MESSAGES
                System.Diagnostics.Debug.WriteLine("Incorrect modifier " + args.KeyboardDevice.Modifiers + ", expecting " + currentSequence.Modifiers, "[" + MultiKeyGestureConverter.Default.ConvertToString(this) + "]");
#endif
                return false;
            }

            //Check if the current key is not correct...
            if (currentKey != key)
            {
                //The current key is not correct, abort the match...
                ResetState();
#if DEBUG_MESSAGES
                System.Diagnostics.Debug.WriteLine("Incorrect key " + key + ", expecting " + currentKey, "[" + MultiKeyGestureConverter.Default.ConvertToString(this) + "]");
#endif
                return false;
            }

            //Move on the index, pointing to the next key...
            currentKeyIndex++;

            //Check if the key is the last of the current sequence...
            if (currentKeyIndex == keySequences[currentSequenceIndex].Keys.Length)
            {
                //The key is the last of the current sequence, go to the next sequence...
                currentSequenceIndex++;
                currentKeyIndex = 0;
            }

            //Check if the sequence is the last one of the gesture...
            if (currentSequenceIndex != keySequences.Length)
            {
                //If the key is not the last one, get the current date time, handle the match event but do nothing...
                lastKeyPress = DateTime.Now;
                inputEventArgs.Handled = true;
#if DEBUG_MESSAGES
                System.Diagnostics.Debug.WriteLine("Waiting for " + (m_KeySequences.Length - m_CurrentSequenceIndex) + " sequences", "[" + MultiKeyGestureConverter.Default.ConvertToString(this) + "]");
#endif
                return false;
            }

            //The gesture has finished and was correct, complete the match operation...
            ResetState();
            inputEventArgs.Handled = true;
#if DEBUG_MESSAGES
            System.Diagnostics.Debug.WriteLine("Gesture completed " + MultiKeyGestureConverter.Default.ConvertToString(this), "[" + MultiKeyGestureConverter.Default.ConvertToString(this) + "]");
#endif
            return true;
        }

        /// <summary>
        ///   Resets the state of the gesture.
        /// </summary>
        void ResetState()
        {
            currentSequenceIndex = 0;
            currentKeyIndex = 0;
        }
    }

    public class KeySequence
    {
        /// <summary>
        ///   Gets the sequence of keys.
        /// </summary>
        /// <value> The sequence of keys. </value>
        public Key[] Keys { get; }

        /// <summary>
        ///   Gets the modifiers to be applied to the sequence.
        /// </summary>
        /// <value> The modifiers to be applied to the sequence. </value>
        public ModifierKeys Modifiers { get; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KeySequence" /> class.
        /// </summary>
        public KeySequence(ModifierKeys modifiers, params Key[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            if (keys.Length < 1)
                throw new ArgumentException(@"At least 1 key should be provided", nameof(keys));

            Keys = new Key[keys.Length];
            keys.CopyTo(Keys, 0);
            Modifiers = modifiers;
        }

        /// <summary>
        ///   Returns a <see cref="string" /> that represents the current <see cref="object" /> .
        /// </summary>
        /// <returns> A <see cref="string" /> that represents the current <see cref="object" /> . </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Modifiers != ModifierKeys.None)
            {
                if ((Modifiers & ModifierKeys.Control) != ModifierKeys.None)
                    builder.Append("Ctrl+");
                if ((Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
                    builder.Append("Alt+");
                if ((Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
                    builder.Append("Shift+");
                if ((Modifiers & ModifierKeys.Windows) != ModifierKeys.None)
                    builder.Append("Windows+");
            }

            builder.Append(Keys[0]);

            for (var i = 1; i < Keys.Length; i++)
            {
                builder.Append("+" + Keys[i]);
            }

            return builder.ToString();
        }
    }

    public class KeyTrigger : TriggerBase<UIElement>
    {
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger), null);

        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTrigger), null);

        public Key Key
        {
            get { return (Key) GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys) GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.KeyDown += OnAssociatedObjectKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyDown -= OnAssociatedObjectKeyDown;
        }

        private void OnAssociatedObjectKeyDown(object sender, KeyEventArgs e)
        {
            var key = (e.Key == Key.System) ? e.SystemKey : e.Key;
            if ((key == Key) && (Keyboard.Modifiers == GetActualModifiers(e.Key, Modifiers)))
            {
                InvokeActions(e);
            }
        }

        static ModifierKeys GetActualModifiers(Key key, ModifierKeys modifiers)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    modifiers |= ModifierKeys.Control;
                    return modifiers;

                case Key.LeftAlt:
                case Key.RightAlt:
                    modifiers |= ModifierKeys.Alt;
                    return modifiers;

                case Key.LeftShift:
                case Key.RightShift:
                    modifiers |= ModifierKeys.Shift;
                    break;
            }

            return modifiers;
        }
    }


    /// <summary>
    ///   Class used to define a converter for the <see cref="MultiKeyGesture" /> class.
    /// </summary>
    /// <remarks>
    ///   At the moment it is able to convert strings like <c>Alt+K,R</c> in proper multi-key gestures.
    /// </remarks>
    public class MultiKeyGestureConverter : TypeConverter
    {
        /// <summary>
        ///   The default instance of the converter.
        /// </summary>
        public static readonly MultiKeyGestureConverter DefaultConverter = new MultiKeyGestureConverter();

        /// <summary>
        ///   The inner key converter.
        /// </summary>
        static readonly KeyConverter KeyConverter = new KeyConverter();

        /// <summary>
        ///   The inner modifier key converter.
        /// </summary>
        static readonly ModifierKeysConverter ModifierKeysConverter = new ModifierKeysConverter();

        /// <summary>
        ///   Tries to get the modifier equivalent to the specified string.
        /// </summary>
        /// <param name="str"> The string. </param>
        /// <param name="modifier"> The modifier. </param>
        /// <returns> <c>True</c> if a valid modifier was found; otherwise, <c>false</c> . </returns>
        static bool TryGetModifierKeys(string str, out ModifierKeys modifier)
        {
            switch (str.ToUpper())
            {
                case "CONTROL":
                case "CTRL":
                    modifier = ModifierKeys.Control;
                    return true;

                case "SHIFT":
                    modifier = ModifierKeys.Shift;
                    return true;

                case "ALT":
                    modifier = ModifierKeys.Alt;
                    return true;

                case "WINDOWS":
                case "WIN":
                    modifier = ModifierKeys.Windows;
                    return true;

                default:
                    modifier = ModifierKeys.None;
                    return false;
            }
        }

        /// <summary>
        ///   Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context"> An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="sourceType"> A <see cref="System.Type" /> that represents the type you want to convert from. </param>
        /// <returns> true if this converter can perform the conversion; otherwise, false. </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        ///   Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context"> An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="culture"> The <see cref="System.Globalization.CultureInfo" /> to use as the current culture. </param>
        /// <param name="value"> The <see cref="object" /> to convert. </param>
        /// <returns> An <see cref="object" /> that represents the converted value. </returns>
        /// <exception cref="System.NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var str = (value as string);

            if (!string.IsNullOrEmpty(str))
            {
                var sequences = str.Split(',');
                string[] keyStrings;

                var keySequences = new List<KeySequence>();


                foreach (var sequence in sequences)
                {
                    var modifier = ModifierKeys.None;
                    var keys = new List<Key>();
                    keyStrings = sequence.Split('+');
                    var modifiersCount = 0;

                    ModifierKeys currentModifier;
                    string temp;
                    while ((temp = keyStrings[modifiersCount]) != null &&
                           TryGetModifierKeys(temp.Trim(), out currentModifier))
                    {
                        modifiersCount++;
                        modifier |= currentModifier;
                    }

                    for (var i = modifiersCount; i < keyStrings.Length; i++)
                    {
                        var keyString = keyStrings[i];
                        {
                            var key = (Key) KeyConverter.ConvertFrom(keyString.Trim());
                            keys.Add(key);
                        }
                    }

                    keySequences.Add(new KeySequence(modifier, keys.ToArray()));
                }

                return new MultiKeyGesture(str, keySequences.ToArray());
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        ///   Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context"> An <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="culture"> A <see cref="System.Globalization.CultureInfo" /> . If null is passed, the current culture is assumed. </param>
        /// <param name="value"> The <see cref="object" /> to convert. </param>
        /// <param name="destinationType"> The <see cref="System.Type" /> to convert the <paramref name="value" /> parameter to. </param>
        /// <returns> An <see cref="object" /> that represents the converted value. </returns>
        /// <exception cref="System.ArgumentNullException">The
        ///   <paramref name="destinationType" />
        ///   parameter is null.</exception>
        /// <exception cref="System.NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is MultiKeyGesture gesture)
                {
                    var builder = new StringBuilder();

                    for (var i = 0; i < gesture.KeySequences.Length; i++)
                    {
                        if (i > 0)
                            builder.Append(", ");

                        var sequence = gesture.KeySequences[i];
                        if (sequence.Modifiers != ModifierKeys.None)
                        {
                            builder.Append((string) ModifierKeysConverter.ConvertTo(context, culture,
                                sequence.Modifiers, destinationType));
                            builder.Append("+");
                        }

                        builder.Append((string) KeyConverter.ConvertTo(context, culture, sequence.Keys[0],
                            destinationType));

                        for (var j = 1; j < sequence.Keys.Length; j++)
                        {
                            builder.Append("+");
                            builder.Append((string) KeyConverter.ConvertTo(context, culture, sequence.Keys[0],
                                destinationType));
                        }
                    }

                    return builder.ToString();
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}