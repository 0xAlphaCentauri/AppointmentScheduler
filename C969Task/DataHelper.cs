using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace testmsqlconnect
{
    class DataHelper
    {
        static DataSet calendarDataset; //test
        private static Dictionary<int, Hashtable> _appointments = new Dictionary<int, Hashtable>();
        private static Dictionary<int, Hashtable> _iDs = new Dictionary<int, Hashtable>();
        private static int _currentUserId;
        private static string _currentUserName;
        public static string conString = "server=wgudb.ucertify.com;user id=U06osQ;pwd= 53688826858; persistsecurityinfo=True;database=U06osQ; convert zero datetime = True;";

        //lamba get/set functions reduce code size from 64 lines to 10. Lambas allow compaction of code
        //test data
        static public void setCalendarSet(DataSet set) => calendarDataset = set;

        static public DataSet getCalendarSet() => calendarDataset;

        //test data
        public static int getCurrentUserId() => _currentUserId;

        public static void setCurrentUserId (int currentUserId) =>_currentUserId = currentUserId;

        public static string getCurrentUserName() => _currentUserName;

        public static void setCurrentUserName(string currentUserName) => _currentUserName = currentUserName;

        public static Dictionary<int, Hashtable> getAppointments()=> _appointments;
        public static void setAppointments(Dictionary<int, Hashtable> appointments) => _appointments = appointments;

        public static int newID(List<int> idList)
        {
            int highestID = 0;
            foreach (int id in idList)
            {
                if (id > highestID)
                highestID = id;
            }
            return highestID + 1;
        }

        public static string createTimestamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //lamba used to provide efficiency in code for a quick timestamp and to minimize line usage in DataHelper

        //public static string createTimestamp() {
        //    DateTime timestamp = new DateTime(2008, 4, 10, 6, 30, 0);
        //    return timestamp.ToString("u");
        //}

        public static int createID(string table)
        { 
            MySqlConnection c = new MySqlConnection("server = wgudb.ucertify.com; user id = U06osQ; pwd = 53688826858; persistsecurityinfo = True; database = U06osQ");
            c.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT {table+"Id"} FROM {table}", c);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<int> idList = new List<int>();
            while (rdr.Read())
            {
                idList.Add(Convert.ToInt32(rdr[0]));
            }
            rdr.Close();
            c.Close();
            return newID(idList);
        }

        static public int createRecord(string timestamp, string userName, string table, string partOfQuery, int userId = 0)
        {
            int recId = createID(table);
            string recInsert;
            if (userId == 0)
            {
                recInsert = $"INSERT INTO {table}" +
            $" VALUES ('{recId}', {partOfQuery}, '{timestamp}', '{userName}', '{timestamp}', '{userName}')";
            }

            else
            {
                recInsert = $"INSERT INTO {table} (appointmentId, customerId, start, end, type, userId, createDate, createdBy, lastUpdate, lastUpdateBy)" +
                $" VALUES ('{recId}', {partOfQuery}, '{userId}', '{timestamp}', '{userName}', '{timestamp}', '{userName}')";
            }

                MySqlConnection c = new MySqlConnection("server = wgudb.ucertify.com; user id = U06osQ; pwd = 53688826858; persistsecurityinfo = True; database = U06osQ");
                c.Open();
                MySqlCommand cmd = new MySqlCommand(recInsert, c);
                cmd.ExecuteNonQuery();
                c.Close();

                return recId;
            }
        
        //static public int createRecord(string timestamp, string userName, string table, string partOfQuery, int userId = 0)
        //{
        //    int recId = createID(table);
        //    string recInsert;
        //    if (userId == 0)
        //    {
        //        recInsert = $"INSERT INTO {table}" +
        //            $" VALUES ('{recId}', {partOfQuery}, '{timestamp}', '{userName}', '{timestamp}', '{userName}')";
        //    }
        //    else
        //    {
        //        recInsert = $"INSERT INTO {table} (appointmentId, customerId, start, end, type, userId, createDate, createdBy, lastUpdate, lastUpdateBy)" +
        //            $" VALUES ('{recId}', {partOfQuery}, '{userId}', '{timestamp}', '{userName}', '{timestamp}', '{userName}')";
        //    }

        //    MySqlConnection c = new MySqlConnection("server=wgudb.ucertify.com;user id=U06osQ;pwd= 53688826858; persistsecurityinfo=True;database=U06osQ");
        //    c.Open();
        //    MySqlCommand cmd = new MySqlCommand(recInsert, c);
        //    cmd.ExecuteNonQuery();
        //    c.Close();

        //    return recId;
        //}

        static public int findCustomer(string search)
        {
            int customerId;
            string query;
            if (int.TryParse(search, out customerId))
            {
                query = $"SELECT customerId FROM customer WHERE customerId = '{search.ToString()}'";
            }
            else
            {
                query = $"SELECT customerId FROM customer WHERE customerName LIKE '{search}'";
            }
            MySqlConnection c = new MySqlConnection("server = wgudb.ucertify.com; user id = U06osQ; pwd = 53688826858; persistsecurityinfo = True; database = U06osQ");
            c.Open();
            MySqlCommand cmd = new MySqlCommand(query, c);
            MySqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                rdr.Read();
                customerId = Convert.ToInt32(rdr[0]);
                rdr.Close(); c.Close();
                return customerId;
            }
            return 0;
        }

        static public Dictionary<string, string> getCustomerDetails(int customerId)
        {
            string query = $"SELECT * FROM customer WHERE customerId = '{customerId.ToString()}'";
            MySqlConnection c = new MySqlConnection("server=wgudb.ucertify.com;user id=U06osQ;pwd= 53688826858; persistsecurityinfo=True;database=U06osQ");
            c.Open();
            MySqlCommand cmd = new MySqlCommand(query, c);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            Dictionary<string, string> customerDict = new Dictionary<string, string>();
            //customer table
            customerDict.Add("customerName", rdr[1].ToString());
            customerDict.Add("addressId", rdr[2].ToString());
            customerDict.Add("active", rdr[3].ToString());
            rdr.Close();

            query = $"SELECT * FROM address WHERE addressId = '{customerDict["addressId"]}'";
            cmd = new MySqlCommand(query, c);
            rdr = cmd.ExecuteReader();
            rdr.Read();

            //address table det. 
            customerDict.Add("address", rdr[1].ToString());
            customerDict.Add("cityId", rdr[3].ToString());
            customerDict.Add("zip", rdr[4].ToString());
            customerDict.Add("phone", rdr[5].ToString());
            rdr.Close();

            query = $"SELECT * FROM city WHERE cityId = '{customerDict["cityId"]}'";
            cmd = new MySqlCommand(query, c);
            rdr = cmd.ExecuteReader();
            rdr.Read();

            //city table
            customerDict.Add("city", rdr[1].ToString());
            customerDict.Add("countryId", rdr[2].ToString());
            rdr.Close();

            query = $"SELECT * FROM country WHERE countryId = '{customerDict["countryId"]}'";
            cmd = new MySqlCommand(query, c);
            rdr = cmd.ExecuteReader();
            rdr.Read();

            //cuontry table det
            customerDict.Add("country", rdr[1].ToString());
            rdr.Close();
            c.Close();

            return customerDict;
        }
        static public Dictionary<string, string> getAppointmentDetails(string appointmentId)
        {
            string query = $"SELECT * FROM appointment WHERE appointmentId = '{appointmentId}'";
            MySqlConnection c = new MySqlConnection("server=wgudb.ucertify.com;user id=U06osQ;pwd= 53688826858; persistsecurityinfo=True;database=U06osQ");
            c.Open();
            MySqlCommand cmd = new MySqlCommand(query, c);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            Dictionary<string, string> appointmentDict = new Dictionary<string, string>();
            appointmentDict.Add("appointmentId", appointmentId);
            appointmentDict.Add("customerId", rdr[1].ToString());
            appointmentDict.Add("type", rdr[7].ToString());
            appointmentDict.Add("start", rdr[9].ToString());
            appointmentDict.Add("end", rdr[10].ToString());
            rdr.Close();

            return appointmentDict;
        }

        ////static public string convertToTimezone(string dateTime)// => ("MM/dd/yyyy hh:mm:ss");
        ////{

        ////    DateTime utcDateTime = DateTime.Now; //not right

        ////    DateTime localDateTime = utcDateTime.ToLocalTime();

        ////    return localDateTime.ToString("MM/dd/yyyy hh:mm:ss");
        ////}
        ///
        static public string convertToTimezone(string dateTime)// => ("MM/dd/yyyy hh:mm:ss");
        {

            DateTime utcDateTime = DateTime.Parse(dateTime); //not right

            DateTime localDateTime = utcDateTime.ToLocalTime();

            return localDateTime.ToString("MM/dd/yyyy hh:mm:ss");
        }

        //testme
        //static public bool checkCalendar(DateTime overlapCheckStart, DateTime overlapCheckEnd, int appId)
        //{
        //    foreach (DataTable table in calendarDataset.Tables)
        //    {
        //        foreach(DataRow row in table.Rows)
        //        {
        //            if ((((DateTime)row.ItemArray[7] >= overlapCheckStart && (DateTime)row.ItemArray[7] <= overlapCheckEnd) || ((DateTime)row.ItemArray[8] >= overlapCheckStart && (DateTime)row.ItemArray[8] <= overlapCheckEnd)) && (Convert.ToInt32(row.ItemArray[0]) != appId))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //delete me 

        public bool checkForOverlappingAppointments(DateTime apptStartTime, DateTime apptEndTime)
        {

            MySqlConnection conn = new MySqlConnection(conString);

            bool overlap = false;

            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT EXISTS(SELECT * FROM appointment WHERE start <= @end AND end >= @start)";
                cmd.Parameters.AddWithValue("@start", apptStartTime);
                cmd.Parameters.AddWithValue("@end", apptEndTime);

                if (cmd.ExecuteScalar().ToString() == "1")
                {
                    overlap = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown checking for overlapping appointments: " + ex);
            }
            finally
            {
                conn.Close();
            }

            return overlap;
        }


        //test me
        public DataTable getCustomers()
        {
            MySqlConnection conn = new MySqlConnection(conString);

            DataTable customersDataTable = new DataTable();

            //If column does not exist, add column.
            if (!customersDataTable.Columns.Contains("ID")) { customersDataTable.Columns.Add("ID", typeof(int)); }
            if (!customersDataTable.Columns.Contains("Name")) { customersDataTable.Columns.Add("Name", typeof(string)); }
            if (!customersDataTable.Columns.Contains("Phone Number")) { customersDataTable.Columns.Add("Phone Number", typeof(string)); }
            if (!customersDataTable.Columns.Contains("Address")) { customersDataTable.Columns.Add("Address", typeof(string)); }
            if (!customersDataTable.Columns.Contains("Address 2")) { customersDataTable.Columns.Add("Address 2", typeof(string)); }
            if (!customersDataTable.Columns.Contains("City")) { customersDataTable.Columns.Add("City", typeof(string)); }
            if (!customersDataTable.Columns.Contains("Postal Code")) { customersDataTable.Columns.Add("Postal Code", typeof(string)); }
            if (!customersDataTable.Columns.Contains("Country")) { customersDataTable.Columns.Add("Country", typeof(string)); }

            try
            {
                conn.Open();
                string query = "SELECT customer.customerId, customer.customerName, address.phone, address.address, address.address2, address.postalCode, city.city, country.country FROM customer " +
                    "LEFT JOIN address ON customer.addressId = address.addressId " +
                    "LEFT JOIN city ON address.cityId = city.cityId " +
                    "LEFT JOIN country ON city.countryId = country.countryId";
                MySqlCommand command = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        customersDataTable.Rows.Add(reader["customerId"], reader["customerName"], reader["phone"], reader["address"], reader["address2"], reader["city"], reader["postalCode"], reader["country"]);

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown getting customers: " + ex);
            }
            finally
            {
                conn.Close();
            }

            return customersDataTable;

        }
    }
}
