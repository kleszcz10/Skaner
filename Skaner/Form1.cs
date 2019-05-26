using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using Xceed.Words.NET;

namespace Skaner
{
    public partial class Form1 : Form
    {
        public SpeechSynthesizer Synth = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
            Synth.SetOutputToDefaultAudioDevice();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileStream = openFileDialog1.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var row = line.Split(';');
                        row[1] = row[1].Trim();
                        InputList.Items.Add(new ListViewItem(row));
                    }
                }
                inputCounter.Text = "Wprowadzono: " + InputList.Items.Count;
            }
            
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            var selectedColumn = this.comboBox1.SelectedIndex + 1;
            if (e.KeyCode == Keys.Enter)
            {
                if (InputList.Items.Count == 0 && CheckedList.Items.Count == 0)
                {
                    MessageBox.Show("Lista przedmiotów jest pusta. Proszę wczytać plik z danymi.");
                    label1.Text = "####";
                }
                else
                {
                    var foundItem = InputList.Items.Cast<ListViewItem>().Where(x => x.SubItems[selectedColumn].Text == textBox1.Text).FirstOrDefault();
                    if(foundItem != null)
                    {
                        label1.Text = foundItem.SubItems[0].Text;
                        CheckedList.Items.Add((ListViewItem)foundItem.Clone());
                        InputList.Items.Remove(foundItem);
                        checkCounter.Text = "Odczytano: " + CheckedList.Items.Count;
                        Synth.SpeakAsync("Numer " + label1.Text);
                        if (InputList.Items.Count == 0)
                        {
                            Synth.SpeakAsync("Koniec listy");
                        }
                    }
                    else
                    {
                        foundItem = CheckedList.Items.Cast<ListViewItem>().Where(x => x.SubItems[selectedColumn].Text == textBox1.Text).FirstOrDefault();
                        if(foundItem != null)
                        {
                            label1.Text = foundItem.SubItems[0].Text;
                            Synth.SpeakAsync("Duplikat, numer " + label1.Text);
                        }
                        else if (InputList.Items.Count == 0)
                        {
                            Synth.SpeakAsync("Koniec listy");
                        }
                        else
                        {
                            label1.Text = "Brak";
                            Synth.SpeakAsync("Brak pozycji w bazie");
                        }
                    }
                    textBox1.Text = "";
                    this.ActiveControl = textBox1;
                } 
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(CheckedList.Items.Count < 1)
            {
                MessageBox.Show("Brak pozycji do zapisania", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (DocX document = DocX.Load(@".\Template\template.docx"))
            {
                var table = document.Tables.First();
                var rowPattern = table.Rows[1];
                foreach(ListViewItem item in CheckedList.Items)
                {
                    var items = item.SubItems;
                    AddItemToTable(table, rowPattern, new string[] { $"{items[0].Text ?? items[0].Text : ''}", "", $"{items[1].Text ?? items[1].Text: ''}", $"{items[2].Text ?? items[2].Text: ''}" });
                }
                rowPattern.Remove();
                this.saveFileDialog1.FileName = "formularz.docx";
                this.saveFileDialog1.Filter = "Dokument Word(*.docx)| *.docx";
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    document.SaveAs(this.saveFileDialog1.FileName);
                    var removeItemsFromTable = MessageBox.Show("Czy chesz wyczyścić zawartość tabeli?", "Czy wyczyścić?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(removeItemsFromTable == DialogResult.Yes)
                    {
                        CheckedList.Items.Clear();
                    }
                }
                
            }
        }

        private static void AddItemToTable(Table table, Row rowPattern, string[] rowData)
        {
            // Insert a copy of the rowPattern at the last index in the table.
            var newItem = table.InsertRow(rowPattern, table.RowCount - 1);

            // Replace the default values of the newly inserted row.
            newItem.ReplaceText("%NUMBER%", rowData[0]);
            newItem.ReplaceText("%TYPE%", rowData[1]);
            newItem.ReplaceText("%SERIAL_NUMBERS%", rowData[2]);
            newItem.ReplaceText("%INVENTORY_NUMBERS%", rowData[3]);
        }

        private void ClearListBtn_Click(object sender, EventArgs e)
        {
            var clearCheckedList = MessageBox.Show("Czy chcesz wyczyścić listę?", "Jesteś pewien?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(clearCheckedList == DialogResult.Yes)
            {
                CheckedList.Items.Clear();
            }
        }
    }
}
