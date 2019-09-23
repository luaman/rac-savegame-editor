using System;
using System.Collections.Generic;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace RACSaveGameEditor
{
    public class SaveGamePage : Frame
    {
        [UI] private ListBox savegameListBox;

        private SaveGameContainer container;
        private byte[] fileData;
        private Dictionary<string, List<SaveGameItem>> saveGameData;
        
        public SaveGamePage(SaveGameContainer container) : this(container, new Builder("MainWindow.glade"))
        {
        }

        private SaveGamePage(SaveGameContainer container, Builder builder) : base(builder.GetObject("savegameFrame").Handle)
        {
            builder.Autoconnect(this);
            this.container = container;
            fileData = container.Load();
            switch (container.type) {
                case GameType.RAC:
                    saveGameData = GameItems.RacOneItems;
                    break;
                case GameType.GC:
                    saveGameData = GameItems.RacTwoItems;
                    break;
                case GameType.UYA:
                    saveGameData = GameItems.RacThreeItems;
                    break;
                case GameType.QFB:
                    saveGameData = GameItems.RacQuestBooty;
                    break;
                default:
                    saveGameData = new Dictionary<string, List<SaveGameItem>>();
                    break;
            }

            LoadValues();
            ConstructWidgets();

            ShowAll();
        }

        private void LoadValues() {
            foreach (var section in saveGameData) {
                foreach (var item in section.Value) {
                    item.ReadValue(fileData);
                }
            }
        }

        private void ConstructWidgets() {
            var headRow = new ListBoxRow();
            var gameNameLabel = new Label(); 
            gameNameLabel.Markup = String.Format("<b><u>{0}</u></b>", GameTypeFormatter.ToString(container.type));
            gameNameLabel.UseMarkup = true;
            headRow.Add(gameNameLabel);
            savegameListBox.Add(headRow);

            foreach (var section in saveGameData) {
                var row = new ListBoxRow();
                var headingLabel = new Label(); 
                headingLabel.Markup = String.Format("<b>{0}</b>", section.Key);
                headingLabel.UseMarkup = true;
                row.Add(headingLabel);
                savegameListBox.Add(row);

                foreach (var item in section.Value) {
                    var itemRow = new ListBoxRow();
                    var itemLayout = new Box(Orientation.Horizontal, 50);
                    itemLayout.CenterWidget = new Separator(Orientation.Vertical);
                    itemRow.Add(itemLayout);
                    var itemLabel = new Label(item.name);
                    itemLayout.PackStart(itemLabel, true, true, 20);
                    var itemWidget = item.CreateEditWidget();
                    itemLayout.PackEnd(itemWidget, true, true, 50);
                    savegameListBox.Add(itemRow);
                }
            }
        }

        public void ExportData(String filename)
        {
            foreach (var section in saveGameData) {
                foreach (var item in section.Value) {
                    item.WriteValue(ref fileData);
                }
            }

            // container.Save(fileData); // TODO: enable this when ready
        }
    }
}