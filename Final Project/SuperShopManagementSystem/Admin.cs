using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperShopManegementSystem;

namespace SuperShopManagementSystem
{
    public partial class Admin : Form
    {
        private int eye = 1;
        private DataAccess Da { get; set; }
        internal string userId { get; set; }
        internal Login login { get; set; }

        #region Constructors
        public Admin()
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.PopulateGridView();
        }
        public Admin(String userId, Login login)
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.PopulateGridView();
            this.userId = userId;
            this.login = login;
        }

        #endregion

        #region Profile

        #region Refresh Profile On Load
        private void Admin_Load(object sender, EventArgs e)
        {
            this.btnProducts_Click(sender, e);
            this.cmbCategory.SelectedIndex = 9;

            //this.userId = "shamim";
            string query = "select * from UserInformation where UserId = '" + this.userId + "';";

            var ds = this.Da.ExecuteQueryTable(query);
            this.txtUserId.Text = ds.Rows[0][0].ToString();
            this.txtUserName.Text = ds.Rows[0][1].ToString();
            this.txtPassword.Text = ds.Rows[0][8].ToString();
            this.txtPhone.Text = ds.Rows[0][6].ToString();
            this.dtpDOB.Text = ds.Rows[0][4].ToString();
            this.txtAddressProfile.Text = ds.Rows[0][5].ToString();

            /// dgvProducts selection 
            dgvProductView.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));
            dgvProductView.DefaultCellStyle.SelectionForeColor = Color.White;
            
            dgvEmployeeView.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));
            dgvEmployeeView.DefaultCellStyle.SelectionForeColor = Color.White;
            double allSell = 0;
            for(int i = 0; i < this.dgvHistory.RowCount; i++)
            {
                allSell += double.Parse((this.dgvHistory.Rows[i].Cells["historyTotal"].Value).ToString());
            }
            this.lblAllTimeSell.Text = allSell.ToString();
        }
        internal void Refresh_Profile()
        {
            this.Admin_Load(null, null);
        }
        #endregion

        #region Edit & save on profile tab
        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.ToggleReadOnly();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtAddressProfile.Text) || string.IsNullOrEmpty(this.txtPassword.Text) || string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.txtUserName.Text) || string.IsNullOrEmpty(this.dtpDOB.Text))
                {
                    MessageBox.Show("Please fill all info and Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                string query = "update UserInformation" +
                    " set UserName = '" + this.txtUserName.Text +
                    "', UserId = '" + this.txtUserId.Text +
                    "', Password = '" + this.txtPassword.Text +
                    "', DateOfBirth = '" + this.dtpDOB.Text +
                    "', Address = '" + this.txtAddressProfile.Text +
                    "' where UserId = '" + this.txtUserId.Text + "'";

                var rowCount = this.Da.ExecuteDMLQuery(query);
                if (rowCount == 1)
                {
                    if (eye < 0)
                        this.btnEye_Click(sender, new EventArgs());
                    MessageBox.Show("Data updated successfully");
                }
                else
                    MessageBox.Show("Data upgradation failed");
                this.ToggleReadOnly();
            }catch(Exception Ex)
            {
                MessageBox.Show("Something went wrong!! Please try agian later.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        
        private void btnEye_Click(object sender, EventArgs e)
        {
            this.pnlRight.Focus();
            try
            {
                eye *= -1;
                if (eye < 0)
                {
                    string filePath = System.IO.Path.GetFullPath(@"..\..\Resources\002-view.png");
                    Bitmap eyeOpen = new Bitmap(filePath);
                    this.btnEye.Image = eyeOpen;
                    this.txtPassword.UseSystemPasswordChar = false;
                }
                else
                {
                    string filePath = System.IO.Path.GetFullPath(@"..\..\Resources\001-hide.png");
                    Bitmap eyeClose = new Bitmap(filePath);
                    this.btnEye.Image = eyeClose;
                    this.txtPassword.UseSystemPasswordChar = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void ToggleReadOnly()
        {
            this.btnEdit.Enabled = (this.btnEdit.Enabled == true ? false : true);
            this.btnSave.Enabled = (this.btnSave.Enabled == true ? false : true);
            this.txtPassword.Enabled = (this.txtPassword.Enabled == true ? false : true);
            this.txtPhone.Enabled = (this.txtPhone.Enabled == true ? false : true);
            this.txtUserName.Enabled = (this.txtUserName.Enabled == true ? false : true);
            this.dtpDOB.Enabled = (this.dtpDOB.Enabled == true ? false : true);
            this.btnEye.Enabled = (this.btnEye.Enabled == true ? false : true);
            this.txtAddressProfile.Enabled = (this.txtAddressProfile.Enabled == true ? false : true);
        }

        #endregion

        #endregion

        #region Show Product List
        private void PopulateGridView(string sql = "select * from ProductInformation;")
        {
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvProductView.AutoGenerateColumns = false;
            this.dgvProductView.DataSource = ds.Tables[0];
            this.txtProductId.Text = this.AutoGenerateId();
        }

#endregion

        #region Auto Generate Id For Product
        private string AutoGenerateId()
        {
            string id = "P-";
            string sql = "select * from ProductInformation;";
            var ds = this.Da.ExecuteQueryTable(sql);
            int i = 1;
            if (ds.Rows.Count != 0)
            {
                i = Int32.Parse(ds.Rows[ds.Rows.Count - 1][0].ToString().Substring(2)) + 1;
                id += i.ToString("D3");
            }
            else
            {
                id += i.ToString("D3");
            }

            return id;
        }
        #endregion

        #region Tab Toogle Admin
        private void btnProducts_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectTab(0);
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectTab(1);
            this.PopulateGridViewEmployee();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            this.tabControl.SelectTab(2);

            string sql = "select * from SellInformation;";
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvHistory.AutoGenerateColumns = false;
            this.dgvHistory.DataSource = ds.Tables[0];
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            if (this.btnEye.Enabled == true)
            {
                this.ToggleReadOnly();
                this.btnEye_Click(sender, e);
            }
            this.tabControl.SelectTab(3);
        }
        #endregion
       
        #region Clear Page
        private void RefreshContent()
        {
            this.txtProductName.Clear();
            this.txtPrice.Clear();
            this.dtpExpiryDate.Text = "";
            this.dtpStockDate.Text = "";
            this.txtQuantity.Text = "";
            this.txtSearchProduct.Clear();
            this.txtEmployeeName.Text = "";
            this.txtSalary.Text = "";
            this.dtpJoiningDate.Text = "";
            this.dtpDateOfBirth.Text = "";
            this.txtAddress.Text = "";
            this.txtPhoneNumber.Clear();
            this.txtSearchEmployee.Clear();
            this.txtPasswordEmployee.Clear();
             this.cmbCategory.SelectedIndex = 9;
        }
       
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.RefreshContent();
        }
        private void btnClearEmployee_Click(object sender, EventArgs e)
        {
            this.RefreshContent();

        }
        #endregion

        #region SearchProduct

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                var sql = "select * from ProductInformation where ProductName like '%" + this.txtSearchProduct.Text + "%' or ProductId like '%" + this.txtSearchProduct.Text + "%' or Price like '%" + this.txtSearchProduct.Text + "%'or Quantity like '%" + this.txtSearchProduct.Text + "%';";
                this.PopulateGridView(sql);

            }
            catch (Exception exc)
            {
                MessageBox.Show("Searching Failed. Error: " + exc.Message);
            }
        }
        #endregion

        #region Show Info In Text Box

        private void dgvProductView_DoubleClick(object sender, EventArgs e)
        {


            try
            {
                this.txtProductId.Text = this.dgvProductView.CurrentRow.Cells["ProductId"].Value.ToString();
                this.txtProductName.Text = this.dgvProductView.CurrentRow.Cells["ProductName"].Value.ToString();
                this.cmbCategory.Text = this.dgvProductView.CurrentRow.Cells["Category"].Value.ToString();
                this.txtPrice.Text = this.dgvProductView.CurrentRow.Cells["Price"].Value.ToString();
                this.txtQuantity.Text = this.dgvProductView.CurrentRow.Cells["Quantity"].Value.ToString();
                this.dtpStockDate.Text = this.dgvProductView.CurrentRow.Cells["StockDate"].Value.ToString();
                this.dtpExpiryDate.Text = this.dgvProductView.CurrentRow.Cells["ExpiryDate"].Value.ToString();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Can not double click right now. Error : " + exc.Message);
            }
        }


        #endregion

        #region Delete Product Operation
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var id = this.dgvProductView.CurrentRow.Cells[0].Value.ToString();
                var name = this.dgvProductView.CurrentRow.Cells[1].Value.ToString();

                var sql = "delete from ProductInformation where ProductId = '" + id + "';";
                int count = this.Da.ExecuteDMLQuery(sql);

                if (count == 1)
                    MessageBox.Show(name + " has been deleted successfully");
                else
                    MessageBox.Show("Data deletion failed");

                this.PopulateGridView();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Can not delete right now.Error: " + exc.Message);
            }
        }

        #endregion

        #region Add Update Operations
        private bool IsValidToSaveData()
        {
            if (double.Parse(this.txtQuantity.Text) <= 0 || double.Parse(this.txtPrice.Text) <= 0 ||  String.IsNullOrEmpty(this.txtProductId.Text) || String.IsNullOrEmpty(this.txtProductName.Text) ||
                String.IsNullOrEmpty(this.cmbCategory.Text) || String.IsNullOrEmpty(this.txtPrice.Text) || String.IsNullOrWhiteSpace(this.txtQuantity.Text))
            {
                return false;
            }
            else
                return true;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsValidToSaveData())
                {
                    MessageBox.Show("Invalid opration. Please fill up all the information");
                    return;
                }

                var query = "select * from ProductInformation where ProductId = '" + this.txtProductId.Text + "';";
                var ds = this.Da.ExecuteQuery(query);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    //update
                    var sql = @"update ProductInformation
                                set ProductName = '" + this.txtProductName.Text + @"',
                                Category = '" + this.cmbCategory.Text + @"', 
                                Price = '" + this.txtPrice.Text + @"',
                                Quantity = " + this.txtQuantity.Text + @",
                                StockDate = '" + this.dtpStockDate.Text + @"',
                                ExpiryDate = '" + this.dtpExpiryDate.Text + @"'
                                where ProductId = '" + this.txtProductId.Text + "';";
                    int count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data updated successfully");
                    else
                        MessageBox.Show("Data upgradation failed");
                }
                else
                {
                    //insert
                    var sql = @"insert into ProductInformation values('" + this.txtProductId.Text + "','" + this.txtProductName.Text + "','" + this.cmbCategory.Text + "','" + this.txtPrice.Text + " TK'," + this.txtQuantity.Text + ",'" + this.dtpStockDate.Text + "','" + this.dtpExpiryDate.Text + "');";
                    int count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data insertion successfull");
                    else
                        MessageBox.Show("Data insertion failed");
                }

                this.PopulateGridView();
                this.RefreshContent();
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured: " + exc.Message);
            }

        }
        #endregion
        
        #region Show Employee List
        
        private void PopulateGridViewEmployee(string sql = "select * from UserInformation where Role = 'Employee';")
        {
            var ds = this.Da.ExecuteQuery(sql);
            this.dgvEmployeeView.AutoGenerateColumns = false;
            this.dgvEmployeeView.DataSource = ds.Tables[0];
            this.txtEmployeeId.Text = this.AutoGenerateIdEmployee();
        }
        #endregion

        #region Auto Generate Id Employee
        private string AutoGenerateIdEmployee()
        {
            string id = "E-";
            string sql = "select * from UserInformation where Role = 'Employee';";
            var ds = this.Da.ExecuteQueryTable(sql);
            int i = 1;
            if (ds.Rows.Count != 0)
            {
                i = Int32.Parse(ds.Rows[ds.Rows.Count - 1][0].ToString().Substring(2)) + 1;
                id += i.ToString("D3");
            }
            else
            {
                id += i.ToString("D3");
            }

            return id;
        }


        #endregion

        #region Search Employee
        private void txtSearchEmployee_TextChanged(object sender, EventArgs e)
        {
            var sql = "select * from UserInformation where UserName like '%" + this.txtSearchEmployee.Text + "%' and Role = 'Employee';";
            this.PopulateGridViewEmployee(sql);
        }

        #endregion

        #region Show Employee Info IN Textbox

        private void dgvEmployeeView_DoubleClick(object sender, EventArgs e)
        {
            this.txtEmployeeId.Text = this.dgvEmployeeView.CurrentRow.Cells["EmployeeId"].Value.ToString();
            this.txtEmployeeName.Text = this.dgvEmployeeView.CurrentRow.Cells["EmployeeName"].Value.ToString();
            this.txtSalary.Text = this.dgvEmployeeView.CurrentRow.Cells["Salary"].Value.ToString();
            this.dtpJoiningDate.Text = this.dgvEmployeeView.CurrentRow.Cells["JoiningDate"].Value.ToString();
            this.dtpDateOfBirth.Text = this.dgvEmployeeView.CurrentRow.Cells["DateOfBirth"].Value.ToString();
            this.txtAddress.Text = this.dgvEmployeeView.CurrentRow.Cells["Address"].Value.ToString();
            this.txtPhoneNumber.Text = this.dgvEmployeeView.CurrentRow.Cells["PhoneNumber"].Value.ToString();
            this.txtPasswordEmployee.Text = this.dgvEmployeeView.CurrentRow.Cells["Password"].Value.ToString();
        }

        #endregion

        #region Employee ADD and Update operation 

        private bool IsValidToSaveDataEmployee()
        {
            if (double.Parse(this.txtSalary.Text) <= 0  || String.IsNullOrEmpty(this.txtEmployeeId.Text) || String.IsNullOrEmpty(this.txtEmployeeName.Text) ||
                String.IsNullOrEmpty(this.txtSalary.Text) || String.IsNullOrEmpty(this.txtAddress.Text) || String.IsNullOrWhiteSpace(this.txtPhoneNumber.Text) || String.IsNullOrWhiteSpace(this.txtPasswordEmployee.Text))
            {
                return false;
            }
            else
                return true;
        }


       

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsValidToSaveDataEmployee())
                {
                    MessageBox.Show("Invalid opration. Please fill up all the information");
                    return;
                }
                var query = "select * from UserInformation where UserId = '" + this.txtEmployeeId.Text + "';";
                var ds = this.Da.ExecuteQuery(query);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    //update
                    var sql = @"update UserInformation
                                set UserName = '" + this.txtEmployeeName.Text + @"',
                                Salary = '" + this.txtSalary.Text + @"', 
                                JoiningDate = '" + this.dtpJoiningDate.Text + @"',
                                DateOfBirth = '" + this.dtpDateOfBirth.Text + @"',
                                Address = '" + this.txtAddress.Text + @"',
                                PhoneNumber = '" + this.txtPhoneNumber.Text + @"',
                                Password = '" + this.txtPasswordEmployee.Text + @"'
                                where UserId = '" + this.txtEmployeeId.Text + "';";
                    int count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data updated successfully");
                    else
                        MessageBox.Show("Data upgradation failed");
                }
                else
                {
                    //insert
                    var sql = @"insert into UserInformation values('" + this.txtEmployeeId.Text + "','" + this.txtEmployeeName.Text + "','" + this.txtSalary.Text + " TK','" + this.dtpJoiningDate.Text + "','" + this.dtpDateOfBirth.Text + "','" + this.txtAddress.Text + "','" + this.txtPhoneNumber.Text + "','Employee','" + this.txtPasswordEmployee.Text + "');";
                    int count = this.Da.ExecuteDMLQuery(sql);

                    if (count == 1)
                        MessageBox.Show("Data insertion successfull");
                    else
                        MessageBox.Show("Data insertion failed");
                }

                this.PopulateGridViewEmployee();
                this.RefreshContent();
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured: " + exc.Message);
            }
        }
        #endregion

        #region Employee Delete Operation
        private void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                var id = this.dgvEmployeeView.CurrentRow.Cells["EmployeeId"].Value.ToString();
                var name = this.dgvEmployeeView.CurrentRow.Cells["EmployeeName"].Value.ToString();

                var sql = "delete from UserInformation where UserId = '" + id + "';";
                int count = this.Da.ExecuteDMLQuery(sql);

                if (count == 1)
                    MessageBox.Show(name + " has been deleted successfully");
                else
                    MessageBox.Show("Data deletion failed");

                this.PopulateGridViewEmployee();
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured: " + exc.Message);
            }

        }



        #endregion

        #region Logout
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.pnlOrange.Focus();
            this.login.Show();
            this.Hide();
        }

        #endregion

        #region Form Closing Cross 
        private void Admin_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Environment.Exit(1);
        }
        #endregion


        
    }
}
