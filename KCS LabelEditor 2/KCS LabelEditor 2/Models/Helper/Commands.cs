using System.Windows.Input;

namespace KCS_LabelEditor_2.Helper
{
    class CustomCommands
    {
        public static readonly RoutedUICommand Save = new RoutedUICommand
        (
            "Save", "Save", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand New = new RoutedUICommand
        (
            "New", "New", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.N, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Rename = new RoutedUICommand
        (
            "Rename", "Rename", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.R, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Delete = new RoutedUICommand
        (
            "Delete", "Delete", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.Delete, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Translate = new RoutedUICommand
        (
            "Translate", "Translate", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.T, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand PasteToVs = new RoutedUICommand
        (
            "PasteToVs", "PasteToVs", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.P, ModifierKeys.Control) }
        );

    }
}
