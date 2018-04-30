using System;
using System.Windows.Threading;

namespace AVA_LabelEditor.Objects
{
    public class BackgroundTimer
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private bool DontReload { get; set; }

        private readonly AVA_LabelEditor.MainWindow _mainWindow;

        public BackgroundTimer(AVA_LabelEditor.MainWindow mainWindow)
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
            DontReload = _mainWindow.HandleChangedFile();
        }

        public void Reset()
        {
            if(_timer.IsEnabled)
                _timer.Stop();

            DontReload = false;

            if(!_timer.IsEnabled)
                _timer.Start();
        }

        public bool IsRunning()
        {
            return _timer.IsEnabled;
        }
    }
}
