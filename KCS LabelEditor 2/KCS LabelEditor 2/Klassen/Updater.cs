using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoUpdaterDotNET;

namespace KCS_LabelEditor_2
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
