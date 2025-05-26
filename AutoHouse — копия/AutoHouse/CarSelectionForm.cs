using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AutohausManagementSystem
{
    public partial class CarSelectionForm : Form
    {
        private readonly DatabaseHelper dbHelper;
        private readonly int clientId;
        private readonly int brandId;
        private List<CarModel> availableCars;
        private DataGridView dgvCars;
        private Button btnTestDrive;
        private Button btnBack;

        public CarSelectionForm(DatabaseHelper dbHelper, int clientId, int brandId)
        {
            this.dbHelper = dbHelper;
            this.clientId = clientId;
            this.brandId = brandId;
            this.availableCars = new List<CarModel>();
            InitializeComponent();
            LoadAvailableCars();
        }

        private void InitializeComponent()
        {
            this.Text = "Выбор автомобиля";
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Label lblTitle = new Label();
            lblTitle.Text = "Доступные автомобили";
            lblTitle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(50, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            this.dgvCars = new DataGridView();
            this.dgvCars.Location = new System.Drawing.Point(50, 60);
            this.dgvCars.Size = new System.Drawing.Size(500, 350);
            this.dgvCars.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvCars.MultiSelect = false;
            this.dgvCars.ReadOnly = true;
            this.dgvCars.Columns.Add("Id", "ID");
            this.dgvCars.Columns.Add("Model", "Модель");
            this.dgvCars.Columns.Add("Color", "Цвет");
            this.dgvCars.Columns.Add("Price", "Цена");
            this.dgvCars.Columns["Id"].Visible = false;
            this.dgvCars.Columns["Price"].DefaultCellStyle.Format = "C";
            this.Controls.Add(this.dgvCars);

            this.btnTestDrive = new Button();
            this.btnTestDrive.Text = "Записаться на тест-драйв";
            this.btnTestDrive.Location = new System.Drawing.Point(300, 420);
            this.btnTestDrive.Size = new System.Drawing.Size(200, 40);
            this.btnTestDrive.Click += new EventHandler(this.btnTestDrive_Click);
            this.Controls.Add(this.btnTestDrive);

            this.btnBack = new Button();
            this.btnBack.Text = "Назад";
            this.btnBack.Location = new System.Drawing.Point(50, 420);
            this.btnBack.Size = new System.Drawing.Size(200, 40);
            this.btnBack.Click += new EventHandler(this.btnBack_Click);
            this.Controls.Add(this.btnBack);
        }

        private void LoadAvailableCars()
        {
            availableCars = dbHelper.GetAvailableCars(brandId);
            if (availableCars == null || availableCars.Count == 0)
            {
                MessageBox.Show("Нет доступных автомобилей выбранного бренда", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnBack.PerformClick();
                return;
            }

            dgvCars.Rows.Clear();
            foreach (var car in availableCars)
            {
                dgvCars.Rows.Add(
                    car.Id,
                    car.ModelName,
                    car.Color,
                    car.Price);
            }
        }

        private void btnTestDrive_Click(object? sender, EventArgs e)
        {
            if (dgvCars.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите автомобиль для тест-драйва", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int carId = (int)dgvCars.SelectedRows[0].Cells["Id"].Value;
            string modelName = dgvCars.SelectedRows[0].Cells["Model"].Value.ToString() ?? "";

            var testDriveForm = new TestDriveForm(dbHelper, clientId, carId, modelName);
            testDriveForm.Show();
            this.Hide();
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            var catalogForm = new CarCatalogForm(dbHelper, clientId);
            catalogForm.Show();
            this.Hide();
        }
    }
}