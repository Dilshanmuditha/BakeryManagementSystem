using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BakeryManagementSystem
{
    public partial class Customer : Form
    {
        private OrdersFilter ordersFilter;
        public Customer()
        {
            InitializeComponent();
            ordersFilter = new OrdersFilter();
        }

        SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

        private void Customer_Load(object sender, EventArgs e)
        {
            this.Refresh();
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");
            conn.Open();
            string query = "select * from customerDetails";
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
            lblAddress.ForeColor = ThemeColor.PrimaryColor;
            lblId.ForeColor = ThemeColor.PrimaryColor;
            lblPno.ForeColor = ThemeColor.PrimaryColor;
            lblName.ForeColor = ThemeColor.SecondaryColor;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            String name = txtName.Text;
            String address = txtAddress.Text;
            int mPhone = int.Parse(txtPno.Text);
            
            string query = "insert into CustomerDetails values('" + name + "','" + address + "','" + mPhone + "')";
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Record added Successfully!", "Registered Customer!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void cmbId_SelectedIndexChanged(object sender, EventArgs e)
        {
            string no = cmbId.Text;
            
            if (no != "New Register")
            {
                conn.Open();
                string query = "SELECT * FROM vw_CustomerDetails WHERE Id = '" + no + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader row = cmd.ExecuteReader();
                while (row.Read())
                {
                    txtName.Text = row[1].ToString();
                    txtAddress.Text = row[2].ToString();
                    txtPno.Text = row[3].ToString();

                }
                conn.Close();

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;

                int customerIdInt = int.Parse(no);
                GetOrderHistory(customerIdInt);
            }
            else
            {
                cmbId.Text = "";
                txtName.Text = "";
                txtAddress.Text = "";
                txtPno.Text = "";

                btnAdd.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtpStart.Value;
            DateTime endDate = dtpEnd.Value;
            int customerId = int.Parse((string)cmbId.Text);

            ShowOrderHistoryByDate(customerId,startDate,endDate);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

            string Query = "SELECT * FROM CustomerDetails";

            SqlDataAdapter adapter = new SqlDataAdapter(Query, conn);
            DataSet ds = new DataSet();

            adapter.Fill(ds, "customerDetails");
            dataGridView1.DataSource = ds.Tables["customerDetails"];
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure, Do you really want to delete this record?", "Delete Customer!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string no = cmbId.Text;

                string query = "delete from CustomerDetails where Id = '" + no + "'";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Record Deleted Successfully!", "Deleted Customer!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Refresh();
            }
            else if (result == DialogResult.No)
            {
                this.Close();
            }
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string no = cmbId.Text;

            if (no != "New Register")
            {

                string Name = txtName.Text;
                string Address = txtAddress.Text;
                int mPhone = int.Parse(txtPno.Text);

                string query = "Update CustomerDetails set Name='" + Name + "',Address='" + Address + "',Pno='" + mPhone + "' where Id = '" + no + "'";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Record updated Successfully!", "updated Customer!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Refresh();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void GetOrderHistory(int customerId)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

            {
                conn.Open();


                    string query = "SELECT * FROM OrderHistoryView WHERE CustomerId = @customerId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }

                conn.Close();
            }
        }
        private void ShowOrderHistoryByDate(int customerId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234"))
            {
                conn.Open();

                string query = "SELECT * FROM dbo.GetOrderHistoryByCustomerAndDate(@customerId, @startDate, @endDate)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }

                conn.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void lblPno_Click(object sender, EventArgs e)
        {

        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }

        private void lblId_Click(object sender, EventArgs e)
        {

        }

        private void lblAddress_Click(object sender, EventArgs e)
        {

        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPno_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void dtpEnd_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtpStart_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
