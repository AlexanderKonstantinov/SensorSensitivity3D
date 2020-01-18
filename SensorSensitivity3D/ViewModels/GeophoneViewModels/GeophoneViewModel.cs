using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using System;
using System.Windows.Input;
using devDept.Eyeshot;
using SensorSensitivity3D.Domain.Enums;
using SensorSensitivity3D.Domain.Models;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophoneViewModel : BaseViewModel
    {
        private readonly GeophoneConversionService _geophoneConversionService;

        private readonly Model _model;
        private readonly CustomEntityList _entityList;

        /// <summary>
        /// Вызывается при возвращении к панели с геофонами
        /// Сообщает подписчику, стоит ли оставить геофон в коллекции
        /// </summary>
        public event Action<GeophoneOperation> Back;
        public bool IsGeophonePanel { get; set; }
        public string PanelTitle { get; private set; }

        public GeophoneModel EditedGeophone { get; set; }


        public GeophoneViewModel() { }
        public GeophoneViewModel(Model model, CustomEntityList entities)
        {
            _model = model;
            _entityList = entities;
        }

        /// <summary>
        /// Активировать панель редактирования или добавления нового геофона,
        /// сделав её видимой и инициализировав привязываемые объекты
        /// </summary>
        /// <param name="originalGeophone">Геофон для редактирования</param>
        /// <param name="panelTitle">Заголовок панели</param>
        public void ActivateGeophoneViewModel(string panelTitle, GeophoneModel originalGeophone)
        {
            PanelTitle = panelTitle;

            EditedGeophone = originalGeophone;
            
            IsGeophonePanel = true;
        }


        private RelayCommand _changeColorCommand;
        public ICommand ChangeColorCommand
            => _changeColorCommand ??= new RelayCommand(ExecuteChangeColorCommand);

        private void ExecuteChangeColorCommand(object o)
        {
            EditedGeophone.Color = o.ToString();
            OnPropertyChanged(nameof(EditedGeophone));
        }


        private RelayCommand _backCommand;
        public ICommand BackCommand
            => _backCommand ??= new RelayCommand(ExecuteBackCommand, null);

        private void ExecuteBackCommand(object obj)
        {
            IsGeophonePanel = false;

            EditedGeophone.ResetGeophoneSettings();

            Back?.Invoke(GeophoneOperation.None);
        }


        private RelayCommand _saveGeophoneCommand;
        public ICommand SaveGeophoneCommand
            => _saveGeophoneCommand ??= new RelayCommand(ExecuteSaveGeophoneCommand, CanExecuteSaveGeophoneCommand);

        private void ExecuteSaveGeophoneCommand(object obj)
        {
            IsGeophonePanel = false;

            Back?.Invoke(GeophoneOperation.Save);
        }

        private bool CanExecuteSaveGeophoneCommand(object obj) 
            => EditedGeophone?.OriginalGeophone != null 
               && EditedGeophone.IsChanged;


        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand ??= new RelayCommand(ExecuteAddGeophoneCommand, CanExecuteAddGeophoneCommand);

        private void ExecuteAddGeophoneCommand(object obj)
        {
            IsGeophonePanel = (bool) obj;

            Back?.Invoke(GeophoneOperation.SaveAndContinueAdding);
        }

        private bool CanExecuteAddGeophoneCommand(object obj)
            => EditedGeophone?.OriginalGeophone == null || EditedGeophone.IsChanged;


        private RelayCommand _resetGeophoneCommand;
        public ICommand ResetGeophoneCommand
            => _resetGeophoneCommand ??= new RelayCommand(ExecuteResetGeophoneCommand, CanExecuteResetGeophoneCommand);

        private void ExecuteResetGeophoneCommand(object obj)
        {
            EditedGeophone.ResetGeophoneSettings();
        }

        private bool CanExecuteResetGeophoneCommand(object obj)
            => EditedGeophone?.OriginalGeophone != null && EditedGeophone.IsChanged;


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
