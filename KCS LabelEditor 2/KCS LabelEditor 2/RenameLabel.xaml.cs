﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using KCS_LabelEditor_2.Helper;
using KCS_LabelEditor_2.Properties;
using static KCS_LabelEditor_2.Helper.Helper;

namespace KCS_LabelEditor_2
{
    /// <summary>
    /// Interaction logic for RenameLabel.xaml
    /// </summary>
    public partial class RenameLabel
    {
        private MainWindow _mainWindow;
        public RenameLabel(MainWindow mainWindow, string oldId)
        {

            _mainWindow = mainWindow;
            InitializeComponent();

            OldIdTextbox.Text = oldId;

            DataContext = this;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowWithoutCommandbar.RemoveCommandBar(this);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            DialogResult = true;
            Close();

        }

        private bool ValidateForm()
        {
            var ok = true;

            if (string.IsNullOrWhiteSpace(NewIdTextbox.Text))
                ok = CheckFailed(General.Validate_Id_Empty_Message, General.Validate_Id_Empty_Title);

            if (NewIdTextbox.Text == OldIdTextbox.Text)
                ok = CheckFailed(General.Validate_Rename_OldEqualsNew_Message, General.Validate_Rename_OldEqualsNew_Title);

            if (_mainWindow.Labels.IdExists(NewIdTextbox.Text))
                ok = CheckFailed(General.Validate_Id_Exists_Message, General.Validate_Id_Exists_Title);

            if (NewIdTextbox.Text.Contains("="))
                ok = CheckFailed(General.Validate_Id_InvalidCharacter_Message, General.Validate_Id_InvalidCharacter_Title);

            return ok;
        }
    }
}
