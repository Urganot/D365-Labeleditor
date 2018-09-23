using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using AVA_LabelEditor.CustomExceptions;

namespace AVA_LabelEditor.Server
{
    public class Server : IDisposable
    {

        private readonly ServiceHost _serviceHost;

        /// <summary>
        /// Contains all connected clients
        /// </summary>
        public Dictionary<Guid, ILabelEditorServiceCallBack> ClientList;

        /// <summary>
        /// Constructor for the Sever class
        /// </summary>
        /// <param name="mainWindow">An Instance of the MainWindow class</param>
        public Server(MainWindow mainWindow)
        {
            _serviceHost = new ServiceHost(new LabelEditorService(mainWindow));
            ClientList = new Dictionary<Guid, ILabelEditorServiceCallBack>();
        }

        /// <summary>
        /// Opens the connection
        /// </summary>
        public void Start()
        {
            if (_serviceHost.State == CommunicationState.Opening || _serviceHost.State == CommunicationState.Opened)
                throw new ConnectionAlreadyOpenException(Properties.Exceptions.ConnectionAlreadyOpen);

            _serviceHost.Open();
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public void Stop()
        {
            if (_serviceHost.State == CommunicationState.Closed || _serviceHost.State == CommunicationState.Closing)
                throw new ConnectionAlreadyClosedException(Properties.Exceptions.ConnectionAlreadyClosed);
            _serviceHost.Close();
        }

        /// <summary>
        /// Checks if the connection is open
        /// </summary>
        /// <returns>True if connection is openend</returns>
        public bool IsRunning()
        {
            return _serviceHost.State == CommunicationState.Opened;

        }

        /// <summary>
        /// Validates the client list
        /// </summary>
        /// <returns>True if clients are valid</returns>
        private bool ValidateClients()
        {
            bool ok = true;

            if (!ClientList.Any())
                ok = Helper.Helper.CheckFailed(Properties.MainWindow.NoClientConnectedMessage,
                    Properties.MainWindow.NoClientConnectedTitle);

            return ok;
        }

        /// <summary>
        /// Calls the PasteLabel on all valid clients
        /// </summary>
        /// <param name="text">The text to paste</param>
        public void PasteLabel(string text)
        {
            if (ValidateClients())
            {
                foreach (var client in ClientList)
                {
                    try
                    {
                        client.Value.PasteLabel(text);
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                        MessageBox.Show(Properties.MainWindow.ClientNotFoundMessage,
                            Properties.MainWindow.ClientNotFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClientList.Remove(client.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a client to the list of connected clients
        /// </summary>
        /// <param name="client">The client object</param>
        /// <param name="guid">A Guid to identify the client</param>
        public void Register(ILabelEditorServiceCallBack client, Guid guid)
        {
            if (!ClientList.ContainsKey(guid))
            {
                ClientList.Add(guid, client);
            }
        }

        public void Dispose()
        {
            ((IDisposable) _serviceHost)?.Dispose();
        }
    }
}
