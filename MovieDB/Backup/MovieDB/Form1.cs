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
        int movieIDInt;

        #region Form1 constructor
        public Form1()
        {

            InitializeComponent();
            // iniciate DB connection
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=moviedb.mdb";
            try
            {

                database = new OleDbConnection(connectionString);
                database.Open();
                //SQL query to list movies
                string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
                loadDataGrid(queryString);
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
            SQLQuery.Connection = null;
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
            dataGridView1.Columns[1].Width = 340;
            dataGridView1.Columns[3].Width = 55;
            dataGridView1.Columns[4].Width = 50;
            dataGridView1.Columns[5].Width = 80;
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

        #region refresh button
        private void button2_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
            loadDataGrid(queryString);
        }
        #endregion

        #region Input
        private void button6_Click(object sender, EventArgs e)
        {
            string typeString;
            try
            {
                typeString = comboBox1.SelectedItem.ToString();
            }
            catch (Exception ex) {
                MessageBox.Show("You must enter movie type\nError: " + ex.Message + "");
                return;
            }
            int type = 0;
            string name = textBox1.Text.ToString();
            string publisher = textBox2.Text.ToString();
            string year = textBox3.Text.ToString();
            int yr = 0;
            if (year != "")
            {
                yr = CheckYear(year);
            }
            string previewed;
            if (radioButton1.Checked == true)
            {
                previewed = "Yes";
            }
            else
            {
                previewed = "No";
            }
            if (yr != 1)
            {
                if (typeString == "Adventure") type = 1;
                if (typeString == "Comedy") type = 2;
                if (typeString == "Action") type = 3;
                if (typeString == "Cartoon") type = 4;
                if (typeString == "Romantic") type = 5;
                if (typeString == "Fantasy") type = 6;
                if (typeString == "Thriller") type = 7;
                if (typeString == "Historic") type = 8;
                if (typeString == "Drama") type = 9;
                if (typeString == "Horor") type = 10;
                if (typeString == "Sci-Fi") type = 11;
                if (typeString == "Crime") type = 12;
                if (typeString == "Biografy") type = 13;
                if (typeString == "Documentary") type = 14;
                string SQLString ="";
     
                    if (year == "")
                    {
                        SQLString = "INSERT INTO movie(Title, Publisher, Previewed, typeID) VALUES('" + name.Replace("'", "''") + "','" + publisher + "','" + previewed + "'," + type + ");";
                    }
                    else
                    {
                        MessageBox.Show(yr.ToString());
                        SQLString = "INSERT INTO movie(Title, Publisher, Previewed, MovieYear, typeID) VALUES('" + name.Replace("'", "''") + "','" + publisher + "','" + previewed + "'," + yr + "," + type + ");";
                    }


                OleDbCommand SQLCommand = new OleDbCommand();
                SQLCommand.CommandText = SQLString;
                SQLCommand.Connection = database;
                int response = -1;
                try
                {
                    response = SQLCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (response >= 1) MessageBox.Show("Movie is added to database","Successful",MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                comboBox1.ResetText();
                radioButton1.Checked = radioButton2.Checked = false;
            }
            else
            {
                MessageBox.Show("The year format is not correct!\nPlease try to pick a valid year.", "Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Clear();
                textBox3.Focus();
            }
        }

        public int CheckYear(string year)
        {
            int yr = int.Parse(year);
            if (yr >= 2100 || yr <= 1900)
            {
                return 1;
            }
            else
            {
                return yr;
            }
        }

        #endregion

        #region Delete/Edit button handling
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";

            int currentRow = int.Parse(e.RowIndex.ToString());
            try
            {
                string movieIDString = dataGridView1[0, currentRow].Value.ToString();
                movieIDInt = int.Parse(movieIDString);
            }
            catch (Exception ex) { }
            // edit button
            if (dataGridView1.Columns[e.ColumnIndex] == editButton && currentRow >= 0)
            {
                string title = dataGridView1[1, currentRow].Value.ToString();
                string publisher = dataGridView1[2, currentRow].Value.ToString();
                string previewed = dataGridView1[3, currentRow].Value.ToString();
                string year = dataGridView1[4, currentRow].Value.ToString();
                string type = dataGridView1[5, currentRow].Value.ToString();
                //runs form 2 for editing    
                Form2 f2 = new Form2();
                f2.title = title;
                f2.publisher = publisher;
                f2.previewed = previewed;
                f2.year = year;
                f2.type  = type;
                f2.movieID = movieIDInt;
                f2.Show();
                dataGridView1.Update();
              
            }
            // delete button
            else if (dataGridView1.Columns[e.ColumnIndex] == deleteButton && currentRow >= 0)
            {
                // delete sql query
                string queryDeleteString = "DELETE FROM movie where movieID = "+movieIDInt+"";
                OleDbCommand sqlDelete = new OleDbCommand();
                sqlDelete.CommandText = queryDeleteString;
                sqlDelete.Connection = database;
                sqlDelete.ExecuteNonQuery();
                loadDataGrid(queryString);
            }
             
         }
        #endregion
         
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region search by title
        private void button1_Click(object sender, EventArgs e)
        {
            string title = textBox4.Text.ToString();
            if (title != "")
            {
                string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND movie.title LIKE '" + title + "%'";
                loadDataGrid(queryString);
            }
            else
            {
                MessageBox.Show("You muste enter movie title","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region search by type
        private void button5_Click(object sender, EventArgs e)
        {
            int type = 0;
            string typeString = comboBox2.SelectedItem.ToString();
            if (typeString == "Adventure") type = 1;
            if (typeString == "Comedy") type = 2;
            if (typeString == "Action") type = 3;
            if (typeString == "Cartoon") type = 4;
            if (typeString == "Romantic") type = 5;
            if (typeString == "Fantasy") type = 6;
            if (typeString == "Thriller") type = 7;
            if (typeString == "Historic") type = 8;
            if (typeString == "Drama") type = 9;
            if (typeString == "Horor") type = 10;
            if (typeString == "Sci-Fi") type = 11;
            if (typeString == "Crime") type = 12;
            if (typeString == "Biografy") type = 13;
            if (typeString == "Documentary") type = 14;
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND movie.typeID = " + type + "";
            loadDataGrid(queryString);
        }
        #endregion

        #region search by year
        private void button4_Click(object sender, EventArgs e)
        {
            string firstYear = textBox5.Text.ToString();
            string secondYear = textBox6.Text.ToString();;
            int yr1 = CheckYear(firstYear);
            int yr2 = CheckYear(secondYear);
            if ((yr1 != 1 && yr2 != 1) && yr1 <= yr2)
            {
                string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND movie.MovieYear BETWEEN " + yr1 + " AND " + yr2 + "";
                loadDataGrid(queryString);
            }
            else
            {
                MessageBox.Show("The year format isn't correct, pleas check again.","Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Clear();
                textBox5.Focus();
                textBox6.Clear();
            }
        }
        #endregion

        #region search previewed movies
        private void button3_Click(object sender, EventArgs e)
        {
            string previewed;
            if (radioButton3.Checked == true) previewed = "Yes";
            else previewed = "No";
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movietype WHERE movietype.typeID = movie.typeID AND Previewed ='" + previewed + "'";
            loadDataGrid(queryString);
        }
        #endregion

        private void button6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(null, null);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string queryString = "SELECT movieID, Title, Publisher, Previewed, MovieYear, Type FROM movie,movieType WHERE movietype.typeID = movie.typeID";
            loadDataGrid(queryString);
        }

    }
}