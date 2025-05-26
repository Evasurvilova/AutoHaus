using System;
using System.Windows.Forms;

namespace AutohausManagementSystem
{
    public partial class ConfirmationForm : Form
    {
        private Label lblTitle;
        private Label lblMessage;
        private Button btnClose;

        public ConfirmationForm(string title, string message)
        {
            InitializeComponent(title, message);
        }

        private void InitializeComponent(string title, string message)
        {
            this.Text = "Подтверждение";
            this.ClientSize = new System.Drawing.Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.lblTitle = new Label();
            this.lblTitle.Text = title;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(50, 30);
            this.lblTitle.AutoSize = true;
            this.Controls.Add(this.lblTitle);

            this.lblMessage = new Label();
            this.lblMessage.Text = message;
            this.lblMessage.Font = new System.Drawing.Font("Arial", 10);
            this.lblMessage.Location = new System.Drawing.Point(50, 70);
            this.lblMessage.AutoSize = true;
            this.Controls.Add(this.lblMessage);

            this.btnClose = new Button();
            this.btnClose.Text = "Закрыть";
            this.btnClose.Location = new System.Drawing.Point(150, 120);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            this.Controls.Add(this.btnClose);
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}