using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;
using AVA_LabelEditor.Properties;
using GoogleTranslate;
using AddLabel = AVA_LabelEditor.AddLabel;

namespace AVA_LabelEditor.Lists
{
    public class Labels : ObservableList
    {
        /// <summary>
        /// List of all Labels
        /// </summary>
        public ObservableCollection<Label> All = new ObservableCollection<Label>();

        /// <summary>
        /// The selected <see cref="Label"/>
        /// </summary>
        public Label Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Selected labels backing field
        /// </summary>
        private Label _selected;

        /// <summary>
        /// Constructor for the Labels class
        /// </summary>
        /// <param name="mainWindow">An instance of the MainWindow class</param>
        public Labels(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Deletes a<see cref="Label"/> by flagging it
        /// </summary>
        public void Delete()
        {

            if (!ValidateDelete())
                return;

            All.ToList()
                .Where(x => Equals(x.FileId, Selected.FileId) && x.Id == Selected.Id).ToList()
                .ForEach(x => x.Deleted = true);

            ((ICollectionView)MainWindow.MainGrid.ItemsSource).Refresh();
        }

        /// <summary>
        /// Translates a <see cref="Label"/>
        /// </summary>
        public void Translate()
        {
            if (!ValidateTranslate())
                return;

            All.ToList()
                .Where(x => x.Id == Selected.Id && !Equals(x.Language, Selected.Language) && Equals(x.FileId, Selected.FileId)).ToList()
                .ForEach(y => y.Text = Helper.Helper.FixTranslatedText(Translation.Translate(Selected.Text, Selected.Language.ToString(), y.Language.ToString())));
        }

        /// <summary>
        /// Validation before translating
        /// </summary>
        /// <returns>True if <see cref="Label"/> can be translated</returns>
        private bool ValidateTranslate()
        {
            var ok = true;

            if (Selected == null)
                ok = Helper.Helper.CheckFailed(General.NoLabelSelectedMessage, General.NoLabelSelectedTitle);

            return ok;
        }

        /// <summary>
        /// Saves the Labels
        /// </summary>
        /// <param name="e">Determines weither the normal save function is called or the window is closing</param>
        public void Save(EventArgs e = null)
        {
            if (!ValidateSave())
                return;

            MainWindow.Timer.Stop();

            foreach (var readFile in MainWindow.ReadFilesNew.All)
            {
                WriteFile(readFile);

                readFile.Reset();
            }

            MainWindow.Changed = false;
            MainWindow.Timer.Start();
        }

        /// <summary>
        /// Validation before saving labels
        /// </summary>
        /// <param name="e">Determines weither the normal save function is called or the window is closing</param>
        /// <returns>True if Labels can be saved</returns>
        private bool ValidateSave(EventArgs e = null)
        {
            var ok = true;

            if (MainWindow.Models.Selected.Locked)
                ok = Helper.Helper.CheckFailed(General.Validate_Reload_Locked_Message, General.Validate_Reload_Locked_Title);

            if (MainWindow.ReadFilesNew.All.Any(x => x.Changed))
            {
                ok = Helper.Helper.CheckFailed(General.FileChangedCantSaveMessage, General.FileChangedCantSaveTitle);
            }

            return ok;
        }
        /// <summary>
        /// Writes the file to Disc
        /// </summary>
        /// <param name="readFile">The file to write</param>
        /// //Chaitanya Narasimha: date: 25-Sept-2018: Added try catch block arround the Write File method below to handle access issues.
        private void WriteFile(LabelFile readFile)
        {
            try
            {
                using (var writer = new StreamWriter(readFile.Path))
                {
                    foreach (var label in All.Where(x => Equals(x.Language, readFile.Language) && Equals(x.FileId, readFile.FileId) && !x.Deleted))
                    {
                        writer.WriteLine(label.Id + "=" + label.Text);
                        if (!string.IsNullOrWhiteSpace(label.Comment))
                            writer.WriteLine(" " + readFile.CommentSymbol + label.Comment);
                    }
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show("You may not have enough prevliges to the file in the selected model, please provide the preveliges:"+ex.Message);
            }
        }

        /// <summary>
        /// Adds a new Label
        /// </summary>
        /// <param name="id">The <see cref="Label"/>s id</param>
        /// <param name="text">The <see cref="Label"/>s text</param>
        /// <param name="language">The <see cref="Label"/>s <see cref="Language"/></param>
        /// <returns></returns>
        public Label AddNewLabel(string id, string text, Language language)
        {
            if (Settings.Default.AutoTranslate && !Equals(language, MainWindow.Languages.Selected))
                text = Helper.Helper.FixTranslatedText(Translation.Translate(text, MainWindow.Languages.Selected.ToString(), language.ToString()));

            var label = new Label(MainWindow)
            {
                FileId = MainWindow.FileIds.Selected,
                Id = id,
                Language = language,
                Text = text
            };
            Add(label);

            if (Equals(MainWindow.Languages.Selected, label.Language))
            {
                Selected = label;
                MainWindow.MoveToSelectedItem();
            }

            return label;
        }

        /// <summary>
        /// Adds the given <see cref="Label"/> to the List
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to add</param>
        public void Add(Label label)
        {
            All.Add(label);
        }


        /// <summary>
        /// Checks if a Label id already exists
        /// </summary>
        /// <param name="id">Id to check</param>
        /// <returns>True if Id does already exist</returns>
        public bool IdExists(string id)
        {
            return All.Any(x => x.Id == id);
        }

        /// <summary>
        /// Validates data before deleting
        /// </summary>
        /// <returns>True if label can be deleted</returns>
        private bool ValidateDelete()
        {
            var ok = true;

            if (Selected == null)
                ok = Helper.Helper.CheckFailed(General.NoLabelSelectedMessage, General.NoLabelSelectedTitle);

            var result = MessageBox.Show(string.Format(General.DeleteLabelConfirmMessage, Selected.Id), General.DeleteLabelConfirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            ok = ok && result == MessageBoxResult.Yes;

            return ok;
        }



        /// <summary>
        /// Returns the view object to use in the UI
        /// </summary>
        /// <returns>The <see cref="Labels"/> <see cref="ICollectionView"/> object</returns>
        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        /// <summary>
        /// Creates new labels
        /// </summary>
        /// <param name="dialog">An instance of the AddLabel class</param>
        /// <returns>List of the inserted Labels</returns>
        public Dictionary<string, List<Label>> AddLabel(AddLabel dialog)
        {
            var id = dialog.Id.Text;
            var text = dialog.Text.Text;
            var helpText = dialog.HelpText.Text;
            var viewText = dialog.ViewText.Text;
            var maintainText = dialog.MaintainText.Text;

            var addedLabels = new Dictionary<string, List<Label>>();

            foreach (var language in MainWindow.Languages.All)
            {
                if (addedLabels.ContainsKey("base"))
                    addedLabels["base"].Add(AddNewLabel(id, text, language));
                else
                    addedLabels.Add("base", new List<Label> { AddNewLabel(id, text, language) });

                if (!string.IsNullOrWhiteSpace(helpText))
                {
                    if (addedLabels.ContainsKey("help"))
                        addedLabels["help"].Add(AddNewLabel(id + "Help", helpText, language));
                    else
                        addedLabels.Add("help", new List<Label> { AddNewLabel(id + "Help", helpText, language) });
                }

                if (!string.IsNullOrWhiteSpace(viewText))
                {
                    if (addedLabels.ContainsKey("view"))
                        addedLabels["view"].Add(AddNewLabel(id + "View", viewText, language));
                    else
                        addedLabels.Add("view", new List<Label> { AddNewLabel(id + "View", viewText, language) });
                }

                if (!string.IsNullOrWhiteSpace(maintainText))
                {
                    if (addedLabels.ContainsKey("maintain"))
                        addedLabels["maintain"].Add(AddNewLabel(id + "Maintain", maintainText, language));
                    else
                        addedLabels.Add("maintain", new List<Label> { AddNewLabel(id + "Maintain", maintainText, language) });
                }
            }

            return addedLabels;
        }

        /// <summary>
        /// Renames a LabelId
        /// </summary>
        /// <param name="newId">The new Id</param>
        public void Rename(string newId)
        {
            if (!ValidateRename())
                return;

            All.ToList()
                .Where(x => x.FileId.Equals(Selected.FileId) && x.Id == Selected.Id).ToList()
                .ForEach(y => y.Id = newId);
        }

        /// <summary>
        /// Validateing before renaming a label id
        /// </summary>
        /// <returns>True if LabelId can be renamed</returns>
        public bool ValidateRename()
        {
            var ok = true;

            if (Selected == null)
                ok = Helper.Helper.CheckFailed(General.NoLabelSelectedMessage, General.NoLabelSelectedTitle);


            return ok;
        }

        /// <summary>
        /// Clears the Labels data
        /// </summary>
        public void Clear()
        {
            All.Clear();
        }
    }
}
