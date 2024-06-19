Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json
Imports System.Diagnostics

Public Class CreateSubmissionForm

    Private client As HttpClient = New HttpClient()
    Private stopwatch As Stopwatch = New Stopwatch()
    Private WithEvents timer As Timer = New Timer()

    Private Sub CreateSubmissionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        ' Initialize the stopwatch
        stopwatch.Start()
        ' Display the stopwatch time in the TextBox
        txtStopwatch.Text = stopwatch.Elapsed.ToString("mm':'ss':'ff")
        ' Set timer interval and start the timer
        timer.Interval = 100 ' Update every 100 milliseconds
        timer.Start()
    End Sub

    Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
        ' Toggle the stopwatch
        If stopwatch.IsRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        txtStopwatch.Text = stopwatch.Elapsed.ToString("mm':'ss':'ff")
    End Sub

    Private Sub timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
        ' Update the TextBox with the current stopwatch time
        txtStopwatch.Text = stopwatch.Elapsed.ToString("mm':'ss':'ff")
    End Sub

    Private Sub CreateSubmissionForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Stop the timer when the form is closing
        timer.Stop()
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Gather form data and send it to the backend
        Dim submission As New Submission()
        submission.Name = txtName.Text
        submission.Email = txtEmail.Text
        submission.PhoneNumber = txtPhoneNumber.Text
        submission.GithubLink = TextBox4.Text
        submission.StopwatchTime = stopwatch.ElapsedMilliseconds

        ' Serialize the submission object to JSON
        Dim submissionJson = JsonConvert.SerializeObject(submission)
        MessageBox.Show("Submission successful!")
        ' Make API call to submit the submission
        Dim response = Await client.PostAsync("http://localhost:3000/submit", New StringContent(submissionJson, Encoding.UTF8, "application/json"))

        ' Check the response status
        If response.IsSuccessStatusCode Then
            MessageBox.Show("Submission successful!")
        Else
            MessageBox.Show("Submission failed. Please check the console for more information.")
            Console.WriteLine(Await response.Content.ReadAsStringAsync())
        End If
    End Sub

    Private Sub CreateSubmissionForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' Handle keyboard shortcuts
        If e.Control AndAlso e.KeyCode = Keys.S Then
            btnSubmit.PerformClick()
        End If
        If e.Control AndAlso e.KeyCode = Keys.T Then
            btnToggleStopwatch.PerformClick()
        End If
    End Sub
End Class

Public Class Submission
    Public Property Name As String
    Public Property Email As String
    Public Property PhoneNumber As String
    Public Property GithubLink As String
    Public Property StopwatchTime As Long
End Class
