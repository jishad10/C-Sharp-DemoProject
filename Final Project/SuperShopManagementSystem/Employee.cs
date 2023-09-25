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
    public partial class Employee : Form
    {
        #region Fields and constructor
        private int eye = 1;
        private DataAccess Da { get; set; }
        internal string UserName { get; set; }
        private Login Login { get; }
        public Employee()
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.PopulateGridView();
        }
        public Employee(string userName, Login login)
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.UserName = userName;
            this.Login = login;
        }
        #endregion

        #region On Load & Profile
        private void Employee_Load(object sender, EventArgs e)
        {
            this.btnHome_Click(sender, e);
            this.cmbCategory.SelectedIndex = 0;

            //this.UserName = "E-001";

            string query = "select * from UserInformation where UserId = '" + this.UserName + "';";

            var ds = this.Da.ExecuteQueryTable(query);
            this.txtUserId.Text = ds.Rows[0][0].ToString();
            this.txtUserName.Text = ds.Rows[0][1].ToString();
            this.txtPassword.Text = ds.Rows[0][8].ToString();
            this.txtPhone.Text = ds.Rows[0][6].ToString();
            this.dtpDOB.Text = ds.Rows[0][4].ToString();

            /// dgvProducts
            
            dgvProducts.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCart.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.White;
        }
        #endregion

        #region Reset Home & refresh profile
        internal void Refresh_Profile()
        {
            this.Employee_Load(null, null);

        }
        private void resetHome()
        {
            this.txtSearchProduct.Text = "";
            this.txtSearchProduct_empty();
            this.cmbCategory.SelectedIndex = 0;
        }

        #endregion

        #region buttons

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.btnHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            this.btnProfile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            this.btnHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));
            this.tabControl.SelectTab(0);
            this.resetHome();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            this.btnHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            this.btnProfile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            this.btnHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));
            this.tabControl.SelectTab(1);
            string sql = "select * from SellInformation where SellerName = '"+this.txtUserName.Text+"';";
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvHistory.AutoGenerateColumns = false;
            this.dgvHistory.DataSource = ds.Tables[0];
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            this.btnHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            this.btnHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            this.btnProfile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(66)))), ((int)(((byte)(35)))));

            if (this.btnEye.Enabled == true)
            {
                this.ToggleReadOnly();
                this.btnEye_Click(sender, e);
            }
            this.tabControl.SelectTab(2);
        }

        #region Edit & save on profile tab

            private void btnEdit_Click(object sender, EventArgs e)
            {
                this.ToggleReadOnly();
            }
            private void btnSave_Click(object sender, EventArgs e)
            {
                if (string.IsNullOrEmpty(this.txtPassword.Text) || string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.txtUserName.Text) || string.IsNullOrEmpty(this.dtpDOB.Text))
                {
                    MessageBox.Show("Please fill all info and Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                string query = "update UserInformation" +
                    " set UserName = '" + this.txtUserName.Text +
                    "', UserId = '" + this.txtUserId.Text +
                    "', Password = '" + this.txtPassword.Text +
                    "', DateOfBirth = '" + this.dtpDOB.Text +
                    "' where UserId = '" + this.UserName + "'";
                var rowCount = this.Da.ExecuteDMLQuery(query);
                if (rowCount == 1)
                {
                    if(eye < 0)
                    this.btnEye_Click(sender, new EventArgs());
                MessageBox.Show("Data updated successfully");
                }
                else
                    MessageBox.Show("Data upgradation failed");
                this.ToggleReadOnly();
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
        }

        #endregion


        #endregion

        #region search Box,discount box & category
        private void txtDiscount_MouseClick(object sender, MouseEventArgs e)
        {
            this.txtDiscount_edit();
        }
        private void txtDiscount_Leave(object sender, EventArgs e)
        {
            this.txtDicount_empty();
        }

        private void txtDicount_empty()
        {
            if (this.txtDiscount.Text == "")
            {
                this.txtDiscount.Text = "in %";
                this.txtDiscount.ForeColor = Color.Gray;
            }
        }
        private void txtDiscount_edit()
        {
            if (this.txtDiscount.Text == "in %")
            {
                this.txtDiscount.Clear();
                this.txtDiscount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            }
        }
        private void txtSearchProduct_MouseClick(object sender, MouseEventArgs e)
        {
            txtSearchProduct_search();
        }

        private void txtSearchProduct_Leave(object sender, EventArgs e)
        {
            txtSearchProduct_empty();
        }

        private void txtSearchProduct_empty()
        {
            if (this.txtSearchProduct.Text == "")
            {
                this.txtSearchProduct.Text = "Search";
                this.txtSearchProduct.ForeColor = Color.DarkGray;
            }
        }
        private void txtSearchProduct_search()
        {
            if (this.txtSearchProduct.Text == "Search")
            {
                this.txtSearchProduct.Clear();
                this.txtSearchProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(48)))));
            }
        }
        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            var sql = "Select * from ProductInformation where ProductName LIKE '" + this.txtSearchProduct.Text + "%' or Category LIKE '" + this.txtSearchProduct.Text + "%' or Price LIKE '" + this.txtSearchProduct.Text + "%'";
            this.PopulateGridView(sql);
        }
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (this.cmbCategory.SelectedIndex == 0) this.PopulateGridView();
            else this.PopulateGridView("Select * from ProductInformation where Category = '" + this.cmbCategory.Text + "';");
        }
        #endregion



        #region Logout
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.pnlOrange.Focus();
            this.Login.Show();
            this.Hide();
        }

        #endregion

        #region Form Closing Cross 
        private void Employee_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Environment.Exit(1);
        }
        #endregion

        #region Gridview population & cart
        private void PopulateGridView(string sql = "select * from ProductInformation;")
        {
            var ds = this.Da.ExecuteQuery(sql);

            this.dgvCart.AutoGenerateColumns = false;
            this.dgvProducts.AutoGenerateColumns = false;
            if (ds.Tables[0].Rows.Count > 0)
            {
                this.dgvProducts.DataSource = ds.Tables[0];
            }
        }

        private void dgvProducts_DoubleClick(object sender, EventArgs e)
        {
            
            string cartProductName = this.dgvProducts.CurrentRow.Cells["ProductName"].Value.ToString();
            string cartPrice = this.dgvProducts.CurrentRow.Cells["Price"].Value.ToString();
            string cartQuantity = "1";
            string cartTotal = cartPrice;
            string cartProductId = this.dgvProducts.CurrentRow.Cells["ProductId"].Value.ToString();
            string cartTransactionId = "autogen";
            string cartSellerName = this.txtUserName.Text;


            for (int i = 0; i < dgvCart.Rows.Count; i++)
            {
                string columnValue = this.dgvCart.Rows[i].Cells["cartProductId"].Value.ToString();
                if (columnValue.Equals(cartProductId))
                {
                    MessageBox.Show("Already in the cart!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            this.dgvCart.Rows.Add();
            int rows = dgvCart.Rows.Count - 1;
            this.dgvCart.Rows[rows].Cells["cartProductName"].Value = cartProductName;
            this.dgvCart.Rows[rows].Cells["cartPrice"].Value = cartPrice;
            this.dgvCart.Rows[rows].Cells["cartQuantity"].Value = cartQuantity;
            this.dgvCart.Rows[rows].Cells["cartTotal"].Value = cartTotal;
            this.dgvCart.Rows[rows].Cells["cartProductId"].Value = cartProductId;
            this.dgvCart.Rows[rows].Cells["cartSellerName"].Value = cartSellerName;
            this.dgvCart.Rows[rows].Cells["cartTransactionId"].Value = cartTransactionId;

            this.txtUpdateQuantity.Text = "1";
            this.txtUpdateQuantity.Focus();
            this.txtUpdateQuantity.SelectionStart = 0;
            this.txtUpdateQuantity.SelectionLength = this.txtUpdateQuantity.Text.Length;
            dgvCart.CurrentCell = dgvCart.Rows[rows].Cells[0];
            dgvCart.CurrentRow.Selected = false;
            dgvCart.Rows[rows].Selected = true;
            string priceWithTk = this.dgvCart.CurrentRow.Cells["cartPrice"].Value.ToString();
            string[] price = priceWithTk.Split('T');
            this.lblSubtotal.Text = (double.Parse(this.lblSubtotal.Text) + double.Parse(price[0])).ToString();
            this.lblVat.Text = (double.Parse(this.lblSubtotal.Text) * 0.05).ToString();
            this.lblTotal.Text = (double.Parse(this.lblSubtotal.Text) + double.Parse(this.lblVat.Text)).ToString();
        }


        private void dgvCart_DoubleClick(object sender, EventArgs e)
        {
            this.txtUpdateQuantity.Text = this.dgvCart.CurrentRow.Cells["cartQuantity"].Value.ToString();
            this.txtUpdateQuantity.Focus();
            this.txtUpdateQuantity.SelectionStart = 0;
            this.txtUpdateQuantity.SelectionLength = this.txtUpdateQuantity.Text.Length;
        }

        private void btnUpdateCart_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this.dgvProducts.CurrentRow.Cells["Stock"].Value.ToString());
            try
            {
                if(double.Parse(this.txtUpdateQuantity.Text) <= 0)
                {
                    MessageBox.Show("You have to take atleast 1 item! Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }else if(double.Parse(this.dgvProducts.CurrentRow.Cells["Stock"].Value.ToString()) < double.Parse(this.txtUpdateQuantity.Text))
                {
                    MessageBox.Show("Stock Maximum limit reached! Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string cartTotalCurrentWithTk = this.dgvCart.CurrentRow.Cells["cartTotal"].Value.ToString();
                string[] cartTotalCurrent = cartTotalCurrentWithTk.Split('T');
                this.lblSubtotal.Text = (double.Parse(this.lblSubtotal.Text) - double.Parse(cartTotalCurrent[0])).ToString();

                string priceWithTk = this.dgvCart.CurrentRow.Cells["cartPrice"].Value.ToString();
                string[] price = priceWithTk.Split('T');
                double perUnitPrice = double.Parse(price[0]);
                this.dgvCart.CurrentRow.Cells["cartTotal"].Value = (perUnitPrice * double.Parse(this.txtUpdateQuantity.Text));
                this.lblSubtotal.Text = (double.Parse(this.lblSubtotal.Text) + double.Parse(this.dgvCart.CurrentRow.Cells["cartTotal"].Value.ToString())).ToString();
                this.lblVat.Text = (double.Parse(this.lblSubtotal.Text) * 0.05).ToString();
                this.lblTotal.Text = (double.Parse(this.lblSubtotal.Text) + double.Parse(this.lblVat.Text)).ToString();
                this.dgvCart.CurrentRow.Cells["cartTotal"].Value += " Tk";
                this.dgvCart.CurrentRow.Cells["cartQuantity"].Value = this.txtUpdateQuantity.Text;
                this.txtUpdateQuantity.Clear();

            }
            catch(Exception Ex)
            {
                MessageBox.Show("Something went wrong! Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnDeleteCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvCart.Rows.Count == 0)
                {
                    MessageBox.Show("Cart Empty. Nothing to delete!!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                string cartTotalCurrentWithTk = this.dgvCart.CurrentRow.Cells["cartTotal"].Value.ToString();
                string[] cartTotalCurrent = cartTotalCurrentWithTk.Split('T');
                this.lblSubtotal.Text = (double.Parse(this.lblSubtotal.Text) - double.Parse(cartTotalCurrent[0])).ToString();
                this.lblVat.Text = (double.Parse(this.lblSubtotal.Text) * 0.05).ToString();
                this.lblTotal.Text = (double.Parse(this.lblSubtotal.Text) + double.Parse(this.lblVat.Text)).ToString();
                int row = this.dgvCart.CurrentRow.Index;
                this.dgvCart.Rows.RemoveAt(row);
            }
            catch(Exception Ex)
            {
                MessageBox.Show("Something went wrong! Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void btnDiscountApply_Click(object sender, EventArgs e)
        {
            if(double.Parse(this.txtDiscount.Text) > 100)
            {
                MessageBox.Show("Maximum Discount is 100% ! Try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.lblSubtotal.Text = (double.Parse(this.lblSubtotal.Text) - (double.Parse(this.lblSubtotal.Text)*(double.Parse(this.txtDiscount.Text)/100)) ).ToString();
            //this.lblVat.Text = (double.Parse(this.lblSubtotal.Text) * 0.05).ToString();
            this.lblTotal.Text = (double.Parse(this.lblSubtotal.Text) + double.Parse(this.lblVat.Text)).ToString();
            // copy this to enable
            this.btnDeleteCart.Enabled = false;
            this.btnDiscountApply.Enabled = false;
            this.txtDiscount.Text += " % Applied";
            this.txtDiscount.Enabled = false;
            

        }

        #endregion

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            this.dgvCart.Rows.Clear();
            this.lblSubtotal.Text = "0";
            this.lblTotal.Text = "0";
            this.lblVat.Text = "0";
            this.btnDeleteCart.Enabled = true;
            this.btnDiscountApply.Enabled = true;
            this.txtDiscount.Enabled = true;
            this.txtDiscount.Text = "";
            this.txtUpdateQuantity.Text = "";
            this.txtDiscount_Leave(sender, e);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string trxId = AutoGenerateId();
            int count = 0;
            for (int i = 0; i < dgvCart.Rows.Count; i++)
            {
                string discount = this.txtDiscount.Text;
                if (!discount.Contains("Applied")) discount = "NA";
                var sql = @"insert into SellInformation values('" + this.dgvCart.Rows[i].Cells["cartProductId"].Value.ToString() + "','" + this.dgvCart.Rows[i].Cells["cartProductName"].Value.ToString() + "','" + this.dgvCart.Rows[i].Cells["cartQuantity"].Value.ToString() + "','" + this.dgvCart.Rows[i].Cells["cartPrice"].Value.ToString() + "','" + this.dgvCart.Rows[i].Cells["cartTotal"].Value.ToString() + "','" + trxId + "','" + this.dgvCart.Rows[i].Cells["cartSellerName"].Value.ToString() + "','" + DateTime.Today.ToString("dd/MM/yyyy") + "','" + this.lblSubtotal.Text + "','" + discount + "','" + this.lblTotal.Text + "');";
                count += this.Da.ExecuteDMLQuery(sql);
                string updatedQuantity = (double.Parse(this.dgvProducts.CurrentRow.Cells["Stock"].Value.ToString()) - double.Parse(this.dgvCart.Rows[i].Cells["cartQuantity"].Value.ToString())).ToString();
                var stockDeduct = "update ProductInformation set Quantity = '" + updatedQuantity + "' where ProductId = '" + this.dgvCart.Rows[i].Cells["cartProductId"].Value.ToString() + "';";
                this.Da.ExecuteDMLQuery(stockDeduct);
            }
            if (count != 0)
            MessageBox.Show("Data insertion successfull");
            else
            MessageBox.Show("Data insertion failed");

            this.btnClearCart_Click(sender, e);
            this.PopulateGridView();
        }


        #region Auto Generate Id For Transaction
        private string AutoGenerateId()
        {
            string id = "Trx-";
            string sql = "select TransactionId from SellInformation;";
            var ds = this.Da.ExecuteQueryTable(sql);
            int i = 1;
            if (ds.Rows.Count != 0)
            {
                i = Int32.Parse(ds.Rows[ds.Rows.Count - 1][0].ToString().Substring(4)) + 1;
                id += i.ToString("D3");
            }
            else
            {
                id += i.ToString("D3");
            }

            return id;
        }
        #endregion

        
    }
}
