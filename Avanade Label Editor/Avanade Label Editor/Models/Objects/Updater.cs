using AutoUpdaterDotNET;

namespace AVA_LabelEditor.Objects
{
    class Updater
    {

        /// <summary>
        /// Sets defaultvalues for the updater
        /// </summary>
        public void Init()
        {
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 5;
        }

        /// <summary>
        /// Starts the updater
        /// </summary>
        public void Start()
        {
            // Disabled until a new update method is decided
            return;
            AutoUpdater.Start("http://urganot.net/LabelEditor/updates/UpdateFile.xml");
        }

    }
}
