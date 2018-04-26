using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using KCS_LabelEditor_2.CustomExceptions;
using static KCS_LabelEditor_2.Helper.Helper;

namespace KCS_LabelEditor_2.Server
{
    public class Server
    {
        private readonly ServiceHost _serviceHost;

        public List<ILabelEditorServiceCallBack> ClientList;


        public Server(MainWindow mainWindow)
        {
            _serviceHost = new ServiceHost(new LabelEditorService(mainWindow));
            ClientList = new List<ILabelEditorServiceCallBack>();
        }


        public void Start()
        {
            if (_serviceHost.State == CommunicationState.Opening || _serviceHost.State == CommunicationState.Opened)
                throw new ConnectionAlreadyOpenException("Connection is already open or opening.");

            _serviceHost.Open();
        }

        public void Stop()
        {
            if (_serviceHost.State == CommunicationState.Closed || _serviceHost.State == CommunicationState.Closing)
                throw new ConnectionAlreadyClosedException("Connection is already closed or closing");
            _serviceHost.Close();
        }

        public bool IsRunning()
        {
            return _serviceHost.State == CommunicationState.Opened;

        }

        private bool ValidateClients()
        {
            bool ok = true;

            if (!ClientList.Any())
                ok = CheckFailed(Properties.MainWindow.NoClientConnectedMessage,
                    Properties.MainWindow.NoClientConnectedTitle);

            return ok;
        }

        public void PasteLabel(string fullId)
        {
            if (ValidateClients())
            {
                foreach (var client in ClientList)
                {
                    try
                    {
                        client.PasteLabel(fullId);
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                        MessageBox.Show(Properties.MainWindow.ClientNotFoundMessage,
                            Properties.MainWindow.ClientNotFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClientList.Remove(client);
                        break;
                    }
                }
            }
        }

        public void Register(ILabelEditorServiceCallBack client)
        {
            if (!ClientList.Contains(client))
            {
                ClientList.Add(client);
            }
        }
    }
}
