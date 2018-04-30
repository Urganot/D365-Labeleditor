//------------------------------------------------------------------------------
// <copyright file="AvanadeLabelEditorExtension.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;
using AutoComment2015.Helper;
using Avanade_Label_Editor_Extension.LabelEditorServices;
using EnvDTE;
using EnvDTE80;

namespace Avanade_Label_Editor_Extension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AvanadeLabelEditorExtension
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f943980a-5f69-490d-9815-cdb353dcbf38");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvanadeLabelEditorExtension"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AvanadeLabelEditorExtension(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this._package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AvanadeLabelEditorExtension Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return this._package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AvanadeLabelEditorExtension(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {

                var binding = new BasicHttpBinding
                {
                };
                var endpoint = new EndpointAddress("http://localhost:2112/com");
                var proxy = new LabelEditorServiceClient(binding, endpoint);


                var dte = (DTE)ServiceProvider.GetService(typeof(DTE));
                var activeDocument = dte.ActiveDocument;

                if (activeDocument != null)
                {
                    TextSelection selection = (TextSelection)dte.ActiveDocument.Selection;
                    string text = selection.Text;
                    if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                    {
                        MessageBox.Show("Please Select Label");
                    }
                    else
                    {
                        var dte2 = (DTE2)ServiceProvider.GetService(typeof(DTE));

                        var helper = new Helper(_package, dte2);

                        var addedLabels = proxy.CreateNewLabel(text);

                        string result;
                        if (addedLabels.ContainsKey("base"))
                        {
                            result = addedLabels["base"].First().FullId;
                        }
                        else
                        {
                            result = "asdas";
                        }

                        helper.InsertText(Helper.GetCurrentTextView(), result);
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
