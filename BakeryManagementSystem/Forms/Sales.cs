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

namespace BakeryManagementSystem.Forms
{
    public partial class Sales : Form
    {
        public Sales()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

            string Query = "SELECT * FROM Orders";

            SqlDataAdapter adapter = new SqlDataAdapter(Query, conn);
            DataSet ds = new DataSet();

            adapter.Fill(ds, "Orders");
            dataGridView1.DataSource = ds.Tables["Orders"];
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtpStart.Value;
            DateTime endDate = dtpEnd.Value;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime calculationDate = dateTimePicker1.Value;  // Specify the desired calculation date

            using (SqlConnection connection = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234"))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("CalculateTotalAmountByDate", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CalculationDate", calculationDate);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime orderDate = reader.GetDateTime(0);
                    decimal totalAmount = reader.GetDecimal(1);

                    string amount = totalAmount.ToString();
                    textBox1.Text = amount;
                    
                }

                connection.Close();
            }
        }
    }
}
