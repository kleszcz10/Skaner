﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skaner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                        InputList.Items.Add(new ListViewItem(row));
                    }
                }
                inputCounter.Text = "Wprowadzono: " + InputList.Items.Count;
            }
            
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (InputList.Items.Count == 0)
                {
                    MessageBox.Show("Lista przedmiotów jest pusta. Proszę wczytać plik z danymi.");
                    label1.Text = "####";
                }
                else
                {
                    var foundItem = InputList.Items.Cast<ListViewItem>().Where(x => x.SubItems[1].Text == textBox1.Text).FirstOrDefault();
                    if(foundItem != null)
                    {
                        label1.Text = foundItem.SubItems[0].Text;
                        CheckedList.Items.Add((ListViewItem)foundItem.Clone());
                        InputList.Items.Remove(foundItem);
                        checkCounter.Text = "Odczytano: " + CheckedList.Items.Count;
                    }
                    else
                    {
                        foundItem = CheckedList.Items.Cast<ListViewItem>().Where(x => x.SubItems[1].Text == textBox1.Text).FirstOrDefault();
                        if(foundItem != null)
                        {
                            label1.Text = foundItem.SubItems[0].Text;
                        }
                        else
                        {
                            label1.Text = "Brak";
                        }
                    }
                    textBox1.Text = "";
                    this.ActiveControl = textBox1;
                } 
            }
        }
    }
}
