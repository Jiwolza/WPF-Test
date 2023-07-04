Public Class ucAchievement
    Private id As Integer
    Private clTop As clHandleAchievements

    Public Sub New(ByVal _tc As clHandleAchievements, ByVal _id As Integer)
        InitializeComponent()
        clTop = _tc
        id = _id
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        clTop.Claim_Me(Me, id)
    End Sub

End Class
