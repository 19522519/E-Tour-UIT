using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;

namespace Tour
{
    public partial class RegisterAccount : Form
    {
        string randomcode;
        public static string to;
        System.Text.RegularExpressions.Regex rEMail = new System.Text.RegularExpressions.Regex(@"^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$");
        DataConnection dataConnection;
        SqlCommand sqlCommand;
        public RegisterAccount()
        {
            dataConnection = new DataConnection();
            InitializeComponent();
        }
        static string Encrypt(string value)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(value));
                return Convert.ToBase64String(data);
            }
        }
        bool CheckDuplicateEmail(string email)
        {
            SqlConnection sqlConnection = dataConnection.getConnect();
            sqlConnection.Open();
            sqlCommand = new SqlCommand("select count(*) from UserID where Email=@Email", sqlConnection);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.Parameters.AddWithValue("@email", email);
            int numrecord = (int)sqlCommand.ExecuteScalar();
            if (numrecord > 0)
                return false;
            return true;
        }

        private void SignUpbtn_Click(object sender, EventArgs e)
        {
            string sql = "Insert into UserID(Ho,Ten,Email,Password,SĐT) values(@Ho,@Ten,@Email,@Password,@SĐT)";
            SqlConnection sqlConnection;
            if (CheckDuplicateEmail(txbGmail.Text) == false)
            {
                MessageBox.Show("Your Email is already in use");
            }
            else if (txbGmail.Text == "")
            {
                MessageBox.Show("Please enter FULL INFORMATION!!!");
            }
            else if (txbPass.Text == "")
                MessageBox.Show("Please Enter Password!!!");
            else if (txbConfirm.Text != txbPass.Text)
                MessageBox.Show("Password not match!!!");
            else if (randomcode != txbCode.Text.ToString() || txbGmail.Text.ToString() != email)
            {
                MessageBox.Show("Code Box is wrong or empty!!!");
            }
            else
            {
                using (sqlConnection = dataConnection.getConnect())
                {
                    label12.Text = "Checked";
                    label12.ForeColor = Color.Green;
                    SignUpbtn.Enabled = true;
                    sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Ho", txbHo.Text.Trim());
                    sqlCommand.Parameters.AddWithValue("@Ten", txbTen.Text.Trim());
                    sqlCommand.Parameters.AddWithValue("@SĐT", txbSDT.Text.Trim());
                    sqlCommand.Parameters.AddWithValue("@Email", txbGmail.Text.Trim());
                    sqlCommand.Parameters.AddWithValue("@Password", Encrypt(txbPass.Text.Trim()));
                    sqlCommand.ExecuteNonQuery();
                    //sqlCon.Close();
                    MessageBox.Show("SignUp success!!!");
                    Clear();
                }
            }
        }
        void Clear()
        {
            txbHo.Text = txbTen.Text = txbSDT.Text = txbGmail.Text = txbPass.Text = txbConfirm.Text = "";
        }

        private void txbSDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        string email;
        private void txbGmail_Validating(object sender, CancelEventArgs e)
        {
            if (txbGmail.Text.Length > 0)
            {
                if (!rEMail.IsMatch(txbGmail.Text))
                {
                    MessageBox.Show("Invalidate Email", "Error");
                    txbGmail.SelectAll();
                    SignUpbtn.Enabled = false;
                    //e.Cancel = true;
                }
                else
                {
                    SignUpbtn.Enabled = true;
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txbGmail.Text.Length > 0)
            {
                if (rEMail.IsMatch(txbGmail.Text))
                {
                    email = txbGmail.Text.ToString();
                    string from, pass, messageBody;
                    Random random = new Random();
                    randomcode = (random.Next(100000, 999999)).ToString();
                    MailMessage message = new MailMessage();
                    to = (txbGmail.Text).ToString();
                    from = "testing01.uit@gmail.com";
                    pass = "tnuouslqfpdncmjp";
                    messageBody = "Verification code exists for your email :" + randomcode;
                    message.To.Add(to);
                    message.From = new MailAddress(from);
                    message.Body = messageBody;
                    message.Subject = "Confirm Email Code";
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.EnableSsl = true;
                    smtp.Port = 587;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(from, pass);
                    try
                    {
                        smtp.Send(message);
                        MessageBox.Show("Code send success!!!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void txbGmail_TextChanged(object sender, EventArgs e)
        {
            txbCode.Text = "";
        }

        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
