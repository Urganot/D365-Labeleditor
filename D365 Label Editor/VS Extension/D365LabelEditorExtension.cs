﻿//------------------------------------------------------------------------------
// <copyright file="D365LabelEditorExtension.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows.Forms;
using AutoComment2015.Helper;
using D365_Label_Editor_Extension.LabelEditorServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace D365_Label_Editor_Extension
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class D365LabelEditorExtension
    {
        private static Guid _guid;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f943980a-5f69-490d-9815-cdb353dcbf38");

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="D365LabelEditorExtension" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private D365LabelEditorExtension(Package package)
        {
            _package = package;

            if (_package == null)
                throw new ArgumentNullException(nameof(package));

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            commandService?.AddCommand(new MenuCommand(CreateLabel, new CommandID(CommandSet, 0x100)));
            commandService?.AddCommand(new MenuCommand(SearchLabel, new CommandID(CommandSet, 0x101)));
            commandService?.AddCommand(new MenuCommand(Connect, new CommandID(CommandSet, 0x104)));

            _guid = Guid.NewGuid();
        }

        public static ILabelEditorServiceChannel Proxy
        {
            get
            {
                var binding = new NetTcpBinding();

                var ctx = new InstanceContext(new LabelEditorServiceCallBack());
                var endpoint = new EndpointAddress("net.tcp://localhost:2113");

                var channel = new DuplexChannelFactory<ILabelEditorServiceChannel>(ctx,
                    new ServiceEndpoint(ContractDescription.GetContract(typeof(ILabelEditorService)), binding,
                        endpoint)).CreateChannel();
                channel.OperationTimeout = new TimeSpan(0,5,0);
                return channel;
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static D365LabelEditorExtension Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new D365LabelEditorExtension(package);
            TryOpenConnection(false);
        }

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void CreateLabel(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!TryOpenConnection())
                return;

            try
            {
                var dte = (DTE) ServiceProvider.GetService(typeof(DTE));
                if (dte == null)
                    throw new ArgumentNullException(Strings.DteNotFound);

                var activeDocument = dte.ActiveDocument;

                if (activeDocument != null)
                {
                    var selection = (TextSelection) dte.ActiveDocument.Selection;
                    var text = selection.Text;
                    if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                    {
                        MessageBox.Show(Strings.NoTextSelectedMessage, Strings.NoTextSelectedTitle);
                    }
                    else
                    {
                        var dte2 = (DTE2) ServiceProvider.GetService(typeof(DTE));

                        var helper = new Helper(dte2);

                        var addedLabels = Proxy.CreateNewLabel(text);

                        if (addedLabels.ContainsKey("base"))
                        {
                            var result = addedLabels["base"].First().FullId;
                            helper.InsertText(Helper.GetCurrentTextView(), result);
                        }
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void SearchLabel(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!TryOpenConnection())
                return;

            try
            {
                var dte = (DTE) ServiceProvider.GetService(typeof(DTE));
                if (dte == null)
                    throw new ArgumentNullException(Strings.DteNotFound);

                var activeDocument = dte.ActiveDocument;

                if (activeDocument != null)
                {
                    var selection = (TextSelection) dte.ActiveDocument.Selection;
                    var text = selection.Text;
                    if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                        MessageBox.Show(Strings.NoTextSelectedMessage, Strings.NoTextSelectedTitle);
                    else
                        Proxy.SearchLabel(text);
                }
            }
            catch (EndpointNotFoundException)
            {
                EndpointNotFound();
            }
        }

        private void Connect(object sender, EventArgs e)
        {
            if (TryOpenConnection())
                MessageBox.Show(Strings.ConnectSuccessful);
        }


        private static bool IsConnectionOpen()
        {
            return Proxy.State == CommunicationState.Opened;
        }

        private static bool TryOpenConnection(bool showMessage = true)
        {
            if (IsConnectionOpen())
                return true;

            try
            {
                Proxy.Open();
                Proxy.Register(_guid);
                return true;
            }
            catch (EndpointNotFoundException)
            {
                if (showMessage)
                    EndpointNotFound();

                return false;
            }
        }

        public static void EndpointNotFound()
        {
            MessageBox.Show(Strings.ServiceNotFoundMessage, Strings.ServiceNotFoundTitle, MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}