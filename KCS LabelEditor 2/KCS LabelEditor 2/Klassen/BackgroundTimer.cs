using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace KCS_LabelEditor_2.Klassen
{
    public class BackgroundTimer
    {
        public DispatcherTimer Timer { get; set; }

        public Boolean DontReload { get; set; }

        private MainWindow _mainWindow;

        public BackgroundTimer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        private void Init()
        {
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += BackGroundChangesTimerHandler;
        }

        public void Start()
        {
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        private void BackGroundChangesTimerHandler(object sender, EventArgs e)
        {
            bool changed = false;
            foreach (var labelFile in _mainWindow.ReadFilesNew.All)
            {
                labelFile.Changed = labelFile.OriginalHash != labelFile.NewHash();
                if (labelFile.Changed)
                {
                    changed = labelFile.Changed;
                    break;
                }
            }

            if (!changed || DontReload)
                return;

            Timer.Stop();
            var result = MessageBox.Show("Datei wurde geändert soll diese neu geladen werden?", "Datei geändert", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    _mainWindow.ReloadLabels();
                    break;
                case MessageBoxResult.No:
                    DontReload = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsRunning()
        {
            return Timer.IsEnabled;
        }
    }
}
