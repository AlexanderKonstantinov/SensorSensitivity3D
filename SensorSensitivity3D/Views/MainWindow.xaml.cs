using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using devDept.Eyeshot;
using SensorSensitivity3D.DAL;
using SensorSensitivity3D.ViewModels;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Environment = devDept.Eyeshot.Environment;
using ToolBar = devDept.Eyeshot.ToolBar;

namespace SensorSensitivity3D.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        
        public MainWindow()
        {
            InitializeComponent();

            // performance settings
            Model.DisplayMode = displayType.Rendered;
            Model.Renderer = rendererType.Native;
            Model.ShowFps = true;
            Model.Shaded.ShowEdges = false;
            Model.Shaded.ShowInternalWires = false;
            Model.Shaded.ShadowMode = devDept.Graphics.shadowType.None;
            Model.Rendered.ShowEdges = false;
            Model.Rendered.RealisticShadowQuality = devDept.Graphics.realisticShadowQualityType.Low;
            Model.Rendered.ShadowMode = devDept.Graphics.shadowType.None;
            Model.AskForAntiAliasing = true;
            Model.ObjectManipulator.Apply();

            var sectionLayer = "Section";
            Model.Layers.Add(new Layer(sectionLayer, System.Drawing.Color.Red, null, 3, true));

            // Указание на путь к БД или создание файла БД
            var resDir = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
            Directory.CreateDirectory(resDir);

            var dbPath = Path.Combine(resDir, "SS3D.sqlite");

            Context.ContextConfiguring(dbPath);

            if (!File.Exists(dbPath))
                (new Context()).Database.EnsureCreated();

            _viewModel = new MainViewModel(Model);

            DataContext = _viewModel;

            InitializeToolbarButtons();

            //GeophoneTabItem.Content = new Geophones();

            //var factory = new Context.ContextFactory();

            //using (var dataContext = factory.CreateDbContext(null))
            //{
            //    //dataContext.Geophone.Add(new MyEntity { MyColumn = "aaa" });
            //    //dataContext.MyTable.Add(new MyEntity { MyColumn = "bbb" });
            //    //dataContext.MyTable.Add(new MyEntity { MyColumn = "ccc" });

            //    //dataContext.SaveChanges();

            //    //foreach (var cat in dataContext.MyTable.ToList())
            //    //{
            //    //    Console.WriteLine($@"CategoryId= {cat.MyColumn}, CategoryName = {cat.MyColumn}");
            //    //}

            //    Console.ReadLine();
            //}
        }

        private void InitializeToolbarButtons()
        {
            
            // removes some of the standard Eyeshot buttons
            var buttons = new List<ToolBarButton>();

            var separatorButton = new devDept.Eyeshot.ToolBarButton(
                null,
                "Separator",
                "",
                devDept.Eyeshot.ToolBarButton.styleType.Separator,
                true);
            
            var settingsButton = new devDept.Eyeshot.ToolBarButton(
                new BitmapImage(GetUriFromResource("rightPanel.png")),
                "RightPanelButton",
                "Показать меню объектов",
                devDept.Eyeshot.ToolBarButton.styleType.PushButton,
                true);


            //// Add a separator button            
            //buttons.Add(new devDept.Eyeshot.ToolBarButton(null, "Separator", "", devDept.Eyeshot.ToolBarButton.styleType.Separator, true));

            //buttons.Add(model1.GetToolBar().Buttons[0]);
            //buttons.Add(model1.GetToolBar().Buttons[1]);

            //// Add a separator button            
            //buttons.Add(new devDept.Eyeshot.ToolBarButton(null, "Separator", "", devDept.Eyeshot.ToolBarButton.styleType.Separator, true));

            //BitmapImage usersBmp = new BitmapImage(GetUriFromResource("users.png"));
            //buttons.Add(new devDept.Eyeshot.ToolBarButton(usersBmp, "MyPushButton", "MyPushButton", devDept.Eyeshot.ToolBarButton.styleType.PushButton, true));

            //BitmapImage gearsBmp = new BitmapImage(GetUriFromResource("gears.png"));
            //buttons.Add(new devDept.Eyeshot.ToolBarButton(gearsBmp, "MyToggleButton", "MyToggleButton", devDept.Eyeshot.ToolBarButton.styleType.ToggleButton, true));

            //Model.GetToolBar().Buttons = new ToolBarButtonList(Model.GetToolBar(), buttons);

            //var leftToolbar = new devDept.Eyeshot.ToolBar();
            //leftToolbar.Position = ToolBar.positionType.HorizontalTopLeft;
            //leftToolbar.Buttons.Add(settingsButton);

            var rightToolbar = new ToolBar();
            rightToolbar.Position = ToolBar.positionType.HorizontalTopRight;

        }

        private static Uri GetUriFromResource(string resourceFilename)
        {
            return new Uri(@"pack://application:,,,/Resources/Icons/" + resourceFilename);
        }

        private void RightPanel_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IInputElement focusableElement = FocusManagerHelper.GetFocusedElement(this);

            //if (focusableElement is RadSplitButton)
                //RightPanel.Focus();
        }

        private void NewConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            NewConfigurationPanel.Visibility = Visibility.Visible;
            NewConfigurationButton.Visibility = Visibility.Collapsed;
            NewConfigurationText.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NewConfigurationPanel.Visibility == Visibility.Visible 
                && !NewConfigurationText.IsMouseOver
                && !AddConfigButton.IsMouseOver)
            {
                NewConfigurationPanel.Visibility = Visibility.Collapsed;
                NewConfigurationButton.Visibility = Visibility.Visible;
            }
        }

        private void SwitchNameConfigFieldVisibility(object sender, RoutedEventArgs e)
        {
            NameConfigField.IsOpen = !NameConfigField.IsOpen;

            if (NameConfigField.IsOpen)
                EditedConfigName.Focus();   
        }

        private void EditedConfigName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (EditedConfigName.CaretIndex == 0)
                EditedConfigName.CaretIndex = EditedConfigName.Text.Length;
        }

        private void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            NewConfigurationPanel.Visibility = Visibility.Collapsed;
            NewConfigurationButton.Visibility = Visibility.Visible;
        }

        private void ModelSpaceLeave(object sender, MouseEventArgs e)
        {
            EntityInfo.Text = null;
        }

        private void Model_SelectionChanged(object sender, Environment.SelectionChangedEventArgs e)
        {
            EntityPopup.IsOpen = false;
            EntityPopup.IsOpen = !string.IsNullOrEmpty(EntityInfo.Text);
        }


        private void ShowHideRightPanel_OnClick(object sender, EventArgs e)
        {
            RightPanel.Visibility = RightPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }
        
        private void LoadConfig(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void HideSettingsPopup(object sender, MouseButtonEventArgs e)
        {
            SettingsPopup.IsOpen = false;
        }

        private void HideSettingsPopup(object sender, RadRoutedEventArgs e)
        {
            SettingsPopup.IsOpen = false;
        }

        private void ModelMouseEnter(object sender, MouseEventArgs e)
        {
            Model.Focus();
        }
    }
}