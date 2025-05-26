using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AutohausManagementSystem
{
    public partial class CarCatalogForm : Form
    {
        private readonly DatabaseHelper dbHelper;
        private readonly int clientId;
        private List<CarBrand> carBrands;
        private FlowLayoutPanel pnlBrands;
        private Button btnBack;

        public CarCatalogForm(DatabaseHelper dbHelper, int clientId)
        {
            this.dbHelper = dbHelper;
            this.clientId = clientId;
            this.carBrands = new List<CarBrand>();
            InitializeComponent();
            LoadCarBrands();
        }

        private void InitializeComponent()
        {
            this.Text = "Каталог автомобилей";
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Label lblTitle = new Label();
            lblTitle.Text = "Выберите марку автомобиля";
            lblTitle.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(50, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            this.pnlBrands = new FlowLayoutPanel();
            this.pnlBrands.Location = new System.Drawing.Point(50, 60);
            this.pnlBrands.Size = new System.Drawing.Size(500, 400);
            this.pnlBrands.AutoScroll = true;
            this.Controls.Add(this.pnlBrands);

            this.btnBack = new Button();
            this.btnBack.Text = "Назад";
            this.btnBack.Location = new System.Drawing.Point(50, 470);
            this.btnBack.Size = new System.Drawing.Size(100, 30);
            this.btnBack.Click += new EventHandler(this.btnBack_Click);
            this.Controls.Add(this.btnBack);

            this.FormClosed += new FormClosedEventHandler(this.CarCatalogForm_FormClosed);
        }

        private void LoadCarBrands()
        {
            carBrands = dbHelper.GetCarBrands();
            if (carBrands == null || carBrands.Count == 0)
            {
                MessageBox.Show("Нет доступных автомобильных брендов", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            pnlBrands.Controls.Clear();

            foreach (var brand in carBrands)
            {
                var btn = new Button
                {
                    Text = $"{brand.Name} {(brand.Country != null ? $"({brand.Country})" : "")}",
                    Tag = brand.Id,
                    Width = 200,
                    Height = 50,
                    Margin = new Padding(10)
                };
                btn.Click += BrandButton_Click;
                pnlBrands.Controls.Add(btn);
            }
        }

        private void BrandButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is int brandId)
            {
                var carSelectionForm = new CarSelectionForm(dbHelper, clientId, brandId);
                carSelectionForm.Show();
                this.Hide();
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            var registrationForm = new AuthForm(dbHelper);
            registrationForm.Show();
            this.Hide();
        }

        private void CarCatalogForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}