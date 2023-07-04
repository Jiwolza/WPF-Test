Imports System.CodeDom
Imports System.Data
Imports System.Reflection

Public Class clHandleProgress
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

    Private isDelayedForStory As Boolean = False

    Public isTyping As Boolean = False
    Public isSkipping As Boolean = False
#End Region

#Region "Region: Subs and Functions"
    Public Sub New(_parent As MainWindow)
        MyBase.New(_parent)
        Me.parent = _parent
        dt_total = Load_CSV_Data(AppDomain.CurrentDomain.BaseDirectory.ToString & "csvs\progress.csv")
        dt_applicable = dt_total.Clone()
        parent.lbStoryHint.Visibility = Visibility.Hidden
    End Sub

    Public Overloads Function Check_Applicable() As Boolean
        Dim isApllicable As Boolean = False
        Dim rows_applicable() As DataRow


        'If isDelayedForStory Then
        '    Return True
        'End If

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

    Public Sub Action()
        If dt_applicable.Rows.Count > 0 Then

            'If isDelayedForStory Then
            parent.pProgress()
            dt_applicable.Rows.RemoveAt(0)

            'isDelayedForStory = False

            'Else
            '    isDelayedForStory = True
            '    parent.progress += 1
            'End If

            'parent.progress += 1
            'If dt_applicable.Rows.Item(0)("sub") <> "" Then
            'Try
            'CallByName(parent, dt_applicable.Rows.Item(0)("sub"), CallType.Method)
            'Catch ex As Exception
            'Debug.WriteLine(ex.Message)
            'End Try
            'End If


        End If


    End Sub

#End Region

End Class
