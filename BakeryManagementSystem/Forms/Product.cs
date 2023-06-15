using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BakeryManagementSystem
{
    public partial class Product : Form
    {
        public Product()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

        private void Product_Load(object sender, EventArgs e)
        {
            this.Refresh();
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");
            conn.Open();
            string query = "select * from Product";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader row = cmd.ExecuteReader();

            cmbId.Items.Clear();
            cmbId.Items.Add("New Register");

            while (row.Read())
            {
                cmbId.Items.Add(row[0].ToString());
                
            }
            conn.Close();
            cmbId.SelectedIndex = 0;

            LoadTheme();
        }
        private void LoadTheme()
        {
            /*foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }*/
            
            lblId.ForeColor = ThemeColor.PrimaryColor;
            label1.ForeColor = ThemeColor.PrimaryColor;
            lblName.ForeColor = ThemeColor.SecondaryColor;
        }

        private void lblHeader_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbId_SelectedIndexChanged(object sender, EventArgs e)
        {
            string no = cmbId.Text;

            if (no != "New Register")
            {
                conn.Open();
                string query = "select * from Product where Id = '" + no + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader row = cmd.ExecuteReader();
                while (row.Read())
                {
                    txtName.Text = row[1].ToString();
                    textBox1.Text = row[2].ToString();

                }
                conn.Close();

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;

            }
            else
            {
                cmbId.Text = "";
                txtName.Text = "";
                textBox1.Text = "";

                btnAdd.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            String name = txtName.Text;
            String price = textBox1.Text;

            string query = "insert into Product values('" + name + "','" + price + "')";
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Record added Successfully!", "Registered Product!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                string msg = "Insert Error";
                msg += ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string no = cmbId.Text;

            if (no != "New Register")
            {

                string Name = txtName.Text;
                string price = textBox1.Text;
                

                string query = "Update Product set Name='" + Name + "',Price='" + price + "' where Id = '" + no + "'";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Record updated Successfully!", "updated Product!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Refresh();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure, Do you really want to delete this record?", "Delete Product!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string no = cmbId.Text;

                string query = "delete from Product where Id = '" + no + "'";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Record Deleted Successfully!", "Deleted Product!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Refresh();
            }
            else if (result == DialogResult.No)
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

            string Query = "SELECT * FROM Product";

            SqlDataAdapter adapter = new SqlDataAdapter(Query, conn);
            DataSet ds = new DataSet();

            adapter.Fill(ds, "Product");
            dataGridView1.DataSource = ds.Tables["Product"];
        }
    }
}
