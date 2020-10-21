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
    public partial class DeleteAppointment : Form
    {
        public DeleteAppointment()
        {
            InitializeComponent();
        }
        public MainScreen mainScreenObject;
        public static Dictionary<string, string> appointmentDetails = new Dictionary<string, string>();

        public static bool deleteAppointment()
        {
            MySqlConnection c = new MySqlConnection(DataHelper.conString);
            c.Open();

            //Customer delete
            string recDelete = $"DELETE FROM appointment" +
                $" WHERE appointmentId = '{appointmentDetails["appointmentId"]}'";
            MySqlCommand cmd = new MySqlCommand(recDelete, c);
            int appointmentDeleted = cmd.ExecuteNonQuery();

            c.Close();

            if (appointmentDeleted != 0)
                return true;
            else
                return false;      
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string appointmentId = searchBox.Text;
            appointmentDetails = DataHelper.getAppointmentDetails(appointmentId);
            customerIdLabel.Text = appointmentDetails["customerId"];
            typeLabel.Text = appointmentDetails["type"];
            startLabel.Text = appointmentDetails["start"];
            endLabel.Text = appointmentDetails["end"];
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            //Delete confirmation
            DialogResult confirmDelete = MessageBox.Show("Are you sure you want to delete this appointment?", "Deletion Confirm", MessageBoxButtons.YesNo);
            if (confirmDelete == DialogResult.Yes)
            {
                //actual deletion
                if (deleteAppointment())
                {
                    mainScreenObject.updateCalendar();
                    MessageBox.Show($"Appointment: {appointmentDetails["appointmentId"]} was successfully deleted");
                }
                else
                    MessageBox.Show($"Appointment: {appointmentDetails["appointmentId"]} failed to delete");
            }
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
