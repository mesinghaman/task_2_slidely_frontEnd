Imports System.Net.Http
Imports Newtonsoft.Json

Public Class ViewSubmissionsForm

    Private client As New HttpClient()
    Private currentIndex As Integer = 0
    Private isEditing As Boolean = False ' Flag to track edit mode

    Private Sub ViewSubmissionsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initial loading of submission
        LoadSubmission(0)
        Me.KeyPreview = True ' Enable KeyDown events
    End Sub

    Private Sub LoadSubmission(index As Integer)
        Try
            Dim response = client.GetAsync("http://localhost:3000/read?index=" & index).Result
            response.EnsureSuccessStatusCode()
            Dim submission = JsonConvert.DeserializeObject(Of Submission)(response.Content.ReadAsStringAsync().Result)

            ' Update UI with submission data
            txtName.Text = submission.Name
            txtEmail.Text = submission.Email
            txtPhone.Text = submission.PhoneNumber
            txtGithubLink.Text = submission.GithubLink

            ' Convert milliseconds to "mm':'ss':'ff" format
            Dim totalMilliseconds As Long = submission.StopwatchTime
            Dim ts As New TimeSpan(totalMilliseconds * TimeSpan.TicksPerMillisecond)
            Dim formattedTime As String = $"{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds:00}"

            txtStopwatch.Text = formattedTime

            ' Toggle edit mode based on flag
            ToggleEditMode(False) ' Disable edit mode after loading
            btnSubmit.Enabled = False ' Disable submit button initially

            currentIndex = index
        Catch ex As Exception
            MessageBox.Show("Error loading submission: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        LoadSubmission(currentIndex - 1)
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        LoadSubmission(currentIndex + 1)
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Try
            Dim response = client.DeleteAsync("http://localhost:3000/delete/" & currentIndex).Result
            response.EnsureSuccessStatusCode()

            MessageBox.Show("Submission deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Load the next submission after deletion
            LoadSubmission(currentIndex)
        Catch ex As Exception
            MessageBox.Show("Error deleting submission: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        ' Toggle edit mode
        ToggleEditMode(True)
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Validate and submit updated data
        If ValidateFields() Then
            Try
                Dim updatedSubmission As New Submission With {
                    .Name = txtName.Text,
                    .Email = txtEmail.Text,
                    .PhoneNumber = txtPhone.Text,
                    .GithubLink = txtGithubLink.Text,
                    .StopwatchTime = Long.Parse(txtStopwatch.Text)
                }

                Dim json = JsonConvert.SerializeObject(updatedSubmission)
                Dim content = New StringContent(json, System.Text.Encoding.UTF8, "application/json")

                Dim response = client.PutAsync("http://localhost:3000/edit/" & currentIndex, content).Result
                response.EnsureSuccessStatusCode()

                MessageBox.Show("Submission updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Reload the updated submission
                LoadSubmission(currentIndex)
            Catch ex As Exception
                MessageBox.Show("Error updating submission: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function ValidateFields() As Boolean
        ' Basic validation example (add more as needed)
        If String.IsNullOrWhiteSpace(txtName.Text) OrElse
           String.IsNullOrWhiteSpace(txtEmail.Text) OrElse
           String.IsNullOrWhiteSpace(txtPhone.Text) OrElse
           String.IsNullOrWhiteSpace(txtGithubLink.Text) OrElse
           String.IsNullOrWhiteSpace(txtStopwatch.Text) Then
            MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ' Additional validation logic can be added here (e.g., email format)

        Return True
    End Function

    Private Sub ToggleEditMode(enableEdit As Boolean)
        If enableEdit Then
            ' Enable editing
            txtName.ReadOnly = False
            txtEmail.ReadOnly = False
            txtPhone.ReadOnly = False
            txtGithubLink.ReadOnly = False
            txtStopwatch.ReadOnly = False
            btnSubmit.Enabled = True
            btnEdit.Enabled = False
            btnDelete.Enabled = False
            btnNext.Enabled = False
            btnPrevious.Enabled = False
        Else
            ' Disable editing
            txtName.ReadOnly = True
            txtEmail.ReadOnly = True
            txtPhone.ReadOnly = True
            txtGithubLink.ReadOnly = True
            txtStopwatch.ReadOnly = True
            btnSubmit.Enabled = False
            btnEdit.Enabled = True
            btnDelete.Enabled = True
            btnNext.Enabled = True
            btnPrevious.Enabled = True
        End If

        isEditing = enableEdit
    End Sub

    Private Sub ViewSubmissionsForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' Handle keyboard shortcuts
        If e.Control AndAlso e.KeyCode = Keys.P Then
            btnPrevious.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.N Then
            btnNext.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.D Then
            btnDelete.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.E Then
            btnEdit.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.S AndAlso isEditing Then
            btnSubmit.PerformClick()
        End If
    End Sub

End Class


