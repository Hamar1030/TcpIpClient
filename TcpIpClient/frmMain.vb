Public Class frmMain

    Private m_clsClient As clsClient
    Public Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        m_clsClient = New clsClient
    End Sub

    Private Sub btnOpenClose_Click(sender As Object, e As EventArgs) Handles btnOpenClose.Click
        If btnOpenClose.Text = "Open" Then
            If m_clsClient.Open(New Net.IPAddress(New Byte() {127, 0, 0, 1}), 5000) = False Then
                MessageBox.Show("Openに失敗しました。" & m_clsClient.ErrorMessage, "エラー", MessageBoxButtons.OK)
                Exit Sub
            End If

            btnOpenClose.Text = "Close"
            grbConnectionSetting.Enabled = False
            grbCommunication.Enabled = True

        ElseIf btnOpenClose.Text = "Close" Then
            If m_clsClient.Close() = False Then
                MessageBox.Show("Closeに失敗しました。" & m_clsClient.ErrorMessage, "エラー", MessageBoxButtons.OK)
            End If

            btnOpenClose.Text = "Open"
            grbConnectionSetting.Enabled = True
            grbCommunication.Enabled = False

        End If

    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        If m_clsClient.Send(txtSend.Text) = False Then
            MessageBox.Show("送信に失敗しました。" & m_clsClient.ErrorMessage, "エラー", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub btnRead_Click(sender As Object, e As EventArgs) Handles btnRead.Click
        Dim message As String = ""
        If m_clsClient.Read(message) = False Then
            MessageBox.Show("受信に失敗しました。" & m_clsClient.ErrorMessage, "エラー", MessageBoxButtons.OK)
            Exit Sub
        End If

        message = message & txtRead.Text

        txtRead.Text = message
    End Sub
End Class
