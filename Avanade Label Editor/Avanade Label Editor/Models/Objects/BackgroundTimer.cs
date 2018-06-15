using System;
using System.Windows.Threading;

namespace AVA_LabelEditor.Objects
{
    public class BackgroundTimer
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        /// <summary>
        /// Saves the ussers chaoise wether he wants to reload a changes file or not
        /// </summary>
        private bool DontReload { get; set; }

        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Constructor for the BackroundTimer class
        /// </summary>
        /// <param name="mainWindow">An instance of the MainWindow class</param>
        public BackgroundTimer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// Sets the timers properties to its default values
        /// </summary>
        public void Init()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += BackGroundChangesTimerHandler;
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// This method is executed every time the timer reaches its set intervall
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Resets the timer
        /// After the method the timer is running
        /// </summary>
        public void Reset()
        {
            if(_timer.IsEnabled)
                _timer.Stop();

            DontReload = false;

            if(!_timer.IsEnabled)
                _timer.Start();
        }

        /// <summary>
        /// Checks if the timer is running
        /// </summary>
        /// <returns>True if the timer is running</returns>
        public bool IsRunning()
        {
            return _timer.IsEnabled;
        }
    }
}
