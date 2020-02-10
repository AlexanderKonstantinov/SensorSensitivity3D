using SensorSensitivity3D.Infrastructure;
using System;
using System.Windows.Input;
using devDept.Eyeshot;
using SensorSensitivity3D.Domain.Enums;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.ViewModels.Base;

using static SensorSensitivity3D.Services.ModelInteractionService;


namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophoneViewModel : BaseViewModel
    {
        /// <summary>
        /// Вызывается при возвращении к панели с геофонами
        /// Сообщает подписчику, стоит ли оставить геофон в коллекции
        /// </summary>
        public event Action<GeophoneOperation, GeophoneModel> Back;
        public bool IsGeophonePanel { get; set; }
        public string PanelTitle { get; private set; }
        public bool IsEditedMode { get; set; }

        public GeophoneModel EditedGeophone { get; set; }


        public GeophoneViewModel() { }

        /// <summary>
        /// Активировать панель редактирования или добавления нового геофона,
        /// сделав её видимой и инициализировав привязываемые объекты
        /// </summary>
        /// <param name="editedGeophone">Геофон для редактирования</param>
        public void ActivateGeophoneViewModel(GeophoneModel editedGeophone)
        {
            IsEditedMode = editedGeophone != null;

            if (IsEditedMode)
            {
                EditedGeophone = editedGeophone;
                PanelTitle = "Редактировать геофон";
            }
            else
            {
                EditedGeophone = new GeophoneModel();
                PanelTitle = "Добавить геофон";
            }
            
            IsGeophonePanel = true;
        }


        private RelayCommand _changeGeophoneCommand;
        public ICommand ChangeGeophoneCommand
            => _changeGeophoneCommand ??= new RelayCommand(ExecuteChangeGeophoneCommand);

        private void ExecuteChangeGeophoneCommand(object o)
        {
            RemoveEntities(EditedGeophone.Entities);

            EditedGeophone.InitEntities();

            AddEntities(EditedGeophone.Entities);
        }


        private RelayCommand _backCommand;
        public ICommand BackCommand
            => _backCommand ??= new RelayCommand(ExecuteBackCommand, null);

        private void ExecuteBackCommand(object obj)
        {
            IsGeophonePanel = false;

            RemoveEntities(EditedGeophone.Entities);

            if (IsEditedMode)
            {
                EditedGeophone.ResetGeophoneSettings();
                AddEntities(EditedGeophone.Entities);
            }

            Back?.Invoke(GeophoneOperation.None, null);
        }


        private RelayCommand _saveGeophoneCommand;
        public ICommand SaveGeophoneCommand
            => _saveGeophoneCommand ??= new RelayCommand(ExecuteSaveGeophoneCommand, CanExecuteSaveGeophoneCommand);

        private void ExecuteSaveGeophoneCommand(object obj)
        {
            IsGeophonePanel = false;

            EditedGeophone.AcceptChanges(); // кажется это лишнее

            Back?.Invoke(GeophoneOperation.Edit, EditedGeophone);
        }

        private bool CanExecuteSaveGeophoneCommand(object obj) 
            => IsEditedMode 
            && (EditedGeophone?.IsChanged ?? false)
            && !string.IsNullOrEmpty(EditedGeophone.DisplayName);


        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand ??= new RelayCommand(ExecuteAddGeophoneCommand, CanExecuteAddGeophoneCommand);

        private void ExecuteAddGeophoneCommand(object obj)
        {
            var needToContinue = (bool) obj;

            IsGeophonePanel = false;

            if (IsEditedMode)
            {
                var newGeophone = new GeophoneModel(EditedGeophone);

                EditedGeophone.ResetGeophoneSettings();
                AddEntities(EditedGeophone.Entities);
                
                EditedGeophone = newGeophone;
            }

            Back?.Invoke(needToContinue 
                ? GeophoneOperation.AddAndContinueAdding
                : GeophoneOperation.Add, 
                EditedGeophone);
        }

        private bool CanExecuteAddGeophoneCommand(object obj)
            => !IsEditedMode 
            || (EditedGeophone?.IsChanged ?? false) 
            && !string.IsNullOrEmpty(EditedGeophone.DisplayName);


        private RelayCommand _resetGeophoneCommand;
        public ICommand ResetGeophoneCommand
            => _resetGeophoneCommand ??= new RelayCommand(ExecuteResetGeophoneCommand, CanExecuteResetGeophoneCommand);

        private void ExecuteResetGeophoneCommand(object obj)
        {
            RemoveEntities(EditedGeophone.Entities);

            EditedGeophone.ResetGeophoneSettings();

            AddEntities(EditedGeophone.Entities);            
        }

        private bool CanExecuteResetGeophoneCommand(object obj)
            => IsEditedMode && (EditedGeophone?.IsChanged ?? false);


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
