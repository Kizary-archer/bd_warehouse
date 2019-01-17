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
        string queryString,delstr;
        

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

             dataGridView1.Columns[0].Visible = false;

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
                OleDbCommand sqlDelete = new OleDbCommand();
                sqlDelete.CommandText = "" + delstr + " " + IDInt + ")";
                sqlDelete.Connection = database;
                sqlDelete.ExecuteNonQuery();
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
            if (tabControl1.SelectedIndex == 1)
            {
                try
                {

                    OleDbDataAdapter dataAdapter = null;
                    OleDbCommand sqlInsert = new OleDbCommand();
                    sqlInsert.CommandText = "SELECT clients.id_client,clients.name_client, clients.surname_client, clients.patronymic_client FROM clients";
                    sqlInsert.Connection = database;
                    DataTable data = new DataTable();
                    dataAdapter = new OleDbDataAdapter(sqlInsert);
                    dataAdapter.Fill(data);
                    comboBox1.DataSource = data;
                    comboBox1.DisplayMember = "name_client";
                    comboBox1.ValueMember = "id_client";
                    ////////клиенты
                    DataTable data2 = new DataTable();
                    sqlInsert.CommandText = "SELECT name_tariffs,id_tariffs FROM tariffs";
                    dataAdapter = new OleDbDataAdapter(sqlInsert);
                    dataAdapter.Fill(data2);
                    comboBox2.DataSource = data2;
                    comboBox2.DisplayMember = "name_tariffs";
                    comboBox2.ValueMember = "id_tariffs";




                }
                catch (Exception ex) { MessageBox.Show("¬ведены некорректные данные"); }
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            table = "clients";
            delstr = "DELETE passport.*, clients.Id_passport FROM passport INNER JOIN clients ON passport.id_passport = clients.Id_passport WHERE((clients.Id_passport) =";
            queryString = "SELECT clients.Id_passport,clients.name_client, clients.surname_client, clients.patronymic_client, clients.phone,passport.Date_issues, passport.Date_of_birth, passport.issued_by FROM passport INNER JOIN clients ON passport.id_passport = clients.Id_passport";
            loadDataGrid(queryString);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            table = "contracts";
            delstr = "DELETE contracts.* FROM contracts WHERE((id_contracts) =";
            queryString = "SELECT contracts.id_contracts,clients.name_client, clients.surname_client,clients.patronymic_client, clients.phone, tariffs.name_tariffs,status_contracts.status, contracts.date_of_conclusion FROM status_contracts INNER JOIN(tariffs INNER JOIN (clients INNER JOIN contracts ON clients.id_client = contracts.id_client) ON tariffs.id_tariffs = contracts.id_tariffs) ON status_contracts.id_status = contracts.status GROUP BY contracts.id_contracts,clients.name_client, clients.surname_client, clients.patronymic_client, clients.phone, tariffs.name_tariffs, status_contracts.status, contracts.date_of_conclusion";
            loadDataGrid(queryString);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OleDbCommand sqlInsert = new OleDbCommand();
                sqlInsert.CommandText = "INSERT INTO passport (Date_issues, Date_of_birth, issued_by)VALUES ('" + Convert.ToString(dateTimePicker2.Text) + "', '" + Convert.ToString(dateTimePicker3.Text) + "', '" + textBox7.Text + "')";
                sqlInsert.Connection = database;
                sqlInsert.ExecuteNonQuery();
                sqlInsert.CommandText = "SELECT MAX(id_passport) FROM passport";
                string Maxid = Convert.ToString(sqlInsert.ExecuteScalar());
                sqlInsert.CommandText = "INSERT INTO clients (name_client,surname_client,patronymic_client,phone,Id_passport)VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "', '" + Maxid + "')";
                sqlInsert.ExecuteNonQuery();
                MessageBox.Show(Maxid);
            }
            catch (Exception ex) { MessageBox.Show("¬ведены некорректные данные"); }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OleDbCommand sqlInsert = new OleDbCommand();
                sqlInsert.CommandText = "INSERT INTO contracts (id_client,id_tariffs,date_of_conclusion,status)VALUES ('" + Convert.ToString(comboBox1.SelectedValue) + "', '" + Convert.ToString(comboBox2.SelectedValue) + "','" + Convert.ToString(dateTimePicker1.Text) + "','1')";
                sqlInsert.Connection = database;
                sqlInsert.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show("¬ведены некорректные данные"); }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            table = "product";
            delstr = "DELETE product.* FROM product WHERE((id_product) =";
            queryString = "SELECT product.id_product,product.name_product, product.number_product, storage_cells.id_cell, clients.name_client, clients.surname_client, clients.patronymic_client, clients.phone FROM(clients INNER JOIN product ON clients.id_client = product.id_client) INNER JOIN storage_cells ON product.id_product = storage_cells.id_product";
            loadDataGrid(queryString);
        }
    }
}