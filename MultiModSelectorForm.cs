using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MultiModSelectorForm : Form
    {
        public string SelectedMod { get; private set; }

        public MultiModSelectorForm(IEnumerable<string> modNames)
        {
            Text = "Choose which variant to activate";
            Width = 300;
            Height = 200;
            StartPosition = FormStartPosition.CenterParent;

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            var radioButtons = new List<RadioButton>();
            foreach (var name in modNames)
            {
                var rb = new RadioButton { Text = name, AutoSize = true };
                panel.Controls.Add(rb);
                radioButtons.Add(rb);
            }
            if (radioButtons.Any())
                radioButtons[0].Checked = true;

            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 80 };
            btnOk.Click += (s, e) =>
            {
                var chosen = radioButtons.FirstOrDefault(r => r.Checked);
                SelectedMod = chosen?.Text;
            };

            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 80 };

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 40,
                Padding = new Padding(10)
            };
            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);

            Controls.Add(panel);
            Controls.Add(btnPanel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}