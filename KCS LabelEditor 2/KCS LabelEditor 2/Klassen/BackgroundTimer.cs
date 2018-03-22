using System;
using System.Windows;
using System.Windows.Threading;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    public class BackgroundTimer
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private bool DontReload { get; set; }

        private readonly MainWindow _mainWindow;

        public BackgroundTimer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Init()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += BackGroundChangesTimerHandler;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
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

            _timer.Stop();
            var result = MessageBox.Show(General.FileChangedReloadMessage, General.FileChangedReloadTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
            return _timer.IsEnabled;
        }
    }
}
