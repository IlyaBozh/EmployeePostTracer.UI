
using EmployeePostTracer.Http.HttpServices;
using EmployeePostTracer.Http.HttpServices.Interfaces;
using EmployeePostTracer.Http.Models;
using System.Windows;

namespace EmployeePostTracer.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IHttpEmployeeService _httpEmployeeService;
    public MainWindow()
    {
        InitializeComponent();

        _httpEmployeeService = new HttpEmployeeService();
    }

    private void Button_Registration_Click(object sender, RoutedEventArgs e)
    {
        var employee = new RegistrationEmployeeRequest()
        {
            FirstName = TextBox_Name.Text,
            LastName = TextBox_LastName.Text,
            Password = TextBox_PasswordRegistration.Text,
            Email = TextBox_EmailRegistration.Text,
            Patronymic = TextBox_Patronymic.Text,
        };

        _httpEmployeeService.Register(employee);

        TabControl_MainTabControl.SelectedIndex = 0;
    }

    private void Button_TransitionToRegistration_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 1;

    private void Button_TransitionToAuthorization_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 0;

    private void Button_Authorization_Click(object sender, RoutedEventArgs e)
    {
        var loginData = new LoginRequest()
        {
            Email = TextBox_EmailAuthorization.Text,
            Password = TextBox_PasswordAuthorization.Text,
        };

        _httpEmployeeService.Login(loginData);

        TabControl_MainTabControl.SelectedIndex = 2;
    }

    private async void Button_BackAccaunt_Click(object sender, RoutedEventArgs e)
    {
        var employeeInfo = await _httpEmployeeService.GetById();

        TextBlock_Fio.Text = $"{employeeInfo.LastName} {employeeInfo.FirstName} {employeeInfo.Patronymic}";
        TextBlock_Email.Text = employeeInfo.Email;
    }

    private void Button_Account_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 7;

    private void Button_SenderEmails_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 3;
}
