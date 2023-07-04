Imports System.Data

Public Class clHandleBosses
    Inherits clCSVdata

    Private parent As MainWindow
    Private dt_total, dt_applicable, dt_fightable As DataTable

    Public Sub New(_parent As MainWindow)
        MyBase.New(_parent)
        Me.parent = _parent
        dt_total = Load_CSV_Data(AppDomain.CurrentDomain.BaseDirectory.ToString & "csvs\bosses.csv")
        dt_applicable = dt_total.Clone()
        dt_fightable = dt_total.Clone()
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

    Public Sub Action()
        Dim uc As ucBoss

        For Each row As DataRow In dt_applicable.Rows
            uc = New ucBoss(Me, CInt(row("id")))
            uc.lbcBoss.Content = "Test " & row("name")

            parent.spBosses.Children.Add(uc)

            dt_fightable.Rows.Add(row.ItemArray)

        Next


        dt_applicable.Clear()
    End Sub

    Public Sub Fight_Me(ByVal _bos As ucBoss, ByVal _id As Integer)

        Dim result() As DataRow = dt_fightable.Select("id = " & _id)
        For Each row As DataRow In result

            'Wenn gewinnt
            If row("power") > 99 Then

                Select Case row("prizeobj")
                    Case "powerlevel"
                    'parent.score_f += CInt(row("prizeval"))
                    Case "pushup_e"
                        parent.ucSTRpushup.Increase_E(CSng((row("prizeval"))))
                        'parent.ucSTRpushup.Set_Score_Parameters(parent.ucSTRpushup.l, parent.ucSTRpushup.e.Add(New clValue(CSng(row("prizeval")))))
                    Case Else
                        Debug.Write("Achievement reward object not found: " & row("prizeobj"))
                End Select
                row.Delete()


            End If



        Next
        dt_fightable.AcceptChanges()
        parent.spBosses.Children.Remove(_bos)

    End Sub
End Class
