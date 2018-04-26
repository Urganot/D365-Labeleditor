using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Communication;
using KCS_LabelEditor_2.Klassen.CustomExceptions;

namespace KCS_LabelEditor_2.Klassen
{
    class Server
    {
        private readonly ServiceHost _serviceHost;

        public Server(MainWindow mainWindow)
        {
            _serviceHost = new ServiceHost(new LabelEditorService(mainWindow));
        }


        public void Start()
        {
            if(_serviceHost.State == CommunicationState.Opening || _serviceHost.State==CommunicationState.Opened)
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

    }
}
