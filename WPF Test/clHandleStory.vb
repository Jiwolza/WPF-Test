Imports System.Data

Public Class clHandleStory
    Inherits clCSVdata

#Region "Region: Declaration - CONSTANTS"
    Const C_NAME As String = "Lizzy"
    Const C_GENDER As Boolean = False                   'is_male?
    Const C_TYPESPEED_BASE As Integer = 40              'wait milliseconds per char
    Const C_TYPESPEED_VARIANCE As Integer = 15          '+- of base as random diviation

    Const C_SPACE_HINT_FIRST_DELAY As Integer = 1600    'wait befor showing spacebar tip when dialog is done
    Const C_SPACE_HINT_BLINK_SPEED As Integer = 800     'delay to show and hide the hint
#End Region
#Region "Region: Declaration - Global Variables"
    Private parent As MainWindow
    Private dt_total, dt_applicable As DataTable

    Public isTyping As Boolean = False
    Public isSkipping As Boolean = False
#End Region

#Region "Region: Subs and Functions"
    Public Sub New(_parent As MainWindow)
        MyBase.New(_parent)
        Me.parent = _parent
        dt_total = Load_CSV_Data(AppDomain.CurrentDomain.BaseDirectory.ToString & "csvs\story.csv")
        dt_applicable = dt_total.Clone()
        parent.lbStoryHint.Visibility = Visibility.Hidden
    End Sub

    Public Overloads Function Check_Applicable() As Boolean
        Dim isApllicable As Boolean = False
        Dim rows_applicable() As DataRow
        rows_applicable = MyBase.Check_Applicable(dt_total)

        'What to do if results were found
        For Each row As DataRow In rows_applicable
            dt_applicable.Rows.Add(row.ItemArray)
            dt_total.Rows.Remove(row)
        Next

        If dt_applicable.Rows.Count > 0 Then
            isApllicable = True
        End If

        Return isApllicable

    End Function

    Public Async Sub Action()
        Dim strStory As String
        Dim random As New Random
        Dim intBlink As Integer = 0

        If dt_applicable.Rows.Count > 0 Then
            If parent.playAnimations Then

                isTyping = True
                parent.Game_Pause_Toggle()
                Canvas.SetLeft(parent.cvStory, 0)

                For Each row As DataRow In dt_applicable.Rows
                    strStory = Adjust_Text(row("text"))
                    For i As Integer = 1 To strStory.Length - 1
                        If isSkipping Then
                            isSkipping = False
                            Exit For
                        End If
                        parent.tbStory.Text = Left(strStory, i)
                        Await Task.Delay(random.Next(C_TYPESPEED_BASE - C_TYPESPEED_VARIANCE, C_TYPESPEED_BASE + C_TYPESPEED_VARIANCE))
                    Next

                    parent.tbStory.Text = strStory

                    While Not isSkipping
                        Await Task.Delay(20)
                        If intBlink = CInt(C_SPACE_HINT_FIRST_DELAY / 20) + CInt(C_SPACE_HINT_BLINK_SPEED / 20) Then
                            If parent.lbStoryHint.Visibility = Visibility.Visible Then
                                parent.lbStoryHint.Visibility = Visibility.Hidden
                            Else
                                parent.lbStoryHint.Visibility = Visibility.Visible
                            End If
                            intBlink = CInt(C_SPACE_HINT_FIRST_DELAY / 20)
                        Else
                            intBlink += 1
                        End If
                    End While

                    isSkipping = False
                    parent.lbStoryHint.Visibility = Visibility.Hidden
                    intBlink = 0

                    'dt_applicable.Rows.Remove(row)
                Next
                Canvas.SetLeft(parent.cvStory, 1300)
                isTyping = False
                parent.Game_Pause_Toggle()
            End If
            dt_applicable.Clear()

        End If

    End Sub

    Private Function Adjust_Text(ByVal _text As String) As String
        Adjust_Text = _text

        Adjust_Text = Adjust_Text.Replace("%name", C_NAME)
        Adjust_Text = Adjust_Text.Replace("%Name", C_NAME)

        If C_GENDER Then
            Adjust_Text = Adjust_Text.Replace("%He", "He")
            Adjust_Text = Adjust_Text.Replace("%he", "he")
            Adjust_Text = Adjust_Text.Replace("%His", "His")
            Adjust_Text = Adjust_Text.Replace("%his", "his")
            Adjust_Text = Adjust_Text.Replace("%Himself", "Himself")
            Adjust_Text = Adjust_Text.Replace("%himself", "himself")
        Else
            Adjust_Text = Adjust_Text.Replace("%He", "She")
            Adjust_Text = Adjust_Text.Replace("%he", "she")
            Adjust_Text = Adjust_Text.Replace("%His", "Her")
            Adjust_Text = Adjust_Text.Replace("%his", "her")
            Adjust_Text = Adjust_Text.Replace("%Himself", "Herself")
            Adjust_Text = Adjust_Text.Replace("%himself", "herself")
        End If

    End Function

#End Region

End Class
