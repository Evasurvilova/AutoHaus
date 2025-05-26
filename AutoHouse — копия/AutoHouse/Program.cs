using System;
using System.Windows.Forms;

namespace AutohausManagementSystem
{
    static class Program
    {
        public static DatabaseHelper? DbHelper { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // ������������� ����������� � ��
                DbHelper = new DatabaseHelper("localhost", "Autohaus", "postgres", "7994821Kk.");

                Application.Run(new AuthForm(DbHelper));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"����������� ������ ��� ������� ����������: {ex.Message}",
                    "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}