using AutoUpdaterDotNET;

namespace AVA_LabelEditor.Objects
{
    class Updater
    {

        public void Init()
        {
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 5;

        }
        public void Start()
        {
            AutoUpdater.Start("http://urganot.net/LabelEditor/updates/UpdateFile.xml");
        }

    }
}
