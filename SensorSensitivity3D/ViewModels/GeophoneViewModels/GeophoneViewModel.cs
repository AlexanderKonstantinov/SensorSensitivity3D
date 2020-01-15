using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using System;
using System.Windows.Input;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophoneViewModel : BaseViewModel
    {
        private readonly GeophoneConversionService _geophoneConversionService;

        /// <summary>
        /// Вызывается при возвращении к панели с геофонами
        /// </summary>
        public event Action<Geophone> Back;

        public bool IsGeophonePanel { get; set; }

        public Geophone OriginalGeophone { get; private set; }
        public Geophone EditedGeophone { get; set; }

        public string PanelTitle { get; private set; }


        public GeophoneViewModel() { }

        public GeophoneViewModel(GeophoneConversionService service)
        {
            _geophoneConversionService = service;
        }

        /// <summary>
        /// Активировать панель редактирования или добавления нового геофона,
        /// сделав её видимой и инициализировав привязываемые объекты
        /// </summary>
        /// <param name="originalGeophone">Оригинальный геофон</param>
        /// <param name="editedGeophone">Геофон для редактирования, полученный на основании модели</param>
        /// <param name="panelTitle">Заголовок панели</param>
        public void ActivateGeophoneViewModel(Geophone originalGeophone, Geophone editedGeophone, string panelTitle)
        {
            PanelTitle = panelTitle;

            OriginalGeophone = originalGeophone;

            EditedGeophone = editedGeophone;

            IsGeophonePanel = true;
        }


        private RelayCommand _backCommand;
        public ICommand BackCommand
            => _backCommand ??= new RelayCommand(ExecuteBackCommand, null);

        private void ExecuteBackCommand(object obj)
        {
            Back?.Invoke(null);

            IsGeophonePanel = false;
        }


        private RelayCommand _changeColorCommand;
        public ICommand ChangeColorCommand
            => _changeColorCommand ??= new RelayCommand(ExecuteChangeColorCommand);

        private void ExecuteChangeColorCommand(object o)
        {
            EditedGeophone.Color = o.ToString();

            OnPropertyChanged(nameof(EditedGeophone));
        }


        private RelayCommand _saveGeophoneCommand;
        public ICommand SaveGeophoneCommand
            => _saveGeophoneCommand ??= new RelayCommand(ExecuteSaveGeophoneCommand, CanExecuteSaveGeophoneCommand);

        private void ExecuteSaveGeophoneCommand(object obj)
        {
            OriginalGeophone = _geophoneConversionService.Copy(OriginalGeophone, EditedGeophone);

            Back?.Invoke(OriginalGeophone);

            IsGeophonePanel = false;
        }

        private bool CanExecuteSaveGeophoneCommand(object obj) 
            => OriginalGeophone != null 
            && !OriginalGeophone.Equals(EditedGeophone);


        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand ??= new RelayCommand(ExecuteAddGeophoneCommand, CanExecuteAddGeophoneCommand);

        private void ExecuteAddGeophoneCommand(object obj)
        {
            var addYet = (bool) obj;

            OriginalGeophone = _geophoneConversionService.CopyConstructor(EditedGeophone);

            Back?.Invoke(OriginalGeophone);

            IsGeophonePanel = addYet;
        }

        private bool CanExecuteAddGeophoneCommand(object obj)
            => OriginalGeophone is null || !OriginalGeophone.Equals(EditedGeophone);


        private RelayCommand _resetGeophoneCommand;
        public ICommand ResetGeophoneCommand
            => _resetGeophoneCommand ??= new RelayCommand(ExecuteResetGeophoneCommand, CanExecuteResetGeophoneCommand);

        private void ExecuteResetGeophoneCommand(object obj)
        {
            EditedGeophone = _geophoneConversionService.Copy(EditedGeophone, OriginalGeophone);

            OnPropertyChanged(nameof(EditedGeophone));
        }

        private bool CanExecuteResetGeophoneCommand(object obj)
            => OriginalGeophone != null && !OriginalGeophone.Equals(EditedGeophone);


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
