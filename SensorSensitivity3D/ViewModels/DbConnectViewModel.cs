using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;

namespace SensorSensitivity3D.ViewModels
{
    public class DbConnectViewModel : BaseViewModel
    {
        /// <summary>
        /// Вызывается при успешном подключении к базе данных
        /// </summary>
        public Action<string> OnSuccessConnection;

        private string ConnectionString 
            => $@"Data Source={ServerName};Initial Catalog={DbName};User ID={UserName};Password={Password};Connection Timeout=5";

        public string ServerName { get; set; }
        public string  DbName { get; set; }
        public string  UserName { get; set; }
        public string  Password { get; set; }
        
        public DbConnectViewModel()
        {
            ServerName = "(local)";
            DbName = "Apatit";
            UserName = "sa";
            Password = "2006";
        }


        private RelayCommand _connectDBCommand;
        public ICommand ConnectDBCommand
            => _connectDBCommand ??= new RelayCommand(ExecuteConnectDBCommand, CanExecuteConnectDBCommand);

        /// <summary>
        /// Попытка установления соединения с БД
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteConnectDBCommand(object obj)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OnSuccessConnection?.Invoke(ConnectionString);
                }
                catch (SqlException)
                {
                    MessageBox.Show(
                        "Подключение к базе данных не выполнено. Проверьте достоверность введенных данных.",
                        "Ошибка подключения",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private bool CanExecuteConnectDBCommand(object obj)
            => !(string.IsNullOrEmpty(ServerName)
               || string.IsNullOrEmpty(DbName));

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
