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
    public struct Customer
    {
        public string customerName;
        public int numberOfApps;
    }
    public partial class CustomerReport : Form
    {
        public CustomerReport()
        {
            InitializeComponent();
            customerReportDgv.DataSource = getReport();
        }

        public static DataTable getReport()
        {
            Dictionary<int, Hashtable> appointments = DataHelper.getAppointments();

            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Customer");
            dt.Columns.Add("Appointments");

            IEnumerable<string> customers = appointments.Select(i => i.Value["customerName"].ToString()).Distinct();

            foreach (string customer in customers)
            {
                DataRow row = dt.NewRow();
                row["Customer"] = customer;
                row["Appointments"] = appointments.Where(i => i.Value["customerName"].ToString() == customer.ToString()).Count().ToString();
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
