Public Class MainWindow
#Region "Region: Declaration - CONSTANTS"
    Const C_FPS As Integer = 30
    Const C_SEP_FRAMES As SByte = 5
#End Region
#Region "Region: Declaration - Global Variables"
    Public swIGTime As Stopwatch

    Private cScreenState As Char = "i"      'i=Intro; m=Menu; s=Settings; a=Achievements; l=Leaderboards, g=InGame
    Private isRunning As Boolean = False
    Private isPaused As Boolean = False

    Public progress As Integer = 0
    Public playAnimations As Boolean = True
    Public isAnimating As Boolean = False

    Private frameCounter As SByte = 1

    Private Handler_Progress As clHandleProgress
    Private Handler_Achievement As clHandleAchievements
    Private Handler_Story As clHandleStory
    Private Handler_Bosses As clHandleBosses

    Public ucSTR As ucStat
    Public ucSPD As ucStat
    Public ucCON As ucStat
    Private ucStatList As New List(Of ucStat)

    Public ucSTRpushup As ucAction
    Public ucSTRsitup As ucAction
    Public ucSTRsquad As ucAction
    Private clicked_uc As ucAction
    Private ucActionList As New List(Of ucAction)

    Public score As clValue
#End Region

#Region "Region: Buttons - Window"

    Private Sub BtnWinMax_Click(sender As Object, e As RoutedEventArgs)
        If Me.WindowState = WindowState.Maximized Then
            btnWinMax.Content = "🗖"
            Me.WindowState = WindowState.Normal
        Else
            btnWinMax.Content = "🗗"
            Me.WindowState = WindowState.Maximized
        End If
    End Sub
    Private Sub BtnWinClose_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
    Private Sub BtnWinMin_Click(sender As Object, e As RoutedEventArgs)
        Me.WindowState = WindowState.Minimized
    End Sub

#End Region
#Region "Region: Buttons - Menu"

    Private Sub btnMenStart_Click(sender As Object, e As RoutedEventArgs) Handles btnMenStart.Click
        Game_Start_New()
    End Sub
    Private Sub btnMenContinue_Click(sender As Object, e As RoutedEventArgs) Handles btnMenContinue.Click
        Game_Continue()
    End Sub
    Private Sub btnMenSettings_Click(sender As Object, e As RoutedEventArgs) Handles btnMenSettings.Click
        Change_Screen_State("s")
    End Sub
    Private Sub btnMenAchievements_Click(sender As Object, e As RoutedEventArgs) Handles btnMenAchievements.Click
        Change_Screen_State("a")
    End Sub
    Private Sub btnMenScore_Click(sender As Object, e As RoutedEventArgs) Handles btnMenLeaderBoards.Click
        Change_Screen_State("l")
    End Sub
    Private Sub btnMenExit_Click(sender As Object, e As RoutedEventArgs) Handles btnMenExit.Click
        Me.Close()
    End Sub

#End Region

#Region "Region: Key Events"
    Private Sub Window_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Escape
                Game_Exit_To_Menu()
            Case Key.Space
                If Handler_Story.isTyping Then
                    Handler_Story.isSkipping = True
                Else
                    Game_Pause_Toggle()
                End If
                e.Handled = True
            Case Key.Tab
                lbProgress.Visibility = Visibility.Visible
                lbcProgress.Visibility = Visibility.Visible
                lbScore.Visibility = Visibility.Visible
                lbcScore.Visibility = Visibility.Visible
                lbcTimer.Visibility = Visibility.Visible
            Case Key.LeftCtrl
                Test_Sub()
        End Select
    End Sub

    Private Sub Window_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Tab
                lbProgress.Visibility = Visibility.Hidden
                lbcProgress.Visibility = Visibility.Hidden
                lbScore.Visibility = Visibility.Hidden
                lbcScore.Visibility = Visibility.Hidden
                lbcTimer.Visibility = Visibility.Hidden
        End Select
    End Sub

    Private Sub cvStory_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles cvStory.MouseDown
        Handler_Story.isSkipping = True
    End Sub
    Private Sub tbStory_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles tbStory.MouseDown
        Handler_Story.isSkipping = True
    End Sub
    Private Sub Rectangle_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.ChangedButton = MouseButton.Left Then
            Me.DragMove()
        End If
    End Sub
#End Region

#Region "Region: Subs - StartUp"
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Instantiate()
        btnMenContinue.Visibility = Visibility.Hidden
        btnWinPause.Visibility = Visibility.Hidden
        btnWinMenu.Visibility = Visibility.Hidden
        Change_Screen_State("m")
    End Sub
    Private Sub Instantiate()
        swIGTime = New Stopwatch
        score = New clValue()
        Handler_Progress = New clHandleProgress(Me)
        Handler_Achievement = New clHandleAchievements(Me)
        Handler_Story = New clHandleStory(Me)
        Handler_Bosses = New clHandleBosses(Me)
    End Sub
#End Region
#Region "Region: Subs - Menu"

    Private Sub Change_Screen_State(ByVal _new_state As Char)
        'Hide current canvas
        Select Case cScreenState
            Case "m"
                Canvas.SetLeft(cvMenu, -1300)
                cvMenu.Visibility = Visibility.Hidden
            Case "g"
                Canvas.SetLeft(cvGame, 1300)
                cvGame.Visibility = Visibility.Hidden
                btnWinPause.Visibility = Visibility.Hidden
                btnWinMenu.Visibility = Visibility.Hidden
            Case "s"
                Canvas.SetLeft(cvSettings, -1300)
                cvSettings.Visibility = Visibility.Hidden
            Case "a"
                Canvas.SetLeft(cvAchievements, -1300)
                cvAchievements.Visibility = Visibility.Hidden
            Case "l"
                Canvas.SetLeft(cvLeaderBoards, -1300)
                cvLeaderBoards.Visibility = Visibility.Hidden
        End Select

        'Show designated canvas
        Select Case _new_state
            Case "m"
                Canvas.SetLeft(cvMenu, 0)
                cvMenu.Visibility = Visibility.Visible
            Case "g"
                Canvas.SetLeft(cvGame, 0)
                cvGame.Visibility = Visibility.Visible
                btnWinPause.Visibility = Visibility.Visible
                btnWinMenu.Visibility = Visibility.Visible
            Case "s"
                Canvas.SetLeft(cvSettings, 0)
                cvSettings.Visibility = Visibility.Visible
            Case "a"
                Canvas.SetLeft(cvAchievements, 0)
                cvAchievements.Visibility = Visibility.Visible
            Case "l"
                Canvas.SetLeft(cvLeaderBoards, 0)
                cvLeaderBoards.Visibility = Visibility.Visible
        End Select
        cScreenState = _new_state
    End Sub

#End Region
#Region "Region: Subs - GameLoop"
    '--------------------------------------------------------------------
    'Game Controller
    '--------------------------------------------------------------------
    Private Sub Game_Start_New()
        Change_Screen_State("g")
        btnMenContinue.Visibility = Visibility.Visible

        'pCreateUCs()
        'pHideAll()

        progress = 0
        pProgress()

        swIGTime.Reset()
        If isPaused Then
            Game_Pause_Toggle()
        End If
        Start_GameLoop()
        Handler_Story.isTyping = False
    End Sub
    Private Sub Game_Continue()
        Change_Screen_State("g")
        If Not Handler_Story.isTyping And isPaused = False Then
            Start_GameLoop()
        End If
    End Sub
    Public Sub Game_Pause_Toggle()
        If cScreenState = "g" Then
            If isPaused Then
                Start_GameLoop()
                rtHeader.Fill = Me.FindResource("colBackground1")
                btnWinPause.Content = "⏸"
            Else
                Stop_GameLoop()
                'rtHeader.Fill = New SolidColorBrush(System.Windows.Media.Colors.DimGray)
                rtHeader.Fill = Me.FindResource("colBackground3")
                btnWinPause.Content = "▷"
            End If
            isPaused = Not isPaused
        End If
    End Sub
    Private Sub Game_Exit_To_Menu()
        If cScreenState = "g" Then
            Stop_GameLoop()
        End If
        If cScreenState = "s" Then
            'Save Settings?
        End If
        Change_Screen_State("m")
    End Sub

    '--------------------------------------------------------------------
    'Game Loop
    '--------------------------------------------------------------------
    Private Async Sub Start_GameLoop()
        Dim swFrameTime As New Stopwatch
        Dim tickrate As Integer = 1000 / C_FPS
        Dim sleepDelta As Integer

        swIGTime.Start()
        isRunning = True

        While isRunning ' Or frameCounter <> 1
            swFrameTime.Reset()
            swFrameTime.Start()
            Handle_Every_Frame()
            Handle_Seperate_Frames(frameCounter)
            frameCounter = If(frameCounter = C_SEP_FRAMES, 1, frameCounter + 1)
            swFrameTime.Stop()
            sleepDelta = tickrate - CInt(swFrameTime.ElapsedMilliseconds)
            If sleepDelta > 0 Then
                'Debug.WriteLine(sleepDelta)
                Await Task.Delay(sleepDelta)
            Else
                Debug.WriteLine("Frames can't keep up. Framecounter: " & frameCounter.ToString & "; Delta: " & sleepDelta.ToString)
            End If
        End While
    End Sub
    Private Sub Stop_GameLoop()
        isRunning = False
        swIGTime.Stop()
    End Sub
    Private Sub Handle_Every_Frame()
        'Update GameTimer
        lbcTimer.Content = swIGTime.Elapsed.ToString("hh\:mm\:ss\.ff")
        lbcProgress.Content = progress
        Handle_UCs()
        Update_Score()
    End Sub
    Private Sub Handle_Seperate_Frames(ByVal _frame As SByte)
        Select Case _frame
            Case 1
                'check Progress
                If Handler_Progress.Check_Applicable() Then
                    progress += 1
                    'Handler_Progress.Action()
                End If
            Case 2
                'handle story
                If Handler_Story.Check_Applicable() Then
                    Handler_Story.Action()
                End If
            Case 3
                Handler_Progress.Action()
            Case 4
                'handle achievements
                If Handler_Achievement.Check_Applicable() Then
                    Handler_Achievement.Action()
                End If
            Case 5
                'handle bosse
                If Handler_Bosses.Check_Applicable() Then
                    Handler_Bosses.Action()
                End If

        End Select
    End Sub

    '--------------------------------------------------------------------
    'Methods
    '--------------------------------------------------------------------
    Private Sub Handle_UCs()

        If clicked_uc IsNot Nothing Then
            If clicked_uc.queue = 0 And clicked_uc.isCycling = False And clicked_uc.Check_timer Then
                For Each uc3 In ucActionList
                    uc3.Set_Button_Enabled(True)
                    uc3.Set_Color(Me.FindResource("colGray"))
                    uc3.Set_Endless_Visibility(Visibility.Visible)
                Next
                clicked_uc = Nothing
            End If
        Else
            For Each uc In ucActionList
                If uc.wasClicked Then
                    clicked_uc = uc
                    ucActionList.Remove(uc)
                    For Each uc2 In ucActionList
                        uc2.Set_Button_Enabled(False)
                        uc2.Set_Color(Me.FindResource("colGray"))
                        uc2.Set_Endless_Visibility(Visibility.Hidden)
                    Next
                    ucActionList.Add(uc)
                    uc.wasClicked = False
                    uc.Set_Button_Enabled(True)
                    uc.Set_Color(Me.FindResource("colWhite"))
                    Exit For
                End If
            Next
        End If
    End Sub
    Private Sub Update_Score()
        'Dim score_temp As Integer

        score.Clear()
        For Each uc In ucActionList
            score.Add(uc.v)
        Next

        ucSTR.Set_L(score)
        score = ucSTR.Get_Value

        lbcScore.Content = score.Write_V


    End Sub

#End Region

#Region "Region: Subs - Progress"

    Public Async Sub pProgress()

        Dim check_progress_step As Integer = 0 ' pCreateUCs() pHideAll()
        If check_progress_step = progress Then
            ucSTRpushup = New ucAction(Me, "Do a Pushup", 0, 0, 0.01)
            ucSTRpushup.Set_CD_Parameters(0, 0, 0)
            ucSTRsitup = New ucAction(Me, "Train Situp", 3, 0, 2)
            ucSTRsitup.Set_CD_Parameters(2000, 1000, 0)
            ucSTRsquad = New ucAction(Me, "Perform Squad", 3, 0, 0.5)
            ucSTRsquad.Set_CD_Parameters(200, 100, 0)

            ucSTR = New ucStat("STR:", New clValue, " ") '*
            ucSPD = New ucStat("SPD:", New clValue, "+")
            ucCON = New ucStat("CON:", New clValue, " ")


            'vbBoss.Visibility = Visibility.Hidden
            'vbAchievement.Visibility = Visibility.Hidden
            cvAchievement.Background = Me.FindResource("colBackground0")
            cvBoss.Background = Me.FindResource("colBackground0")

            vbBody.Visibility = Visibility.Hidden
            'lbScore.Visibility = Visibility.Hidden
            'lbcScore.Visibility = Visibility.Hidden
            'lbcTimer.Visibility = Visibility.Hidden
            btnWinPause.Visibility = Visibility.Hidden
            'lbBodyTitle.Visibility = Visibility.Hidden

            spBodyHeader.Visibility = Visibility.Hidden

            lbProgress.Visibility = Visibility.Hidden
            lbcProgress.Visibility = Visibility.Hidden
            lbScore.Visibility = Visibility.Hidden
            lbcScore.Visibility = Visibility.Hidden
            lbcTimer.Visibility = Visibility.Hidden

            Exit Sub
        End If

        check_progress_step += 1 ' pFirstDialog()
        If check_progress_step = progress Then
            ' do nothing, first dialog
            Exit Sub
        End If

        check_progress_step += 1 ' pShowFirstActionItem()
        If check_progress_step = progress Then

            spBody.Children.Add(ucSTRpushup)
            ucActionList.Add(ucSTRpushup)

            Canvas.SetLeft(vbBody, 260)
            vbBody.Width = 680
            spBody.Height = 60

            vbBody.Opacity = 0
            vbBody.Visibility = Visibility.Visible

            If playAnimations And Not isAnimating Then
                isAnimating = True
                For i As Double = 0 To 1 Step 0.01
                    Await Task.Delay(10)
                    vbBody.Opacity = i
                Next
                isAnimating = False
            End If
            cvBody.Opacity = 1
            Exit Sub
        End If

        check_progress_step += 1 ' pChangeActionText1()
        If check_progress_step = progress Then
            ucSTRpushup.Set_Title("Do another Pushup")
            Exit Sub
        End If

        check_progress_step += 1 ' pChangeActionText2()
        If check_progress_step = progress Then
            ucSTRpushup.Set_Title("Do one more Pushup")
            Exit Sub
        End If

        check_progress_step += 1 ' pIntroduceScore()
        If check_progress_step = progress Then
            ucSTRpushup.Progress_Visuals()
            Exit Sub
        End If

        check_progress_step += 1 ' pIntroduceCooldown()
        If check_progress_step = progress Then
            ucSTRpushup.Set_CD_Parameters(200, 25, 0)
            ucSTRpushup.Progress_Visuals()
            Exit Sub
        End If

        check_progress_step += 1 ' pIntroduceQueue()
        If check_progress_step = progress Then
            ucSTRpushup.Progress_Visuals()
            Exit Sub
        End If

        check_progress_step += 1 ' pShowSecondActionItem()
        If check_progress_step = progress Then
            spBody.Children.Add(ucSTRsitup)
            ucActionList.Add(ucSTRsitup)

            spBody.Children.Add(ucSTRsquad)
            ucActionList.Add(ucSTRsquad)

            If playAnimations And Not isAnimating Then
                isAnimating = True
                For i As Integer = 1 To 58
                    Await Task.Delay(8)
                    spBody.Height = 60 + i * 2
                Next
                isAnimating = False
            End If
            spBody.Height = 176
            Exit Sub
        End If

        check_progress_step += 1 ' pShowEffectValue()
        If check_progress_step = progress Then
            ucSTRpushup.Progress_Visuals()
            ucSTRsitup.Progress_Visuals()
            ucSTRsquad.Progress_Visuals()
            Exit Sub
        End If

        check_progress_step += 1 ' pAdvancedControls()
        If check_progress_step = progress Then
            ucSTRpushup.Progress_Visuals()
            ucSTRsitup.Progress_Visuals()
            ucSTRsquad.Progress_Visuals()

            ucStatList.Add(ucSTR)
            spBodyHeader.Children.Add(ucSTR)

            spBodyHeader.Visibility = Visibility.Visible
            Exit Sub
        End If




        check_progress_step += 1 ' Items instantiated, but not added yet
        If check_progress_step = progress + 100 Then
            ucStatList.Add(ucSPD)
            spBodyHeader.Children.Add(ucSPD)

            ucStatList.Add(ucCON)
            spBodyHeader.Children.Add(ucCON)

            Exit Sub
        End If

    End Sub


    Public Sub Set_Progress(ByRef _progress As Integer)

        playAnimations = False
        progress = 3
        For i = 2 To _progress - 2
            progress += 1
            pProgress()
        Next


        If Handler_Story.Check_Applicable() Then
            Handler_Story.Action()
        End If

        playAnimations = True

    End Sub

#End Region

#Region "Region: Subs - Other"

    Private Sub Test_Sub()

        'Dim vTest, vTest2 As clValue
        'vTest = New clValue(1230)
        'vTest2 = New clValue(8.24, 1)
        'MessageBox.Show(vTest.Write_V)
        'MessageBox.Show(vTest2.Write_V)
        'vTest.Add(vTest2)
        'MessageBox.Show(vTest.Write_V)


        'vTest.Multiply(vTest2)
        'MessageBox.Show(vTest.Write_V)

        ' progress = 10

        'If isAnimating Then
        '    isAnimating = False
        '    
        'Else
        '    isAnimating = True
        'End If

        Set_Progress(10)
        ucSTRpushup.l.Add(New clValue(100, 0))

    End Sub




#End Region
End Class
