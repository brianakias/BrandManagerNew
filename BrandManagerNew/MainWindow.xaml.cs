using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BrandManagerNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Domain Domain { get; set; }
        private BrandRepository brandRepository { get; set; }
        private UserInputValidation validation { get; set; }

        private Thickness thicknessOfSelectedButton = new Thickness(3);
        public MainWindow()
        {
            InitializeComponent();
            HideAll();
            brandRepository = new BrandRepository();
            validation = new UserInputValidation();
            Domain = new Domain(validation, brandRepository);
        }

        /// <summary>
        /// Logic for when create brand button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateBrand(object sender, RoutedEventArgs e)
        {
            InitializeUIElements();
            createBrandButton.BorderThickness = thicknessOfSelectedButton;
            idTextBox.IsReadOnly = true;
            idTextBox.Background = Brushes.LightGray;
            brandNameTextBox.IsReadOnly = false;
            brandNameTextBox.Background = Brushes.White;
            isEnabledBox.IsEnabled = true;
        }

        private void Button_Click_ReadBrands(object sender, RoutedEventArgs e)
        {
            InitializeUIElements();
            readBrandButton.BorderThickness = thicknessOfSelectedButton;
            idTextBox.IsReadOnly = false;
            idTextBox.Background = Brushes.White;
            brandNameTextBox.IsReadOnly = true;
            brandNameTextBox.Background = Brushes.LightGray;
            isEnabledBox.IsEnabled = false;
        }

        /// <summary>
        /// Logic for when update brand button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_UpdateBrand(object sender, RoutedEventArgs e)
        {
            InitializeUIElements();
            updateBrandButton.BorderThickness = thicknessOfSelectedButton;
            idTextBox.IsReadOnly = false;
            idTextBox.Background = Brushes.White;
            brandNameTextBox.IsReadOnly = false;
            brandNameTextBox.Background = Brushes.White;
            isEnabledBox.IsEnabled = true;
        }


        /// <summary>
        /// Logic for when delete brand button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_DeleteBrand(object sender, RoutedEventArgs e)
        {
            InitializeUIElements();
            deleteButton.BorderThickness = thicknessOfSelectedButton;
            idTextBox.IsReadOnly = false;
            idTextBox.Background = Brushes.White;
            brandNameTextBox.IsReadOnly = true;
            brandNameTextBox.Background = Brushes.LightGray;
            isEnabledBox.IsEnabled = false;

        }

        /// <summary>
        /// Resets the border thickness of the 4 CRUD buttons.
        /// </summary>

        private void InitializeUIElements()
        {
            HideAll();
            EmptyAllFields();
            ToggleVisibility();
            ResetBorderAllThicknesses();
        }
        private void ResetBorderAllThicknesses()
        {
            createBrandButton.BorderThickness = new Thickness(1);
            readBrandButton.BorderThickness = new Thickness(1);
            updateBrandButton.BorderThickness = new Thickness(1);
            deleteButton.BorderThickness = new Thickness(1);
        }

        /// <summary>
        /// Toggles the visibility of all listed ui elements between collapsed/visible
        /// </summary>
        private void ToggleVisibility()
        {
            List<Control> controls = new List<Control>
            {
                brandNameLabel,
                brandNameTextBox,
                isEnabledLabel,
                isEnabledBox,
                submitButton,
                idLabel,
                idTextBox
            };

            foreach (Control control in controls)
            {
                if (control.Visibility == Visibility.Collapsed)
                {
                    control.Visibility = Visibility.Visible;
                }
                else
                {
                    control.Visibility = Visibility.Collapsed;
                }

            }
        }

        private void HideAll()
        {
            List<Control> controls = new List<Control>
            {
                brandNameLabel,
                brandNameTextBox,
                isEnabledLabel,
                isEnabledBox,
                submitButton,
                idLabel,
                idTextBox
            };

            foreach (Control control in controls)
            {
                control.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Empties the 3 input fields
        /// </summary>
        private void EmptyAllFields()
        {
            idTextBox.Text = string.Empty;
            brandNameTextBox.Text = string.Empty;
            isEnabledBox.IsChecked = false;
        }

        /// <summary>
        /// Logic for when submit button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            bool inCreateState = createBrandButton.BorderThickness == thicknessOfSelectedButton;
            bool inReadState = readBrandButton.BorderThickness == thicknessOfSelectedButton;
            bool inUpdateState = updateBrandButton.BorderThickness == thicknessOfSelectedButton;
            bool inDeleteState = deleteButton.BorderThickness == thicknessOfSelectedButton;
            string id = idTextBox.Text;
            string name = brandNameTextBox.Text;
            bool flag = (bool)isEnabledBox.IsChecked;

            if (inCreateState)
            {
                int recordsAffected = Domain.CreateRecord(name, flag);
                Domain.ConfirmOneRecordWasAffected(recordsAffected);
                MessageBox.Show("Record created", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (inReadState)
            {
                List<Brand> record = Domain.ReadRecord(id);
                dataGrid.ItemsSource = record;
                MessageBox.Show("Record read", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (inUpdateState)
            {
                int recordsAffected = Domain.UpdateRecord(id, name, flag);
                Domain.ConfirmOneRecordWasAffected(recordsAffected);
                MessageBox.Show("Record updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (inDeleteState)
            {
                int recordsAffected = Domain.DeleteRecord(id);
                Domain.ConfirmOneRecordWasAffected(recordsAffected);
                MessageBox.Show("Record deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
