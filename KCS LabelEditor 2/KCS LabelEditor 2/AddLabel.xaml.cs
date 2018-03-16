﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KCS_LabelEditor_2
{
    /// <summary>
    /// Interaction logic for AddLabel.xaml
    /// </summary>
    public partial class AddLabel : Window
    {
        private MainWindow MainWindow;

        public AddLabel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        private bool ValidateForm()
        {
            var ok = true;
            if (string.IsNullOrWhiteSpace(Id.Text))
            {
                MessageBox.Show("Id darf nicht leer sein", "Id darf nicht leer sein", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(Text.Text))
            {
                MessageBox.Show("Text darf nicht leer sein", "Text darf nicht leer sein", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (MainWindow.IdExists(Id.Text))
            {
                MessageBox.Show("Id ist bereits vorhanden.", "Id gefunden", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            return ok;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidateForm())
            {
                return;
            }

            DialogResult = true;
            this.Close();
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

        }
    }
}
