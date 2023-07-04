Public Class ucBoss
    Private id As Integer
    Private clTop As clHandleBosses

    Public Sub New(ByVal _tc As clHandleBosses, ByVal _id As Integer)
        InitializeComponent()
        clTop = _tc
        id = _id
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        clTop.Fight_Me(Me, id)
    End Sub

End Class
