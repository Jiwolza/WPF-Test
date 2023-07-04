Imports System.Data
Imports System.IO

Public Class clCSVdata
    Private parent As MainWindow
    Public Sub New(_parent As MainWindow)
        Me.parent = _parent
    End Sub

    Public Function Load_CSV_Data(ByVal _path As String) As DataTable
        Dim SR As StreamReader = New StreamReader(_path)
        Dim line As String = SR.ReadLine()
        Dim strArray As String() = line.Split(";"c)
        Dim row As DataRow
        Dim dt = New DataTable()

        'Column names from first csv line
        For Each s As String In strArray
            dt.Columns.Add(s)
        Next

        'Add data for each line
        Do
            line = SR.ReadLine
            If Not line = String.Empty Then
                row = dt.NewRow()
                row.ItemArray = line.Split(";"c)
                For Each s As String In strArray
                    If s.Contains("ge_") And row(s) = "" Then
                        row(s) = "-1"
                    ElseIf s.Contains("le_") And row(s) = "" Then
                        row(s) = "2147483647"
                    End If
                Next
                dt.Rows.Add(row)
            Else
                Exit Do
            End If
        Loop
        Return dt
    End Function

    Public Function Check_Applicable(ByVal _dt As DataTable) As DataRow()
        'Selection of applicable rows in table
        Dim result() As DataRow = _dt.Select("active = 'x' AND " &
            parent.progress & " >= Convert(ge_progress, 'System.Int32') AND " &
            parent.swIGTime.ElapsedMilliseconds & " >= Convert(ge_igtime, 'System.Int32') AND " &
            parent.swIGTime.ElapsedMilliseconds & " <= Convert(le_igtime, 'System.Int32') AND " &
            parent.score.Get_Int & " >= Convert(ge_score, 'System.Int32') AND " &
            parent.score.Get_Int & " <= Convert(le_score, 'System.Int32') AND " &
            parent.ucSTRpushup.l.Get_Int & " >= Convert(ge_pushup_l, 'System.Int32') AND " &
            parent.ucSTRpushup.l.Get_Int & " <= Convert(le_pushup_l, 'System.Int32')
            ")
        Return result
    End Function

End Class
