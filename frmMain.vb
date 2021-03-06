﻿Imports MaterialSkin
Public Class frmMain
    Private mScanner As VirusTotalScanner
    Private mResults As List(Of ScanResult)
    Private mResultIndex As Integer
    Private mMD5 As String
    Private mSHA256 As String
    Private mSHA512 As String

    Private Sub lblLink2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblLink2.LinkClicked
        Try
            System.Diagnostics.Process.Start(lblLink2.Text)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub


    Private Property ResultIndex As Integer
        Get
            Return mResultIndex
        End Get
        Set(value As Integer)
            Try
                mResultIndex = value
                If IsNothing(mResults) Then mResults = New List(Of ScanResult)
                If mResultIndex < 1 Then mResultIndex = 1
                If mResultIndex > mResults.Count Then mResultIndex = mResults.Count
                cmdNext.Enabled = CBool(mResultIndex < mResults.Count)
                cmdPrev.Enabled = CBool(mResultIndex > 1)
                lblPage.Enabled = CBool(mResultIndex <> 0)
                lblPage.Text = String.Format("{0:0} of {1:0}", mResultIndex, mResults.Count)
                If mResultIndex = 0 Then
                    prpMain.SelectedObject = Nothing
                Else
                    prpMain.SelectedObject = mResults(mResultIndex - 1)
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End Set
    End Property

    Private Sub cmdOpen_Click_1(sender As Object, e As EventArgs) Handles cmdOpen.Click
        Dim Link As String
        Try
            If IsNothing(mScanner) Then
                If txtKey.Text = "" Then
                    txtKey.Text = "006ba59175ac1efc8ed7066312eac3586ec27cf34925a937d670a165427f68a3"
                End If
                mScanner = New VirusTotalScanner(txtKey.Text)
                mScanner.UseTLS = True
                txtKey.Enabled = False
                MaterialFlatButton1.Enabled = False
            End If
            If dlgOpen.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                txtFile.Text = dlgOpen.FileName
                mMD5 = FileHasher.GetMD5(txtFile.Text)
                mSHA256 = FileHasher.GetSHA256(txtFile.Text)
                mSHA512 = FileHasher.GetSHA512(txtFile.Text)
                lblMD52.Text = mMD5
                lblSHA2562.Text = mSHA256
                lblSHA5122.Text = mSHA512
                Link = mScanner.GetPublicFileScanLink(mSHA256)
                lblLink2.Text = Link
                lblLink2.Links.Clear()
                lblLink2.Links.Add(0, Link.Length)
                cmdReport.Enabled = True
                cmdSubmit.Enabled = True
                cmdRescan.Enabled = True
                cmdComment.Enabled = True
                cmdPrev.Enabled = True
                cmdNext.Enabled = True
                MD5Copybtn.Enabled = True
                SHA256Copybtn.Enabled = True
                SHA512Copybtn.Enabled = True
                URLCopybtn.Enabled = True
                lblLink2.Enabled = True
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cmdSubmit_Click_1(sender As Object, e As EventArgs) Handles cmdSubmit.Click
        Try
            mResults = mScanner.SubmitFile(txtFile.Text)
            ResultIndex = 1
        Catch ex As Exception
            mResults = Nothing
            ResultIndex = 0
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cmdNext_Click(sender As Object, e As EventArgs) Handles cmdNext.Click
        ResultIndex -= 1
    End Sub

    Private Sub cmdPrev_Click(sender As Object, e As EventArgs) Handles cmdPrev.Click
        ResultIndex -= 1
    End Sub

    Private Sub cmdComment_Click_1(sender As Object, e As EventArgs) Handles cmdComment.Click
        Dim Comment As String
        Try
            Commentdlg.txtComment.Text = ""
            Commentdlg.Commentbtn.Enabled = False
            Commentdlg.ShowDialog()

            If Commentdlg.DialogResult = DialogResult.OK Then
                Comment = Commentdlg.txtComment.Text.ToString
            Else
                Return
            End If
            If Comment = "" Then Return
            mResults = mScanner.CreateComment(mSHA256, Comment)
            ResultIndex = 1
        Catch ex As Exception
            mResults = Nothing
            ResultIndex = 0
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cmdRescan_Click(sender As Object, e As EventArgs) Handles cmdRescan.Click
        Try
            mResults = mScanner.RescanFile(mSHA256)
            ResultIndex = 1
        Catch ex As Exception
            mResults = Nothing
            ResultIndex = 0
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cmdReport_Click(sender As Object, e As EventArgs) Handles cmdReport.Click
        Dim Report As Report
        Try
            Report = mScanner.GetFileReport(mSHA256)
            prpMain.SelectedObject = Report
        Catch ex As Exception
            prpMain.SelectedObject = Nothing
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub MaterialFlatButton1_Click(sender As Object, e As EventArgs) Handles MaterialFlatButton1.Click
        txtKey.Text = "006ba59175ac1efc8ed7066312eac3586ec27cf34925a937d670a165427f68a3"
    End Sub

    Private Sub MD5Copybtn_Click(sender As Object, e As EventArgs) Handles MD5Copybtn.Click
        Clipboard.SetText(lblMD52.Text)
    End Sub

    Private Sub SHA256Copybtn_Click(sender As Object, e As EventArgs) Handles SHA256Copybtn.Click
        Clipboard.SetText(lblSHA2562.Text)
    End Sub

    Private Sub SHA512Copybtn_Click(sender As Object, e As EventArgs) Handles SHA512Copybtn.Click
        Clipboard.SetText(lblSHA5122.Text)
    End Sub

    Private Sub URLCopybtn_Click(sender As Object, e As EventArgs) Handles URLCopybtn.Click
        Clipboard.SetText(lblLink2.Text)
    End Sub
End Class
