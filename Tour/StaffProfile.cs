using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;

namespace Tour
{
    public partial class StaffProfile : Form
    {
        DataConnection dataConnection;
        SqlCommand sqlCommand;
        public StaffProfile()
        {
            dataConnection = new DataConnection();
            InitializeComponent();
        }

        private void StaffProfile_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserName != string.Empty)
            {
                txbGmail.Text = Properties.Settings.Default.UserName;
            }
            SqlConnection sqlConnection = dataConnection.getConnect();
            sqlConnection.Open();
            sqlCommand = new SqlCommand("Select Ho,Ten,SĐT from UserID where Email=@Email", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Email", txbGmail.Text);
            SqlDataReader da = sqlCommand.ExecuteReader();
            while (da.Read())
            {
                txbHo.Text = da.GetValue(0).ToString();
                txbTen.Text = da.GetValue(1).ToString();
                txbSDT.Text = da.GetValue(2).ToString();
            }
            sqlConnection.Close();
        }

        private void backbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
