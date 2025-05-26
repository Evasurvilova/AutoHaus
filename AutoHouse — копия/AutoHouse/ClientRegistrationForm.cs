using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Npgsql;

namespace AutohausManagementSystem
{
    public partial class AuthForm : Form
    {
        private readonly DatabaseHelper dbHelper;
        private Button btnLogin;
        private Button btnSwitchToRegister;
        private MaskedTextBox mtxtPhone;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private MaskedTextBox mtxtPhoneReg;
        private TextBox txtEmail;
        private TextBox txtLicenseNumber;
        private Panel pnlLogin;
        private Panel pnlRegister;
        private Label lblTitle;

        public AuthForm(DatabaseHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Авторизация";
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Общий заголовок
            lblTitle = new Label();
            lblTitle.Text = "Авторизация";
            lblTitle.Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(180, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            // Панель входа
            pnlLogin = new Panel();
            pnlLogin.Location = new System.Drawing.Point(50, 70);
            pnlLogin.Size = new System.Drawing.Size(400, 150);
            pnlLogin.BorderStyle = BorderStyle.FixedSingle;

            Label lblPhone = new Label();
            lblPhone.Text = "Номер телефона:";
            lblPhone.Location = new System.Drawing.Point(20, 30);
            pnlLogin.Controls.Add(lblPhone);

            mtxtPhone = new MaskedTextBox();
            mtxtPhone.Mask = "+7(000)000-0000";
            mtxtPhone.Location = new System.Drawing.Point(150, 30);
            mtxtPhone.Size = new System.Drawing.Size(200, 20);
            mtxtPhone.ValidatingType = typeof(string);
            mtxtPhone.BeepOnError = true;
            mtxtPhone.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            mtxtPhone.KeyPress += mtxtPhone_KeyPress;
            pnlLogin.Controls.Add(mtxtPhone);

            btnLogin = new Button();
            btnLogin.Text = "Вход";
            btnLogin.Location = new System.Drawing.Point(150, 70);
            btnLogin.Size = new System.Drawing.Size(150, 30);
            btnLogin.Click += btnLogin_Click;
            pnlLogin.Controls.Add(btnLogin);

            this.Controls.Add(pnlLogin);

            // Панель регистрации
            pnlRegister = new Panel();
            pnlRegister.Location = new System.Drawing.Point(50, 70);
            pnlRegister.Size = new System.Drawing.Size(400, 250);
            pnlRegister.BorderStyle = BorderStyle.FixedSingle;
            pnlRegister.Visible = false;

            Label lblFirstName = new Label();
            lblFirstName.Text = "Имя:";
            lblFirstName.Location = new System.Drawing.Point(20, 20);
            pnlRegister.Controls.Add(lblFirstName);

            txtFirstName = new TextBox();
            txtFirstName.Location = new System.Drawing.Point(150, 20);
            txtFirstName.Size = new System.Drawing.Size(200, 20);
            pnlRegister.Controls.Add(txtFirstName);

            Label lblLastName = new Label();
            lblLastName.Text = "Фамилия:";
            lblLastName.Location = new System.Drawing.Point(20, 50);
            pnlRegister.Controls.Add(lblLastName);

            txtLastName = new TextBox();
            txtLastName.Location = new System.Drawing.Point(150, 50);
            txtLastName.Size = new System.Drawing.Size(200, 20);
            pnlRegister.Controls.Add(txtLastName);

            Label lblPhoneReg = new Label();
            lblPhoneReg.Text = "Телефон:";
            lblPhoneReg.Location = new System.Drawing.Point(20, 80);
            pnlRegister.Controls.Add(lblPhoneReg);

            mtxtPhoneReg = new MaskedTextBox();
            mtxtPhoneReg.Mask = "+7(000)000-0000";
            mtxtPhoneReg.Location = new System.Drawing.Point(150, 80);
            mtxtPhoneReg.Size = new System.Drawing.Size(200, 20);
            mtxtPhoneReg.ValidatingType = typeof(string);
            mtxtPhoneReg.BeepOnError = true;
            mtxtPhoneReg.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            mtxtPhoneReg.KeyPress += mtxtPhone_KeyPress;
            pnlRegister.Controls.Add(mtxtPhoneReg);

            Label lblEmail = new Label();
            lblEmail.Text = "Email (необязательно):";
            lblEmail.Location = new System.Drawing.Point(20, 110);
            pnlRegister.Controls.Add(lblEmail);

            txtEmail = new TextBox();
            txtEmail.Location = new System.Drawing.Point(150, 110);
            txtEmail.Size = new System.Drawing.Size(200, 20);
            pnlRegister.Controls.Add(txtEmail);

            Label lblLicense = new Label();
            lblLicense.Text = "Номер прав (необязательно):";
            lblLicense.Location = new System.Drawing.Point(20, 140);
            pnlRegister.Controls.Add(lblLicense);

            txtLicenseNumber = new TextBox();
            txtLicenseNumber.Location = new System.Drawing.Point(150, 140);
            txtLicenseNumber.Size = new System.Drawing.Size(200, 20);
            pnlRegister.Controls.Add(txtLicenseNumber);

            Button btnRegister = new Button();
            btnRegister.Text = "Зарегистрироваться";
            btnRegister.Location = new System.Drawing.Point(150, 180);
            btnRegister.Size = new System.Drawing.Size(150, 30);
            btnRegister.Click += btnRegister_Click;
            pnlRegister.Controls.Add(btnRegister);

            this.Controls.Add(pnlRegister);

            // Кнопка переключения между формами
            btnSwitchToRegister = new Button();
            btnSwitchToRegister.Text = "У меня еще нет аккаунта";
            btnSwitchToRegister.Location = new System.Drawing.Point(150, 330);
            btnSwitchToRegister.Size = new System.Drawing.Size(200, 30);
            btnSwitchToRegister.Click += btnSwitchToRegister_Click;
            this.Controls.Add(btnSwitchToRegister);
        }

        private void mtxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            // Удаляем все нецифровые символы
            string digits = Regex.Replace(phone, @"[^\d]", "");

            // Приводим к формату 89*********
            if (digits.StartsWith("7") && digits.Length == 11)
                return "8" + digits.Substring(1);
            else if (digits.StartsWith("8") && digits.Length == 11)
                return digits;
            else if (digits.Length == 10)
                return "8" + digits;
            else
                return digits; // оставляем как есть, если формат не распознан
        }

        private bool IsValidFullName(string fullName)
        {
            return Regex.IsMatch(fullName, @"^[а-яА-ЯёЁa-zA-Z\s]{2,}(?:\s+[а-яА-ЯёЁa-zA-Z]{2,})+$");
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            string phone = FormatPhoneNumber(mtxtPhone.Text);

            if (phone.Length != 11 || !phone.StartsWith("89"))
            {
                MessageBox.Show("Пожалуйста, введите полный номер телефона в формате +7(9**)*******", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? clientId = dbHelper.GetClientIdByPhone(phone);

            if (clientId.HasValue)
            {
                var catalogForm = new CarCatalogForm(dbHelper, clientId.Value);
                catalogForm.Show();
                this.Hide();
            }
            else
            {
                var result = MessageBox.Show("Аккаунт не найден. Хотите зарегистрироваться?",
                    "Аккаунт не найден",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    pnlLogin.Visible = false;
                    pnlRegister.Visible = true;
                    lblTitle.Text = "Регистрация";
                    mtxtPhoneReg.Text = mtxtPhone.Text;
                    txtFirstName.Focus();
                }
            }
        }

        private void btnRegister_Click(object? sender, EventArgs e)
        {
            string fullName = $"{txtFirstName.Text} {txtLastName.Text}";

            if (!IsValidFullName(fullName))
            {
                MessageBox.Show("Пожалуйста, введите корректное имя и фамилию (только буквы)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phone = FormatPhoneNumber(mtxtPhoneReg.Text);

            if (phone.Length != 11 || !phone.StartsWith("89"))
            {
                MessageBox.Show("Пожалуйста, введите полный номер телефона в формате +7(9**)*******", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int clientId = dbHelper.AddClient(
                txtFirstName.Text.Trim(),
                txtLastName.Text.Trim(),
                phone,
                string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                string.IsNullOrWhiteSpace(txtLicenseNumber.Text) ? null : txtLicenseNumber.Text.Trim());

            if (clientId > 0)
            {
                var catalogForm = new CarCatalogForm(dbHelper, clientId);
                catalogForm.Show();
                this.Hide();
            }
        }

        private void btnSwitchToRegister_Click(object? sender, EventArgs e)
        {
            pnlLogin.Visible = !pnlLogin.Visible;
            pnlRegister.Visible = !pnlRegister.Visible;
            lblTitle.Text = pnlLogin.Visible ? "Авторизация" : "Регистрация";
            btnSwitchToRegister.Text = pnlLogin.Visible ? "У меня еще нет аккаунта" : "У меня уже есть аккаунт";

            if (pnlRegister.Visible && !string.IsNullOrEmpty(mtxtPhone.Text))
            {
                mtxtPhoneReg.Text = mtxtPhone.Text;
            }
        }
    }
}