using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace testmsqlconnect
{
    public partial class DeleteCustomer : Form
    {
        public DeleteCustomer()
        {
            InitializeComponent();
        }

        public static Dictionary<string, string> customerDetails = new Dictionary<string, string>();

        public bool deleteCustomer()
        {
            MySqlConnection c = new MySqlConnection("server=wgudb.ucertify.com;user id=U06osQ;pwd= 53688826858; persistsecurityinfo=True;database=U06osQ");
            c.Open(); 

            //deletes customer
            string recUpdate = $"DELETE FROM customer" +
                $" WHERE customerName = '{customerDetails["customerName"]}'";
            MySqlCommand cmd = new MySqlCommand(recUpdate, c);
            int customerUpdated = cmd.ExecuteNonQuery();

            c.Close();

            if (customerUpdated != 0/* && addressUpdated != 0 && cityUpdated != 0 && countryUpdated != 0 */)
                return true;
            else
                return false;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            //searches db for customer and displays data in labels
            int customerId = DataHelper.findCustomer(searchBar.Text);
            if (customerId != 0)
            {
                customerDetails = DataHelper.getCustomerDetails(customerId);
                nameLabel.Text = customerDetails["customerName"];
                phoneLabel.Text = customerDetails["phone"];
                addressLabel.Text = customerDetails["address"];
                cityLabel.Text = customerDetails["city"];
                zipLabel.Text = customerDetails["zip"];
                countryLabel.Text = customerDetails["country"];
                if (customerDetails["active"] == "True")
                    activeLabel.Text = "True";
                else
                    activeLabel.Text = "False";
            }
            else
            {
                MessageBox.Show("Unable to find a customer by that name");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            //dialogbox to confirm delete
            DialogResult confirmDelete = MessageBox.Show("Are you sure you want to delete this contact?", "Deletion Confirm", MessageBoxButtons.YesNo);
            if (confirmDelete == DialogResult.Yes)
            {
                if (deleteCustomer())
                    MessageBox.Show($"Customer: {customerDetails["customerName"]} was successfully deleted");
                else
                    MessageBox.Show($"Customer: {customerDetails["customerName"]} could not be deleted");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
