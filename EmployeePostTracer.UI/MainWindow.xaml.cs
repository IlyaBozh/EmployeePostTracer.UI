
using EmployeePostTracer.Http.HttpServices;
using EmployeePostTracer.Http.HttpServices.Interfaces;
using EmployeePostTracer.Http.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;

namespace EmployeePostTracer.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IHttpEmployeeService _httpService;

    private List<EmployeeMainInfoResponse> _employees;
    private List<LetterMainInfoResponse> _incomingLetters;
    private List<LetterMainInfoResponse> _sendedLetters;

    public MainWindow()
    {
        InitializeComponent();

        _httpService = new HttpService();
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

        _httpService.Register(employee);

        TabControl_MainTabControl.SelectedIndex = 0;

        TextBox_Name.Text = "";
        TextBox_LastName.Text = "";
        TextBox_LastName.Text = "";
        TextBox_EmailRegistration.Text = "";
        TextBox_Patronymic.Text = "";
    }

    private void Button_TransitionToRegistration_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 1;

    private void Button_TransitionToAuthorization_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 0;

    private async void Button_Authorization_Click(object sender, RoutedEventArgs e)
    {
        var loginData = new LoginRequest()
        {
            Email = TextBox_EmailAuthorization.Text,
            Password = TextBox_PasswordAuthorization.Text,
        };

        await _httpService.Login(loginData);

        TabControl_MainTabControl.SelectedIndex = 2;

        FillInformation();

        TextBox_EmailAuthorization.Text = "";
        TextBox_PasswordAuthorization.Text = "";
    }

    private void Button_BackAccaunt_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 2;

    private async void Button_Account_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 7;

        var employeeInfo = await _httpService.GetById();

        TextBlock_Fio.Text = $"{employeeInfo.LastName.Replace(" ", "")} {employeeInfo.FirstName.Replace(" ", "")} {employeeInfo.Patronymic.Replace(" ", "")}";
        TextBlock_Email.Text = employeeInfo.Email.Replace(" ", "");
    }

    private void Button_SenderEmails_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 3;

    private void Button_WriteEmail_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 4;
    }

    private void Button_Send_Click(object sender, RoutedEventArgs e)
    {
        var letter = new AddLetterRequest()
        {
            Header = TextBox_Header.Text,
            Content = TextBox_Content.Text,
            RecipientId = _employees[ComboBox_Recipient.SelectedIndex].Id,
            Recipient = ComboBox_Recipient.Items[ComboBox_Recipient.SelectedIndex].ToString()
        };

        _httpService.SendLetter(letter);

        TabControl_MainTabControl.SelectedIndex = 2;

        TextBox_Header.Text = string.Empty;
        TextBox_Content.Text = string.Empty;

        FillInformation();
    }

    private void Button_Back_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 2;

    private void Button_Cancel_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 2;
        TextBox_Header.Text = string.Empty;
        TextBox_Content.Text = string.Empty;
    }

    private async void Button_OpenAccountEdit_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 8;

        var employee = await _httpService.GetById();

        TextBox_EditName.Text = employee.FirstName.Replace(" ", "");
        TextBox_EditLastName.Text = employee.LastName.Replace(" ", "");
        TextBox_EditPatronymic.Text = employee.Patronymic.Replace(" ", "");
    }

    private void Button_CancelAccauntEdit_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 7;

    private void Button_EditAccount_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 7;

        var newEmployee = new UpdateEmployeeRequest()
        {
            FirstName = TextBox_EditName.Text,
            LastName = TextBox_EditLastName.Text,
            Patronymic = TextBox_EditPatronymic.Text
        };

        _httpService.UpdateAccount(newEmployee);
    }

    private async void Button_AccountDelete_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 0;
        await _httpService.DeleteAccount();
    }

    private void Button_BackEmail_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 2;
        TextBlock_Header.Text = string.Empty;
        TextBlock_Content.Text = string.Empty;
        TextBlock_Date.Text = "Дата отправки: ";
        TextBlock_Sender.Text = "От: ";
        TextBlock_Recipient.Text = "Кому: ";
    }

    private async void MenuItem_ReadLetter_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 6;

        var letter = await _httpService.GetLetterById(_incomingLetters[ListView_IncomingEmails.SelectedIndex].Id);

        TextBlock_Header.Text = letter.Header.Replace(" ", "");
        TextBlock_Content.Text = letter.Content.Replace(" ", "");
        TextBlock_Date.Text += $" {letter.SendingDate}";
        TextBlock_Sender.Text += $" {letter.Sender.Replace(" ", "")}";
        TextBlock_Recipient.Text += $" {letter.Recipient.Replace(" ", "")}";
    }

    private async void MenuItem_ReadSendedLetter_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 6;

        var letter = await _httpService.GetLetterById(_sendedLetters[ListView_SendedEmails.SelectedIndex].Id);

        TextBlock_Header.Text = letter.Header.Replace(" ", "");
        TextBlock_Content.Text = letter.Content.Replace(" ", "");
        TextBlock_Date.Text += $" {letter.SendingDate}";
        TextBlock_Sender.Text += $" {letter.Sender}";
        TextBlock_Recipient.Text += $" {letter.Recipient}";
    }

    private async void MenuItem_EditLetter_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 5;

        var letter = await _httpService.GetLetterById(_sendedLetters[ListView_SendedEmails.SelectedIndex].Id);

        TextBox_HeaderEdit.Text = letter.Header;
        TextBox_ContentEdit.Text = letter.Content;
    }

    private void Button_CancelEdit_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 3;

    private void Button_Edit_Click(object sender, RoutedEventArgs e)
    {
        TabControl_MainTabControl.SelectedIndex = 3;

        var newLetter = new UpdateLetterRequest() 
        {
            Header = TextBox_HeaderEdit.Text,
            Content = TextBox_ContentEdit.Text
        };

        _httpService.EditLetter(newLetter, _sendedLetters[ListView_SendedEmails.SelectedIndex].Id);
    }

    private void MenuItem_DeleteLetter_Click(object sender, RoutedEventArgs e)
    {
        _httpService.DeleteLetter(_sendedLetters[ListView_SendedEmails.SelectedIndex].Id);
        FillInformation();
    }

    private async void FillInformation()
    {
        _employees = await _httpService.GetAll();

        ComboBox_Recipient.ItemsSource = _employees;

        _incomingLetters = await _httpService.GetAllByRecipientId();

        ListView_IncomingEmails.ItemsSource = _incomingLetters;

        _sendedLetters = await _httpService.GetAllBySenderId();

        ListView_SendedEmails.ItemsSource = _sendedLetters;
    }

    private void Button_AccountExit_Click(object sender, RoutedEventArgs e) => TabControl_MainTabControl.SelectedIndex = 0;
}

