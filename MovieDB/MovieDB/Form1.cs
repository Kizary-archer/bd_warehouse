using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb; // <- for database methods

namespace MovieDB
{
    public partial class Form1 : Form
    {
        public OleDbConnection database;
        DataGridViewButtonColumn editButton;
        DataGridViewButtonColumn deleteButton;
        int IDInt;
        string table;

        #region Form1 constructor
        public Form1()
        {

            InitializeComponent();
            // iniciate DB connection
            string connectionString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=Warehouse.mdb";
            try
            {
                database = new OleDbConnection(connectionString);
                database.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        #endregion

        #region Load dataGrid
        public void loadDataGrid(string sqlQueryString) {

            OleDbCommand SQLQuery = new OleDbCommand();
            DataTable data = null;
            dataGridView1.DataSource = null;
            OleDbDataAdapter dataAdapter = null;
            dataGridView1.Columns.Clear(); // <-- clear columns
            //---------------------------------
            SQLQuery.CommandText = sqlQueryString;
            SQLQuery.Connection = database;
            data = new DataTable();
            dataAdapter = new OleDbDataAdapter(SQLQuery);
            dataAdapter.Fill(data);
            dataGridView1.DataSource = data;
            dataGridView1.AllowUserToAddRows = false; // remove the null line
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["id_client"].Visible = false;
            dataGridView1.Columns["id_passport"].Visible = false;
            // insert edit button into datagridview
            editButton = new DataGridViewButtonColumn();
            editButton.HeaderText = "Edit";
            editButton.Text = "Edit";
            editButton.UseColumnTextForButtonValue = true;
            editButton.Width = 80;
            dataGridView1.Columns.Add(editButton);
            // insert delete button to datagridview
            deleteButton = new DataGridViewButtonColumn();
            deleteButton.HeaderText = "Delete";
            deleteButton.Text = "Delete";
            deleteButton.UseColumnTextForButtonValue = true;
            deleteButton.Width = 80;
            dataGridView1.Columns.Add(deleteButton);
        }
        #endregion
        #region Delete/Edit button handling
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentRow = int.Parse(e.RowIndex.ToString());
            try
            {
                string IDString = dataGridView1[0, currentRow].Value.ToString();
                IDInt = int.Parse(IDString);
            }
            catch (Exception ex) { MessageBox.Show("error"); }
            // edit button
            if (dataGridView1.Columns[e.ColumnIndex] == editButton && currentRow >= 0)
            {
                dataGridView1.Update();
            }
            // delete button
            else if (dataGridView1.Columns[e.ColumnIndex] == deleteButton && currentRow >= 0)
            {
                // delete sql query
                string queryDeleteString = "DELETE FROM clients where id_client = " + IDInt + "";
                OleDbCommand sqlDelete = new OleDbCommand();
                sqlDelete.CommandText = queryDeleteString;
                sqlDelete.Connection = database;
                sqlDelete.ExecuteNonQuery();
                string queryString = "SELECT * FROM " + table + "";
                loadDataGrid(queryString);
            }

        }
        #endregion

        private void izlazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        #region Close database connection
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            database.Close();
        }
        #endregion


         
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            table = "clients";
            string queryString = "SELECT * FROM " + table + "";
            loadDataGrid(queryString);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}