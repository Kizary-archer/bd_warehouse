using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace MovieDB
{
    public partial class Form2 : Form
    {
        public string year, publisher, title, previewed, type;
        public int movieID;
        public Form2()
        { 
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = title;
            textBox2.Text = publisher;
            textBox3.Text = year;
            comboBox1.Text = type;
            if (previewed == "Yes") radioButton1.Checked = true;
            else if (previewed == "No") radioButton2.Checked = true;
        }

    }
}