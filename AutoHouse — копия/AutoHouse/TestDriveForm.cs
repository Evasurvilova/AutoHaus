using System;
using System.Windows.Forms;

namespace AutohausManagementSystem
{
    public partial class TestDriveForm : Form
    {
        private readonly DatabaseHelper dbHelper;
        private readonly int clientId;
        private readonly int carId;
        private readonly string carModel;
        private DateTimePicker dtpDate;
        private DateTimePicker dtpTime;
        private Button btnSubmit;
        private Button btnBack;

        public TestDriveForm(DatabaseHelper dbHelper, int clientId, int carId, string carModel)
        {
            this.dbHelper = dbHelper;
            this.clientId = clientId;
            this.carId = carId;
            this.carModel = carModel;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Запись на тест-драйв";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Label lblTitle = new Label();
            lblTitle.Text = $"Тест-драйв {carModel}";
            lblTitle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(50, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            Label lblDate = new Label();
            lblDate.Text = "Дата:";
            lblDate.Location = new System.Drawing.Point(50, 70);
            this.Controls.Add(lblDate);

            this.dtpDate = new DateTimePicker();
            this.dtpDate.Location = new System.Drawing.Point(150, 70);
            this.dtpDate.Size = new System.Drawing.Size(200, 20);
            this.dtpDate.MinDate = DateTime.Today;
            this.Controls.Add(this.dtpDate);

            Label lblTime = new Label();
            lblTime.Text = "Время:";
            lblTime.Location = new System.Drawing.Point(50, 110);
            this.Controls.Add(lblTime);

            this.dtpTime = new DateTimePicker();
            this.dtpTime.Location = new System.Drawing.Point(150, 110);
            this.dtpTime.Size = new System.Drawing.Size(200, 20);
            this.dtpTime.Format = DateTimePickerFormat.Time;
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Value = DateTime.Now.AddHours(1);
            this.Controls.Add(this.dtpTime);

            this.btnSubmit = new Button();
            this.btnSubmit.Text = "Записаться";
            this.btnSubmit.Location = new System.Drawing.Point(150, 160);
            this.btnSubmit.Size = new System.Drawing.Size(100, 30);
            this.btnSubmit.Click += new EventHandler(this.btnSubmit_Click);
            this.Controls.Add(this.btnSubmit);

            this.btnBack = new Button();
            this.btnBack.Text = "Назад";
            this.btnBack.Location = new System.Drawing.Point(150, 200);
            this.btnBack.Size = new System.Drawing.Size(100, 30);
            this.btnBack.Click += new EventHandler(this.btnBack_Click);
            this.Controls.Add(this.btnBack);
        }

        private void btnSubmit_Click(object? sender, EventArgs e)
        {
            DateTime scheduledDateTime = dtpDate.Value.Date + dtpTime.Value.TimeOfDay;

            if (scheduledDateTime < DateTime.Now.AddHours(1))
            {
                MessageBox.Show("Выберите время минимум на час позже текущего", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int testDriveId = dbHelper.ScheduleTestDrive(
                clientId,
                carId,
                scheduledDateTime);

            if (testDriveId > 0)
            {
                MessageBox.Show($"Вы записаны на тест-драйв {carModel} на {scheduledDateTime:dd.MM.yyyy HH:mm}",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Возвращаемся на главную страницу
                var catalogForm = new CarCatalogForm(dbHelper, clientId);
                catalogForm.Show();
                this.Hide();
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            var carSelectionForm = new CarSelectionForm(dbHelper, clientId, carId);
            carSelectionForm.Show();
            this.Hide();
        }
    }
}