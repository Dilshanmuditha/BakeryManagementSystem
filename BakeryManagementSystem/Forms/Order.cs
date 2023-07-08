using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BakeryManagementSystem.Forms
{
    public partial class Order : Form
    {
        private OrderItem OrderItem;
        public Order()
        {
            InitializeComponent();
            OrderItem = new OrderItem();
        }
        private List<OrderItem> orders = new List<OrderItem>();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Retrieve values from input fields and dropdowns
            string customerName = cmbCName.SelectedItem.ToString();
            string itemName = cmbItem.SelectedItem.ToString();
            int quantity = int.Parse(numericUpDown1.Text);
            decimal itemPrice = decimal.Parse(txtPrice.Text);


                decimal totalPrice;

                SqlConnection connection = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");

                {
                    SqlCommand command = new SqlCommand("CalculateTotalPrice", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@ItemPrice", itemPrice);

                    SqlParameter totalPriceParam = new SqlParameter("@TotalPrice", SqlDbType.Decimal);
                    totalPriceParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(totalPriceParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    totalPrice = (decimal)totalPriceParam.Value;
                }

                string price = totalPrice.ToString();
               
        
                // Retrieve the CustomerID and ItemID based on the selected names
                int customerID;
                int itemID;
             {

                // Retrieve the CustomerID
                string customerQuery = "SELECT Id FROM CustomerDetails WHERE Name = @CustomerName";
                SqlCommand customerCommand = new SqlCommand(customerQuery, connection);
                customerCommand.Parameters.AddWithValue("@CustomerName", customerName);
                customerID = (int)customerCommand.ExecuteScalar();

                // Retrieve the ItemID
                string itemQuery = "SELECT Id FROM Product WHERE Name = @ItemName";
                SqlCommand itemCommand = new SqlCommand(itemQuery, connection);
                itemCommand.Parameters.AddWithValue("@ItemName", itemName);
                itemID = (int)itemCommand.ExecuteScalar();
             }

            // Create an Order object and add it to the list
            OrderItem order = new OrderItem
            {
                CustomerID = customerID,
                ItemID = itemID,
                Quantity = quantity,
                Price = price,
            };
            orders.Add(order);

            // Display the order in a list or update a display area on the form
            listBox1.Items.Add(order);
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            int customerID = OrderItem.CustomerID;

            decimal totalPrice = CalculateTotalOrderPrice(customerID);
            SqlConnection connection = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");
            connection.Open();
            foreach (OrderItem order in orders)
            {
                
                

                {
                    string query = "INSERT INTO Orders (CustomerID,TotalAmount, ItemID, Quantity) VALUES (@CustomerID,@TotalAmount, @ItemID, @Quantity)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                    command.Parameters.AddWithValue("@ItemID", order.ItemID);
                    command.Parameters.AddWithValue("@Quantity", order.Quantity);
                    command.Parameters.AddWithValue("@TotalAmount", totalPrice);

                    command.ExecuteNonQuery();


                }

            }
            connection.Close();

            MessageBox.Show("Record added Successfully!", "Order Added...!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            orders.Clear();
            listBox1.Items.Clear();
        }

        private void Order_Load(object sender, EventArgs e)
        {
            this.Refresh();
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");
            conn.Open();

            string query1 = "SELECT * FROM CustomerDetails";
            SqlCommand cmd1 = new SqlCommand(query1, conn);
            SqlDataReader row1 = cmd1.ExecuteReader();

            cmbCName.Items.Clear();

            while (row1.Read())
            {
                cmbCName.Items.Add(row1.GetString(1)); 
            }

            row1.Close(); 

            string query2 = "SELECT * FROM Product";
            SqlCommand cmd2 = new SqlCommand(query2, conn);
            SqlDataReader row2 = cmd2.ExecuteReader();

            cmbItem.Items.Clear();

            while (row2.Read())
            {
                cmbItem.Items.Add(row2.GetString(1));
            }

            row2.Close();

            conn.Close();

            cmbItem.SelectedIndex = 0;
            cmbCName.SelectedIndex = 0;

            //LoadTheme();
        }

        private decimal CalculateTotalOrderPrice(int customerID)
        {
            decimal totalPrice = 0;

            foreach (OrderItem order in orders)
            {
                   
                    decimal orderPrice = decimal.Parse(order.Price);

                    totalPrice += orderPrice;
                
            }

            return totalPrice;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int customerID = OrderItem.CustomerID;

            decimal totalPrice = CalculateTotalOrderPrice(customerID);

            textBox1.Text = totalPrice.ToString();
           
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");
            conn.Open();
            string no = cmbItem.Text;
            
            string query = "select * from Product where Name = '" + no + "'";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader row = cmd.ExecuteReader();

            while (row.Read())
            {
                String itemName = (String)row["Name"];
                txtPrice.Text = row[2].ToString();
                int availableQuantity = GetAvailableQuantity(itemName);
                label2.Text = availableQuantity.ToString();
            }



            conn.Close();

        }

       
        public int GetAvailableQuantity(String itemName)
        {
            int availableQuantity = 0;

            SqlConnection connection = new SqlConnection(@"Data Source=DILSHAN-ROG\MSSQLSERVER01;Initial Catalog=BakeryManagementSystem;User ID=root1;Password=1234");
            {
             connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT dbo.GetAvailableQuantity(@itemName)", connection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);
                    availableQuantity = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return availableQuantity;
        }

        private void cmbCName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
