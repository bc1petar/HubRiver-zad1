using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace phoneBook1
{
    public partial class phoneBookApp : Form
    {

        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;Initial Catalog='phonebookdb';username=root;password=");
        MySqlCommand command;
        string id;

        public phoneBookApp()
        {
            InitializeComponent();   
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(inputFirstName.Text.Trim() != "" && inputLastName.Text.Trim() != "" && inputPhoneNumber.Text.Trim() != "")
            {
                Match match = Regex.Match(inputPhoneNumber.Text, @"\(\d{3}\)-\d{8}");
                if (match.Success)
                {
                    string insertQuery = "INSERT INTO phonebook(firstName, lastName, phoneNumber) VALUES('" + inputFirstName.Text + "','" + inputLastName.Text + "','" + inputPhoneNumber.Text + "')";
                    executeMyQuery(insertQuery);
                    updateDGV();
                }
                else
                {
                    MessageBox.Show("Wrong phone number format !!!");
                }
            }
            else
            {
                MessageBox.Show("Please fill up all the fields !!!");
            }
        }

        private void phoneBookApp_Load(object sender, EventArgs e){ updateDGV(); }


        public void updateDGV()
        {
            string selectQuery = "SELECT * FROM phonebook";
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, connection);
            adapter.Fill(table);
            inputFirstName.Text = "";
            inputLastName.Text = "";
            inputPhoneNumber.Text = "";
            searchData("");
            dgvPhoneBookList.DataSource = table;
        }

        private void dgvPhoneBookList_MouseClick(object sender, MouseEventArgs e)
        {
            id = dgvPhoneBookList.CurrentRow.Cells[0].Value.ToString();
            inputFirstName.Text = dgvPhoneBookList.CurrentRow.Cells[1].Value.ToString();
            inputLastName.Text = dgvPhoneBookList.CurrentRow.Cells[2].Value.ToString();
            inputPhoneNumber.Text = dgvPhoneBookList.CurrentRow.Cells[3].Value.ToString();
        }

        public void openConnection()
        {
            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void closeConnection()
        {
            if(connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public void executeMyQuery(string query)
        {
            try
            {
                openConnection();
                command = new MySqlCommand(query, connection);
                if(command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Query Executed");
                }
                else
                {
                    MessageBox.Show("Query Not Executed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                closeConnection();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (inputFirstName.Text.Trim() != "" && inputLastName.Text.Trim() != "" && inputPhoneNumber.Text.Trim() != "") 
            {
                Match match = Regex.Match(inputPhoneNumber.Text, @"\(\d{3}\)-\d{8}");
                if (match.Success) {
                    string updateQuery = "UPDATE `phonebook` SET firstName='" + inputFirstName.Text + "',lastName='" + inputLastName.Text + "',phoneNumber='" + inputPhoneNumber.Text + "' WHERE id=" + int.Parse(id);
                    executeMyQuery(updateQuery);
                    updateDGV();
                }
                else
                {
                    MessageBox.Show("Wrong phone number format !!!");
                }
            }
            else
            {
                MessageBox.Show("Please fill up all the fields !!!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string deleteQuery = "DELETE FROM phonebook WHERE id=" + int.Parse(id);
            executeMyQuery(deleteQuery);
            updateDGV();
        }

        public void searchData(string value)
        {
            string searchQuery = "SELECT * FROM phonebook WHERE CONCAT(lastName) LIKE '%" + value + "%'";
            MySqlDataAdapter adapter = new MySqlDataAdapter(searchQuery, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dgvPhoneBookList.DataSource = table;
        }

        private void inputSearch_TextChanged(object sender, EventArgs e)
        {
            searchData(inputSearch.Text);
        }

    }
}
