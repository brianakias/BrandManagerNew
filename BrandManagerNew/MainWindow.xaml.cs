using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrandManagerNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HideAll();
            BrandRepository brandRepository = new BrandRepository();
            brandRepository.CreateTableIfNotExists("brands");
        }

        /// <summary>
        /// Logic for when create brand button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateBrand(object sender, RoutedEventArgs e)
        {
            HideAll();
            EmptyAllFields();
            ToggleVisibility();
            ResetBorderAllThicknesses();
            createBrandButton.BorderThickness = new Thickness(3);
            idTextBox.IsReadOnly = true;
            idTextBox.Background = Brushes.LightGray;
            brandNameTextBox.IsReadOnly = false;
            brandNameTextBox.Background = Brushes.White;
            isEnabledBox.IsEnabled = true;
        }

        private void Button_Click_ReadBrands(object sender, RoutedEventArgs e)
        {
            HideAll();
            EmptyAllFields();
            ToggleVisibility();
            ResetBorderAllThicknesses();
            readBrandButton.BorderThickness = new Thickness(3);
            idTextBox.IsReadOnly = false;
            idTextBox.Background = Brushes.White;
            brandNameTextBox.IsReadOnly = false;
            brandNameTextBox.Background = Brushes.White;
            isEnabledBox.IsEnabled = true;

            BrandRepository brandRepository = new BrandRepository();
            var brands = brandRepository.ReadRecords();
            dataGrid.ItemsSource = brands;
        }

        /// <summary>
        /// Logic for when update brand button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_UpdateBrand(object sender, RoutedEventArgs e)
        {
            HideAll();
            EmptyAllFields();
            ToggleVisibility();
            ResetBorderAllThicknesses();
            updateBrandButton.BorderThickness = new Thickness(3);
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
            HideAll();
            EmptyAllFields();
            ToggleVisibility();
            ResetBorderAllThicknesses();
            deleteButton.BorderThickness = new Thickness(3);
            idTextBox.IsReadOnly = false;
            idTextBox.Background = Brushes.White;
            brandNameTextBox.IsReadOnly = true;
            brandNameTextBox.Background = Brushes.LightGray;
            isEnabledBox.IsEnabled = false;

        }

        /// <summary>
        /// Resets the border thickness of the 4 CRUD buttons.
        /// </summary>
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
            bool inCreateState = idTextBox.IsReadOnly == true && brandNameTextBox.IsReadOnly == false && isEnabledBox.IsEnabled == true;
            bool inUpdateState = idTextBox.IsReadOnly == false && brandNameTextBox.IsReadOnly == false && isEnabledBox.IsEnabled == true;
            bool inDeleteState = idTextBox.IsReadOnly == false && brandNameTextBox.IsReadOnly == true && isEnabledBox.IsEnabled == false;
            BrandRepository brandRepository = new BrandRepository();
            Brand brand = new Brand()
            {
                Name = brandNameTextBox.Text,
                IsEnabled = (bool)isEnabledBox.IsChecked,
            };

            if (inCreateState)
            {
                brandRepository.CreateRecord(brand);
                MessageBox.Show("Record created", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (inUpdateState)
            {
                if (idTextBox.Text == "") return;
                brand.Id = int.Parse(idTextBox.Text);
                brandRepository.UpdateRecord(brand);
                MessageBox.Show("Record updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (inDeleteState)
            {
                if (idTextBox.Text == "") return;
                int id = int.Parse(idTextBox.Text);
                brandRepository.DeleteRecord(id);
                MessageBox.Show("Record deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
