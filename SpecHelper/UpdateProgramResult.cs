using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpecHelper
{
    public partial class UpdateProgramResult : Form
    {

        public UpdateProgramResult()
        {
            InitializeComponent();
            middleSp.Click += new System.EventHandler(this.localized_Click);
            noChangeList.Click += new System.EventHandler(this.localized_Click);
            failList.Click += new System.EventHandler(this.localized_Click);
            successList.Click += new System.EventHandler(this.localized_Click);
        }

        public void AddToList(string item, Status type)
        {
            switch (type)
            {
                case Status.Success:
                    successList.Items.Add(item);
                    break;
                case Status.Fail:
                    failList.Items.Add(item);
                    break;
                case Status.MiddleSP:
                    middleSp.Items.Add(item);
                    break;
                case Status.Localized:
                    localized.Items.Add(item);
                    break;
                default:
                    noChangeList.Items.Add(item);
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ImportExportExcelHelper.Instance.ExportProgramUpdateResultToExcel(Application.StartupPath, successList.Items, failList.Items, noChangeList.Items, middleSp.Items, localized.Items);
        }

        private void localized_Click(object sender, EventArgs e)
        {
            var listItem = sender as ListBox;
            try
            {
                Clipboard.SetText(listItem.SelectedItem.ToString());
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}
