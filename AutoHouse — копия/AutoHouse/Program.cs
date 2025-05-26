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
                // Инициализация подключения к БД
                DbHelper = new DatabaseHelper("localhost", "Autohaus", "postgres", "7994821Kk.");

                Application.Run(new AuthForm(DbHelper));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при запуске приложения: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}