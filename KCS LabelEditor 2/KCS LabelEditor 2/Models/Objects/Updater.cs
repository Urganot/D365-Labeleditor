using AutoUpdaterDotNET;

namespace KCS_LabelEditor_2.Objects
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
