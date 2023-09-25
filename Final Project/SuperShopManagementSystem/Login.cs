using SuperShopManagementSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperShopManegementSystem
{
    public partial class Login : Form
    {
        private DataAccess Da { get; set; }

        internal Employee employee { get; set; }
        internal Admin admin { get; set; }

        public Login()
        {
            InitializeComponent();
            this.Da = new DataAccess();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.panelRight.Focus();
            if (this.txtLoginId.Text.Length == 0 || this.txtLoginPassword.Text.Length == 0)
            {
                MessageBox.Show("Please enter all information!", "", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string query = "select Role from UserInformation where UserId = '" + this.txtLoginId.Text + "' and Password = '" + this.txtLoginPassword.Text + "';";
            var ds = this.Da.ExecuteQuery(query);
            if (ds.Tables[0].Rows.Count == 1)
            {
                if (ds.Tables[0].Rows[0][0].ToString() == "Employee")
                {
                    if(employee == null)
                    {
                        employee = new Employee(this.txtLoginId.Text, this);
                    }
                    else
                    {
                        employee.UserName = this.txtLoginId.Text;
                        employee.Refresh_Profile();
                    }
                    employee.Show();
                    this.Hide();
                }
                else if(ds.Tables[0].Rows[0][0].ToString() == "Admin") 
                {
                    if(admin == null)
                    {
                        admin = new Admin(this.txtLoginId.Text, this);
                    }else
                    {
                        admin.userId = this.txtLoginId.Text;
                        admin.Refresh_Profile();
                    }
                    admin.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Your credential is wrong!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else MessageBox.Show("Your credential is wrong!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                btnLogin_Click(this, new EventArgs());

            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = e.SuppressKeyPress = true;
                btnClear_Click(this, new EventArgs());
            }
        }

        private void txtLoginId_KeyDown(object sender, KeyEventArgs e)
        {
            KeyPress(sender, e);
        }

        private void txtLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            KeyPress(sender, e);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtLoginId.Clear();
            this.txtLoginPassword.Clear();
            this.panelRight.Focus();
        }

        int eye = 1;
        private void btnEye_Click(object sender, EventArgs e)
        {
            this.panelRight.Focus();
            try
            {
                eye *= -1;
                if (eye < 0)
                {
                    string filePath = System.IO.Path.GetFullPath(@"..\..\Resources\002-view.png");
                    Bitmap eyeOpen = new Bitmap(filePath);
                    this.btnEye.Image = eyeOpen;
                    this.txtLoginPassword.UseSystemPasswordChar = false;
                }
                else
                {
                    string filePath = System.IO.Path.GetFullPath(@"..\..\Resources\001-hide.png");
                    Bitmap eyeClose = new Bitmap(filePath);
                    this.btnEye.Image = eyeClose;
                    this.txtLoginPassword.UseSystemPasswordChar = true;
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Environment.Exit(1);
        }
    }
}
