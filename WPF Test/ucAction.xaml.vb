Public Class ucAction
    'l = level
    'e = effect (on governing attribute)
    'v = value = l * ( prod e )

    'c = cooldown
    'cm = cooldown min (can never be below this value)
    'cf = cooldown flat
    'cl = cooldown factor linear 
    'cr = cooldown reduction (flat)
    'c = cf + cl * level - cr (if c < cm -> c = cm)

#Region "Region: Declaration - Variables"

    Public wasClicked As Boolean = False
    Public wasCanceled As Boolean = False
    Public isCycling As Boolean = False
    Public tsCooldownValue As TimeSpan = TimeSpan.FromMilliseconds(1234)
    Private clTop As MainWindow
    Private tsCooldownActive As TimeSpan

    Private visual_progress As Integer = 0
    'Private isAnimating As Boolean = True

    Public queue As Integer
    Private isCancelEnabled As Boolean = False
    Private isEndlessEnabled As Boolean = False

    'Score:
    Public l As clValue
    Public e As clValue
    Public v As clValue

    'Cooldown:
    Public c As Integer    'cooldown 'c = cf + cl * level - cr  [if c < cm -> c = cm]
    Public cf As Integer   'cooldown flat
    Public cl As Integer   'cooldown linear factor
    Public cr As Integer   'cooldown reduction

#End Region

#Region "Region: Subs & Fuctions: Public"

    Public Sub New(ByVal _tc As MainWindow, ByVal _title As String, _iteration As Integer, _level As Single, _effect As Single)
        InitializeComponent()

        clTop = _tc
        btnAction.Content = _title

        l = New clValue(_level)
        e = New clValue(_effect)
        v = New clValue(l)
        v.Multiply(e)

        lbLcont.Content = l.Write_V
        lbEcont.Content = e.Write_V
        lbVcont.Content = v.Write_V

        'Set_Iteration(_iteration, False)
        'Progress_Visuals()
        Set_Visuals(_iteration)

    End Sub
    Public Sub Set_CD_Parameters(ByVal _cf As Integer, ByVal _cl As Integer, ByVal _cr As Integer)
        cf = _cf
        cl = _cl
        cr = _cr
        tsCooldownValue = TimeSpan.FromMilliseconds(Calculate_CD())
        lbCDcont.Content = tsCooldownValue.ToString("ss\.ff")
    End Sub
    Public Sub Set_Score_Parameters(ByVal _l As clValue, ByVal _e As clValue)
        l = _l
        e = _e
        v.v = l.v
        v.d = l.d
        v.Multiply(e)

        lbLcont.Content = l.Write_V
        lbEcont.Content = e.Write_V
        lbVcont.Content = v.Write_V
    End Sub
    Public Sub Increase_E(ByVal _e As Single)
        e.Add(New clValue(_e))
        v.v = l.v
        v.d = l.d
        v.Multiply(e)
        lbEcont.Content = e.Write_V
        lbVcont.Content = v.Write_V

    End Sub
    Public Sub Set_Title(ByVal _title As String)
        btnAction.Content = _title
    End Sub
    Public Function Check_timer() As Boolean
        If tsCooldownActive < clTop.swIGTime.Elapsed Or wasCanceled Then
            If Not wasCanceled Then
                l.Add(New clValue(1))
                'v = l

                v.v = l.v
                v.d = l.d
                v.Multiply(e)
                lbLcont.Content = l.Write_V
                lbVcont.Content = v.Write_V
                tsCooldownValue = TimeSpan.FromMilliseconds(Calculate_CD())
            End If
            lbCDcont.Content = tsCooldownValue.ToString("ss\.ff")
            If isCycling Or queue > 0 Then
                tsCooldownActive = clTop.swIGTime.Elapsed + tsCooldownValue
                If queue > 0 Then
                    queue -= 1
                    lbQcont.Content = queue.ToString("00")
                    If queue = 0 Then btnCancelQueue.Visibility = Visibility.Hidden
                End If
            Else
                btnCancelAction.Visibility = Visibility.Hidden
            End If

            wasCanceled = False
            Return True
        Else
            lbCDcont.Content = (tsCooldownActive - clTop.swIGTime.Elapsed).ToString("ss\.ff")
            If isCancelEnabled And Not isCycling Then btnCancelAction.Visibility = Visibility.Visible
            Return False
        End If
    End Function
    Public Sub Set_Button_Enabled(ByVal _enabled As Boolean)
        btnAction.IsEnabled = _enabled
    End Sub
    Public Sub Set_Endless_Visibility(ByVal _vis As Visibility)
        If isEndlessEnabled Then
            btnEndlessQueue.Visibility = _vis
        End If
    End Sub
    Public Sub Set_Color(ByVal _col As Color)
        Me.FindResource("scbCurrent").Color = _col
    End Sub

#End Region
#Region "Region: Subs & Fuctions: Private"
    Private Function Calculate_CD() As Integer
        Dim cd_calc As Integer
        cd_calc = CInt(cf + l.Get_Dbl * cl - cr)
        If cd_calc < cf Then
            cd_calc = cf
        End If
        Return cd_calc
    End Function

#End Region

#Region "Region: Subs & Fuctions: Progress"

    Public Async Sub Progress_Visuals()
        Dim check_progress_step As Integer = 0

        If check_progress_step = visual_progress Then
            lbQ.Visibility = Visibility.Hidden
            lbAcont.Visibility = Visibility.Hidden
            lbQcont.Visibility = Visibility.Hidden
            lbL.Visibility = Visibility.Hidden
            lbLcont.Visibility = Visibility.Hidden
            lbE.Visibility = Visibility.Hidden
            lbEcont.Visibility = Visibility.Hidden
            lbV.Visibility = Visibility.Hidden
            lbVcont.Visibility = Visibility.Hidden
            lbCD.Visibility = Visibility.Hidden
            lbCDcont.Visibility = Visibility.Hidden
            borAction.Visibility = Visibility.Hidden
            btnCancelQueue.Visibility = Visibility.Hidden
            btnCancelAction.Visibility = Visibility.Hidden
            btnEndlessQueue.Visibility = Visibility.Hidden
            btnAction.Width = 400
            btnAction.Height = 50
            Canvas.SetLeft(btnAction, 0)

            visual_progress += 1
            Exit Sub
        End If
        check_progress_step += 1

        If check_progress_step = visual_progress Then 'Show Score
            If clTop.playAnimations And Not clTop.isAnimating Then
                clTop.isAnimating = True
                lbL.Opacity = 0
                lbL.Visibility = Visibility.Visible
                lbLcont.Opacity = 0
                lbLcont.Visibility = Visibility.Visible
                borAction.Opacity = 0
                borAction.Visibility = Visibility.Visible
                For i As Integer = 1 To 100
                    Await Task.Delay(8)
                    btnAction.Height = CInt(50 - (i - 2) / 4)
                    lbL.Opacity = CDbl(i / 100)
                    lbLcont.Opacity = CDbl(i / 100)
                    borAction.Opacity = CDbl(i / 100)
                Next
                clTop.isAnimating = False
            End If
            btnAction.Height = 26
            lbL.Opacity = 100
            lbL.Visibility = Visibility.Visible
            lbLcont.Opacity = 100
            lbLcont.Visibility = Visibility.Visible
            borAction.Opacity = 100
            borAction.Visibility = Visibility.Visible

            visual_progress += 1
            Exit Sub
        End If
        check_progress_step += 1

        If check_progress_step = visual_progress Then 'Introduce Cooldown
            If clTop.playAnimations And Not clTop.isAnimating Then
                clTop.isAnimating = True
                lbCD.Opacity = 0
                lbCD.Visibility = Visibility.Visible
                lbCDcont.Opacity = 0
                lbCDcont.Visibility = Visibility.Visible
                For i As Integer = 1 To 100
                    Await Task.Delay(8)
                    lbCD.Opacity = CDbl(i / 100)
                    lbCDcont.Opacity = CDbl(i / 100)
                Next
                clTop.isAnimating = False
            End If
            lbCD.Opacity = 100
            lbCD.Visibility = Visibility.Visible
            lbCDcont.Opacity = 100
            lbCDcont.Visibility = Visibility.Visible

            visual_progress += 1
            Exit Sub
        End If
        check_progress_step += 1

        If check_progress_step = visual_progress Then 'Introduce Queueing
            If clTop.playAnimations And Not clTop.isAnimating Then
                clTop.isAnimating = True
                lbQ.Opacity = 0
                lbQ.Visibility = Visibility.Visible
                lbQcont.Opacity = 0
                lbQcont.Visibility = Visibility.Visible
                For i As Integer = 1 To 100
                    Await Task.Delay(8)
                    lbQ.Opacity = CDbl(i / 100)
                    lbQcont.Opacity = CDbl(i / 100)
                Next
                clTop.isAnimating = False
            End If
            lbQ.Opacity = 100
            lbQ.Visibility = Visibility.Visible
            lbQcont.Opacity = 100
            lbQcont.Visibility = Visibility.Visible

            visual_progress += 1
            Exit Sub
        End If
        check_progress_step += 1

        If check_progress_step = visual_progress Then 'Show Effect and Value
            If clTop.playAnimations And Not clTop.isAnimating Then
                clTop.isAnimating = True
                lbE.Opacity = 0
                lbE.Visibility = Visibility.Visible
                lbEcont.Opacity = 0
                lbEcont.Visibility = Visibility.Visible
                lbV.Opacity = 0
                lbV.Visibility = Visibility.Visible
                lbVcont.Opacity = 0
                lbVcont.Visibility = Visibility.Visible
                For i As Integer = 1 To 100
                    Await Task.Delay(8)
                    lbE.Opacity = CDbl(i / 100)
                    lbEcont.Opacity = CDbl(i / 100)
                    lbV.Opacity = CDbl(i / 100)
                    lbVcont.Opacity = CDbl(i / 100)
                Next
                clTop.isAnimating = False
            End If
            lbE.Opacity = 100
            lbE.Visibility = Visibility.Visible
            lbEcont.Opacity = 100
            lbEcont.Visibility = Visibility.Visible
            lbV.Opacity = 100
            lbV.Visibility = Visibility.Visible
            lbVcont.Opacity = 100
            lbVcont.Visibility = Visibility.Visible

            visual_progress += 1
            Exit Sub
        End If
        check_progress_step += 1

        If check_progress_step = visual_progress Then 'Enable advanced Queueing
            If clTop.playAnimations And Not clTop.isAnimating Then
                clTop.isAnimating = True

                clTop.isAnimating = False
            End If

            isCancelEnabled = True
            isEndlessEnabled = True

            'btnEndlessQueue.Visibility = Visibility.Visible

            visual_progress += 1
            Exit Sub
        End If

    End Sub

    Public Sub Set_Visuals(ByRef _step As Integer)
        Dim save_isAnimating As Boolean = clTop.playAnimations
        clTop.playAnimations = False
        visual_progress = 0
        For i = 0 To _step
            Progress_Visuals()
        Next
        clTop.playAnimations = save_isAnimating

    End Sub

#End Region

#Region "Region: Buttons"

    Private Sub btnAction_Click(sender As Object, _e As RoutedEventArgs) Handles btnAction.Click
        If visual_progress < 2 Then
            l.Add(New clValue(1))
            v.v = l.v
            v.d = l.d
            v.Multiply(e)
            lbLcont.Content = l.Write_V
        ElseIf visual_progress = 2 Then
            If tsCooldownActive < clTop.swIGTime.Elapsed Then
                wasClicked = True
                tsCooldownActive = clTop.swIGTime.Elapsed + tsCooldownValue
            End If

        ElseIf visual_progress > 2 Then
            If tsCooldownActive < clTop.swIGTime.Elapsed Then
                wasClicked = True
                tsCooldownActive = clTop.swIGTime.Elapsed + tsCooldownValue
            Else
                If Not isCycling Then
                    queue += 1
                    If isCancelEnabled Then btnCancelQueue.Visibility = Visibility.Visible
                    lbQcont.Content = queue.ToString("00")
                End If
            End If
        End If

    End Sub

    Private Sub btnEndlessQueue_Click(sender As Object, e As RoutedEventArgs) Handles btnEndlessQueue.Click
        lbQcont.FontSize = 20
        lbQcont.Content = "∞"
        queue = 0
        isCycling = True

        If clTop.swIGTime.Elapsed > tsCooldownActive Then
            wasClicked = True
            tsCooldownActive = clTop.swIGTime.Elapsed + tsCooldownValue
        End If

        btnCancelQueue.Visibility = Visibility.Visible
        btnEndlessQueue.Visibility = Visibility.Hidden
        btnCancelAction.Visibility = Visibility.Hidden
    End Sub
    Private Sub btnCancelQueue_Click(sender As Object, e As RoutedEventArgs) Handles btnCancelQueue.Click
        queue = 0
        lbQcont.FontSize = 12
        lbQcont.Content = queue.ToString("00")
        isCycling = False
        'If isEndlessEnabled Then btnEndlessQueue.Visibility = Visibility.Visible
        btnCancelQueue.Visibility = Visibility.Hidden
    End Sub
    Private Sub btnCancelAction_Click(sender As Object, e As RoutedEventArgs) Handles btnCancelAction.Click
        wasCanceled = True
        tsCooldownActive = clTop.swIGTime.Elapsed - tsCooldownValue
    End Sub


    Private Sub btnAction_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles btnAction.PreviewMouseLeftButtonDown
        If visual_progress < 3 Then
            Set_Color(Me.FindResource("colWhite"))
        End If
    End Sub
    Private Sub btnAction_PreviewMouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles btnAction.PreviewMouseLeftButtonUp
        If visual_progress < 3 Then
            Set_Color(Me.FindResource("colGray"))
        End If
    End Sub

#End Region

#Region "Region: Test"

#End Region

End Class
