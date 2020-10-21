using System;
using System.Collections;
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
    public partial class AddAppointment : Form
    {
        bool validAppt; //delete me
        public AddAppointment()
        {
            InitializeComponent();
            endTimePicker.Value = endTimePicker.Value.AddHours(1);

            DataHelper data = new DataHelper();
            dataGridView1.DataSource = data.getCustomers();
        }

        public MainScreen mainScreenObject;
        private void startTimePicker_ValueChanged(object sender, EventArgs e)
        {
            //DataHelper data = new DataHelper();

            //bool overlap = data.checkForOverlappingAppointments(startTimePicker.Value.ToUniversalTime(), endTimePicker.Value.ToUniversalTime());

            //if (overlap)
            //{
            //    validAppt = false;
            //    MessageBox.Show("There are overlapping appt times.");
            //}
        }

        private void endTimePicker_ValueChanged(object sender, EventArgs e)
        {
            //DataHelper data = new DataHelper();

            //bool overlap = data.checkForOverlappingAppointments(startTimePicker.Value.ToUniversalTime(), endTimePicker.Value.ToUniversalTime());

            //if (overlap)
            //{
            //    validAppt = false;
            //    MessageBox.Show("There are overlapping appt times.");
            //}
            //if (startTimePicker.Value.ToUniversalTime() < endTimePicker.Value.ToUniversalTime())
            //{
            //    if (!overlap)
            //    {
            //        validAppt = true;
            //    }
            //}
        }
        public static bool appHasConflict(DateTime startTime, DateTime endTime)
        {
            foreach (var app in DataHelper.getAppointments().Values)
            {
                if (startTime < DateTime.Parse(app["end"].ToString()) && DateTime.Parse(app["start"].ToString()) < endTime)
                    return true;
            }
            return false;
        }

        public static bool appIsOutsideBusinesshours(DateTime startTime, DateTime endTime)
        {
            startTime = startTime.ToLocalTime();
            endTime = endTime.ToLocalTime();
            DateTime businessStart = DateTime.Today.AddHours(8); // 8 AM
            DateTime businessEnd = DateTime.Today.AddHours(17); //5 PM
            if (startTime.TimeOfDay > businessStart.TimeOfDay && startTime.TimeOfDay < businessEnd.TimeOfDay &&
                endTime.TimeOfDay > businessStart.TimeOfDay && endTime.TimeOfDay < businessEnd.TimeOfDay)
                return false;

            return true;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
    
            string timestamp = DataHelper.createTimestamp();
            int userId = DataHelper.getCurrentUserId();
            string username = DataHelper.getCurrentUserName();
            DateTime startTime = startTimePicker.Value.ToUniversalTime();
            DateTime endTime = endTimePicker.Value.ToUniversalTime();
            //bool overlapCheck = DataHelper.checkCalendar(startTime, endTime, 0);

            DataHelper data = new DataHelper();

            bool overlap = data.checkForOverlappingAppointments(startTimePicker.Value.ToUniversalTime(), endTimePicker.Value.ToUniversalTime());

            if (overlap)
            {
                validAppt = false;
                MessageBox.Show("There are overlapping appt times.");
            }
            if (startTimePicker.Value.ToUniversalTime() < endTimePicker.Value.ToUniversalTime())
            {
                if (!overlap)
                {
                    validAppt = true;
                }
            }

            if (overlap)
            {
                validAppt = false;
                MessageBox.Show("There are overlapping appt times.");
            }


            try
            {
                if (appHasConflict(startTime, endTime))
                    throw new appointmentException();
                else if (appIsOutsideBusinesshours(startTime, endTime))
                {
                    MessageBox.Show("Please schedule the appointment within business hours.");
                }
                else
                {
                    try
                    {
                        if (appIsOutsideBusinesshours(startTime, endTime))
                            throw new appointmentException();
                        else if (!validAppt)
                        {
                            MessageBox.Show("There are overlapping appointment times.");
                            //throw new appointmentException();
                        }
                        else 
                        {
                            DataHelper.createRecord(timestamp, username, "appointment", $"'{customerIdTextBox.Text}', '{startTimePicker.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}', '{endTimePicker.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}', '{typeTextBox.Text}'", userId);
                            mainScreenObject.updateCalendar();
                            Close();
                        }
                    }
                    catch (appointmentException ex) { ex.businessHours(); }
                }
            }
            catch (appointmentException ex) { ex.appOverlap(); }
        }
        //test
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }



//private void AddAppointment_Load(object sender, EventArgs e)
//        {
//            string sql = "SELECT * FROM customer";
//            MySqlConnection connection = new MySqlConnection(DataHelper.conString);
//            connection.Open();
//            MySqlCommand cmd = new MySqlCommand(sql, connection);
//            MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
//            MySqlCommandBuilder bldr = new MySqlCommandBuilder(ada);
//            DataSet dSet = new DataSet();
//            ada.Fill(dSet, "customer");
//            DataTable cList = dSet.Tables["customer"];
//            dataGridView1.DataSource = cList.Tables["customer"];
//            dataGridView1.ReadOnly = true;
//            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//            connection.Close();
//        }
        //tset
    }
}
